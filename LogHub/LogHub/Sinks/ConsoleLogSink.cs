using LogHub.Formatting;
using LogHub.Models;

namespace LogHub.Sinks;

public class ConsoleLogSink : ILogSink
{
    private readonly LogFormatter _formatter;

    public ConsoleLogSink(LogFormatter formatter)
    {
        _formatter = formatter;
    }

    public void Write(LogRecord record)
    {
        Console.WriteLine(_formatter.ToPlainText(record));
    }
}