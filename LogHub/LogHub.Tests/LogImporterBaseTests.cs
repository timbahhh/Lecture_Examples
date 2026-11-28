using LogHub.Sources;
using Xunit;

namespace LogHub.Tests;

public class LogImporterBaseTests
{
    [Fact]
    public void Import_ShouldCallOverriddenValidationHook()
    {
        var importer = new TestImporter(
            new[] { "valid", "invalid", "also-valid" });

        var result = importer.Import().ToList();

        Assert.Equal(
            new[] { "valid", "also-valid" },
            result);

        Assert.Equal(3, importer.ValidationCalls);
    }

    [Fact]
    public void Import_ShouldUseDefaultValidationHook()
    {
        var importer = new DefaultTestImporter(
            new[] { "valid", "", "   ", "also-valid" });

        var result = importer.Import().ToList();

        Assert.Equal(
            new[] { "valid", "also-valid" },
            result);
    }

    private class TestImporter : LogImporterBase<string>
    {
        private readonly IEnumerable<string> _lines;

        public int ValidationCalls { get; private set; }

        public TestImporter(IEnumerable<string> lines)
        {
            _lines = lines;
        }

        protected override IEnumerable<string> Read() => _lines;

        protected override string? Parse(string line) => line;

        protected override bool Validate(string line)
        {
            ValidationCalls++;
            return line != "invalid";
        }
    }

    private class DefaultTestImporter : LogImporterBase<string>
    {
        private readonly IEnumerable<string> _lines;

        public DefaultTestImporter(IEnumerable<string> lines)
        {
            _lines = lines;
        }

        protected override IEnumerable<string> Read() => _lines;

        protected override string? Parse(string line) => line;
    }
}
