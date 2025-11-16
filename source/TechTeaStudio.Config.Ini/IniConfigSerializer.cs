using System.Globalization;
using System.Reflection;
using TechTeaStudio.Config;

public class IniConfigSerializer<T> : IConfigSerializer<T> where T : class, new()
{
	private static readonly PropertyInfo[] _properties = typeof(T).GetProperties();
	private static readonly char[] LineSeparators = ['\r', '\n'];
	private static readonly char[] KeyValueSeparator = ['='];

	public T Deserialize(string content)
	{
		var config = new T();
		var lines = content.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		foreach (var line in lines)
		{
			if (line.Length > 0 && line[0] == '[' && line[^1] == ']') continue;
			
			var keyValue = line.Split(KeyValueSeparator, 2, StringSplitOptions.TrimEntries);
			if (keyValue.Length == 2)
			{
				var property = Array.Find(_properties, p => p.Name == keyValue[0]);
				if (property != null && property.CanWrite)
				{
					var convertedValue = ConvertValue(keyValue[1], property.PropertyType);
					property.SetValue(config, convertedValue);
				}
			}
		}

		return config;
	}

	public string Serialize(T config)
	{
		var lines = _properties
			.Where(p => p.CanRead)
			.Select(prop => $"{prop.Name}={prop.GetValue(config)}");
		return string.Join(Environment.NewLine, lines);
	}

	private static object? ConvertValue(string value, Type targetType)
	{
		if (targetType == typeof(string))
		{
			return string.IsNullOrWhiteSpace(value) ? string.Empty : value;
		}

		if (string.IsNullOrWhiteSpace(value))
			return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

		if (targetType == typeof(int) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intVal))
			return intVal;

		if (targetType == typeof(long) && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal))
			return longVal;

		if (targetType == typeof(double) && double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleVal))
			return doubleVal;

		if (targetType == typeof(float) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatVal))
			return floatVal;

		if (targetType == typeof(bool) && bool.TryParse(value, out var boolVal))
			return boolVal;

		if (targetType == typeof(decimal) && decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalVal))
			return decimalVal;

		if (targetType.IsEnum && Enum.TryParse(targetType, value, true, out var enumVal))
			return enumVal;

		if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			var underlyingType = Nullable.GetUnderlyingType(targetType);
			if (underlyingType != null)
				return ConvertValue(value, underlyingType);
		}

		try
		{
			return Convert.ChangeType(value, targetType);
		}
		catch
		{
			return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
		}
	}
}
