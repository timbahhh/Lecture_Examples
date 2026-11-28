using LogHub.Filtering;
using LogHub.Parsing;
using LogHub.Sinks;
using LogHub.Sources;
using LogHub.Models;

namespace LogHub.Processing;

public class LogProcessor
{
    private readonly ILogImporter? _importer;
    private readonly W3CLogParser? _parser;
    private readonly LogFilter _filter;
    private readonly IReadOnlyCollection<ILogSink> _sinks;

    public int Written { get; private set; }
    public int Skipped { get; private set; }

    public LogProcessor(
        ILogImporter importer,
        LogFilter filter,
        IEnumerable<ILogSink> sinks)
    {
        _importer = importer;
        _filter = filter;
        _sinks = sinks.ToList();
    }

    public LogProcessor(
        W3CLogParser parser,
        LogFilter filter,
        IEnumerable<ILogSink> sinks)
    {
        _parser = parser;
        _filter = filter;
        _sinks = sinks.ToList();
    }

    public void Process()
    {
        if (_importer is null)
            throw new InvalidOperationException("Импортёр не задан.");

        foreach (var record in _importer.Import())
            ProcessRecord(record);
    }

    public void ProcessLine(string line)
    {
        if (_parser is null)
            throw new InvalidOperationException("W3C-парсер не задан.");

        if (_parser.ProcessDirective(line))
            return;

        var record = _parser.Parse(line);

        if (record is null)
        {
            Skipped++;
            return;
        }

        ProcessRecord(record);
    }

    private void ProcessRecord(LogRecord record)
    {
        if (!_filter.IsAllowed(record))
        {
            Skipped++;
            return;
        }

        foreach (var sink in _sinks)
            sink.Write(record);

        Written++;
    }
}