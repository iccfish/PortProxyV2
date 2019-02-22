namespace PortProxy.Connection
{
	using System.Net.Sockets;
	using System.Threading.Tasks;

	public interface IStreamValidator
	{
		/// <summary>
		/// 校验指定端口上的校验数据是否正确
		/// </summary>
		/// <returns></returns>
		Task<bool> Validate(ConnectionContext context, NetworkStream stream);

		/// <summary>
		/// 根据指定的端口生成校验数据
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		byte[] GenerateValidationData(int port);
	}
}
