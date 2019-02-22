namespace PortProxy.Connection
{
	using System.IO;

	interface IEncodeStreamFactory
	{
		/// <summary>
		/// 生成加密流
		/// </summary>
		/// <param name="baseStream"></param>
		/// <returns></returns>
		Stream CreateStream(Stream baseStream);
	}
}
