using DriverMatcher.Core.Models;

namespace DriverMatcher.Core.Helpers;

public static class DistanceHelper
{
    public static int SquaredDistance(Location a, Location b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }
}