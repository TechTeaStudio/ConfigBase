namespace TechTeaStudio.Config.Xml.Test;

public class TestConfig
{
    public string Name { get; set; }

    public int Value { get; set; }
}

[TestFixture]
public class XmlConfigSerializerTests
{
    private XmlConfigSerializer<TestConfig> _serializer;

    [SetUp]
    public void Setup()
    {
        _serializer = new XmlConfigSerializer<TestConfig>();
    }

    [Test]
    public void Serialize_WithPopulatedObject_ProducesXmlContainingAllProperties()
    {
        var config = new TestConfig { Name = "Demo", Value = 123 };

        string xml = _serializer.Serialize(config);

        Assert.That(xml, Does.Contain("<TestConfig"), "Root element should exist");
        Assert.That(xml, Does.Contain("<Name>Demo</Name>"), "Name should serialize");
        Assert.That(xml, Does.Contain("<Value>123</Value>"), "Value should serialize");
    }

    [Test]
    public void Deserialize_OfPreviouslySerializedObject_RestoresAllValues()
    {
        var original = new TestConfig { Name = "XYZ", Value = 999 };
        string xml = _serializer.Serialize(original);

        var restored = _serializer.SafeDeserialize(xml);

        Assert.That(restored, Is.Not.Null, "Must be not null");
        Assert.That(restored.Name, Is.EqualTo(original.Name), "Name should equal original");
        Assert.That(restored.Value, Is.EqualTo(original.Value), "Value should equal original");
    }

    [Test]
    public void Deserialize_InvalidXml_ThrowsInvalidOperationException()
    {
        string badXml = "<NotAValid><UnclosedTag>";

        Assert.That(
            () => _serializer.Deserialize(badXml),
            Throws.InvalidOperationException,
            "Malformed XML should throw InvalidOperationException"
        );
    }

    [Test]
    public void Deserialize_OfEmptyContent_ThrowsInvalidOperationException()
    {
        string empty = string.Empty;

        Assert.That(
            () => _serializer.Deserialize(empty),
            Throws.InvalidOperationException,
            "Empty string should throw InvalidOperationException"
        );
    }

    [Test]
    public void Deserialize_WhenXmlRepresentsEmptyElement_ReturnsDefaultInstance()
    {
        string xml =
            @"<?xml version=""1.0"" encoding=""utf-16""?>
                  <TestConfig xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                              xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />";

        var result = _serializer.Deserialize(xml);

        Assert.That(result, Is.Not.Null, "Should return a non-null instance");
        Assert.That(result.Name, Is.Null, "Name should be null by default");
        Assert.That(result.Value, Is.EqualTo(0), "Value should be 0 by default");
    }
}
