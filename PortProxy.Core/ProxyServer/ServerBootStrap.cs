namespace PortProxy.ProxyServer
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;

	using Connection;

	using HttpServer;

	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	using NLog;
	using NLog.Config;
	using NLog.Extensions.Logging;

	using Statistics;

	using LogLevel = NLog.LogLevel;

	public class ServerBootStrap
	{
		private IServiceProvider _serviceProvider;
		private ILogger<ServerBootStrap> _logger;
		private Server _server;

		public void Start()
		{
			//corefx bug of https://github.com/dotnet/corefx/issues/24832
			//new ArgumentException();

			var args = Environment.GetCommandLineArgs();
			var cfg = new ServerConfig();

			cfg.Local = args.Any(s => s == "--local");
			cfg.Port = GetOptionValue(args.FirstOrDefault(s => s.StartsWith("--port="))).ToInt32(5299);
			cfg.BufferSize = GetOptionValue(args.FirstOrDefault(s => s.StartsWith("--buffer="))).ToInt32(4) * 1024;
			cfg.RemoteServer = GetOptionValue(args.FirstOrDefault(s => s.StartsWith("--server="))).DefaultForEmpty(cfg.Local ? "" : "127.0.0.1");
			cfg.RemoteServerPort = GetOptionValue(args.FirstOrDefault(s => s.StartsWith("--sport="))).ToInt32(!cfg.Local ? 1080 : 5299);
			cfg.LocalServer = GetOptionValue(args.FirstOrDefault(s => s.StartsWith("--stat_server=")));

			ConfigTraditionalLog();
			_serviceProvider = BuildDi(cfg);

			//增加控制台日志
			if (args.Contains("--console"))
			{
				LogManager.Configuration.LoggingRules.Add(new LoggingRule(
					"*",
					LogLevel.Debug,
					LogManager.Configuration.FindTargetByName("console")
					)
				);
			}

			_logger = _serviceProvider.GetRequiredService<ILogger<ServerBootStrap>>();

			//log
			var error = cfg.CheckForConfigurationError();
			if (!error.IsNullOrEmpty())
			{
				_logger.LogError("配置错误：" + error);
				return;
			}

			_server = _serviceProvider.GetRequiredService<Server>();
			_server.Start();

			_serviceProvider.GetRequiredService<IHttpServer>().Start();
			_serviceProvider.GetRequiredService<IStatistics>();
		}

#pragma warning disable 1998
		public async Task StopAsync()
#pragma warning restore 1998
		{
			//保存统计数据
			_serviceProvider.GetService<IStatistics>().Save();

			_server.Stop();
			_serviceProvider.GetRequiredService<IHttpServer>().Stop();
		}

		static string GetOptionValue(string str)
		{
			if (string.IsNullOrEmpty(str))
				return null;
			return str.Substring(str.IndexOf('=') + 1);
		}

		static void ConfigTraditionalLog()
		{
			//LogManager.AddHiddenAssembly(Assembly.Load(new AssemblyName("Microsoft.Extensions.Logging.Filter")));
			LogManager.AddHiddenAssembly(Assembly.Load(new AssemblyName("Microsoft.Extensions.Logging")));
			//LogManager.AddHiddenAssembly(Assembly.Load(new AssemblyName("Microsoft.Extensions.Logging.Abstractions")));
			LogManager.AddHiddenAssembly(Assembly.Load(new AssemblyName("NLog.Extensions.Logging")));
		}

		private static IServiceProvider BuildDi(ServerConfig config)
		{
			var services = new ServiceCollection();

			services.AddSingleton(config);
			services.AddTransient<Server>();
			services.AddTransient<IStreamValidator, StreamValidator>();
			services.AddSingleton<IStatistics, PortProxy.Statistics.Statistics>();

			services.AddSingleton<ILoggerFactory, LoggerFactory>();
			services.AddSingleton<IConnectionFactory, ConnectionFactory>();
			services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
			services.AddSingleton<IEnv, Env>();
			services.AddSingleton<ISeed, Seed>();
			services.AddSingleton<IHttpServer, BasicHttpServer>();
			services.AddSingleton<IEncodeStreamFactory, EncodeStreamFactory>();

			var serviceProvider = services.BuildServiceProvider();

			var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
			loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
			LogManager.LoadConfiguration("NLog.config");

			return serviceProvider;
		}
	}
}
