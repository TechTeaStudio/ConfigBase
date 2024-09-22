using TechTeaStudio.Config;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class YamlConfigSerializer<T> : IConfigSerializer<T> where T : class, new()
{
	private readonly IDeserializer _deserializer = new DeserializerBuilder()
		.WithNamingConvention(CamelCaseNamingConvention.Instance)
		.Build();

	private readonly ISerializer _serializer = new SerializerBuilder()
		.WithNamingConvention(CamelCaseNamingConvention.Instance)
		.Build();

	public T Deserialize(string content)
	{
		return _deserializer.Deserialize<T>(content) ?? new T();
	}

	public string Serialize(T config)
	{
		return _serializer.Serialize(config);
	}
}
