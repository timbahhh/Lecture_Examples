using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using LogHub.Configuration;

namespace LogHub.Tests;

[TestCaseOrderer(
    "LogHub.Tests.AlphabeticalOrderer",
    "LogHub.Tests")]
public class ConfigServiceTests
{
    private const string SharedKey = "singleton-order-test";

    [Fact]
    public void ConfigService_ShouldKeepInstancesIndependent()
    {
        var first = new ConfigService();
        var second = new ConfigService();

        first.SetValue(SharedKey, "saved");

        Assert.Null(second.GetValue(SharedKey));
    }

    [Fact]
    public void LazyConfigService_ShouldStoreAndReturnValue()
    {
        var service = LazyConfigService.Instance();

        service.SetValue("language", "ru");

        Assert.Equal("ru", service.GetValue("language"));
    }

    [Fact]
    public void A_ClassicSingleton_ShouldWriteSharedValue()
    {
        var service = ClassicConfigService.Instance();

        service.SetValue(SharedKey, "saved");

        Assert.Equal("saved", service.GetValue(SharedKey));
    }

    [Fact(Skip = "Демонстрация проблемы Singleton: тест проходит отдельно, но падает после теста записи из-за общего состояния.")]
    public void B_ClassicSingleton_ShouldNotContainPreviousValue()
    {
        var service = ClassicConfigService.Instance();

        Assert.Null(service.GetValue(SharedKey));
    }
}

public class AlphabeticalOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
        IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        return testCases.OrderBy(testCase => testCase.DisplayName);
    }
}