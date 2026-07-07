using DriverMatcher.Core.Abstractions;
using DriverMatcher.Core.Algorithms;
using DriverMatcher.Core.Models;
using NUnit.Framework;

namespace DriverMatcher.Tests;

[TestFixture]
public class FinderTests
{
    private List<Driver> _drivers = null!;
    private Order _order = null!;
    private IDriverFinder[] _finders = null!;

    [SetUp]
    public void Setup()
    {
        _drivers = new List<Driver>
        {
            new(1, new Location(10, 10)),
            new(2, new Location(20, 20)),
            new(3, new Location(5, 5)),
            new(4, new Location(15, 5)),
            new(5, new Location(25, 25)),
            new(6, new Location(30, 30)),
        };
        _order = new Order(new Location(12, 12));

        var gridFinder = new GridFinder(gridSize: 10);
        gridFinder.BuildIndex(_drivers);

        _finders = new IDriverFinder[]
        {
            new LinearSearchFinder(),
            new HeapFinder(),
            gridFinder
        };
    }

    [Test]
    public void AllFindersReturnSameOrderedIds_ForK5()
    {
        const int count = 5;
        var expected = _drivers
            .OrderBy(d => DistanceSquared(d.Location, _order.Location))
            .Take(count)
            .Select(d => d.Id)
            .ToList();

        foreach (var finder in _finders)
        {
            var result = finder.FindNearest(_order, _drivers, count);
            var ids = result.Select(d => d.Id).ToList();
            Assert.AreEqual(expected, ids, $"Finder {finder.GetType().Name} returned wrong order");
        }
    }

    [Test]
    public void ReturnEmpty_WhenDriversEmpty()
    {
        var emptyDrivers = new List<Driver>();
        foreach (var finder in _finders)
        {
            var result = finder.FindNearest(_order, emptyDrivers, 5);
            Assert.IsEmpty(result);
        }
    }

    [Test]
    public void ReturnAllDrivers_WhenCountGreaterThanTotal()
    {
        var count = _drivers.Count + 10;
        foreach (var finder in _finders)
        {
            var result = finder.FindNearest(_order, _drivers, count);
            Assert.AreEqual(_drivers.Count, result.Count);
            Assert.IsTrue(result.All(d => _drivers.Contains(d)));
        }
    }

    [Test]
    public void GridFinder_BuildIndexThenSearch_Works()
    {
        var grid = new GridFinder(10);
        grid.BuildIndex(_drivers);
        var result = grid.FindNearest(_order, _drivers, 3);
        Assert.AreEqual(3, result.Count);
    }

    private static int DistanceSquared(Location a, Location b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }
}