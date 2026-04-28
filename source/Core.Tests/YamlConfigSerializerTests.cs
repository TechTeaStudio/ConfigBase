namespace TechTeaStudio.Config.Tests;

public sealed class YamlTestConfig
{
    public string Name { get; set; } = "Default";
    public int Port { get; set; } = 8080;
    public bool Enabled { get; set; } = true;
}

public class YamlConfigSerializerTests
{
    private YamlConfigSerializer<YamlTestConfig> _serializer = null!;

    [SetUp]
    public void Setup()
    {
        _serializer = new YamlConfigSerializer<YamlTestConfig>();
    }

    [Test]
    public void Serialize_PopulatedObject_ProducesYamlWithAllProperties()
    {
        var config = new YamlTestConfig { Name = "TestApp", Port = 9090, Enabled = false };

        string yaml = _serializer.Serialize(config);

        Assert.That(yaml, Does.Contain("testApp").Or.Contain("TestApp"));
        Assert.That(yaml, Does.Contain("9090"));
        Assert.That(yaml, Does.Contain("false"));
    }

    [Test]
    public void Deserialize_OfPreviouslySerializedObject_RestoresAllValues()
    {
        var original = new YamlTestConfig { Name = "MyApp", Port = 5000, Enabled = true };
        string yaml = _serializer.Serialize(original);

        var restored = _serializer.Deserialize(yaml);

        Assert.That(restored, Is.Not.Null);
        Assert.That(restored.Name, Is.EqualTo(original.Name));
        Assert.That(restored.Port, Is.EqualTo(original.Port));
        Assert.That(restored.Enabled, Is.EqualTo(original.Enabled));
    }

    [Test]
    public void Deserialize_EmptyYaml_ReturnsNewInstance()
    {
        var result = _serializer.Deserialize("{}");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Default"));
        Assert.That(result.Port, Is.EqualTo(8080));
    }

    [Test]
    public void Serialize_MultipleCalls_ProduceConsistentOutput()
    {
        var config = new YamlTestConfig { Name = "Stable", Port = 3000, Enabled = true };

        string yaml1 = _serializer.Serialize(config);
        string yaml2 = _serializer.Serialize(config);

        Assert.That(yaml1, Is.EqualTo(yaml2));
    }
}
