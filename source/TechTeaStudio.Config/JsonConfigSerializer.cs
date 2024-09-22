using System.Text.Json;

namespace TechTeaStudio.Config
{
	public class JsonConfigSerializer<T> : IConfigSerializer<T> where T : class, new()
	{
		public T Deserialize(string content)
		{
			return JsonSerializer.Deserialize<T>(content) ?? new T();
		}

		public string Serialize(T config)
		{
			return JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
		}
	}
}
