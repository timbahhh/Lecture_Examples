using LogHub.Filtering;
using LogHub.Formatting;
using LogHub.Parsing;
using LogHub.Processing;
using LogHub.Sinks;
using LogHub.Sources;

if (args.Length == 0)
{
    Console.Error.WriteLine("Укажите путь к файлу W3C-лога.");
    return;
}

var source = new W3CLogFileSource();
var parser = new W3CLogParser();
var filter = new LogFilter();
var formatter = new LogFormatter();

using var sqliteSink = new SqliteLogSink("logs.db");

var sinks = new ILogSink[]
{
    new ConsoleLogSink(formatter),
    sqliteSink
};

var processor = new LogProcessor(parser, filter, sinks);

foreach (var line in source.ReadLines(args[0]))
{
    processor.ProcessLine(line);
}
Console.WriteLine();
Console.WriteLine($"Записано: {processor.Written}");
Console.WriteLine($"Пропущено: {processor.Skipped}");