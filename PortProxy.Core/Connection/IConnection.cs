namespace PortProxy.Connection
{
	using System;
	using System.Threading.Tasks;

	public interface IConnection : IDisposable
	{
		Task ProcessClientAsync();
	}
}
