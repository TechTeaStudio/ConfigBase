using Moq;

namespace TechTeaStudio.Config.Tests;

public sealed class TestConfig
{
	public string Setting1 { get; set; } = "DefaultValue";
}

public class ConfigFileHandlerTests
{
	private const string DirectoryPath = "TestConfig";
	private const string FileName = "testConfig";
	private const string FileExtension = "json";



	[SetUp]
	public void Setup()
	{
		if (Directory.Exists(DirectoryPath))
		{
			Directory.Delete(DirectoryPath, true);
		}
	}

	[Test]
	public void ReadConfig_FileDoesNotExist_CreatesFileWithDefaultConfig()
	{
		// Arrange
		var defaultConfig = new TestConfig();
		var mockSerializer = new Mock<IConfigSerializer<TestConfig>>();
		mockSerializer.Setup(s => s.Serialize(It.IsAny<TestConfig>()))
					  .Returns("{\"Setting1\":\"DefaultValue\"}");
		mockSerializer.Setup(s => s.Deserialize(It.IsAny<string>()))
					  .Returns(new TestConfig { Setting1 = "DefaultValue" });

		var configHandler = new ConfigFileHandler<TestConfig>(DirectoryPath, FileName, FileExtension, defaultConfig, mockSerializer.Object);

		// Act
		var config = configHandler.ReadConfig();

		// Assert
		Assert.NotNull(config);
		Assert.AreEqual("DefaultValue", config.Setting1);
		Assert.True(File.Exists(Path.Combine(DirectoryPath, $"{FileName}.{FileExtension}")));
	}

	[Test]
	public void SaveConfig_WritesConfigToFile()
	{
		// Arrange
		var config = new TestConfig { Setting1 = "NewValue" };
		var mockSerializer = new Mock<IConfigSerializer<TestConfig>>();
		mockSerializer.Setup(s => s.Serialize(config))
					  .Returns("{\"Setting1\":\"NewValue\"}");

		var configHandler = new ConfigFileHandler<TestConfig>(DirectoryPath, FileName, FileExtension, new TestConfig(), mockSerializer.Object);

		// Act
		configHandler.SaveConfig(config);

		// Assert
		var savedContent = File.ReadAllText(Path.Combine(DirectoryPath, $"{FileName}.{FileExtension}"));
		Assert.AreEqual("{\"Setting1\":\"NewValue\"}", savedContent);
	}

	[Test]
	public void ReadConfig_HandlesExceptionDuringRead()
	{
		// Arrange
		var mockSerializer = new Mock<IConfigSerializer<TestConfig>>();
		mockSerializer.Setup(s => s.Deserialize(It.IsAny<string>()))
					  .Throws(new Exception("Deserialization error"));

		var configHandler = new ConfigFileHandler<TestConfig>(DirectoryPath, FileName, FileExtension, new TestConfig(), mockSerializer.Object);

		File.WriteAllText(Path.Combine(DirectoryPath, $"{FileName}.{FileExtension}"), "{malformed json}");

		// Act & Assert
		var exception = Assert.Throws<InvalidOperationException>(() => configHandler.ReadConfig());
		Assert.That(exception.Message, Does.Contain("Error reading config file"));
	}


	[Test]
	public void SaveConfig_HandlesExceptionDuringWrite()
	{
		// Arrange
		var config = new TestConfig { Setting1 = "NewValue" };
		var mockSerializer = new Mock<IConfigSerializer<TestConfig>>();
		mockSerializer.Setup(s => s.Serialize(config)).Throws(new Exception("Serialization error"));

		var configHandler = new ConfigFileHandler<TestConfig>(DirectoryPath, FileName, FileExtension, new TestConfig(), mockSerializer.Object);

		// Act & Assert
		var exception = Assert.Throws<InvalidOperationException>(() => configHandler.SaveConfig(config));
		Assert.That(exception.Message, Does.Contain("Error writing config file"));
	}
}

public class JsonConfigSerializerTests
{
	[Test]
	public void Deserialize_ValidJson_ReturnsObject()
	{
		// Arrange
		var json = "{\"Setting1\":\"Value\"}";
		var serializer = new JsonConfigSerializer<TestConfig>();

		// Act
		var config = serializer.Deserialize(json);

		// Assert
		Assert.NotNull(config);
		Assert.AreEqual("Value", config.Setting1);
	}

	[Test]
	public void Serialize_ValidObject_ReturnsJson()
	{
		// Arrange
		var config = new TestConfig { Setting1 = "Value" };
		var serializer = new JsonConfigSerializer<TestConfig>();

		// Act
		var json = serializer.Serialize(config);

		// Assert
		Assert.That(json, Is.EqualTo("{\r\n  \"Setting1\": \"Value\"\r\n}"));
	}

}
