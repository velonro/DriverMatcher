using DriverMatcher.Core.Abstractions;
using DriverMatcher.Core.Helpers;
using DriverMatcher.Core.Models;

namespace DriverMatcher.Core.Algorithms;

public class LinearSearchFinder : IDriverFinder
{
    public IReadOnlyList<Driver> FindNearest(Order order, IReadOnlyList<Driver> drivers, int count)
    {
        if (drivers.Count == 0 || count <= 0)
            return Array.Empty<Driver>();

        return drivers
            .Select(d => (Driver: d, Dist: DistanceHelper.SquaredDistance(order.Location, d.Location)))
            .OrderBy(x => x.Dist)
            .Take(count)
            .Select(x => x.Driver)
            .ToList();
    }
}