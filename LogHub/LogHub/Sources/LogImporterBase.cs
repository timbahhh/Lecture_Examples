namespace LogHub.Sources;
public abstract class LogImporterBase<T> : IDisposable
{
    public IEnumerable<T> Import()
    {
        Open();

        try
        {
            foreach (var line in Read())
            {
                if (!Validate(line))
                    continue;

                var item = Parse(line);

                if (item is not null)
                    yield return item;
            }
        }
        finally
        {
            Close();
            DisposeCore();
        }
    }
    protected virtual void Open() { }

    protected abstract IEnumerable<string> Read();

    protected abstract T? Parse(string line);

    protected virtual bool Validate(string line)
    {
        return !string.IsNullOrWhiteSpace(line);
    }

    protected virtual void Close() { }

    protected virtual void DisposeCore() { }

    public void Dispose()
    {
        DisposeCore();
    }
}
