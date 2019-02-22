namespace PortProxy.HttpServer
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;

	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	using ProxyServer;

	using Statistics;

	public class BasicHttpServer : IHttpServer
	{
		private readonly IServiceProvider _serviceProvider;
		private HttpListener _httpListener;
		private ILogger<BasicHttpServer> _logger;
		private readonly string _staticRoot;
		private HttpServerConfig _httpServerConfig;
		bool _enabled;

		public BasicHttpServer(IServiceProvider serviceProvider, ILogger<BasicHttpServer> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;

			var config = _serviceProvider.GetService<ServerConfig>();
			_enabled = !config.LocalServer.IsNullOrEmpty();
			if (!_enabled)
			{
				_logger.LogInformation("没有启用本地HTTP状态服务器");
			}
			else
			{
				_logger.LogInformation("正在启动本地HTTP状态服务器...");
				var env = serviceProvider.GetService<IEnv>();
				_httpServerConfig = env.LoadConfig<HttpServerConfig>("httpServer.json");

				var prefix = $"http://{config.LocalServer}/";
				_httpListener = new HttpListener();
				_httpListener.Prefixes.Add(prefix);
				_logger.LogInformation($"HTTP服务监听地址：{prefix}");

				_staticRoot = Path.Combine(env.ProgramRoot, _httpServerConfig.Root) + Path.DirectorySeparatorChar;
				_logger.LogInformation($"HTTP服务静态目录地址：{_staticRoot}");
			}
		}

		public void Start()
		{
			if (!_enabled)
				return;

			try
			{
				_httpListener.Start();
				_logger.LogInformation($"HTTP状态服务器已启动");
				ListenForRequestAsync();
			}
			catch (Exception e)
			{
				_logger.LogError(e, "HTTP状态服务器启动失败");
			}
		}

		async void ListenForRequestAsync()
		{
			while (_httpListener.IsListening)
			{
				try
				{
					var context = await _httpListener.GetContextAsync();
					var request = context.Request;
					var response = context.Response;
					Match match;
					_logger.LogInformation($"{context.Request.RequestTraceIdentifier} - {context.Request.RemoteEndPoint} - {request.HttpMethod} {request.RawUrl} - {request.UserAgent}");

					if (await AuthenticateRequest(context))
					{
						if (request.RawUrl == "/stat.html")
						{
							//状态首页
							await ProcessStatPageAsync(context);
						}
						else if (request.RawUrl == "/stat/current.html")
						{
							//当前活动
							await ProcessStatCurrentPageAsync(context);
						}
						else if ((match = Regex.Match(request.RawUrl, @"/stat/(\d{4}-\d{2}-\d{2})\.html", RegexOptions.IgnoreCase)).Success)
						{
							//每日明细
							var date = match.GetGroupValue(1).ToDateTime();
							await ProcessStatPageAsync(context, date);
						}
						else if (!await ProcessStaticResource(context))
						{
							await SendResponse(response, "Not Found.", 404);
						}
					}

					response.Close();
				}
				catch (Exception)
				{
				}
			}
		}

		async Task<bool> AuthenticateRequest(HttpListenerContext context)
		{
			if (_httpServerConfig.UserName.IsNullOrEmpty() || _httpServerConfig.Password.IsNullOrEmpty())
				return true;

			var valid = false;
			var request = context.Request;
			var response = context.Response;
			var auth = request.Headers["Authorization"];
			if (!auth.IsNullOrEmpty() && auth.StartsWith("Basic "))
			{
				auth = auth.Remove(0, 6);
				var authInfo = Encoding.UTF8.GetString(Convert.FromBase64String(auth)).Split(':');
				if (authInfo.Length == 2)
				{
					valid = authInfo[0] == _httpServerConfig.UserName && authInfo[1] == _httpServerConfig.Password;
				}
			}

			if (!valid)
			{
				response.AddHeader("WWW-Authenticate", "Basic relam=\"Proxy stat authenticate\"");
				await SendResponse(response, "Unauthozied", 401);

				return false;
			}

			return valid;
		}

		/// <summary>
		/// 处理静态资源
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		async Task<bool> ProcessStaticResource(HttpListenerContext context)
		{
			var path = context.Request.RawUrl.Trim('/');
			if (Path.DirectorySeparatorChar != '\\')
			{
				path = path.Replace('/', Path.DirectorySeparatorChar);
			}

			path = Path.Combine(_staticRoot, path);
			if (!path.StartsWith(_staticRoot) || !File.Exists(path))
			{
				return false;
			}

			//是否允许？
			var extension = Path.GetExtension(path).Trim('.');
			if (!_httpServerConfig.AllowExtensions.Contains(extension))
			{
				await SendResponse(context.Response, "The specified resource not available.", 403);
				return true;
			}

			var response = context.Response;
			response.StatusCode = 200;
			response.ContentType = _httpServerConfig.Mime.GetValue(extension).DefaultForEmpty("application/octet-stream");

			using (var fs = File.OpenRead(path))
			{
				response.ContentLength64 = fs.Length;
				await fs.CopyToAsync(response.OutputStream);
			}

			return true;
		}

		async Task ProcessStatPageAsync(HttpListenerContext context, DateTime date)
		{
			var stat = _serviceProvider.GetService<IStatistics>();
			var sb = new StringBuilder();

			BuildHtmlHead($"统计 - {date:yyyy-MM-dd}", sb);
			var maxTime = date.AddDays(1);
			var statLogs = stat.DetailLog.Where(s => s.Key >= date && s.Key < maxTime).OrderByDescending(s => s.Key).ToArray();
			sb.AppendLine("<table><tr><th>时间</th><th>总连接数</th><th>成功连接数</th><th>失败连接数</th><th>上行数据</th><th>下行数据</th></tr>");
			foreach (var statLog in statLogs)
			{
				var dateStr = statLog.Key.ToString("yyyy-MM-dd HH:00");
				sb.AppendLine($"<tr><td><strong>{dateStr}</strong></td>");
				sb.AppendLine($"<td><code>{statLog.Value.ConnectionCount:N0}</code></td>");
				sb.AppendLine($"<td><code>{statLog.Value.SuccessConnectionCount:N0}</code></td>");
				sb.AppendLine($"<td><code>{statLog.Value.FailedConnectionCount:N0}</code></td>");
				sb.AppendLine($"<td><code>{statLog.Value.BytesUp.ToSizeDescription()}</code></td>");
				sb.AppendLine($"<td><code>{statLog.Value.BytesDown.ToSizeDescription()}</code></td>");
			}

			sb.AppendLine("</table>");
			BuildHtmlFooter(sb);
			await SendResponse(context.Response, sb.ToString(), 200);
		}

		async Task ProcessStatCurrentPageAsync(HttpListenerContext context)
		{
			var stat = _serviceProvider.GetService<IStatistics>();
			var sb = new StringBuilder();

			BuildHtmlHead("统计 - 当前活动", sb);

			sb.AppendLine("<table><tr><th>连接ID</th><th>下游节点</th><th>连接时间</th><th>断开时间</th><th>状态</th><th>上行数据</th><th>上行平均速度</th><th>下行数据</th><th>下行平均速度</th></tr>");
			var statLogs = stat.Connections.OrderByDescending(s => s.Value.TraceInfo.StartTime).ToArray();
			if (statLogs.Length == 0)
			{
				sb.AppendLine("<tr><td colspan=\"9\">没有数据</td></tr>");
			}

			foreach (var statLog in statLogs)
			{
				var traceInfo = statLog.Value.TraceInfo;
				sb.AppendLine($"<tr><td><strong>{statLog.Key}</strong></a></td>");
				sb.AppendLine($"<td><strong>{traceInfo.RemoteIp}</strong></a></td>");
				sb.AppendLine($"<td>{traceInfo.StartTime:HH:mm:ss}</td>");
				sb.AppendLine($"<td>{traceInfo.EndTime:HH:mm:ss}</td>");
				sb.AppendLine($"<td>{traceInfo.ConnectionState}</td>");
				sb.AppendLine($"<td>{traceInfo.UpBytes.ToSizeDescription()}</td>");
				sb.AppendLine($"<td>{traceInfo.UpAvgSpeed.ToSizeDescription()}/秒</td>");
				sb.AppendLine($"<td>{traceInfo.DownBytes.ToSizeDescription()}</td>");
				sb.AppendLine($"<td>{traceInfo.DownAvgSpeed.ToSizeDescription()}/秒</td>");
			}

			sb.AppendLine("</table>");

			BuildHtmlFooter(sb);
			await SendResponse(context.Response, sb.ToString(), 200);
		}

		async Task ProcessStatPageAsync(HttpListenerContext context)
		{
			var stat = _serviceProvider.GetService<IStatistics>();
			var sb = new StringBuilder();

			BuildHtmlHead("状态统计", sb);

			//基本统计
			sb.AppendLine("<table><tr><th colspan=\"2\">基本运行数据</th></tr>");
			sb.AppendLine($"<tr><td><strong>本次启动时间</strong></td><td><code>{stat.StartupTime}</code></td></tr>");
			sb.AppendLine($"<tr><td><strong>已运行时间</strong></td><td><code>{stat.CurrentRunTime.ToFriendlyDisplay()}</code></td></tr>");
			sb.AppendLine($"<tr><td><strong>累计运行时间</strong></td><td><code>{stat.TotalRunTime.ToFriendlyDisplay()}</code></td></tr>");
			sb.AppendLine($"<tr><td><strong>当前连接数</strong></td><td>总数 <code>{stat.ConnectionCount:N0}</code> 成功<code>{stat.SuccessConnectionCount:N0}</code> 失败 <code>{stat.FailedConnectionCount:N0}</code></td></tr>");
			sb.AppendLine($"<tr><td><strong>当前数据量</strong></td><td>上行 <code>{stat.CurrentUpBytes.ToSizeDescription()}</code>  下行 <code>{stat.CurrentDownBytes.ToSizeDescription()}</code></td></tr>");
			sb.AppendLine($"<tr><td><strong>累计连接数</strong></td><td>总数 <code>{stat.TotalConnectionCount:N0}</code> 成功 <code>{stat.TotalSuccessConnectionCount:N0}</code> 失败 <code>{stat.TotalFailedConnectionCount:N0}</code></td></tr>");
			sb.AppendLine($"<tr><td><strong>累计数据量</strong></td><td>上行 <code>{stat.TotalUpBytes.ToSizeDescription()}</code>  下行 <code>{stat.TotalDownBytes.ToSizeDescription()}</code></td></tr>");
			sb.AppendLine($"<tr><td><strong>当前活动连接数</strong></td><td><code>{stat.ActiveConnectionCount:N0}</code> 【<a href=\"/stat/current.html\" target=\"_blank\">查看详细</a>】</td></tr>");
			sb.AppendLine("</table><br />");

			//详细统计
			sb.AppendLine("<table><tr><th>日期</th><th>总连接数</th><th>成功</th><th>失败</th><th>上行数据</th><th>下行数据</th></tr>");
			var statLogs = stat.DetailLog.Values.GroupBy(s => s.Time.TrimToDay()).Select(s => s.Sum(s.Key)).ToArray();
			if (statLogs.Length == 0)
			{
				sb.AppendLine("<tr><td colspan=\"6\">没有数据</td></tr>");
			}

			foreach (var statLog in statLogs)
			{
				var dateStr = statLog.Time.ToString("yyyy-MM-dd");
				sb.AppendLine($"<tr><td><a href=\"/stat/{dateStr}.html\" target=\"_blank\"><strong>{dateStr}</strong></a></td>");
				sb.AppendLine($"<td>{statLog.ConnectionCount:N0}</td>");
				sb.AppendLine($"<td>{statLog.SuccessConnectionCount:N0}</td>");
				sb.AppendLine($"<td>{statLog.FailedConnectionCount:N0}</td>");
				sb.AppendLine($"<td>{statLog.BytesUp.ToSizeDescription()}</td>");
				sb.AppendLine($"<td>{statLog.BytesDown.ToSizeDescription()}</td>");
			}

			sb.AppendLine("</table>");

			BuildHtmlFooter(sb);
			await SendResponse(context.Response, sb.ToString(), 200);
		}

		void BuildHtmlHead(string title, StringBuilder sb)
		{
			sb.AppendLine("<!doctype html><html><head><meta name=\"Content-Type\" content=\"text/html;charset=utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=0, minimum-scale=1.0, maximum-scale=1.0\">");
			sb.AppendLine($"<title>{title}</title><link rel=\"stylesheet\" type=\"text/css\" href=\"/assets/common.css\" ></head><body>");
		}

		void BuildHtmlFooter(StringBuilder sb)
		{
			sb.AppendLine("</body></html>");
		}

		async Task SendResponse(HttpListenerResponse response, string content, int code)
		{
			response.StatusCode = code;
			response.ContentType = "text/html; charset=utf-8";

			var buffer = Encoding.UTF8.GetBytes(content);
			response.ContentLength64 = buffer.Length;
			await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
		}

		public void Stop()
		{
			if (!_enabled)
				return;
			_httpListener.Stop();
		}
	}
}
