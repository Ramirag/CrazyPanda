using Core.Models;

namespace Core.RestaurantGetter
{
    public interface IRestaurantsFilter
    {
        Restaurant[] GetFilteredRestaurantsInZone(Zone zone, Filter filter);
    }
}