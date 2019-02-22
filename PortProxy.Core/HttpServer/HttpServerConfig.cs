namespace PortProxy.HttpServer
{
	using System;
	using System.Collections.Generic;

	using Newtonsoft.Json;

	public class HttpServerConfig
	{
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("username")]
		public string UserName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("password")]
		public string Password { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("root")]
		public string Root { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("allowExtensions")]
		public HashSet<string> AllowExtensions { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("mime")]
		public Dictionary<string, string> Mime { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	}
}
