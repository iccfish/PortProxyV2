namespace PortProxy.Connection
{
	public interface IConnectionFactory
	{
		IConnection GetConnection(ConnectionContext context);
	}
}