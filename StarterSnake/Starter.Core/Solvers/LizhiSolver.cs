using Graph.Algorithms;
using Starter.Core.Enumerations;
using System;
using System.Diagnostics;

namespace Starter.Core.Solvers
{
    public class LizhiSolver : BaseSolver
    {
        private readonly Graph.Graph Graph;

        public bool CycleFound { get; private set; } = false;
        public bool PathSafe { get; private set; } = false;

        public long ElapsedMilliseconds { get; private set; }
        public int Steps => Graph.Steps;

        public LizhiSolver(Board board, Snake me)
            : base(board, me)
        {
            Graph = Board.ToGraph();
        }

        public override Direction GetNextDirection()
        {
            var sw = Stopwatch.StartNew();
            var algo = new LizhiDu(Graph);
            algo.Search();
            sw.Stop();
            ElapsedMilliseconds= sw.ElapsedMilliseconds;

            if (Graph.Hc != null)
            {
                CycleFound= true;
                PathSafe= true;

                // Find my current position in the path and take the next index
                var myIndex = Board.GraphIdFromPoint(Me.Head!);
                var myPathPos = Graph.Hc.IndexOf(myIndex);
                (Point a, Point b) options;
                if (myPathPos == 0)
                {
                    options = (
                        Board.PointFromGraphId(Graph.Hc[myPathPos + 1]),
                        Board.PointFromGraphId(Graph.Hc[^1]));
                }
                else if (myPathPos == Graph.Hc.Count)
                {
                    options = (
                        Board.PointFromGraphId(Graph.Hc[0]),
                        Board.PointFromGraphId(Graph.Hc[myPathPos - 1]));
                }
                else
                {
                    options = (
                        Board.PointFromGraphId(Graph.Hc[myPathPos + 1]),
                        Board.PointFromGraphId(Graph.Hc[myPathPos - 1]));
                }

                // Return direction from current position to next point in the path
                if (Board.IsPointSafe(options.a))
                {
                    return Me.Head!.GetDirectionTo(options.a);
                }
                if (Board.IsPointSafe(options.b))
                {
                    return Me.Head!.GetDirectionTo(options.b);
                }
            }
            PathSafe = false;

            // Default to NaiveSolver if no safe path found
            var naiveSolver = new NaiveSolver(Board, Me);
            return naiveSolver.GetNextDirection();
        }
    }
}
