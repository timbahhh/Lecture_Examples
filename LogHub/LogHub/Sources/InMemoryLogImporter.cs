using LogHub.Models;

namespace LogHub.Sources;

public class InMemoryLogImporter : LogImporterBase<LogRecord>, ILogImporter
{
    private readonly InMemoryLogSource _source;
    private readonly Func<string, LogRecord?> _parse;

    public InMemoryLogImporter(
        InMemoryLogSource source,
        Func<string, LogRecord?> parse)
    {
        _source = source;
        _parse = parse;
    }

    protected override IEnumerable<string> Read()
    {
        return _source.ReadLines();
    }

    protected override LogRecord? Parse(string line)
    {
        return _parse(line);
    }
}
