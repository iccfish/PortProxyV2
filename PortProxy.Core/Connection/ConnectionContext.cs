namespace PortProxy.Connection
{
	using System;
	using System.Net.Sockets;

	using Trace;

	public class ConnectionContext : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
		public ConnectionContext(TcpClient client)
		{
			_client = client;
			_traceInfo = new TraceInfo
			{
				RemoteIp = client.Connected ? Client.Client.RemoteEndPoint.ToString() : null,
				Id = Guid.NewGuid(),
				ConnectionState = ConnectionState.ClientConnected,
				Context = this
			};
		}

		private readonly TraceInfo _traceInfo;

		public Guid Id => _traceInfo.Id;

		public ITraceInfo TraceInfo => _traceInfo;

		private readonly TcpClient _client;

		/// <summary>
		/// 获得当前的TCP客户端
		/// </summary>
		public TcpClient Client => _client;

		/// <summary>
		/// 附加上下文
		/// </summary>
		/// <param name="upStream"></param>
		/// <param name="downStream"></param>
		public void AttachDownStream(NetworkStream downStream)
		{
			CheckDisposed();
			DownStream = downStream;
			_traceInfo.StartTime = DateTime.Now;
		}

		public ConnectionState ConnectionState
		{
			get => _traceInfo.ConnectionState;
			set
			{
				_traceInfo.ConnectionState = value;
				if (value == ConnectionState.ConnectionClosed)
					_traceInfo.EndTime = DateTime.Now;
			}
		}

		/// <summary>
		/// 附加上下文
		/// </summary>
		/// <param name="upStream"></param>
		public void AttachUpStream(NetworkStream upStream, TcpClient upClient)
		{
			CheckDisposed();
			UpStream = upStream;
			UpClient = upClient;
		}

		/// <summary>
		/// 上游连接
		/// </summary>
		public NetworkStream UpStream { get; private set; }

		/// <summary>
		/// 上游TCP客户端
		/// </summary>
		public TcpClient UpClient { get; private set; }

		/// <summary>
		/// 下游连接
		/// </summary>
		public NetworkStream DownStream { get; private set; }

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
				_traceInfo.Context = null;
			}
			//TODO 释放非托管资源

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
