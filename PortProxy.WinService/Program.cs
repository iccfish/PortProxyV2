using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PortProxy.WinService
{
	using System.Windows.Forms;

	using ProxyServer;

	static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		static async Task Main(string[] args)
		{
			if (args.Contains("--console"))
			{
				var serverBootStrap = new ServerBootStrap();
				serverBootStrap.Start();
				Console.ReadKey();
				await serverBootStrap.StopAsync();
			}
			else if (args.Contains("--service"))
			{
				var servicesToRun = new ServiceBase[]
					{new PortProxyService(),};
				ServiceBase.Run(servicesToRun);
			}
			else
			{
				if (Environment.OSVersion.Version.Major >= 6)
					SetProcessDPIAware();

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				new InstallService().ShowDialog();
			}
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern bool SetProcessDPIAware();
	}
}
