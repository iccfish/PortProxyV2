namespace PortProxy.Connection
{
	using System;
	using System.Net;
	using System.Net.Mime;
	using System.Net.Sockets;
	using System.Security.Cryptography;
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	using ProxyServer;

	class StreamValidator : IStreamValidator
	{
		IServiceProvider _serviceProvider;
		ISeed _seed;

		private Random _random = new Random();
		RNGCryptoServiceProvider _rngCrypto = new RNGCryptoServiceProvider();
		ILogger<StreamValidator> _logger;

		public StreamValidator(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			_logger = serviceProvider.GetService<ILogger<StreamValidator>>();
			_seed = serviceProvider.GetService<ISeed>();
		}

		public async Task<bool> Validate(ConnectionContext context, NetworkStream stream)
		{
			//5+10秒内随机超时
			var cts = new CancellationTokenSource();
			cts.CancelAfter(5000 + _random.Next(10000));

			if (!await ValidateCore(context, stream, cts.Token))
				return false;

			return true;
		}

		async Task<bool> ValidateCore(ConnectionContext context, NetworkStream stream, CancellationToken token)
		{
			var buffer = new byte[8];
			try
			{
				if (await stream.ReadAsync(buffer, 0, buffer.Length, token) != buffer.Length)
				{
					return false;
				}

				//random length
				var randomLength = BitConverter.ToInt64(buffer, 0) ^ _seed.TimeKey;
				if (randomLength > 20)
				{
					_logger.LogInformation($"[{context.Id}] 验证错误 -> 随机数据长度超过限制");
					return false;
				}
				var waitLength = (int)randomLength + _seed.RandomSeedBegin;
				if (await stream.ReadAsync(new byte[waitLength], 0, waitLength, token) != waitLength)
				{
					_logger.LogInformation($"[{context.Id}] 验证错误 -> 前置冗余数据读取失败");
					return false;
				}

				if (await stream.ReadAsync(buffer, 0, buffer.Length, token) != buffer.Length)
				{
					_logger.LogInformation($"[{context.Id}] 验证错误 -> 校验数据读取失败");
					return false;
				}

				var data = BitConverter.ToInt64(buffer, 0);
				var time = data ^ _seed.TimeKey;

				if (Math.Abs(DateTime.UtcNow.Ticks - time) / 10000 / 1000 > 10)
				{
					//最长前后10秒钟
					_logger.LogInformation($"[{context.Id}] 验证错误 -> 数据时间超过限制，请求时间：{new DateTime(time, DateTimeKind.Utc)}");
					return false;
				}

				if (await stream.ReadAsync(new byte[_seed.RandomSeedEnd], 0, _seed.RandomSeedEnd, token) != _seed.RandomSeedEnd)
				{
					_logger.LogInformation($"[{context.Id}] 验证错误 -> 后置冗余数据读取失败");
					return false;
				}

			}
			catch (Exception ex)
			{
				_logger.LogInformation($"[{context.Id}] 验证错误 -> {ex.Message}");
				return false;
			}

			return true;
		}

		public byte[] GenerateValidationData(int key)
		{
			var randomCount = _random.Next(20);
			var buffer = new byte[_seed.RandomSeedBegin + _seed.RandomSeedEnd + 16 + randomCount];

			_rngCrypto.GetBytes(buffer);
			BitConverter.GetBytes(_seed.TimeKey ^ randomCount).CopyTo(buffer, 0);
			BitConverter.GetBytes(DateTime.UtcNow.Ticks ^ _seed.TimeKey).CopyTo(buffer, 8 + randomCount + _seed.RandomSeedBegin);

			return buffer;
		}
	}
}
