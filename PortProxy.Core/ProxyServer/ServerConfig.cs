namespace PortProxy.ProxyServer
{
	using System;
	using System.Text.RegularExpressions;

	public class ServerConfig
	{
		public int RemoteServerPort { get; set; }

		public string RemoteServer { get; set; }

		public int BufferSize { get; set; }

		public int Port { get; set; }

		public bool Local { get; set; }

		public string LocalServer { get; set; }

		public bool GuiMode { get; set; }

		/// <summary>
		/// 是否是服务模式（仅Windows）
		/// </summary>
		public bool ServiceMode { get; set; } = false;

		/// <summary>
		/// 获得命令行参数
		/// </summary>
		/// <returns></returns>
		public string GetCmdLine()
		{
			return $"{(Local ? "--local" : "")} {(ServiceMode ? "--service" : "")} {(GuiMode ? "--console" : "")} --server={RemoteServer} --sport={RemoteServerPort} --port={Port} --stat_server={LocalServer}";
		}

		public string CheckForConfigurationError()
		{
			if (RemoteServer.IsNullOrEmpty())
			{
				return "必须指定上游服务器地址";
			}

			if (Port < 1025 || Port >= 65535)
			{
				return "请指定有效的本地端口范围（1025~65534）";
			}

			if (RemoteServerPort < 1025 || RemoteServerPort >= 65535)
			{
				return "请指定有效的本地端口范围（1025~65534）";
			}

			if (!LocalServer.IsNullOrEmpty()&&!Regex.IsMatch(LocalServer,@"^(\*|[a-z\d\.-_]+):\d+$", RegexOptions.IgnoreCase))
			{
				return "请输入有效的状态服务器前缀（如：127.0.0.1:7788）";
			}

			return null;
		}
	}
}
