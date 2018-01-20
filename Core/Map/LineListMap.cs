using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace Core.Map
{
    public sealed class LineListMap : IMap
    {
        private static readonly PointComparerByX PointComparerByXObj = new PointComparerByX();
        private static readonly PointComparerByY PointComparerByYObj = new PointComparerByY();

        private static readonly RestaurantIEqualityComparerByX RestaurantIEqualityComparerByXObj =
            new RestaurantIEqualityComparerByX();

        private readonly Restaurant[] _sortedRestaurantsByX;
        private readonly Restaurant[] _sortedRestaurantsByY;

        public LineListMap(IList<Restaurant> restaurants, Size size)
        {
            _sortedRestaurantsByX = restaurants.OrderBy(i => i.Location, PointComparerByXObj).ToArray();
            _sortedRestaurantsByY = restaurants.OrderBy(i => i.Location, PointComparerByYObj).ToArray();
            Size = size;
        }

        public Size Size { get; }

        public Restaurant[] GetRestaurantsInZone(Zone zone)
        {
            if (!TryGetStartIndex(_sortedRestaurantsByX, zone.Point, PointComparerByXObj, out var startIndexByX) ||
                !TryGetEndIndex(_sortedRestaurantsByX, zone.Point1, PointComparerByXObj, out var endIndexByX))
                return new Restaurant[0];

            if (!TryGetStartIndex(_sortedRestaurantsByY, zone.Point, PointComparerByYObj, out var startIndexByY) ||
                !TryGetEndIndex(_sortedRestaurantsByY, zone.Point1, PointComparerByYObj, out var endIndexByY))
                return new Restaurant[0];

            var countItemsByX = endIndexByX - startIndexByX;
            var countItemsByY = endIndexByY - startIndexByY;

            HashSet<Restaurant> hashSet;
            Restaurant[] restaurants;
            int startIndex, endIndex;
            if (countItemsByX < countItemsByY)
            {
                hashSet = CreateRestaurantHashSet(_sortedRestaurantsByX, startIndexByX, endIndexByX);
                restaurants = _sortedRestaurantsByY;
                startIndex = startIndexByY;
                endIndex = endIndexByY;
            }
            else
            {
                hashSet = CreateRestaurantHashSet(_sortedRestaurantsByY, startIndexByY, endIndexByY);
                restaurants = _sortedRestaurantsByX;
                startIndex = startIndexByX;
                endIndex = endIndexByX;
            }

            var result = new List<Restaurant>(hashSet.Count);
            for (var i = startIndex; i <= endIndex; i++)
            {
                if (hashSet.Contains(restaurants[i]))
                {
                    result.Add(restaurants[i]);
                }
            }
            return result.ToArray();
        }

        private static HashSet<Restaurant> CreateRestaurantHashSet(Restaurant[] restaurants, int startIndex,
            int endIndex)
        {
            var hashSet = new HashSet<Restaurant>(RestaurantIEqualityComparerByXObj);
            for (var i = startIndex; i <= endIndex; i++) hashSet.Add(restaurants[i]);

            return hashSet;
        }

        private static bool TryGetStartIndex(Restaurant[] restaurants, Point point, IComparer<Point> comparer,
            out int startIndex)
        {
            var (index, isNearest) = FindIndex(restaurants, point, comparer);
            if (isNearest)
            {
                for (; index <= restaurants.Length - 1; index++)
                {
                    var location = restaurants[index].Location;
                    if (location.X >= point.X && location.Y >= point.Y)
                    {
                        startIndex = index;
                        return true;
                    }
                }

                startIndex = 0;
                return false;

            }

            startIndex = index;
            return true;
        }

        private static bool TryGetEndIndex(Restaurant[] restaurants, Point point, IComparer<Point> comparer,
            out int endIndex)
        {
            var (index, isNearest) = FindIndex(restaurants, point, comparer);
            if (isNearest)
            {
                for (; index >= 0; index--)
                {
                    var location = restaurants[index].Location;
                    if (location.X <= point.X && location.Y <= point.Y)
                    {
                        endIndex = index;
                        return true;
                    }
                }

                endIndex = 0;
                return false;
            }

            endIndex = index;
            return true;
        }

        private static (int index, bool isNearest) FindIndex(Restaurant[] restaurants, Point point,
            IComparer<Point> comparer)
        {
            var startIndex = 0;
            var endIndex = restaurants.Length - 1;
            while (true)
            {
                var middleIndex = (startIndex + endIndex) / 2;
                var locationInMiddleIndex = restaurants[middleIndex].Location;
                if (locationInMiddleIndex.Equals(point)) return (index: middleIndex, isNearest: false);

                if (startIndex >= endIndex)
                {
                    return (index: startIndex, isNearest: true);
                }

                if (comparer.Compare(locationInMiddleIndex, point) > 0)
                    endIndex = middleIndex - 1;
                else
                    startIndex = middleIndex + 1;
            }
        }

        private class PointComparerByX : IComparer<Point>
        {
            public int Compare(Point x, Point y)
            {
                if (x.X == y.X)
                {
                    if (x.Y == y.Y)
                    {
                        return 0;
                    }
                    return x.Y > y.Y ? 1 : -1;
                }

                return x.X > y.X ? 1 : -1;
            }
        }

        private class PointComparerByY : IComparer<Point>
        {
            public int Compare(Point x, Point y)
            {
                if (x.Y == y.Y)
                {
                    if (x.X == y.X)
                    {
                        return 0;
                    }
                    return x.X > y.X ? 1 : -1;
                }

                return x.Y > y.Y ? 1 : -1;
            }
        }

        private class RestaurantIEqualityComparerByX : IEqualityComparer<Restaurant>
        {
            public bool Equals(Restaurant x, Restaurant y)
            {
                return ReferenceEquals(x, y) || x.Location.Equals(y.Location);
            }

            public int GetHashCode(Restaurant obj)
            {
                return obj.Location.GetHashCode();
            }
        }
    }
}