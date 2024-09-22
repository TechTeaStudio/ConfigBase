using System.Xml.Serialization;
using System.IO;
using TechTeaStudio.Config;

public class XmlConfigSerializer<T> : IConfigSerializer<T> where T : class, new()
{
	public T Deserialize(string content)
	{
		var serializer = new XmlSerializer(typeof(T));
		using var reader = new StringReader(content);
		return (T)serializer.Deserialize(reader) ?? new T();
	}

	public string Serialize(T config)
	{
		var serializer = new XmlSerializer(typeof(T));
		using var writer = new StringWriter();
		serializer.Serialize(writer, config);
		return writer.ToString();
	}
}
