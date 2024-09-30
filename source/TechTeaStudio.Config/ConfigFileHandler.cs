using System.IO;
using System.Threading.Tasks;

using TechTeaStudio.Config;

public class ConfigFileHandler<T> where T : class, new()
{
	private readonly string _filePath;
	private readonly T _defaultConfig;
	private readonly IConfigSerializer<T> _serializer;

	/// <summary>Initializes the config handler with file details and a default configuration</summary>
	public ConfigFileHandler(string directoryPath, string fileName, string fileExtension, T defaultConfig, IConfigSerializer<T> serializer)
	{
		_filePath = Path.Combine(directoryPath, $"{fileName}.{fileExtension}");
		_defaultConfig = defaultConfig;
		_serializer = serializer;

		EnsureConfigFileExists();
	}

	/// <summary>Reads the configuration from the file</summary>
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

	/// <summary>Saves the configuration to the file</summary>
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

	/// <summary>Ensures that the configuration file exists</summary>
	private void EnsureConfigFileExists()
	{
		try
		{
			var directoryPath = Path.GetDirectoryName(_filePath);
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
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

	/// <summary>Creates a new configuration file with default values</summary>
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

	/// <summary>Reads the configuration from the file asynchronously</summary>
	public async Task<T> ReadConfigAsync()
	{
		try
		{
			if (!File.Exists(_filePath))
			{
				await CreateConfigFileAsync(_defaultConfig);
			}

			var configContent = await File.ReadAllTextAsync(_filePath);
			return _serializer.Deserialize(configContent);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error reading config file at {_filePath}", ex);
		}
	}

	/// <summary>Saves the configuration to the file asynchronously</summary>
	public async Task SaveConfigAsync(T config)
	{
		try
		{
			var configContent = _serializer.Serialize(config);
			await File.WriteAllTextAsync(_filePath, configContent);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error writing config file at {_filePath}", ex);
		}
	}

	/// <summary>Ensures that the configuration file exists asynchronously</summary>
	private async Task EnsureConfigFileExistsAsync()
	{
		try
		{
			var directoryPath = Path.GetDirectoryName(_filePath);
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			if (!File.Exists(_filePath))
			{
				await CreateConfigFileAsync(_defaultConfig);
			}
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error ensuring config file exists at {_filePath}", ex);
		}
	}

	/// <summary>Creates a new configuration file with default values asynchronously</summary>
	private async Task CreateConfigFileAsync(T defaultConfig)
	{
		try
		{
			var defaultConfigContent = _serializer.Serialize(defaultConfig);
			await File.WriteAllTextAsync(_filePath, defaultConfigContent);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error creating config file at {_filePath}", ex);
		}
	}
}
