using Starter.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starter.Core.Solvers
{
    public abstract class BaseSolver
    {
        protected readonly Board Board;
        protected readonly Snake Me;

        protected BaseSolver(Board board, Snake me)
        {
            Board = board;
            Me = me;
        }

        protected IEnumerable<eDirection> GetValidDirections()
        {
            if (Board == null || Me == null)
            {
                yield break;
            }

            if (Me.Head.X < (Board.Width - 1)
                && !Board.Snakes
                        .Any(s =>
                            s.Body.Any(p =>
                                p.X == Me.Head.X + 1
                                && p.Y == Me.Head.Y)))
            {
                yield return eDirection.Right;
            }
            if (Me.Head.X > 0
                && !Board.Snakes.Any(s => s.Body.Any(p => p.X == Me.Head.X - 1 && p.Y == Me.Head.Y)))
            {
                yield return eDirection.Left;
            }
            if (Me.Head.Y < (Board.Height - 1)
                && !Board.Snakes.Any(s => s.Body.Any(p => p.X == Me.Head.X && p.Y == Me.Head.Y + 1)))
            {
                yield return eDirection.Up;
            }
            if (Me.Head.Y > 0
                && !Board.Snakes.Any(s => s.Body.Any(p => p.X == Me.Head.X && p.Y == Me.Head.Y - 1)))
            {
                yield return eDirection.Down;
            }
        }

        public Point FindClosestFood()
        {
            if (!Board.Food.Any())
            {
                return null;
            }

            var distances = new Dictionary<Point, int>();
            foreach (var food in Board.Food)
            {
                distances[food] = Math.Abs(Me.Head.X - food.X) +
                    Math.Abs(Me.Head.Y - food.Y);
            }

            return distances.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
        }

        public abstract eDirection GetNextDirection();
    }
}
