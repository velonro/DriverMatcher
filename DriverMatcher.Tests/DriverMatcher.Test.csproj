using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DriverMatcher.Core.Abstractions;
using DriverMatcher.Core.Algorithms;
using DriverMatcher.Core.Models;

namespace DriverMatcher.Console.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class FindNearestBenchmark
{
    private List<Driver> _drivers = null!;
    private Order _order = null!;
    private IDriverFinder _linearFinder = null!;
    private IDriverFinder _heapFinder = null!;
    private GridFinder _gridFinder = null!;
    private const int N = 1000;
    private const int M = 1000;
    private const int DriverCount = 100_000;
    private const int TakeCount = 5;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        _drivers = Enumerable.Range(1, DriverCount)
            .Select(i => new Driver(i, new Location(rnd.Next(N), rnd.Next(M))))
            .ToList();
        _order = new Order(new Location(rnd.Next(N), rnd.Next(M)));

        _linearFinder = new LinearSearchFinder();
        _heapFinder = new HeapFinder();
        _gridFinder = new GridFinder(gridSize: 20);
        _gridFinder.BuildIndex(_drivers);
    }

    [Benchmark(Baseline = true)]
    public IReadOnlyList<Driver> LinearSearch() => _linearFinder.FindNearest(_order, _drivers, TakeCount);

    [Benchmark]
    public IReadOnlyList<Driver> HeapSearch() => _heapFinder.FindNearest(_order, _drivers, TakeCount);

    [Benchmark]
    public IReadOnlyList<Driver> GridSearch() => _gridFinder.FindNearest(_order, _drivers, TakeCount);
}