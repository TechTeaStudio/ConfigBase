using System.Text.Json;

namespace TechTeaStudio.Config;

public class JsonConfigSerializer<T> : IConfigSerializer<T> where T : class, new()
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public T Deserialize(string content)
    {
        return JsonSerializer.Deserialize<T>(content, _options) ?? new T();
    }

    public string Serialize(T config)
    {
        return JsonSerializer.Serialize(config, _options);
    }
}
