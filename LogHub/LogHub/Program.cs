
using LogHub.Filtering;
using LogHub.Formatting;
using LogHub.Parsing;
using LogHub.Processing;
using LogHub.Sinks;
using LogHub.Sources;
using LogHub.Graphs;
using Microsoft.Extensions.DependencyInjection;

if (args.Length == 0)
{
    Console.Error.WriteLine("Укажите путь к файлу W3C-лога.");
    return;
}

var services = new ServiceCollection();

services.AddSingleton<W3CLogFileSource>();
services.AddSingleton<W3CLogParser>();
services.AddSingleton<LogFilter>();
services.AddSingleton<LogFormatter>();

var sinks = new ILogSink[]
{
    new ConsoleLogSink(formatter),
    sqliteSink
};

var processor = new LogProcessor(importer, filter, sinks);

processor.Process();

Console.WriteLine();
Console.WriteLine($"Записано: {processor.Written}");
Console.WriteLine($"Пропущено: {processor.Skipped}");

var serviceA = new Node("Service A");
var serviceB = new Node("Service B");
var serviceC = new Node("Service C");
var serviceD = new Node("Service D");

serviceA.AddDependency(serviceB);
serviceA.AddDependency(serviceC);
serviceB.AddDependency(serviceD);

var graph = new ServiceGraph();

foreach (var service in graph.DepthFirst(serviceA))
{
    foreach (var dependency in graph.BreadthFirst(service))
    {
        Console.WriteLine($"{service} → {dependency}");
    }
}

var cycleStart = new Node("Cycle A");
var cycleMiddle = new Node("Cycle B");
var cycleEnd = new Node("Cycle C");

cycleStart.AddDependency(cycleMiddle);
cycleMiddle.AddDependency(cycleEnd);
cycleEnd.AddDependency(cycleStart);

var cycle = graph.FindCycle(cycleStart);

if (cycle is not null)
{
    Console.WriteLine("Найден цикл: " + string.Join(" → ", cycle));
}
else
{
    Console.WriteLine("Циклов не найдено.");
}
var profile = File.ReadAllText("profile.txt").Trim();

if (profile == "console")
{
    services.AddSingleton<ILogSink, ConsoleLogSink>();
}
else if (profile == "file")
{
    services.AddSingleton<ILogSink>(_ => new SqliteLogSink("logs.db"));
}
else
{
    throw new InvalidOperationException(
        $"Неизвестный профиль: {profile}");
}

services.AddSingleton<LogProcessor>();
services.AddSingleton<LogHubApplication>();

using var provider = services.BuildServiceProvider();

var app = provider.GetRequiredService<LogHubApplication>();
app.Run(args[0]);
