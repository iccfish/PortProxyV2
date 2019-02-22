namespace PortProxy.Statistics
{
	using System;
	using System.Collections.Concurrent;

	using Connection;

	public interface IStatistics
	{
		void Save();

		/// <summary>
		/// 注册
		/// </summary>
		/// <param name="context"></param>
		bool Register(ConnectionContext context);

		/// <summary>
		/// 注销
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		bool UnRegister(ConnectionContext context);

		/// <summary>
		/// 总连接数
		/// </summary>
		long TotalConnectionCount { get; }

		long TotalSuccessConnectionCount { get; }
		long TotalFailedConnectionCount { get; }
		long ConnectionCount { get; }
		long SuccessConnectionCount { get; }
		long FailedConnectionCount { get; }
		long CurrentUpBytes { get; }
		long CurrentDownBytes { get; }
		long TotalUpBytes { get; }
		long TotalDownBytes { get; }

		/// <summary>
		/// 启动时间
		/// </summary>
		DateTime StartupTime { get; }

		/// <summary>
		/// 首次运行时间
		/// </summary>
		DateTime FirstRunTime { get; }

		/// <summary>
		/// 历史运行时间
		/// </summary>
		TimeSpan HistoryRunTime { get; }

		/// <summary>
		/// 累计运行时间
		/// </summary>
		TimeSpan TotalRunTime { get; }

		/// <summary>
		/// 当前运行时间
		/// </summary>
		TimeSpan CurrentRunTime { get; }

		/// <summary>
		/// 当前活动连接
		/// </summary>
		ConcurrentDictionary<Guid, ConnectionContext> Connections { get; }

		/// <summary>
		/// 获得当前活动连接数
		/// </summary>
		int ActiveConnectionCount { get; }

		/// <summary>
		/// 详细统计数据
		/// </summary>
		ConcurrentDictionary<DateTime, StatisticsItem> DetailLog { get; }
	}
}
