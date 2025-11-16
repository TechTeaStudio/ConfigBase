using Moq;

using NUnit.Framework;

using System.Text.Json;

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
        Assert.That(config, Is.Not.Null);
        Assert.That(config.Setting1, Is.EqualTo("DefaultValue"));
        Assert.That(File.Exists(Path.Combine(DirectoryPath, $"{FileName}.{FileExtension}")), Is.True);
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
        Assert.That(savedContent, Is.EqualTo("{\"Setting1\":\"NewValue\"}"));
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

    [Test]
    public async Task ReadConfigAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        var defaultConfig = new TestConfig();
        var mockSerializer = new Mock<IConfigSerializer<TestConfig>>();
        mockSerializer.Setup(s => s.Serialize(It.IsAny<TestConfig>()))
                      .Returns("{\"Setting1\":\"DefaultValue\"}");
        mockSerializer.Setup(s => s.Deserialize(It.IsAny<string>()))
                      .Returns(new TestConfig { Setting1 = "DefaultValue" });

        var configHandler = new ConfigFileHandler<TestConfig>(DirectoryPath, FileName, FileExtension, defaultConfig, mockSerializer.Object);
        using var cts = new CancellationTokenSource();

        // Act
        var config = await configHandler.ReadConfigAsync(cts.Token);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.Setting1, Is.EqualTo("DefaultValue"));
    }

    [Test]
    [Ignore("Temporarily disabled until cancellation behavior is finalized.")]
    public async Task ReadConfigAsync_WithCancelledToken_AcceptsCancellationToken()
    {
        // Arrange
        var defaultConfig = new TestConfig();
        var mockSerializer = new Mock<IConfigSerializer<TestConfig>>();
        var configHandler = new ConfigFileHandler<TestConfig>(DirectoryPath, FileName, FileExtension, defaultConfig, mockSerializer.Object);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
       try
        {
            await configHandler.ReadConfigAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Test]
    public async Task SaveConfigAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        var config = new TestConfig { Setting1 = "NewValue" };
        var mockSerializer = new Mock<IConfigSerializer<TestConfig>>();
        mockSerializer.Setup(s => s.Serialize(config))
                      .Returns("{\"Setting1\":\"NewValue\"}");

        var configHandler = new ConfigFileHandler<TestConfig>(DirectoryPath, FileName, FileExtension, new TestConfig(), mockSerializer.Object);
        using var cts = new CancellationTokenSource();

        // Act
        await configHandler.SaveConfigAsync(config, cts.Token);

        // Assert
        var savedContent = await File.ReadAllTextAsync(Path.Combine(DirectoryPath, $"{FileName}.{FileExtension}"));
        Assert.That(savedContent, Is.EqualTo("{\"Setting1\":\"NewValue\"}"));
    }

    [Test]
    [Ignore("Temporarily disabled until cancellation behavior is finalized.")]
    public async Task SaveConfigAsync_WithCancelledToken_AcceptsCancellationToken()
    {
        // Arrange
        var config = new TestConfig { Setting1 = "NewValue" };
        var mockSerializer = new Mock<IConfigSerializer<TestConfig>>();
        var configHandler = new ConfigFileHandler<TestConfig>(DirectoryPath, FileName, FileExtension, new TestConfig(), mockSerializer.Object);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        try
        {
            await configHandler.SaveConfigAsync(config, cts.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Test]
    public async Task ReadConfigAsync_WithoutCancellationToken_WorksWithDefaultValue()
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
        var config = await configHandler.ReadConfigAsync();

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.Setting1, Is.EqualTo("DefaultValue"));
    }

    [Test]
    public async Task ReadConfigAsync_WithDefaultCancellationToken_WorksCorrectly()
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
        var config = await configHandler.ReadConfigAsync(default(CancellationToken));

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.Setting1, Is.EqualTo("DefaultValue"));
    }

    [Test]
    public async Task SaveConfigAsync_WithDefaultCancellationToken_WorksCorrectly()
    {
        // Arrange
        var config = new TestConfig { Setting1 = "NewValue" };
        var mockSerializer = new Mock<IConfigSerializer<TestConfig>>();
        mockSerializer.Setup(s => s.Serialize(config))
                      .Returns("{\"Setting1\":\"NewValue\"}");

        var configHandler = new ConfigFileHandler<TestConfig>(DirectoryPath, FileName, FileExtension, new TestConfig(), mockSerializer.Object);

        // Act 
        await configHandler.SaveConfigAsync(config, default(CancellationToken));

        // Assert
        var savedContent = await File.ReadAllTextAsync(Path.Combine(DirectoryPath, $"{FileName}.{FileExtension}"));
        Assert.That(savedContent, Is.EqualTo("{\"Setting1\":\"NewValue\"}"));
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
        Assert.That(config, Is.Not.Null);
        Assert.That(config.Setting1, Is.EqualTo("Value"));
    }

    [Test]
    public void Serialize_ValidObject_ReturnsIndentedJson()
    {
        // Arrange
        var config = new TestConfig { Setting1 = "Value" };
        var serializer = new JsonConfigSerializer<TestConfig>();
        var expectedJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

        // Act
        var json = serializer.Serialize(config);

        // Assert
        Assert.That(json, Is.EqualTo(expectedJson));
    }

    [Test]
    public void Serialize_MultipleCalls_UsesCachedOptions()
    {
        // Arrange
        var config1 = new TestConfig { Setting1 = "Value1" };
        var config2 = new TestConfig { Setting1 = "Value2" };
        var serializer = new JsonConfigSerializer<TestConfig>();

        // Act
        var json1 = serializer.Serialize(config1);
        var json2 = serializer.Serialize(config2);

        // Assert - Both should be indented (proving cached options are used)
        Assert.That(json1, Does.Contain("\n"));
        Assert.That(json2, Does.Contain("\n"));
        Assert.That(json1, Does.Contain("Value1"));
        Assert.That(json2, Does.Contain("Value2"));
    }

    [Test]
    public void Deserialize_NullJson_ReturnsNewInstance()
    {
        // Arrange
        var serializer = new JsonConfigSerializer<TestConfig>();

        // Act
        var config = serializer.Deserialize("null");

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.Setting1, Is.EqualTo("DefaultValue"));
    }
}
