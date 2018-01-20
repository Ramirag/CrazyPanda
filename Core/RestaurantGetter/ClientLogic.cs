using System;
using Core.Models;

namespace Core.RestaurantGetter
{
    public class ClientLogic : IClientLogic
    {
        private readonly IRestaurantsFilter _restaurantsFilter;

        public ClientLogic(IRestaurantsFilter restaurantsFilter)
        {
            _restaurantsFilter = restaurantsFilter;
        }

        public bool GetRestaurantWithBiggestBurgerCountInZone(Zone zone, Filter filter, out Restaurant restaurant)
        {
            var restaurants = _restaurantsFilter.GetFilteredRestaurantsInZone(zone, filter);
            if (restaurants.Length == 0)
            {
                restaurant = null;
                return false;
            }

            restaurant = restaurants[0];
            for (var i = 1; i < restaurants.Length; i++)
                if (restaurants[i].BurgerCount > restaurant.BurgerCount)
                    restaurant = restaurants[i];
            return true;
        }

        public bool GetNearestRestaurantFromZoneCenter(Zone zone, Filter filter, out Restaurant restaurant)
        {
            var restaurants = _restaurantsFilter.GetFilteredRestaurantsInZone(zone, filter);
            if (restaurants.Length == 0)
            {
                restaurant = null;
                return false;
            }

            var centerX = (zone.Point.X + zone.Point1.X) / 2;
            var centerY = (zone.Point.Y + zone.Point1.Y) / 2;
            restaurant = restaurants[0];
            var minDist = Math.Abs(centerX - restaurant.Location.Y) + Math.Abs(centerY - restaurant.Location.Y);
            for (var i = 1; i < restaurants.Length; i++)
            {
                var dist = Math.Abs(centerX - restaurants[i].Location.Y) +
                           Math.Abs(centerY - restaurants[i].Location.Y);
                if (dist < minDist)
                {
                    restaurant = restaurants[i];
                    minDist = dist;
                }
            }

            return true;
        }
    }
}