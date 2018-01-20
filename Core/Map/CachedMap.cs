using System.Collections.Concurrent;
using Core.Models;

namespace Core.Map
{
    public sealed class CachedMap : IMap
    {
        private readonly ConcurrentDictionary<Zone, Restaurant[]> _cachedRestaurants;

        private readonly IMap _map;

        public CachedMap(IMap map)
        {
            _map = map;
            _cachedRestaurants = new ConcurrentDictionary<Zone, Restaurant[]>();
        }


        public Size Size => _map.Size;

        public Restaurant[] GetRestaurantsInZone(Zone zone)
        {
            return _cachedRestaurants.GetOrAdd(zone, item => _map.GetRestaurantsInZone(item));
        }
    }
}