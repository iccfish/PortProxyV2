namespace PortProxy.Statistics
{
	using System;
	using System.Threading;

	public class StatisticsItem
	{
		public StatisticsItem(DateTime time)
		{
			Time = time;
		}

		public DateTime Time { get; }

		private int _connectionCount;

		public int ConnectionCount
		{
			get => _connectionCount;
			set => _connectionCount = value;
		}

		public void IncreaseConnectionCount() => Interlocked.Increment(ref _connectionCount);

		private int _failedConnectionCount;

		public int FailedConnectionCount
		{
			get => _failedConnectionCount;
			set => _failedConnectionCount = value;
		}

		public void IncreaseFailedConnectionCount() => Interlocked.Increment(ref _failedConnectionCount);

		private int _successConnectionCount;

		public int SuccessConnectionCount
		{
			get => _successConnectionCount;
			set => _successConnectionCount = value;
		}

		public void IncreaseSuccessConnectionCount() => Interlocked.Increment(ref _successConnectionCount);

		private long _bytesUp;

		public long BytesUp
		{
			get => _bytesUp;
			set => _bytesUp = value;
		}

		private long _bytesDown;

		public long BytesDown
		{
			get { return _bytesDown; }
			set => _bytesDown = value;
		}

		public void IncreaseUpDataTransfer(long up) => Interlocked.Add(ref _bytesUp, up);

		public void IncreaseDownDataTransfer(long down) => Interlocked.Add(ref _bytesDown, down);
	}
}
