using System;
using System.Linq;
using Core.DAL;
using Core.Map;
using Core.Models;
using Core.RestaurantGetter;

namespace Client
{
    class Program
    {
        private const int ZoneWidht = 400;
        private const int ZoneHeight = 400;
        private const int ZoneHalfWidht = ZoneWidht / 2;
        private const int ZoneHalfHeight = ZoneHeight / 2;
        private const int MaxZoom = 3;

        private static IMap _map;
        private static IRestaurantsFilter _restaurantsFilter;
        private static IClientLogic _clientLogic;
        private static Filter _currentFilter = Filter.Non;
        private static Point _currentPosition = new Point(0, 0);
        private static int _currentZoom = 1;

        private static void Main()
        {
            var repository = new RandomRepository(1000, 1000, 1000);
            var restaurants = repository.GetRestaurants();
            var mapSize = repository.GetMapSize();
            _map = new LineListMap(restaurants, mapSize);
            _map = new CachedMap(_map);
            _restaurantsFilter = new RestaurantsFilter(_map);
            _clientLogic = new ClientLogic(_restaurantsFilter);

            MainMenu();

            Console.ReadLine();
        }

        private static Zone CalculateVisibleZone(Point centerPoint, int zoom)
        {
            var zoneHalfWidht = ZoneHalfWidht / zoom;
            var zoneHalfHeight = ZoneHalfHeight / zoom;
            var x = centerPoint.X - zoneHalfWidht;
            var x1 = centerPoint.X + zoneHalfWidht;
            var y = centerPoint.Y - zoneHalfHeight;
            var y1 = centerPoint.Y + zoneHalfHeight;

            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x1 > _map.Size.Widht) x1 = _map.Size.Widht;
            if (y1 > _map.Size.Height) y1 = _map.Size.Height;
            return new Zone(new Point(x, y), new Point(x1, y1));
        }

        private static void ShowData()
        {
            var zone = CalculateVisibleZone(_currentPosition, _currentZoom);
            var restaurants = _restaurantsFilter.GetFilteredRestaurantsInZone(zone, _currentFilter);
            var biggest =
                _clientLogic.GetRestaurantWithBiggestBurgerCountInZone(zone, _currentFilter, out var biggeRestaurant)
                    ? biggeRestaurant.ToString()
                    : "NoN";
            Console.WriteLine($"Ресторан с наибольшим числом бургеров: {biggest}");
            if (_currentZoom == MaxZoom)
            {
                var nearest =
                    _clientLogic.GetNearestRestaurantFromZoneCenter(zone, _currentFilter, out var nearestRestaurant)
                        ? nearestRestaurant.ToString()
                        : "NoN";
                Console.WriteLine($"Ближайший ресторан: {nearest}");
            }
            Console.WriteLine();
            foreach (var restaurant in restaurants)
            {
                Console.WriteLine(restaurant);
            }
        }

        private static void MainMenu()
        {
            while (true)
            {
                Console.WriteLine("===============Управление===============");
                Console.WriteLine("Изменить фильтр: 1: Выключен, 2: По наличию туалета, 3: По наличию горячих пирожков, 4: Оба фильтра");
                Console.WriteLine("Изменить текущее местоположение: W: Вперед, A: Влево, S: Назад, D: Вправо, Shift: Бег");
                Console.WriteLine("Изменить Зум: Q: Приблизить, E: Отдалить");
                Console.WriteLine("===============Управление===============");

                Console.WriteLine($"Фильтр: {_currentFilter.ToString()}");
                Console.WriteLine($"Текущее местоположение: [{_currentPosition.X}:{_currentPosition.Y}]");
                Console.WriteLine($"Текущий зум: {_currentZoom}");

                ShowData();

                var key = Console.ReadKey();
                int shift;
                int newCoordinate;
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        _currentFilter = Filter.Non;
                        break;
                    case ConsoleKey.D2:
                        _currentFilter = Filter.RestroomExist;
                        break;
                    case ConsoleKey.D3:
                        _currentFilter = Filter.HotPieExist;
                        break;
                    case ConsoleKey.D4:
                        _currentFilter = Filter.Both;
                        break;
                    case ConsoleKey.W:
                        shift = key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? 10 : 1;
                        newCoordinate = _currentPosition.X - shift;
                        if (newCoordinate < 0)
                        {
                            newCoordinate = 0;
                        }
                        _currentPosition = new Point(newCoordinate, _currentPosition.Y);
                        break;
                    case ConsoleKey.A:
                        shift = key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? 10 : 1;
                        newCoordinate = _currentPosition.Y - shift;
                        if (newCoordinate < 0)
                        {
                            newCoordinate = 0;
                        }
                        _currentPosition = new Point(_currentPosition.X, newCoordinate);
                        break;
                    case ConsoleKey.S:
                        shift = key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? 10 : 1;
                        newCoordinate = _currentPosition.X + shift;
                        if (newCoordinate > _map.Size.Height)
                        {
                            newCoordinate = _map.Size.Height;
                        }
                        _currentPosition = new Point(newCoordinate, _currentPosition.Y);
                        break;
                    case ConsoleKey.D:
                        shift = key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? 10 : 1;
                        newCoordinate = _currentPosition.Y + shift;
                        if (newCoordinate > _map.Size.Widht)
                        {
                            newCoordinate = _map.Size.Widht;
                        }
                        _currentPosition = new Point(_currentPosition.X, newCoordinate);
                        break;
                    case ConsoleKey.Q:
                        if(_currentZoom < MaxZoom)
                            _currentZoom += 1;
                        break;
                    case ConsoleKey.E:
                        if (_currentZoom > 1)
                            _currentZoom -= 1;
                        break;
                }
                Console.Clear();
            }
        }
    }
}
