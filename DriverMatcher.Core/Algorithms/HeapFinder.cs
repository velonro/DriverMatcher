using DriverMatcher.Core.Abstractions;
using DriverMatcher.Core.Helpers;
using DriverMatcher.Core.Models;

namespace DriverMatcher.Core.Algorithms;

public class HeapFinder : IDriverFinder
{
    public IReadOnlyList<Driver> FindNearest(Order order, IReadOnlyList<Driver> drivers, int count)
    {
        if (drivers.Count == 0 || count <= 0)
            return Array.Empty<Driver>();

        var pq = new PriorityQueue<Driver, int>(count + 1);
        foreach (var driver in drivers)
        {
            var dist = DistanceHelper.SquaredDistance(order.Location, driver.Location);
            if (pq.Count < count)
            {
                pq.Enqueue(driver, -dist);
            }
            else if (pq.TryPeek(out _, out int topNegDist) && -topNegDist > dist)
            {
                pq.Dequeue();
                pq.Enqueue(driver, -dist);
            }
        }

        var result = new List<Driver>(pq.Count);
        while (pq.Count > 0)
            result.Add(pq.Dequeue());
        result.Reverse();
        return result;
    }
}