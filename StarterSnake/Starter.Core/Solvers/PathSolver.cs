using Starter.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starter.Core.Solvers
{
    public class PathSolver : BaseSolver
    {
        public List<List<TableCell>> Table { get; set; } = new();

        public PathSolver(Board board, Snake me)
            : base(board, me)
        {
            for (var i = 0; i < Board.Width; i++)
            {
                Table.Add(new List<TableCell>());
                for (var j = 0; j < Board.Height; j++)
                {
                    Table[i].Add(new TableCell());
                }
            }
        }

        public List<Direction> ShortestPathToFood()
        {
            return PathTo(FindClosestFood(), ePathType.Shortest);
        }

        public List<Direction> LongestPathToTail()
        {
            return PathTo(Me.Tail, ePathType.Longest);
        }

        public List<Direction> PathTo(Point destination, ePathType pathType)
        {
            if (pathType == ePathType.Shortest)
            {
                return ShortestPathTo(destination);
            }

            return LongestPathTo(destination);
        }

        public List<Direction> ShortestPathTo(Point destination)
        {
            ResetTable();

            var head = Me.Head;
            Table[head.X][head.Y].Distance = 0;
            
            var queue = new List<Point>();
            queue.Add(head);
            
            while (queue.Any())
            {
                var cur = queue[0];
                queue.RemoveAt(0);
                if (cur == destination)
                {
                    return BuildPath(head, destination);
                }

                var firstDirection = Direction.None;
                if (cur == head)
                {
                    firstDirection = Me.Direction;
                }
                else
                {
                    firstDirection = Table[cur.X][cur.Y].Parent.GetDirectionTo(cur);
                }
                
                // Traverse our "first" direction first, then randomly
                var random = new Random();
                var adjacents = cur.GetAllAdjacent()
                    .OrderBy(a => random.Next())
                    .ToList();
                for (var i = 0; i < adjacents.Count; i++)
                {
                    if (firstDirection != cur.GetDirectionTo(adjacents[i]))
                    {
                        continue;
                    }
                    
                    (adjacents[i], adjacents[0]) = (adjacents[0], adjacents[i]);
                    break;
                }

                // Traverse adjacent positions
                foreach (var position in adjacents)
                {
                    if (!IsValid(position))
                    {
                        continue;
                    }

                    var cell = Table[position.X][position.Y];
                    if (cell.Distance != int.MaxValue)
                    {
                        continue;
                    }
                    cell.Parent = cur;
                    cell.Distance = Table[cur.X][cur.Y].Distance + 1;
                }
            }

            return null;
        }

        public List<Direction> LongestPathTo(Point destination)
        {
            var path = ShortestPathTo(destination);
            if (path == null)
            {
                return null;
            }
            
            ResetTable();
            
            // Set all shortest path positions as visited
            var head = Me.Head;
            var cur = head;
            Table[cur.X][cur.Y].Visited = true;
            foreach (var direction in path)
            {
                cur = cur.GetAdjacent(direction);
                Table[cur.X][cur.Y].Visited = true;
            }

            // Extend path between each pair of positions
            (var idx, cur) = (0, head);
            while (true)
            {
                var curDirection = path[idx];
                var next = cur.GetAdjacent(curDirection);
                Direction[] tests;
                if (curDirection == Direction.Left
                    || curDirection == Direction.Right)
                {
                    tests = new [] { Direction.Up, Direction.Down};
                }
                else
                {
                    tests = new[] { Direction.Left, Direction.Right };
                }

                var extended = false;
                foreach (var testDirection in tests)
                {
                    var curTest = cur.GetAdjacent(testDirection);
                    var nextTest = next.GetAdjacent(testDirection);
                    if (!IsValid(curTest) || !IsValid(nextTest))
                    {
                        continue;
                    }

                    Table[curTest.X][curTest.Y].Visited = true;
                    Table[nextTest.X][nextTest.Y].Visited = true;
                    path.Insert(idx, testDirection);
                    path.Insert(idx + 2, testDirection.Opposite());
                    extended = true;
                    break;
                }

                if (!extended)
                {
                    cur = next;
                    idx++;
                    if (idx > path.Count)
                    {
                        break;
                    }
                }
            }

            return path;
        }

        private void ResetTable()
        {
            foreach (var col in Table)
            {
                foreach (var cell in col)
                {
                    cell.Reset();
                }
            }
        }

        private List<Direction> BuildPath(Point source, Point destination)
        {
            var path = new List<Direction>();
            var position = destination;
            while (position != source)
            {
                var parent = Table[position.X][position.Y].Parent;
                path.Insert(0, parent.GetDirectionTo(position));
                position = parent;
            }

            return path;
        }

        private bool IsValid(Point position)
        {
            return Board.IsPointSafe(position)
                && !Table[position.X][position.Y].Visited;
        }

        public override Direction GetNextDirection()
        {
            return LongestPathToTail().FirstOrDefault();
        }
    }

    public enum ePathType
    {
        Shortest,
        Longest
    }

    public class TableCell
    {
        public Point Parent { get; set; }
        public int Distance { get; set; }
        public bool Visited { get; set; }

        public TableCell()
        {
            Reset();
        }

        public void Reset()
        {
            Parent = null;
            Distance = int.MaxValue;
            Visited = false;
        }
    }
}
