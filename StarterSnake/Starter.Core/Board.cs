﻿using System.Collections.Generic;
using System.Linq;

namespace Starter.Core
{
    /// <summary>
    /// The game board is represented by a standard 2D grid, oriented with (0,0) in the bottom left.
    /// The Y-Axis is positive in the up direction, and X-Axis is positive to the right.
    /// Coordinates begin at zero, such that a board that is 12x12 will have coordinates ranging from [0, 11].
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Height of the game board.
        /// Example: 11
        /// </summary>
        public int Height { get; set; }

        
        /// <summary>
        /// Width of the game board.
        /// Example: 11
        /// </summary>
        public int Width { get; set; }
    
        
        /// <summary>
        /// Array of coordinates representing food locations on the game board.
        /// Example: [{"x": 5, "y": 5}, ..., {"x": 2, "y": 6}]
        /// </summary>
        public IEnumerable<Point> Food { get; set; }

        
        /// <summary>
        /// Array of <see cref="Snake"/>s representing all Battlesnakes remaining on the game
        /// board (including yourself if you haven't been eliminated).
        /// Example: [{"id": "snake-one", ...}, ...]
        /// </summary>
        public IEnumerable<Snake> Snakes { get; set; }

        /// <summary>
        /// Array of coordinates representing hazardous locations on the game board. These
        /// will only appear in some game modes.
        /// Example: [{"x": 0, "y": 0}, ..., {"x": 0, "y": 1}]
        /// </summary>
        public IEnumerable<Point> Hazards { get; set; }
        
        public ePointType GetPointType(Point point)
        {
            if (Food.Any(f => f.X == point.X
                           && f.Y == point.Y))
            {
                return ePointType.Food;
            }
            
            if (Snakes.Any(s =>
                    s.Body.Any(b => b.X == point.X
                                 && b.Y == point.Y)))
            {
                return ePointType.Snake;
            }

            if (Hazards.Any(h => h.X == point.X
                              && h.Y == point.Y))
            {
                return ePointType.Hazard;
            }

            return ePointType.Empty;
        }

        public bool IsPointInside(Point point)
        {
            return point.X >= 0 && point.X < Width
                && point.Y >= 0 && point.Y < Height;
        }

        public bool IsPointSafe(Point point)
        {
            var type = GetPointType(point);
            return IsPointInside(point)
                && (type == ePointType.Food
                    || type == ePointType.Empty);
        }
    }

    public enum ePointType
    {
        Empty,
        Food,
        Snake,
        Hazard
    }
}