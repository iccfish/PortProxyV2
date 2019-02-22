using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PortProxy.WinService
{
	using ProxyServer;

	public partial class PortProxyService : ServiceBase
	{
		private ServerBootStrap _serverBootStrap;

		public PortProxyService()
		{
			InitializeComponent();
			_serverBootStrap = new ServerBootStrap();
		}

		protected override void OnStart(string[] args)
		{
			_serverBootStrap.Start();
		}

		protected override void OnStop()
		{
			_serverBootStrap.StopAsync();
		}
	}
}
