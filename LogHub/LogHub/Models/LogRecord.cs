namespace LogHub.Models;

public class LogRecord
{
    public Dictionary<string, string> Fields { get; }

    public LogRecord(Dictionary<string, string> fields)
    {
        Fields = new Dictionary<string, string>(
            fields,
            StringComparer.OrdinalIgnoreCase);
    }

    public string? GetValue(string fieldName)
    {
        return Fields.TryGetValue(fieldName, out var value)
            ? value
            : null;
    }
}
