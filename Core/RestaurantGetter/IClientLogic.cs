using Core.Models;

namespace Core.RestaurantGetter
{
    public interface IClientLogic
    {
        bool GetRestaurantWithBiggestBurgerCountInZone(Zone zone, Filter filter, out Restaurant restaurant);
        bool GetNearestRestaurantFromZoneCenter(Zone zone, Filter filter, out Restaurant restaurant);
    }
}