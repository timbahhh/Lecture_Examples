namespace LogHub.Sources;

public class InMemoryLogSource
{
    private readonly IEnumerable<string> _lines;

    public InMemoryLogSource(IEnumerable<string> lines)
    {
        _lines = lines;
    }

    public IEnumerable<string> ReadLines()
    {
        return _lines;
    }
}