using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace Core.DAL
{
    public class RandomRepository : IRepository
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _restaurantCount;

        private Restaurant[] _restaurants;

        public RandomRepository(int width, int height, int restaurantCount)
        {
            _width = width;
            _height = height;
            _restaurantCount = restaurantCount;
        }

        public Size GetMapSize()
        {
            return new Size(_width, _height);
        }

        public Restaurant[] GetRestaurants()
        {
            if (_restaurants != null)
            {
                return _restaurants;
            }

            var hashSet = new HashSet<Point>();
            var random = new Random(DateTime.Now.GetHashCode());
            for (var i = 0; i < _restaurantCount; i++)
            {
                Point point;
                do
                {
                    var x = random.Next(0, _width);
                    var y = random.Next(0, _height);
                    point = new Point(x, y);
                } while (!hashSet.Add(point));
            }

            _restaurants = hashSet.Select(point => new Restaurant(point, $"Mc’Donalds at [{point.X};{point.Y}]")
                {
                    BurgerCount = random.Next(0, 1000),
                    RestroomExist = random.Next(-100, 100) > 0,
                    HotPieExist = random.Next(-100, 100) > 0
                })
                .ToArray();
            return _restaurants;
        }
    }
}
