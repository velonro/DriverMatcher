using DriverMatcher.Core.Abstractions;
using DriverMatcher.Core.Helpers;
using DriverMatcher.Core.Models;

namespace DriverMatcher.Core.Algorithms;

public class GridFinder : IDriverFinder
{
    private readonly int _gridSize;
    private readonly Dictionary<(int, int), List<Driver>> _grid = new();

    public GridFinder(int gridSize = 20)
    {
        _gridSize = gridSize;
    }

    public void BuildIndex(IEnumerable<Driver> drivers)
    {
        _grid.Clear();
        foreach (var driver in drivers)
        {
            var cell = GetCell(driver.Location);
            if (!_grid.TryGetValue(cell, out var list))
            {
                list = new List<Driver>();
                _grid[cell] = list;
            }
            list.Add(driver);
        }
    }

    private (int, int) GetCell(Location loc) => (loc.X / _gridSize, loc.Y / _gridSize);

    public IReadOnlyList<Driver> FindNearest(Order order, IReadOnlyList<Driver> drivers, int count)
    {
        if (drivers.Count == 0 || count <= 0)
            return Array.Empty<Driver>();

        if (_grid.Count == 0)
            BuildIndex(drivers);

        var orderCell = GetCell(order.Location);
        var candidates = new List<Driver>();
        int radius = 0;
        while (candidates.Count < count && radius <= 10)
        {
            var cells = GetCellsInRadius(orderCell, radius);
            foreach (var cell in cells)
            {
                if (_grid.TryGetValue(cell, out var list))
                    candidates.AddRange(list);
            }
            if (candidates.Count >= count)
                break;
            radius++;
        }

        return candidates
            .OrderBy(d => DistanceHelper.SquaredDistance(order.Location, d.Location))
            .Take(count)
            .ToList();
    }

    private IEnumerable<(int, int)> GetCellsInRadius((int, int) center, int radius)
    {
        for (int dx = -radius; dx <= radius; dx++)
            for (int dy = -radius; dy <= radius; dy++)
                yield return (center.Item1 + dx, center.Item2 + dy);
    }
}