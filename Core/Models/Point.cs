using System;

namespace Core.Models
{
    public class Point : IEquatable<Point>
    {
        private readonly int _hashCode;

        public Point(int x, int y)
        {
            X = x;
            Y = y;

            _hashCode = 1861411795;
            _hashCode = _hashCode * -1521134295 + X.GetHashCode();
            _hashCode = _hashCode * -1521134295 + Y.GetHashCode();
        }

        public int X { get; }
        public int Y { get; }

        public bool Equals(Point other)
        {
            if (ReferenceEquals(other, this)) return true;
            return X == other.X && Y == other.Y;
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as Point);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}