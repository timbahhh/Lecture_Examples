using LogHub.Models;
using Microsoft.Data.Sqlite;

namespace LogHub.Sinks;

public class SqliteLogSink : ILogSink, IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteLogSink(string databasePath)
    {
        _connection = new SqliteConnection(
            $"Data Source={databasePath}");

        _connection.Open();

        using var command = _connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Logs (
                RecordId INTEGER PRIMARY KEY AUTOINCREMENT
            );
            """;
        command.ExecuteNonQuery();
    }

    public void Write(LogRecord record)
    {
        EnsureColumnsExist(record);

        var columns = record.Fields.Keys.ToList();

        if (columns.Count == 0)
            return;

        var columnNames = string.Join(", ",
            columns.Select(QuoteIdentifier));

        var parameterNames = string.Join(", ",
            columns.Select((_, index) => $"@p{index}"));

        using var command = _connection.CreateCommand();

        command.CommandText =
            $"INSERT INTO Logs ({columnNames}) VALUES ({parameterNames})";

        for (int i = 0; i < columns.Count; i++)
        {
            command.Parameters.AddWithValue(
                $"@p{i}",
                record.Fields[columns[i]]);
        }

        command.ExecuteNonQuery();
    }

    private void EnsureColumnsExist(LogRecord record)
    {
        var existingColumns = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        using (var command = _connection.CreateCommand())
        {
            command.CommandText = "PRAGMA table_info(Logs);";

            using var reader = command.ExecuteReader();

            while (reader.Read())
                existingColumns.Add(reader.GetString(1));
        }

        foreach (var field in record.Fields.Keys)
        {
            if (existingColumns.Contains(field))
                continue;

            using var command = _connection.CreateCommand();

            command.CommandText =
                $"ALTER TABLE Logs ADD COLUMN {QuoteIdentifier(field)} TEXT;";

            command.ExecuteNonQuery();

            existingColumns.Add(field);
        }
    }

    private static string QuoteIdentifier(string identifier)
    {
        return "\"" + identifier.Replace("\"", "\"\"") + "\"";
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}