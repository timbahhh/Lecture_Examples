namespace LogHub.Sources;

public class CsvLogFileSource
{
    private readonly string _filePath;

    public CsvLogFileSource(string filePath)
    {
        _filePath = filePath;
    }

    public IEnumerable<string> ReadLines()
    {
        return File.ReadLines(_filePath);
    }
}