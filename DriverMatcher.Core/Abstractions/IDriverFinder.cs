using DriverMatcher.Core.Models;

namespace DriverMatcher.Core.Abstractions;

public interface IDriverFinder
{
    IReadOnlyList<Driver> FindNearest(Order order, IReadOnlyList<Driver> drivers, int count);
}