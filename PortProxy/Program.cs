using System;

namespace PortProxy
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using System.Threading;

	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	using NLog;
	using NLog.Extensions.Logging;

	using ProxyServer;

	class Program
	{
		static void Main()
		{
			var bootStrap = new ServerBootStrap();

			AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
			{
				bootStrap.StopAsync().Wait();
			};

			bootStrap.Start();
			while (true)
			{
				Thread.Sleep(100);
			}
		}
	}
}
