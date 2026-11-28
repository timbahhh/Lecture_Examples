using LogHub.Sources;

namespace LogHub.Processing;

public class LogHubApplication
{
    private readonly W3CLogFileSource _source;
    private readonly LogProcessor _processor;

    public LogHubApplication(
        W3CLogFileSource source,
        LogProcessor processor)
    {
        _source = source;
        _processor = processor;
    }

    public void Run(string filePath)
    {
        foreach (var line in _source.ReadLines(filePath))
        {
            _processor.ProcessLine(line);
        }

        Console.WriteLine();
        Console.WriteLine($"Записано: {_processor.Written}");
        Console.WriteLine($"Пропущено: {_processor.Skipped}");
    }
}
