using LogHub.Models;

namespace LogHub.Parsing;

public class W3CLogParser
{
    private List<string> _fields = [];

    public bool HasFields => _fields.Count > 0;

    public bool ProcessDirective(string line)
    {
        if (!line.StartsWith("#Fields:", StringComparison.OrdinalIgnoreCase))
            return false;

        var fields = line["#Fields:".Length..]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        if (fields.Count == 0)
            return false;

        _fields = fields;
        return true;
    }

    public LogRecord? Parse(string line)
    {
        if (!HasFields || string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
            return null;

        var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (values.Length != _fields.Count)
            return null;

        var data = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < _fields.Count; i++)
        {
            data[_fields[i]] = values[i];
        }

        return new LogRecord(data);
    }
}
