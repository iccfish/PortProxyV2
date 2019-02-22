namespace PortProxy.ProxyServer
{
	public interface IEnv
	{
		string ProgramRoot { get; }
		string DataRoot { get; }
		string ConfigRoot { get; }

		/// <summary>
		/// 加载数据
		/// </summary>
		/// <param name="path"></param>
		/// <param name="instance"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T LoadConfig<T>(string path, T instance = null)
			where T : class;

		/// <summary>
		/// 保存数据
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="path"></param>
		/// <typeparam name="T"></typeparam>
		void SaveConfig<T>(T instance, string path);

		/// <summary>
		/// 加载数据
		/// </summary>
		/// <param name="path"></param>
		/// <param name="instance"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T LoadData<T>(string path, T instance = null)
			where T : class;

		/// <summary>
		/// 保存数据
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="path"></param>
		/// <typeparam name="T"></typeparam>
		void SaveData<T>(T instance, string path);
	}
}