namespace PortProxy.ProxyServer
{
	using System;
	using System.IO;
	using System.Security.Cryptography;

	using Newtonsoft.Json;

	public class Env : IEnv
	{
		JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Auto
		};

		public string ProgramRoot { get; }

		public string DataRoot { get; }

		public string ConfigRoot { get; }

		public Env()
		{
			ProgramRoot = Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().GetLocation());
			DataRoot = Path.Combine(ProgramRoot, "data");
			ConfigRoot = Path.Combine(ProgramRoot, "config");
		}

		/// <summary>
		/// 加载数据
		/// </summary>
		/// <param name="path"></param>
		/// <param name="instance"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T LoadConfig<T>(string path, T instance = null)
			where T : class => LoadJsonData(Path.Combine(ConfigRoot, path), instance);

		/// <summary>
		/// 保存数据
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="path"></param>
		/// <typeparam name="T"></typeparam>
		public void SaveConfig<T>(T instance, string path) => SaveJsonData(instance, Path.Combine(ConfigRoot, path));

		/// <summary>
		/// 加载数据
		/// </summary>
		/// <param name="path"></param>
		/// <param name="instance"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T LoadData<T>(string path, T instance = null)
			where T : class =>
			LoadJsonData(Path.Combine(DataRoot, path), instance);

		/// <summary>
		/// 保存数据
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="path"></param>
		/// <typeparam name="T"></typeparam>
		public void SaveData<T>(T instance, string path) => SaveJsonData(instance, Path.Combine(DataRoot, path));

		T LoadJsonData<T>(string file, T instance = null)
			where T : class
		{
			if (!File.Exists(file))
				return instance ?? Activator.CreateInstance<T>();

			var content = File.ReadAllText(file);
			if (instance == null)
			{
				return JsonConvert.DeserializeObject<T>(content, _jsonSerializerSettings);
			}

			JsonConvert.PopulateObject(content, instance, _jsonSerializerSettings);
			return instance;
		}

		void SaveJsonData<T>(T instance, string file)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(file));
			File.WriteAllText(file, JsonConvert.SerializeObject(instance, _jsonSerializerSettings));
		}
	}
}
