using LogHub.Models;

namespace LogHub.Sources;

public interface ILogImporter
{
    IEnumerable<LogRecord> Import();
}