namespace PortProxy.Connection
{
	using System;
	using System.IO;
	using System.Net.Sockets;
	using System.Threading.Tasks;

	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	using ProxyServer;

	public class Connection : IConnection
	{
		private ILogger<Connection> _logger;
		private bool _local;
		private readonly IServiceProvider _serviceProvider;
		private readonly ConnectionContext _context;
		private ServerConfig _config;
		ISeed _seed;

		public Connection(IServiceProvider serviceProvider, ConnectionContext context)
		{
			_serviceProvider = serviceProvider;
			_logger = _serviceProvider.GetService<ILogger<Connection>>();
			_config = _serviceProvider.GetService<ServerConfig>();
			_seed = serviceProvider.GetService<ISeed>();
			_context = context;
			_local = _config.Local;
		}

		public async Task ProcessClientAsync()
		{
			var client = _context.Client;
			var id = _context.TraceInfo.Id.ToString();
			var esFactory = _serviceProvider.GetService<IEncodeStreamFactory>();

			_context.ConnectionState = ConnectionState.WaitForValidation;

			var stream = client.GetStream();
			_context.AttachDownStream(stream);

			_logger.LogInformation($"[{id}] 正在验证");
			var port = _local ? _config.RemoteServerPort : _config.Port;
			//验证
			var valid = false;
			var validator = _serviceProvider.GetRequiredService<IStreamValidator>();
			if (!_config.Local)
			{
				try
				{
					_logger.LogInformation($"[{id}] 正在验证请求");
					valid = await validator.Validate(_context, stream);
				}
				catch (Exception e)
				{
					_logger.LogInformation($"#{id} 验证错误：{e.Message}");
				}
				finally
				{
					_logger.LogInformation($"#{id} 验证结果：{valid}");
				}
			}
			else
			{
				valid = true;
			}

			_context.ConnectionState = valid ? ConnectionState.ValidationPassed : ConnectionState.ValidationFailed;

			if (valid)
			{
				var upclient = new TcpClient();
				NetworkStream upstream = null;
				_context.ConnectionState = ConnectionState.ConnectUpPeer;

				var connected = false;

				try
				{
					_logger.LogInformation($"[{id}] 正在连接上游服务器");
					await upclient.ConnectAsync(_config.RemoteServer, _config.RemoteServerPort);
					upstream = upclient.GetStream();
					_logger.LogInformation($"[{id}] 上游服务器连接已打开 {upclient.Client.LocalEndPoint} -> {upclient.Client.RemoteEndPoint}");

					if (_local)
					{
						var buffer = validator.GenerateValidationData(port);
						await upstream.WriteAsync(buffer, 0, buffer.Length);
					}

					_context.AttachUpStream(upstream, upclient);
					_context.ConnectionState = ConnectionState.UpPeerConnected;
					connected = true;
				}
				catch (Exception e)
				{
					_logger.LogError($"[{id}] 未能为打开上游服务器连接: {e.Message}");
					_context.ConnectionState = ConnectionState.UpPeerConnectFailed;
				}

				if (connected)
				{
					_context.ConnectionState = ConnectionState.TunnelEstablished;

					if (_local)
					{
						var target = esFactory.CreateStream(upstream);
						await Task.WhenAll(
							ProcessStreamCopyAsync(true, stream, target),
							ProcessStreamCopyAsync(false, target, stream)
						);
					}
					else
					{
						var target = esFactory.CreateStream(stream);
						await Task.WhenAll(
							ProcessStreamCopyAsync(true, target, upstream),
							ProcessStreamCopyAsync(false, upstream, target)
						);
					}
				}


				try
				{
					upstream?.Dispose();
					upclient.Close();
				}
				catch (Exception e)
				{
					_logger.LogError($"[{id}] 尝试关闭服务器连接的时候发生错误 {e.Message}");
				}
			}
			else
			{
				//随机关闭连接
				var timeout = _seed.GetCloseTime();
				_logger.LogInformation($"[{id}] => 等待 {timeout}ms 关闭");
				await Task.Delay(timeout);
			}

			try
			{
				stream.Dispose();
				client.Client.Close();
				client.Close();
			}
			catch (Exception e)
			{
				_logger.LogError($"[{id}] 尝试关闭客户端连接的时候发生错误 {e.Message}");
			}

			_context.ConnectionState = ConnectionState.ConnectionClosed;
			var info = _context.TraceInfo;
			_logger.LogInformation($"[{id}] 连接已关闭，开始连接时间：{info.StartTime}，结束连接时间：{info.EndTime}，上行数据 {info.DataUpBeginTime}->{info.DataUpEndTime}({info.UpBytes}/{info.UpBytes.ToSizeDescription()})，下行数据 {info.DataDownBeginTime}->{info.DataDownEndTime}({info.DownBytes}/{info.DownBytes.ToSizeDescription()})");
		}

		bool IsSocketConnected(Socket socket)
		{
			if (!socket.Connected)
				return false;

			return !(socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0);
		}

		private async Task<long> ProcessStreamCopyAsync(bool isUp, Stream srcStream, Stream dstStream)
		{
			var count = 0;
			var buffer = new byte[_config.BufferSize];

			do
			{
				try
				{
					count = await srcStream.ReadAsync(buffer, 0, buffer.Length);
					if (isUp)
					{
						_context.TraceInfo.AddUpBytes(count);
					}
					else
					{
						_context.TraceInfo.AddDownBytes(count);
					}

					if (count == 0)
						break;

					await dstStream.WriteAsync(buffer, 0, count);
				}
				catch (Exception)
				{
					break;
				}
			} while (count > 0);

			if (isUp)
			{
				_context.ConnectionState = ConnectionState.ClientDisconnect;
			}
			else
			{
				_context.ConnectionState = ConnectionState.ServerDisconnect;
			}

			try
			{
				dstStream.Close();
				srcStream.Close();
			}
			catch (Exception)
			{
			}

			return isUp ? _context.TraceInfo.UpBytes : _context.TraceInfo.DownBytes;
		}

		#region Dispose方法实现

		/// <summary>
		/// 当前对象已经被释放
		/// </summary>
		public event EventHandler Disposed;

		protected virtual void OnDisposed()
		{
			Disposed?.Invoke(this, EventArgs.Empty);
		}

		bool _disposed;

		/// <summary>
		/// 释放资源
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			_disposed = true;

			if (disposing)
			{
				_context.Dispose();
			}

			OnDisposed();

			//挂起终结器
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 检查是否已经被销毁。如果被销毁，则抛出异常
		/// </summary>
		/// <exception cref="ObjectDisposedException">对象已被销毁</exception>
		protected void CheckDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException(this.GetType().Name);
		}

		#endregion
	}
}
