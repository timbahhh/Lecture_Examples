using System.Text.Json;
using LogHub.Models;

namespace LogHub.Formatting;

public class LogFormatter
{
    public string ToPlainText(LogRecord record)
    {
        return string.Join(" ",
            record.Fields.Select(field => $"{field.Key}={field.Value}"));
    }

    public string ToJson(LogRecord record)
    {
        return JsonSerializer.Serialize(record.Fields);
    }
}
