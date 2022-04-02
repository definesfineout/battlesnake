using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starter.Core.Enumerations
{
    public enum eDirection
    {
        None,
        Up,
        Right,
        Down,
        Left
    }


    public static class eDirectionExtensions
    {
        public static eDirection Opposite(this eDirection direction)
        {
            switch (direction)
            {
                case eDirection.Up: return eDirection.Down;
                case eDirection.Down: return eDirection.Up;
                case eDirection.Left: return eDirection.Right;
                case eDirection.Right: return eDirection.Left;
                case eDirection.None: default: break;
            }

            return eDirection.None;
        }
    }
}
