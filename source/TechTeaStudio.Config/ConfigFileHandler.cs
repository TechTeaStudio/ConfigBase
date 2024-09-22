using TechTeaStudio.Config;

public class ConfigFileHandler<T> where T : class, new()
{
	private readonly string _filePath;
	private readonly T _defaultConfig;
	private readonly IConfigSerializer<T> _serializer;

	public ConfigFileHandler(string directoryPath, string fileName, string fileExtension, T defaultConfig, IConfigSerializer<T> serializer)
	{
		_filePath = Path.Combine(directoryPath, $"{fileName}.{fileExtension}");
		_defaultConfig = defaultConfig;
		_serializer = serializer;

		EnsureConfigFileExists();
	}

	public T ReadConfig()
	{
		try
		{
			if (!File.Exists(_filePath))
			{
				CreateConfigFile(_defaultConfig);
			}

			var configContent = File.ReadAllText(_filePath);
			return _serializer.Deserialize(configContent);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error reading config file at {_filePath}", ex);
		}
	}

	public void SaveConfig(T config)
	{
		try
		{
			var configContent = _serializer.Serialize(config);
			File.WriteAllText(_filePath, configContent);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error writing config file at {_filePath}", ex);
		}
	}

	private void EnsureConfigFileExists()
	{
		try
		{
			if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
			}

			if (!File.Exists(_filePath))
			{
				CreateConfigFile(_defaultConfig);
			}
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error ensuring config file exists at {_filePath}", ex);
		}
	}

	private void CreateConfigFile(T defaultConfig)
	{
		try
		{
			var defaultConfigContent = _serializer.Serialize(defaultConfig);
			File.WriteAllText(_filePath, defaultConfigContent); 
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error creating config file at {_filePath}", ex);
		}
	}
}
