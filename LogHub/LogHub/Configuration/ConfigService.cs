using System.Collections.Generic;

namespace LogHub.Configuration;

public class ConfigService : IConfigService
{
    private readonly Dictionary<string, string> _values = new();

    public string? GetValue(string key)
    {
        return _values.TryGetValue(key, out var value) ? value : null;
    }

    public void SetValue(string key, string value)
    {
        _values[key] = value;
    }
}