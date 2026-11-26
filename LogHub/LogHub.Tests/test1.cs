using LogHub.Filtering;
using LogHub.Models;
using LogHub.Parsing;
using LogHub.Processing;
using LogHub.Sinks;
using Xunit;

namespace LogHub.Tests;

public class LogHubTests
{
    [Fact]
    public void Parser_RecognizesFieldsDirective()
    {
        var parser = new W3CLogParser();

        var result = parser.ProcessDirective(
            "#Fields: date time cs-method sc-status");

        Assert.True(result);
        Assert.True(parser.HasFields);
    }

    [Fact]
    public void Parser_MapsValuesToFields()
    {
        var parser = new W3CLogParser();

        parser.ProcessDirective(
            "#Fields: date time cs-method sc-status");

        var record = parser.Parse(
            "2026-10-01 12:00:00 GET 500");

        Assert.NotNull(record);
        Assert.Equal("GET", record.GetValue("cs-method"));
        Assert.Equal("500", record.GetValue("sc-status"));
    }

    [Fact]
    public void Parser_ReturnsNullForWrongFieldCount()
    {
        var parser = new W3CLogParser();

        parser.ProcessDirective(
            "#Fields: date time cs-method sc-status");

        var record = parser.Parse(
            "2026-10-01 12:00:00 GET");

        Assert.Null(record);
    }

    [Fact]
    public void Parser_IgnoresCommentLines()
    {
        var parser = new W3CLogParser();

        parser.ProcessDirective(
            "#Fields: date time sc-status");

        var record = parser.Parse("#Comment");

        Assert.Null(record);
    }

    [Fact]
    public void Processor_SendsAllowedRecordToAllSinks()
    {
        var firstSink = new FakeLogSink();
        var secondSink = new FakeLogSink();

        var processor = new LogProcessor(
            new W3CLogParser(),
            new LogFilter(),
            new ILogSink[] { firstSink, secondSink });

        processor.ProcessLine("#Fields: date time sc-status");
        processor.ProcessLine("2026-10-01 12:00:00 500");

        Assert.Single(firstSink.Records);
        Assert.Single(secondSink.Records);
    }

    [Fact]
    public void Processor_DoesNotSendFilteredRecord()
    {
        var sink = new FakeLogSink();

        var processor = new LogProcessor(
            new W3CLogParser(),
            new LogFilter(),
            new ILogSink[] { sink });

        processor.ProcessLine("#Fields: date time sc-status");
        processor.ProcessLine("2026-10-01 12:00:00 200");

        Assert.Empty(sink.Records);
    }

    private class FakeLogSink : ILogSink
    {
        public List<LogRecord> Records { get; } = new();

        public void Write(LogRecord record)
        {
            Records.Add(record);
        }
    }
}