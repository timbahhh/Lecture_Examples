using System.Runtime.CompilerServices;
using LogHub.Filtering;
using LogHub.Graphs;
using LogHub.Models;
using LogHub.Processing;
using LogHub.Sinks;
using LogHub.Sources;
using Xunit;

namespace LogHub.Tests;

public class Lab6Tests
{
    // ЧАСТЬ A — 4 обязательных теста

    // 1. Подмена стратегии меняет результат обработки.
    [Fact]
    public void Strategy_ChangingImporterChangesResult()
    {
        var record = new LogRecord(new Dictionary<string, string>());

        var emptyImporter = new FakeImporter(Array.Empty<LogRecord>());
        var recordImporter = new FakeImporter(new[] { record });

        var emptyProcessor = new LogProcessor(
            emptyImporter, new LogFilter(), Array.Empty<ILogSink>());

        var recordProcessor = new LogProcessor(
            recordImporter, new LogFilter(), Array.Empty<ILogSink>());

        emptyProcessor.Process();
        recordProcessor.Process();

        Assert.Equal(0, emptyProcessor.Written);
        Assert.Equal(1, recordProcessor.Written);
    }

    // 2. Шаблонный метод вызывает этапы в правильном порядке.
    [Fact]
    public void TemplateMethod_CallsStepsInCorrectOrder()
    {
        var importer = new OrderedImporter(new[] { "one", "two" });

        importer.Import().ToList();

        Assert.Equal(new[]
        {
            "Open",
            "Read",
            "Validate:one",
            "Parse:one",
            "Validate:two",
            "Parse:two",
            "Close",
            "Dispose"
        }, importer.Calls);
    }

    // 3. Стандартный хук не ломает импортёр, который его не переопределяет.
    [Fact]
    public void TemplateMethod_DefaultHookWorksWithoutOverride()
    {
        var importer = new DefaultImporter(
            new[] { "first", "", " ", "second" });

        Assert.Equal(
            new[] { "first", "second" },
            importer.Import());
    }

    // 4. Переопределённый хук отсекает неподходящие записи.
    [Fact]
    public void TemplateMethod_OverriddenHookFiltersRecords()
    {
        var importer = new FilteringImporter(
            new[] { "valid", "invalid", "valid2" });

        Assert.Equal(
            new[] { "valid", "valid2" },
            importer.Import());
    }

    // ЧАСТЬ B — 6 обязательных тестов

    // 5. Проверка порядка DFS на эталонном графе.
    [Fact]
    public void DFS_ReturnsExpectedOrder()
    {
        var (graph, a, _, _, _) = CreateGraph();

        Assert.Equal(
            new[] { "A", "B", "D", "C" },
            graph.DepthFirst(a).Select(x => x.Name));
    }

    // 6. Проверка порядка BFS на эталонном графе.
    [Fact]
    public void BFS_ReturnsExpectedOrder()
    {
        var (graph, a, _, _, _) = CreateGraph();

        Assert.Equal(
            new[] { "A", "B", "C", "D" },
            graph.BreadthFirst(a).Select(x => x.Name));
    }

    // 7. Проверка вложенных foreach.
    [Fact]
    public void NestedForeach_ReturnsExpectedPairs()
    {
        var (graph, a, _, _, _) = CreateGraph();
        var result = new List<string>();

        foreach (var service in graph.DepthFirst(a))
        {
            foreach (var dependency in graph.BreadthFirst(service))
            {
                result.Add($"{service.Name}->{dependency.Name}");
            }
        }

        Assert.Equal(new[]
        {
            "A->A", "A->B", "A->C", "A->D",
            "B->B", "B->D",
            "D->D",
            "C->C"
        }, result);
    }

    // 8. Обход графа с циклом завершается.
    [Fact]
    public void Traversal_DoesNotLoopOnCycle()
    {
        var (graph, a, _, _) = CreateCycleGraph();

        Assert.Equal(
            new[] { "A", "B", "C" },
            graph.DepthFirst(a).Select(x => x.Name));

        Assert.Equal(
            new[] { "A", "B", "C" },
            graph.BreadthFirst(a).Select(x => x.Name));
    }

    // 9. Пустой граф не возвращает узлов.
    [Fact]
    public void Traversal_HandlesEmptyGraph()
    {
        var graph = new ServiceGraph();

        Assert.Empty(graph.DepthFirst(null!));
        Assert.Empty(graph.BreadthFirst(null!));
        Assert.Null(graph.FindCycle(null!));
    }

    // 10. Граф из одной вершины.
    [Fact]
    public void Traversal_HandlesSingleNode()
    {
        var graph = new ServiceGraph();
        var node = new Node("A");

        Assert.Equal(new[] { "A" },
            graph.DepthFirst(node).Select(x => x.Name));

        Assert.Equal(new[] { "A" },
            graph.BreadthFirst(node).Select(x => x.Name));
    }

    // 11. Обе реализации итератора дают одинаковую последовательность.
    [Fact]
    public void Iterators_ReturnSameSequence()
    {
        var (graph, a, _, _, _) = CreateGraph();

        var manual = new ManualDepthFirstEnumerable(a)
            .Select(x => x.Name);

        var yield = graph.DepthFirstYield(a)
            .Select(x => x.Name);

        Assert.Equal(manual, yield);
    }

    // 12. DFS не посещает общую зависимость дважды.
    [Fact]
    public void DFS_VisitsSharedDependencyOnce()
    {
        var a = new Node("A");
        var b = new Node("B");
        var c = new Node("C");
        var d = new Node("D");

        a.AddDependency(b);
        a.AddDependency(c);
        b.AddDependency(d);
        c.AddDependency(d);

        var result = new ServiceGraph().DepthFirst(a).ToList();

        Assert.Equal(4, result.Count);
        Assert.Equal(4, result.Distinct().Count());
    }

    // 13. BFS не посещает общую зависимость дважды.
    [Fact]
    public void BFS_VisitsSharedDependencyOnce()
    {
        var a = new Node("A");
        var b = new Node("B");
        var c = new Node("C");
        var d = new Node("D");

        a.AddDependency(b);
        a.AddDependency(c);
        b.AddDependency(d);
        c.AddDependency(d);

        var result = new ServiceGraph().BreadthFirst(a).ToList();

        Assert.Equal(4, result.Count);
        Assert.Equal(4, result.Distinct().Count());
    }

    // 14. Поиск цикла возвращает путь.
    [Fact]
    public void FindCycle_ReturnsCyclePath()
    {
        var (graph, a, _, _) = CreateCycleGraph();

        var cycle = graph.FindCycle(a);

        Assert.NotNull(cycle);
        Assert.Equal(
            new[] { "A", "B", "C", "A" },
            cycle!.Select(x => x.Name));
    }

    // 15. Поиск не находит цикл в ациклическом графе.
    [Fact]
    public void FindCycle_ReturnsNullForAcyclicGraph()
    {
        var (graph, a, _, _, _) = CreateGraph();

        Assert.Null(graph.FindCycle(a));
    }

    // 16. Поиск обнаруживает зависимость вершины от самой себя.
    [Fact]
    public void FindCycle_DetectsSelfReference()
    {
        var node = new Node("A");
        node.AddDependency(node);

        var cycle = new ServiceGraph().FindCycle(node);

        Assert.NotNull(cycle);
        Assert.Equal(
            new[] { "A", "A" },
            cycle!.Select(x => x.Name));
    }

    // 17. Ручной итератор можно сбросить.
    [Fact]
    public void ManualIterator_ResetRestartsTraversal()
    {
        var (_, a, _, _, _) = CreateGraph();
        using var iterator =
            new ManualDepthFirstEnumerable(a).GetEnumerator();

        Assert.True(iterator.MoveNext());
        Assert.Equal("A", iterator.Current.Name);

        Assert.True(iterator.MoveNext());
        iterator.Reset();

        Assert.True(iterator.MoveNext());
        Assert.Equal("A", iterator.Current.Name);
    }

    // 18. Отдельные экземпляры ручного итератора независимы.
    [Fact]
    public void ManualIterator_InstancesAreIndependent()
    {
        var (_, a, _, _, _) = CreateGraph();
        var enumerable = new ManualDepthFirstEnumerable(a);

        using var first = enumerable.GetEnumerator();
        using var second = enumerable.GetEnumerator();

        Assert.True(first.MoveNext());
        Assert.True(first.MoveNext());

        Assert.True(second.MoveNext());
        Assert.Equal("A", second.Current.Name);
    }

    private static (ServiceGraph graph, Node a, Node b, Node c, Node d)
        CreateGraph()
    {
        var a = new Node("A");
        var b = new Node("B");
        var c = new Node("C");
        var d = new Node("D");

        a.AddDependency(b);
        a.AddDependency(c);
        b.AddDependency(d);

        return (new ServiceGraph(), a, b, c, d);
    }

    private static (ServiceGraph graph, Node a, Node b, Node c)
        CreateCycleGraph()
    {
        var a = new Node("A");
        var b = new Node("B");
        var c = new Node("C");

        a.AddDependency(b);
        b.AddDependency(c);
        c.AddDependency(a);

        return (new ServiceGraph(), a, b, c);
    }

    private class FakeImporter : ILogImporter
    {
        private readonly IEnumerable<LogRecord> _records;

        public FakeImporter(IEnumerable<LogRecord> records)
        {
            _records = records;
        }

        public IEnumerable<LogRecord> Import() => _records;
    }

    private class DefaultImporter : LogImporterBase<string>
    {
        private readonly IEnumerable<string> _lines;

        public DefaultImporter(IEnumerable<string> lines)
        {
            _lines = lines;
        }

        protected override IEnumerable<string> Read() => _lines;

        protected override string? Parse(string line) => line;
    }

    private class FilteringImporter : LogImporterBase<string>
    {
        private readonly IEnumerable<string> _lines;

        public FilteringImporter(IEnumerable<string> lines)
        {
            _lines = lines;
        }

        protected override IEnumerable<string> Read() => _lines;

        protected override string? Parse(string line) => line;

        protected override bool Validate(string line)
        {
            return line.StartsWith("valid");
        }
    }

    private class OrderedImporter : LogImporterBase<string>
    {
        private readonly IEnumerable<string> _lines;

        public List<string> Calls { get; } = new();

        public OrderedImporter(IEnumerable<string> lines)
        {
            _lines = lines;
        }

        protected override void Open() => Calls.Add("Open");

        protected override IEnumerable<string> Read()
        {
            Calls.Add("Read");
            return _lines;
        }

        protected override bool Validate(string line)
        {
            Calls.Add($"Validate:{line}");
            return true;
        }

        protected override string? Parse(string line)
        {
            Calls.Add($"Parse:{line}");
            return line;
        }

        protected override void Close() => Calls.Add("Close");

        protected override void DisposeCore() => Calls.Add("Dispose");
    }
}
