using System;
using System.Collections.Generic;
using System.Text;

namespace PortProxy.ProxyServer
{
	using System.IO;
	using System.Security.Cryptography;

	class Seed : ISeed
	{
		public int RandomSeedBegin { get; }

		public int RandomSeedEnd { get; }

		public long TimeKey { get; }

		public int InvalidConnectionCloseTime { get; set; }

		public Seed(IEnv env)
		{
			var path = Path.Combine(env.ConfigRoot, "seed");
			byte[] buffer;

			if (!File.Exists(path))
			{
				buffer = new byte[128];
				var rng = new RNGCryptoServiceProvider();
				rng.GetBytes(buffer);

				File.WriteAllBytes(path, buffer);
			}
			else
			{
				buffer = File.ReadAllBytes(path);
			}

			TimeKey = BitConverter.ToInt64(buffer, 0);
			RandomSeedBegin = buffer[8] & 0x1f;
			RandomSeedEnd = buffer[9] & 0x1f;
			InvalidConnectionCloseTime = buffer[10] >> 4 + 5;
		}

		Random _random = new Random();

		public int GetCloseTime() => _random.Next((InvalidConnectionCloseTime + 5) * 1000);
	}
}
