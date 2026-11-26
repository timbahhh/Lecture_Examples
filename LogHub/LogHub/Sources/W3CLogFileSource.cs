namespace LogHub.Sources;

public class W3CLogFileSource
{
    public IEnumerable<string> ReadLines(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл логов не найден.", filePath);

        return File.ReadLines(filePath);
    }
}