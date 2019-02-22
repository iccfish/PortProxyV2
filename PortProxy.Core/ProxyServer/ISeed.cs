namespace PortProxy.ProxyServer
{
	internal interface ISeed
	{
		int RandomSeedBegin { get; }
		int RandomSeedEnd { get; }
		long TimeKey { get; }

		int GetCloseTime();
	}
}