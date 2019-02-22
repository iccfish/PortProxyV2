namespace PortProxy.Statistics
{
	using System;
	using System.Collections.Concurrent;
	using System.Threading;

	using Connection;

	using Microsoft.Extensions.DependencyInjection;

	using Newtonsoft.Json;

	using ProxyServer;

	using Trace;

	using Timer = System.Timers.Timer;

	public class Statistics : IStatistics
	{
		private readonly IServiceProvider _serviceProvider;
		Timer _timer;

		public Statistics(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			StartupTime = DateTime.Now;
			_lastStoreTime = DateTime.Now;

			_serviceProvider.GetService<IEnv>().LoadData(nameof(Statistics) + ".json", this);

			_timer = new Timer
			{
				Interval = 10 * 60 * 1000,
				AutoReset = false
			};
			_timer.Elapsed += (sender, args) =>
			{
				Save();
				_timer.Start();
			};
			_timer.Start();
		}

		DateTime _lastStoreTime;

		public void Save()
		{
			HistoryRunTime += DateTime.Now - _lastStoreTime;
			_lastStoreTime = DateTime.Now;

			_serviceProvider.GetService<IEnv>().SaveData(this, nameof(Statistics) + ".json");
		}

		/// <summary>
		/// 注册
		/// </summary>
		/// <param name="context"></param>
		public bool Register(ConnectionContext context)
		{
			context.Disposed += (_1, _2) => UnRegister(_1 as ConnectionContext);
			context.TraceInfo.UpBytesTransfered += (sender, args) =>
			{
				GetStatisticsItem().IncreaseUpDataTransfer(args.BytesCount);
				Interlocked.Add(ref _currentUpBytes, args.BytesCount);
			};
			context.TraceInfo.DownBytesTransfered += (sender, args) =>
			{
				GetStatisticsItem().IncreaseDownDataTransfer(args.BytesCount);
				Interlocked.Add(ref _currentDownBytes, args.BytesCount);
			};
			context.TraceInfo.ConnectionStateChanged += (_1, _2) =>
			{
				var ctx = _1 as TraceInfo;

				if (ctx.ConnectionState == ConnectionState.ConnectionClosed)
				{
					if (ctx.IsSuccess)
					{
						Interlocked.Increment(ref _successConnectionCount);
						GetStatisticsItem(ctx.StartTime).IncreaseSuccessConnectionCount();
					}
					else
					{
						Interlocked.Increment(ref _failedConnectionCount);
						GetStatisticsItem(ctx.StartTime).IncreaseFailedConnectionCount();
					}
				}
			};
			Interlocked.Increment(ref _connectionCount);
			GetStatisticsItem(context.TraceInfo.StartTime).IncreaseConnectionCount();
			return Connections.TryAdd(context.Id, context);
		}

		/// <summary>
		/// 注销
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public bool UnRegister(ConnectionContext context)
		{
			return Connections.TryRemove(context.Id, out _);
		}

		private long _totalConnectionCount;

		/// <summary>
		/// 总连接数
		/// </summary>
		public long TotalConnectionCount
		{
			get => _totalConnectionCount + ConnectionCount;
			set => _totalConnectionCount = value;
		}

		private long _totalSuccessConnectionCount;

		public long TotalSuccessConnectionCount
		{
			get => _totalSuccessConnectionCount + SuccessConnectionCount;
			set => _totalSuccessConnectionCount = value;
		}

		private long _totalFailedConnectionCount;

		public long TotalFailedConnectionCount
		{
			get => _totalFailedConnectionCount + FailedConnectionCount;
			set => _totalFailedConnectionCount = value;
		}

		private long _connectionCount;

		[JsonIgnore]
		public long ConnectionCount => _connectionCount;

		private long _successConnectionCount;

		[JsonIgnore]
		public long SuccessConnectionCount => _successConnectionCount;

		private long _failedConnectionCount;

		[JsonIgnore]
		public long FailedConnectionCount => _failedConnectionCount;

		private long _currentUpBytes;

		[JsonIgnore]
		public long CurrentUpBytes => _currentUpBytes;

		private long _currentDownBytes;

		[JsonIgnore]
		public long CurrentDownBytes => _currentDownBytes;

		private long _totalUpBytes;

		public long TotalUpBytes
		{
			get => _totalUpBytes + CurrentUpBytes;
			set => _totalUpBytes = value;
		}

		private long _totalDownBytes;

		public long TotalDownBytes
		{
			get => _totalDownBytes + CurrentDownBytes;
			set => _totalDownBytes = value;
		}

		/// <summary>
		/// 启动时间
		/// </summary>
		[JsonIgnore]
		public DateTime StartupTime { get; private set; }

		/// <summary>
		/// 首次运行时间
		/// </summary>
		public DateTime FirstRunTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 历史运行时间
		/// </summary>
		public TimeSpan HistoryRunTime { get; set; }

		/// <summary>
		/// 累计运行时间
		/// </summary>
		[JsonIgnore]
		public TimeSpan TotalRunTime => DateTime.Now - _lastStoreTime + HistoryRunTime;

		/// <summary>
		/// 当前运行时间
		/// </summary>
		[JsonIgnore]
		public TimeSpan CurrentRunTime => DateTime.Now - StartupTime;


		/// <summary>
		/// 当前活动连接
		/// </summary>
		[JsonIgnore]
		public ConcurrentDictionary<Guid, ConnectionContext> Connections { get; } = new ConcurrentDictionary<Guid, ConnectionContext>();

		/// <summary>
		/// 获得当前活动连接数
		/// </summary>
		[JsonIgnore]
		public int ActiveConnectionCount => Connections.Count;

		/// <summary>
		/// 详细统计数据
		/// </summary>
		public ConcurrentDictionary<DateTime, StatisticsItem> DetailLog { get; } = new ConcurrentDictionary<DateTime, StatisticsItem>();

		StatisticsItem GetStatisticsItem(DateTime? time = null) => DetailLog.GetOrAdd((time ?? DateTime.Now).TrimToHour(), x => new StatisticsItem(x));
	}
}
