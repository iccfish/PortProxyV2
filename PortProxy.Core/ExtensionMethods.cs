namespace PortProxy
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Statistics;

	public static class ExtensionMethods
	{
		public static StatisticsItem Sum(this IEnumerable<StatisticsItem> items, DateTime time)
		{
			var item = new StatisticsItem(time)
			{
				BytesUp = items.Sum(s => s.BytesUp),
				BytesDown = items.Sum(s => s.BytesDown),
				ConnectionCount = items.Sum(s => s.ConnectionCount),
				SuccessConnectionCount = items.Sum(s => s.SuccessConnectionCount),
				FailedConnectionCount = items.Sum(s => s.FailedConnectionCount)
			};

			return item;
		}
	}
}
