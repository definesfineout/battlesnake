using Starter.Core.Enumerations;
using System;
using System.Collections.Generic;

namespace Starter.Core
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
            => Equals(obj as Point);

        public bool Equals(Point p)
        {
            if (p is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != p.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (X == p.X) && (Y == p.Y);
        }

        public override int GetHashCode() => (X, Y).GetHashCode();

        public static bool operator ==(Point left, Point right)
        {
            if (left is null)
            {
                if (right is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
            => !(left == right);

        public eDirection GetDirectionTo(Point adjacentPosition)
        {
            if (X == adjacentPosition.X)
            {
                var diff = Y - adjacentPosition.Y;
                if (diff == 1)
                {
                    return eDirection.Down;
                }
                else if (diff == -1)
                {
                    return eDirection.Up;
                }
            }
            else if (Y == adjacentPosition.Y)
            {
                var diff = X - adjacentPosition.X;
                if (diff == 1)
                {
                    return eDirection.Left;
                }
                else if (diff == -1)
                {
                    return eDirection.Right;
                }
            }

            return eDirection.None;
        }

        public Point GetAdjacent(eDirection direction)
        {
            switch (direction)
            {
                case eDirection.Up:
                    return new Point(X, Y + 1);
                case eDirection.Down:
                    return new Point(X, Y - 1);
                case eDirection.Right:
                    return new Point(X + 1, Y);
                case eDirection.Left:
                    return new Point(X - 1, Y);
                case eDirection.None:
                default:
                    break;
            }

            return null;
        }

        public IEnumerable<Point> GetAllAdjacent()
        {
            foreach (eDirection direction in (eDirection[]) Enum.GetValues(typeof(eDirection)))
            {
                if (direction == eDirection.None)
                { 
                    continue;
                }

                yield return GetAdjacent(direction);
            }
        }
    }
}