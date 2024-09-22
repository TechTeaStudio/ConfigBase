using TechTeaStudio.Config;

public class IniConfigSerializer<T> : IConfigSerializer<T> where T : class, new()
{
	public T Deserialize(string content)
	{
		var config = new T();
		var properties = typeof(T).GetProperties();
		var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

		foreach (var line in lines)
		{
			if (line.StartsWith("[") && line.EndsWith("]")) continue; 
			var keyValue = line.Split('=');
			if (keyValue.Length == 2)
			{
				var property = properties.FirstOrDefault(p => p.Name == keyValue[0].Trim());
				if (property != null)
				{
					property.SetValue(config, keyValue[1].Trim());
				}
			}
		}

		return config;
	}

	public string Serialize(T config)
	{
		var properties = typeof(T).GetProperties();
		var lines = properties.Select(prop => $"{prop.Name}={prop.GetValue(config)}");
		return string.Join(Environment.NewLine, lines);
	}
}
