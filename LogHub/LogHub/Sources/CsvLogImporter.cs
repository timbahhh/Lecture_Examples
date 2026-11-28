using LogHub.Models;

namespace LogHub.Sources;

public class CsvLogImporter : LogImporterBase<LogRecord>, ILogImporter
{
    private readonly CsvLogFileSource _source;
    private readonly Func<string, LogRecord?> _parse;

    public CsvLogImporter(
        CsvLogFileSource source,
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
    
    protected override bool Validate(string line)
    {   
        return !string.IsNullOrWhiteSpace(line)
            && line.Contains(',');
    }
}