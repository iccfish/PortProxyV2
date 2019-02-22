namespace PortProxy.Connection
{
	using System;

	public class ConnectionFactory : IConnectionFactory
	{
		private readonly IServiceProvider _serviceProvider;

		public ConnectionFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public IConnection GetConnection(ConnectionContext context)
		{
			return new Connection(_serviceProvider, context);
		}
	}
}