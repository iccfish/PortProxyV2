namespace PortProxy.Connection
{
	using System.IO;

	class EncodeStreamFactory : IEncodeStreamFactory
	{
		/// <summary>
		/// 生成加密流
		/// </summary>
		/// <param name="baseStream"></param>
		/// <returns></returns>
		public Stream CreateStream(Stream baseStream)
		{
			return new EncodeStream(baseStream);
		}
	}
}