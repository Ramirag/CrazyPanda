using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Core.Models;

namespace Core.RestaurantGetter
{
    // TODO Сломано. Надо менять всю концепцию кэширования динамических полей для ликвидации возможной гонки.
    [Obsolete("Сломано. Надо менять всю концепцию кэширования динамических полей для ликвидации возможной гонки.",
        true)]
    public class CachedRestaurantsFilter : IRestaurantsFilter
    {
        private readonly ConcurrentDictionary<Restaurant, ConcurrentStack<Key>> _cachedKeys;
        private readonly ConcurrentDictionary<Key, Restaurant[]> _cachedRestaurants;

        private readonly IRestaurantsFilter _restaurantsFilter;

        public CachedRestaurantsFilter(IRestaurantsFilter restaurantsFilter)
        {
            _restaurantsFilter = restaurantsFilter;
            _cachedRestaurants = new ConcurrentDictionary<Key, Restaurant[]>();
            _cachedKeys = new ConcurrentDictionary<Restaurant, ConcurrentStack<Key>>();
        }

        public Restaurant[] GetFilteredRestaurantsInZone(Zone zone, Filter filter)
        {
            var key = new Key(zone, filter);
            return _cachedRestaurants.GetOrAdd(key, key2 =>
            {
                var restaurants = _restaurantsFilter.GetFilteredRestaurantsInZone(key2.Zone, key2.Filter);
                foreach (var restaurant in restaurants)
                    _cachedKeys.AddOrUpdate(restaurant, res =>
                    {
                        var stack = new ConcurrentStack<Key>();
                        stack.Push(key);
                        return stack;
                    }, (rest, stack) =>
                    {
                        stack.Push(key);
                        return stack;
                    });
                return restaurants;
            });
        }

        private void FillCachedKey(Key key, Restaurant restaurant)
        {
            _cachedKeys.AddOrUpdate(restaurant, res =>
            {
                var stack = new ConcurrentStack<Key>();
                stack.Push(key);
                return stack;
            }, (rest, stack) =>
            {
                stack.Push(key);
                return stack;
            });
        }

        private void FillCachedKeyWhenRestroomExist(Key key, Restaurant[] restaurants)
        {
            foreach (var restaurant in restaurants)
            {
                FillCachedKey(key, restaurant);
                restaurant.RestroomExistChanged += Restaurant_Changed;
            }
        }

        private void Restaurant_Changed(object sender, EventArgs e)
        {
            var restaurant = (Restaurant) sender;
        }

        private void FillCachedKeyWhenHotPieExist(Key key, Restaurant[] restaurants)
        {
        }

        private void FillCachedKeyWhenBoth(Key key, Restaurant[] restaurants)
        {
        }

        private class Key : IEquatable<Key>
        {
            private readonly int _hashCode;

            public Key(Zone zone, Filter filter)
            {
                Zone = zone;
                Filter = filter;

                _hashCode = -1861599978;
                _hashCode = _hashCode * -1521134295 + EqualityComparer<Zone>.Default.GetHashCode(Zone);
                _hashCode = _hashCode * -1521134295 + Filter.GetHashCode();
            }

            public Zone Zone { get; }
            public Filter Filter { get; }

            public bool Equals(Key other)
            {
                return Zone.Equals(other.Zone) &&
                       Filter == other.Filter;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Key);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }
    }
}