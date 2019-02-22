namespace PortProxy.Trace
{
	using System;
	using System.Collections.Generic;

	using Connection;

	public interface ITraceInfo
	{
		string RemoteIp { get; }

		/// <summary>
		/// 连接ID
		/// </summary>
		Guid Id { get; }

		/// <summary>
		/// 启动时间
		/// </summary>
		DateTime StartTime { get; }

		/// <summary>
		/// 结束时间
		/// </summary>
		DateTime? EndTime { get; }

		DateTime? DataUpBeginTime { get; }
		DateTime? DataUpEndTime { get; }
		DateTime? DataDownBeginTime { get; }
		DateTime? DataDownEndTime { get; }

		/// <summary>
		/// 上行字节数
		/// </summary>
		long UpBytes { get; }

		/// <summary>
		/// 平均上行速度
		/// </summary>
		double UpAvgSpeed { get; }

		/// <summary>
		/// 下行字节数
		/// </summary>
		long DownBytes { get; }

		/// <summary>
		/// 平均下行速度
		/// </summary>
		double DownAvgSpeed { get; }

		/// <summary>
		/// 是否成功
		/// </summary>
		bool IsSuccess { get; }

		/// <summary>
		/// 获得或设置当前的节点状态
		/// </summary>
		ConnectionState ConnectionState { get; }

		void AddUpBytes(long count);

		void AddDownBytes(long count);

		/// <summary>
		/// 上行字节统计更新
		/// </summary>
		event EventHandler<BytesTransferEventArgs> UpBytesTransfered;

		/// <summary>
		/// 下行字节统计更新
		/// </summary>
		event EventHandler<BytesTransferEventArgs> DownBytesTransfered;

		/// <summary>
		/// 
		/// </summary>
		event EventHandler ConnectionStateChanged;

		ConnectionContext Context { get; }

		List<StateItem> ConnectionStates { get; }
	}
}
