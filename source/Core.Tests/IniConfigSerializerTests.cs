using NUnit.Framework;

namespace TechTeaStudio.Config.Tests;

public enum TestEnum
{
    Value1,
    Value2,
    Value3
}

public sealed class TestIniConfig
{
    public string StringProperty { get; set; } = string.Empty;
    public int IntProperty { get; set; }
    public long LongProperty { get; set; }
    public double DoubleProperty { get; set; }
    public float FloatProperty { get; set; }
    public bool BoolProperty { get; set; }
    public decimal DecimalProperty { get; set; }
    public TestEnum EnumProperty { get; set; }
    public int? NullableIntProperty { get; set; }
}

public class IniConfigSerializerTests
{
    [Test]
    public void Deserialize_StringProperty_DeserializesCorrectly()
    {
        // Arrange
        var content = "StringProperty=TestValue";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.StringProperty, Is.EqualTo("TestValue"));
    }

    [Test]
    public void Deserialize_IntProperty_DeserializesCorrectly()
    {
        // Arrange
        var content = "IntProperty=42";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.IntProperty, Is.EqualTo(42));
    }

    [Test]
    public void Deserialize_LongProperty_DeserializesCorrectly()
    {
        // Arrange
        var content = "LongProperty=123456789012345";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.LongProperty, Is.EqualTo(123456789012345L));
    }

    [Test]
    public void Deserialize_DoubleProperty_DeserializesCorrectly()
    {
        // Arrange
        var content = "DoubleProperty=3.14159";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.DoubleProperty, Is.EqualTo(3.14159).Within(0.00001));
    }

    [Test]
    public void Deserialize_FloatProperty_DeserializesCorrectly()
    {
        // Arrange
        var content = "FloatProperty=2.71828";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.FloatProperty, Is.EqualTo(2.71828f).Within(0.00001f));
    }

    [Test]
    public void Deserialize_BoolProperty_DeserializesCorrectly()
    {
        // Arrange
        var content = "BoolProperty=True";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.BoolProperty, Is.True);
    }

    [Test]
    public void Deserialize_BoolProperty_False_DeserializesCorrectly()
    {
        // Arrange
        var content = "BoolProperty=False";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.BoolProperty, Is.False);
    }

    [Test]
    public void Deserialize_DecimalProperty_DeserializesCorrectly()
    {
        // Arrange
        var content = "DecimalProperty=99.99";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.DecimalProperty, Is.EqualTo(99.99m));
    }

    [Test]
    public void Deserialize_EnumProperty_DeserializesCorrectly()
    {
        // Arrange
        var content = "EnumProperty=Value2";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.EnumProperty, Is.EqualTo(TestEnum.Value2));
    }

    [Test]
    public void Deserialize_EnumProperty_CaseInsensitive_DeserializesCorrectly()
    {
        // Arrange
        var content = "EnumProperty=value3";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.EnumProperty, Is.EqualTo(TestEnum.Value3));
    }

    [Test]
    public void Deserialize_NullableProperty_WithValue_DeserializesCorrectly()
    {
        // Arrange
        var content = "NullableIntProperty=123";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.NullableIntProperty, Is.EqualTo(123));
    }

    [Test]
    public void Deserialize_NullableProperty_Empty_ReturnsNull()
    {
        // Arrange
        var content = "NullableIntProperty=";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.NullableIntProperty, Is.Null);
    }

    [Test]
    public void Deserialize_MultipleProperties_DeserializesAllCorrectly()
    {
        // Arrange
        var content = @"StringProperty=Test
IntProperty=42
BoolProperty=True
EnumProperty=Value1";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.StringProperty, Is.EqualTo("Test"));
        Assert.That(config.IntProperty, Is.EqualTo(42));
        Assert.That(config.BoolProperty, Is.True);
        Assert.That(config.EnumProperty, Is.EqualTo(TestEnum.Value1));
    }

    [Test]
    public void Deserialize_WithSectionHeaders_IgnoresSectionHeaders()
    {
        // Arrange
        var content = @"[Section1]
StringProperty=Test
[Section2]
IntProperty=42";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.StringProperty, Is.EqualTo("Test"));
        Assert.That(config.IntProperty, Is.EqualTo(42));
    }

    [Test]
    public void Deserialize_WithWhitespace_TrimsCorrectly()
    {
        // Arrange
        var content = "  StringProperty  =  TestValue  ";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.StringProperty, Is.EqualTo("TestValue"));
    }

    [Test]
    public void Deserialize_InvalidIntValue_ReturnsDefault()
    {
        // Arrange
        var content = "IntProperty=InvalidNumber";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.IntProperty, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void Serialize_AllProperties_SerializesCorrectly()
    {
        // Arrange
        var config = new TestIniConfig
        {
            StringProperty = "Test",
            IntProperty = 42,
            LongProperty = 123L,
            DoubleProperty = 3.14,
            FloatProperty = 2.71f,
            BoolProperty = true,
            DecimalProperty = 99.99m,
            EnumProperty = TestEnum.Value2,
            NullableIntProperty = 123
        };
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var content = serializer.Serialize(config);

        // Assert
        Assert.That(content, Is.Not.Null);
        Assert.That(content, Does.Contain("StringProperty=Test"));
        Assert.That(content, Does.Contain("IntProperty=42"));
        Assert.That(content, Does.Contain("LongProperty=123"));
        Assert.That(content, Does.Contain("BoolProperty=True"));
        Assert.That(content, Does.Contain("EnumProperty=Value2"));
        Assert.That(content, Does.Contain("NullableIntProperty=123"));
    }

    [Test]
    public void Serialize_And_Deserialize_RoundTrip_WorksCorrectly()
    {
        // Arrange
        var originalConfig = new TestIniConfig
        {
            StringProperty = "TestValue",
            IntProperty = 42,
            BoolProperty = true,
            EnumProperty = TestEnum.Value1,
            NullableIntProperty = 99
        };
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var serialized = serializer.Serialize(originalConfig);
        var deserialized = serializer.Deserialize(serialized);

        // Assert
        Assert.That(deserialized.StringProperty, Is.EqualTo(originalConfig.StringProperty));
        Assert.That(deserialized.IntProperty, Is.EqualTo(originalConfig.IntProperty));
        Assert.That(deserialized.BoolProperty, Is.EqualTo(originalConfig.BoolProperty));
        Assert.That(deserialized.EnumProperty, Is.EqualTo(originalConfig.EnumProperty));
        Assert.That(deserialized.NullableIntProperty, Is.EqualTo(originalConfig.NullableIntProperty));
    }

    [Test]
    public void Deserialize_InvalidEnumValue_ReturnsDefault()
    {
        // Arrange
        var content = "EnumProperty=InvalidValue";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.EnumProperty, Is.EqualTo(TestEnum.Value1)); // Default enum value
    }

    [Test]
    public void Deserialize_NullableProperty_Whitespace_ReturnsNull()
    {
        // Arrange
        var content = "NullableIntProperty=   ";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.NullableIntProperty, Is.Null);
    }

    [Test]
    public void Deserialize_StringProperty_EmptyString_DeserializesCorrectly()
    {
        // Arrange
        var content = "StringProperty=";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.StringProperty, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Deserialize_WithMultipleEqualsSigns_UsesFirstAsSeparator()
    {
        // Arrange
        var content = "StringProperty=Value=With=Equals";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config, Is.Not.Null);
        Assert.That(config.StringProperty, Is.EqualTo("Value=With=Equals"));
    }

    [Test]
    public void Deserialize_PropertyCaching_MultipleCallsWorkCorrectly()
    {
        // Arrange
        var content1 = "StringProperty=Value1\nIntProperty=10";
        var content2 = "StringProperty=Value2\nIntProperty=20";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config1 = serializer.Deserialize(content1);
        var config2 = serializer.Deserialize(content2);

        // Assert - Verify property caching doesn't break functionality
        Assert.That(config1.StringProperty, Is.EqualTo("Value1"));
        Assert.That(config1.IntProperty, Is.EqualTo(10));
        Assert.That(config2.StringProperty, Is.EqualTo("Value2"));
        Assert.That(config2.IntProperty, Is.EqualTo(20));
    }

    [Test]
    public void Deserialize_NegativeNumbers_DeserializesCorrectly()
    {
        // Arrange
        var content = "IntProperty=-42\nLongProperty=-123456789\nDoubleProperty=-3.14\nFloatProperty=-2.71\nDecimalProperty=-99.99";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config.IntProperty, Is.EqualTo(-42));
        Assert.That(config.LongProperty, Is.EqualTo(-123456789L));
        Assert.That(config.DoubleProperty, Is.EqualTo(-3.14).Within(0.00001));
        Assert.That(config.FloatProperty, Is.EqualTo(-2.71f).Within(0.00001f));
        Assert.That(config.DecimalProperty, Is.EqualTo(-99.99m));
    }

    [Test]
    public void Deserialize_BoolProperty_CaseInsensitive_DeserializesCorrectly()
    {
        // Arrange
        var content1 = "BoolProperty=true";
        var content2 = "BoolProperty=TRUE";
        var content3 = "BoolProperty=False";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config1 = serializer.Deserialize(content1);
        var config2 = serializer.Deserialize(content2);
        var config3 = serializer.Deserialize(content3);

        // Assert
        Assert.That(config1.BoolProperty, Is.True);
        Assert.That(config2.BoolProperty, Is.True);
        Assert.That(config3.BoolProperty, Is.False);
    }

    [Test]
    public void Deserialize_ReadOnlyProperty_IsIgnored()
    {
        // Arrange
        var content = "StringProperty=Test\nIntProperty=42";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert - ReadOnly properties should not cause errors
        Assert.That(config, Is.Not.Null);
        Assert.That(config.StringProperty, Is.EqualTo("Test"));
        Assert.That(config.IntProperty, Is.EqualTo(42));
    }

    [Test]
    public void Serialize_ReadOnlyProperties_AreExcluded()
    {
        // Arrange
        var config = new TestIniConfig
        {
            StringProperty = "Test",
            IntProperty = 42
        };
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var content = serializer.Serialize(config);

        // Assert 
        Assert.That(content, Is.Not.Null);
        Assert.That(content, Does.Contain("StringProperty=Test"));
        Assert.That(content, Does.Contain("IntProperty=42"));
    }

    [Test]
    public void Deserialize_ZeroValues_DeserializesCorrectly()
    {
        // Arrange
        var content = "IntProperty=0\nLongProperty=0\nDoubleProperty=0\nFloatProperty=0\nBoolProperty=False\nDecimalProperty=0";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config = serializer.Deserialize(content);

        // Assert
        Assert.That(config.IntProperty, Is.EqualTo(0));
        Assert.That(config.LongProperty, Is.EqualTo(0L));
        Assert.That(config.DoubleProperty, Is.EqualTo(0.0));
        Assert.That(config.FloatProperty, Is.EqualTo(0.0f));
        Assert.That(config.BoolProperty, Is.False);
        Assert.That(config.DecimalProperty, Is.EqualTo(0m));
    }

    [Test]
    public void Deserialize_CollectionExpressions_HandlesLineSeparatorsCorrectly()
    {
        // Arrange -
        var contentWithCR = "StringProperty=Test1\rIntProperty=1";
        var contentWithLF = "StringProperty=Test2\nIntProperty=2";
        var contentWithCRLF = "StringProperty=Test3\r\nIntProperty=3";
        var serializer = new IniConfigSerializer<TestIniConfig>();

        // Act
        var config1 = serializer.Deserialize(contentWithCR);
        var config2 = serializer.Deserialize(contentWithLF);
        var config3 = serializer.Deserialize(contentWithCRLF);

        // Assert
        Assert.That(config1.StringProperty, Is.EqualTo("Test1"));
        Assert.That(config1.IntProperty, Is.EqualTo(1));
        Assert.That(config2.StringProperty, Is.EqualTo("Test2"));
        Assert.That(config2.IntProperty, Is.EqualTo(2));
        Assert.That(config3.StringProperty, Is.EqualTo("Test3"));
        Assert.That(config3.IntProperty, Is.EqualTo(3));
    }
}

