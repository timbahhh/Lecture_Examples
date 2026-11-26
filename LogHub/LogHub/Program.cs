
using LogHub.Filtering;
using LogHub.Formatting;
using LogHub.Parsing;
using LogHub.Processing;
using LogHub.Sinks;
using LogHub.Sources;
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
