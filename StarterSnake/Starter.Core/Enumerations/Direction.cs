using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starter.Core.Enumerations
{
    public enum Direction
    {
        None,
        Up,
        Right,
        Down,
        Left
    }


    public static class eDirectionExtensions
    {
        public static Direction Opposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                case Direction.None: default: break;
            }

            return Direction.None;
        }
    }
}
