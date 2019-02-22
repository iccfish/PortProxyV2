namespace PortProxy.Trace
{
	using System;

	using Connection;

	public class StateItem
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
		public StateItem(DateTime time, ConnectionState? state)
		{
			Time = time;
			State = state;
		}

		public DateTime Time { get; set; }

		public ConnectionState? State { get; set; }
	}
}
