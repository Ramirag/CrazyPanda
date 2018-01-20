using System;
using System.Linq;
using Core.Map;
using Core.Models;

namespace Core.RestaurantGetter
{
    public class RestaurantsFilter : IRestaurantsFilter
    {
        private readonly IMap _map;

        public RestaurantsFilter(IMap map)
        {
            _map = map;
        }

        public Restaurant[] GetFilteredRestaurantsInZone(Zone zone, Filter filter)
        {
            var restaurants = _map.GetRestaurantsInZone(zone);
            Func<Restaurant, bool> predicate;
            switch (filter)
            {
                case Filter.HotPieExist:
                    predicate = restaurant => restaurant.HotPieExist;
                    break;
                case Filter.RestroomExist:
                    predicate = restaurant => restaurant.RestroomExist;
                    break;
                case Filter.Both:
                    predicate = restaurant => restaurant.HotPieExist && restaurant.RestroomExist;
                    break;
                default: return restaurants;
            }

            return restaurants.Where(predicate).ToArray();
        }
    }
}