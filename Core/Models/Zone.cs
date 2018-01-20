using System;

namespace Core.Models
{
    public class Zone : IEquatable<Zone>
    {
        private readonly int _hashCode;

        public Zone(Point point, Point point1)
        {
            Point = point;
            Point1 = point1;

            _hashCode = 347001049;
            _hashCode = _hashCode * -1521134295 + Point.GetHashCode();
            _hashCode = _hashCode * -1521134295 + Point1.GetHashCode();
        }

        public Point Point { get; }
        public Point Point1 { get; }

        public bool Equals(Zone other)
        {
            if (ReferenceEquals(other, this)) return true;
            return Point.Equals(other.Point) && Point1.Equals(other.Point1);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Zone);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}