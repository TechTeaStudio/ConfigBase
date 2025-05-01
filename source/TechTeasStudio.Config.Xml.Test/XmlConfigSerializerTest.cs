namespace TechTeasStudio.Config.Xml.Test;

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

        Assert.That(xml, Does.Contain("<TestConfig"), "Root should exict");
        Assert.That(xml, Does.Contain("<Name>Demo</Name>"), "Name should serealize");
        Assert.That(xml, Does.Contain("<Value>123</Value>"), "Value should serealize");
    }

    [Test]
    public void Deserialize_OfPreviouslySerializedObject_RestoresAllValues()
    {
        var original = new TestConfig { Name = "XYZ", Value = 999 };
        string xml = _serializer.Serialize(original);

        var restored = _serializer.SafeDeserialize(xml);

        Assert.That(restored, Is.Not.Null, "Must be not null");
        Assert.That(restored.Name, Is.EqualTo(original.Name), "Name should == oldName");
        Assert.That(restored.Value, Is.EqualTo(original.Value), "Value should == oldValue");
    }

    [Test]
    public void Deserialize_InvalidXml_ThrowsInvalidOperationException()
    {
        string badXml = "<NotAValid><UnclosedTag>";

        Assert.That(
            () => _serializer.Deserialize(badXml),
            Throws.InvalidOperationException,
            "InCorrect XML should throw InvalidOperationException"
        );
    }

    [Test]
    public void Deserialize_OfEmptyContent_ThrowsInvalidOperationException()
    {
        string empty = string.Empty;

        Assert.That(
            () => _serializer.Deserialize(empty),
            Throws.InvalidOperationException,
            "Empty string should be InvalidOperationException"
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

        Assert.That(result, Is.Not.Null, "Должен вернуться не-null объект");
        Assert.That(result.Name, Is.Null, "Свойство Name по умолчанию должно быть null");
        Assert.That(result.Value, Is.EqualTo(0), "Свойство Value по умолчанию должно быть 0");
    }
}
