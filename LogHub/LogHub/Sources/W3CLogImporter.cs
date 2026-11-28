using LogHub.Models;
using LogHub.Parsing;

namespace LogHub.Sources;

public class W3CLogImporter : LogImporterBase<LogRecord>, ILogImporter
{
    private readonly W3CLogFileSource _source;
    private readonly W3CLogParser _parser;
    private readonly string _filePath;

    public W3CLogImporter(
        W3CLogFileSource source,
        W3CLogParser parser,
        string filePath)
    {
        _source = source;
        _parser = parser;
        _filePath = filePath;
    }

    protected override IEnumerable<string> Read()
    {
        return _source.ReadLines(_filePath);
    }

    protected override LogRecord? Parse(string line)
    {
        if (_parser.ProcessDirective(line))
            return null;

        return _parser.Parse(line);
    }
}
