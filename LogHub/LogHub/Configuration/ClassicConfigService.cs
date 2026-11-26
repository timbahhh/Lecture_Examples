using System.Collections.Generic;

namespace LogHub.Configuration;

public class ClassicConfigService : IConfigService
{
    private static readonly object LockObject = new();
    private static ClassicConfigService? _instance;

    private readonly Dictionary<string, string> _values = new();

    private ClassicConfigService()
    {
    }

    public static ClassicConfigService Instance()
    {
        lock (LockObject)
        {
            return _instance ??= new ClassicConfigService();
        }
    }

    public string? GetValue(string key)
    {
        return _values.TryGetValue(key, out var value) ? value : null;
    }

    public void SetValue(string key, string value)
    {
        _values[key] = value;
    }
}