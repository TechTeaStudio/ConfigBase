using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

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

	/// <summary>
	/// Deserializes the XML, but first validates it against a schema generated
	/// from type T.  Collects all validation errors (e.g. missing elements,
	/// invalid data) and throws a single exception listing them.
	/// </summary>
	public T SafeDeserialize(string content)
	{
		var xmlSchemas = new XmlSchemas();
		var importer = new XmlReflectionImporter();
		var mapping = importer.ImportTypeMapping(typeof(T));
		var exporter = new XmlSchemaExporter(xmlSchemas);
		exporter.ExportTypeMapping(mapping);

		var schemaSet = new XmlSchemaSet();
		foreach (XmlSchema schema in xmlSchemas)
		{
			schemaSet.Add(schema);
		}

		var settings = new XmlReaderSettings
		{
			ValidationType = ValidationType.Schema,
			Schemas = schemaSet,
			IgnoreWhitespace = true
		};
		var errors = new List<string>();
		settings.ValidationEventHandler += (sender, args) =>
		{
			var lineInfo = (IXmlLineInfo)sender;
			var location = args.Exception != null
				? $"(Line {args.Exception.LineNumber}, Pos {args.Exception.LinePosition})"
				: "";
			errors.Add($"{location} {args.Message}");
		};

		using (var sr = new StringReader(content))
		using (var validatingReader = XmlReader.Create(sr, settings))
		{
			try
			{
				while (validatingReader.Read()) { }
			}
			catch (XmlException xe)
			{
				errors.Add($"XML well-formedness error: {xe.Message} (Line {xe.LineNumber}, Pos {xe.LinePosition})");
			}
		}

		if (errors.Count > 0)
		{
			var msg = $"XML validation failed for type {typeof(T).Name}:{Environment.NewLine}"
					  + string.Join(Environment.NewLine, errors);
			throw new InvalidOperationException(msg);
		}

		var serializer = new XmlSerializer(typeof(T));
		using var readerForDeserial = new StringReader(content);
		return (T)serializer.Deserialize(readerForDeserial) ?? new T();
	}
}
