using Core.Models;

namespace Core.Map
{
    public interface IMap
    {
        Size Size { get; }
        Restaurant[] GetRestaurantsInZone(Zone zone);
    }
}