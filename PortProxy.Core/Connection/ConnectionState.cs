namespace PortProxy.Connection
{
	public enum ConnectionState
	{
		None = -1,

		/// <summary>
		/// 客户端已连接，这是连接后的初始状态
		/// </summary>
		ClientConnected = 0,

		/// <summary>
		/// 等待客户端发送验证数据，这是开始处理的第一步
		/// </summary>
		WaitForValidation = 1,

		/// <summary>
		/// 客户端已通过验证
		/// </summary>
		ValidationPassed = 2,

		/// <summary>
		/// 客户端没有通过验证
		/// </summary>
		ValidationFailed = 3,

		/// <summary>
		/// 连接上游节点
		/// </summary>
		ConnectUpPeer = 4,

		/// <summary>
		/// 上游节点连接失败
		/// </summary>
		UpPeerConnectFailed = 5,

		/// <summary>
		/// 上游节点已连接
		/// </summary>
		UpPeerConnected = 6,

		/// <summary>
		/// 通道已建立
		/// </summary>
		TunnelEstablished = 7,

		/// <summary>
		/// 客户端断开了连接
		/// </summary>
		ClientDisconnect = 8,

		/// <summary>
		/// 服务器断开了连接
		/// </summary>
		ServerDisconnect = 9,

		/// <summary>
		/// 连接已断开
		/// </summary>
		ConnectionClosed = 10
	}
}
