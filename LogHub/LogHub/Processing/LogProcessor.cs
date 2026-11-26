using LogHub.Filtering;
using LogHub.Parsing;
using LogHub.Sinks;

namespace LogHub.Processing;

public class LogProcessor
{
    private readonly W3CLogParser _parser;
    private readonly LogFilter _filter;
    private readonly IReadOnlyCollection<ILogSink> _sinks;

    public int Written { get; private set; }
    public int Skipped { get; private set; }

    public LogProcessor(
        W3CLogParser parser,
        LogFilter filter,
        IEnumerable<ILogSink> sinks)
    {
        _parser = parser;
        _filter = filter;
        _sinks = sinks.ToList();
    }

    public void ProcessLine(string line)
    {
        if (_parser.ProcessDirective(line))
            return;

        var record = _parser.Parse(line);

        if (record is null || !_filter.IsAllowed(record))
        {
            Skipped++;
            return;
        }

        foreach (var sink in _sinks)
        {
            sink.Write(record);
        }

        Written++;
    }
}