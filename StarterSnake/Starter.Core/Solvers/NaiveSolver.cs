using Starter.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starter.Core.Solvers
{
    public class NaiveSolver : BaseSolver
    {
        public List<Direction> ValidDirections
        {
            get
            {
                return GetValidDirections().ToList();
            }
        }

        public NaiveSolver(Board board, Snake me)
            : base(board, me)
        { }

        public override Direction GetNextDirection()
        {
            var direction = GetValidDirections().ToList();

            // If we have no directions, ditch right
            if (direction.Count == 0)
            {
                return Direction.Right;
            }
            
            // Otherwise, pick a random valid direction
            var rng = new Random();
            
            // Filter to direction that gets us closer to food (greedy mode activate!)
            var greedyDirections = direction.Where(d => IsDirectionCloserToFood(d)).ToList();
            if (!greedyDirections.Any())
            {
                greedyDirections = direction.Where(d => !IsDirectionFartherFromFood(d)).ToList();
            }
            if (greedyDirections.Any())
            {
                return greedyDirections[rng.Next(greedyDirections.Count)];
            }

            return direction[rng.Next(direction.Count)];
        }

        public bool IsDirectionCloserToFood(Direction direction)
        {
            var target = FindClosestFood();
            if (target == null)
            {
                //TODO: Example why-you-go-up.json - this was previously `return false;`
                //      and the Silly SillyBoi went up. Why u do dis?
                return true;
            }

            switch(direction)
            {
                case Direction.Up:
                    if (Me.Head.Y < target.Y) return true;
                    break;
                case Direction.Down:
                    if (Me.Head.Y > target.Y) return true;
                    break;
                case Direction.Left:
                    if (Me.Head.X > target.X) return true;
                    break;
                case Direction.Right:
                    if (Me.Head.X < target.X) return true;
                    break;
            }

            return false;
        }

        public bool IsDirectionFartherFromFood(Direction direction)
        {
            var target = FindClosestFood();
            if (target == null)
            {
                return false;
            }

            switch (direction)
            {
                case Direction.Up:
                    if (Me.Head.Y > target.Y) return true;
                    break;
                case Direction.Down:
                    if (Me.Head.Y < target.Y) return true;
                    break;
                case Direction.Left:
                    if (Me.Head.X < target.X) return true;
                    break;
                case Direction.Right:
                    if (Me.Head.X > target.X) return true;
                    break;
            }

            return false;
        }
    }
}
