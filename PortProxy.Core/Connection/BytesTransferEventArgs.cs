namespace PortProxy.Connection
{
	using System;

	public class BytesTransferEventArgs : EventArgs
	{
		public BytesTransferEventArgs(int bytesCount)
		{
			BytesCount = bytesCount;
		}

		public int BytesCount { get; }
	}
}