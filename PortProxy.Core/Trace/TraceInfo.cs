namespace PortProxy.Trace
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using Connection;

	using Newtonsoft.Json;

	public class TraceInfo : ITraceInfo
	{
		private long _upBytes;
		private long _downBytes;

		public List<StateItem> ConnectionStates { get; } = new List<StateItem>();

		/// <summary>
		/// 状态变更
		/// </summary>
		private ConnectionState _connectionState = ConnectionState.None;

		private string _remoteIp;

		public string RemoteIp
		{
			get => _remoteIp;
			set
			{
				if (!_remoteIp.IsNullOrEmpty())
					throw new InvalidOperationException("RemoteIp can not be changed.");
				_remoteIp = value;
			}
		}

		private Guid _id;

		/// <summary>
		/// 连接ID
		/// </summary>
		public Guid Id
		{
			get => _id;
			set
			{
				if (_id != Guid.Empty)
					throw new InvalidOperationException("Id can not be changed.");
				_id = value;
			}
		}

		/// <summary>
		/// 启动时间
		/// </summary>
		public DateTime StartTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 结束时间
		/// </summary>
		public DateTime? EndTime { get; set; }

		public DateTime? DataUpBeginTime { get; set; }
		public DateTime? DataUpEndTime { get; set; }
		public DateTime? DataDownBeginTime { get; set; }
		public DateTime? DataDownEndTime { get; set; }

		[JsonIgnore]
		public ConnectionContext Context { get; set; }

		/// <summary>
		/// 上行字节数
		/// </summary>
		public long UpBytes
		{
			get => _upBytes;
			set => _upBytes = value;
		}

		public void AddUpBytes(long count)
		{
			Interlocked.Add(ref _upBytes, count);
			if (DataUpBeginTime == null)
				DataUpBeginTime = DateTime.Now;
			DataUpEndTime = DateTime.Now;
			OnUpBytesTransfered(new BytesTransferEventArgs((int)count));
		}

		/// <summary>
		/// 平均上行速度
		/// </summary>
		[JsonIgnore]
		public double UpAvgSpeed => DataUpBeginTime == null || DataUpBeginTime == DataUpEndTime ? 0.0 : UpBytes * 1.0 / ((DataUpEndTime ?? DateTime.Now) - DataUpBeginTime.Value).TotalSeconds;

		/// <summary>
		/// 下行字节数
		/// </summary>
		public long DownBytes
		{
			get => _downBytes;
			set => _downBytes = value;
		}

		public void AddDownBytes(long count)
		{
			Interlocked.Add(ref _downBytes, count);
			if (DataDownBeginTime == null)
				DataDownBeginTime = DateTime.Now;
			DataDownEndTime = DateTime.Now;
			OnDownBytesTransfered(new BytesTransferEventArgs((int)count));
		}

		/// <summary>
		/// 平均下行速度
		/// </summary>
		[JsonIgnore]
		public double DownAvgSpeed => DataDownBeginTime == null || DataDownBeginTime == DataDownEndTime ? 0.0 : DownBytes * 1.0 / ((DataDownEndTime ?? DateTime.Now) - DataDownBeginTime.Value).TotalSeconds;

		/// <summary>
		/// 是否成功
		/// </summary>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// 获得或设置当前的节点状态
		/// </summary>
		[JsonIgnore]
		public ConnectionState ConnectionState
		{
			get => _connectionState;
			set
			{
				if (_connectionState == value)
					return;

				IsSuccess |= value == ConnectionState.TunnelEstablished;
				_connectionState = value;
				ConnectionStates.Add(new StateItem(DateTime.Now, value));
				OnConnectionStateChanged();
			}
		}

		/// <summary>
		/// 上行字节统计更新
		/// </summary>
		public event EventHandler<BytesTransferEventArgs> UpBytesTransfered;

		/// <summary>
		/// 下行字节统计更新
		/// </summary>
		public event EventHandler<BytesTransferEventArgs> DownBytesTransfered;

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler ConnectionStateChanged;

		/// <summary>
		/// 引发 <see cref="ConnectionContext.ConnectionStateChanged"/> 事件
		/// </summary>
		protected virtual void OnConnectionStateChanged()
		{
			ConnectionStateChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnUpBytesTransfered(BytesTransferEventArgs e)
		{
			UpBytesTransfered?.Invoke(this, e);
		}

		protected virtual void OnDownBytesTransfered(BytesTransferEventArgs e)
		{
			DownBytesTransfered?.Invoke(this, e);
		}
	}
}
