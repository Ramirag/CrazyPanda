using Core.Models;

namespace Core.DAL
{
    public interface IRepository
    {
        Size GetMapSize();
        Restaurant[] GetRestaurants();
    }
}
