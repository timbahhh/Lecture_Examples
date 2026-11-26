
using LogHub.Models;

namespace LogHub.Filtering;

public class LogFilter
{
    public bool IsAllowed(LogRecord record)
    {
        var status = record.GetValue("sc-status");
        if (!int.TryParse(status, out var statusCode))
            return true;

        return statusCode >= 500;
    }
}
