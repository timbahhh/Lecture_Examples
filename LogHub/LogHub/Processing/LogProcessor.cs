using LogHub.Filtering;
using LogHub.Sinks;
using LogHub.Sources;

namespace LogHub.Processing;

public class LogProcessor
{
    private readonly ILogImporter _importer;
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

    public void Process()
    {
        foreach (var record in _importer.Import())
        {
            if (!_filter.IsAllowed(record))
            {
                Skipped++;
                continue;
            }

            foreach (var sink in _sinks)
            {
                sink.Write(record);
            }

            Written++;
        }
    }
}
