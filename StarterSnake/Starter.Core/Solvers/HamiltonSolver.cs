using Starter.Core.Enumerations;
using System;

namespace Starter.Core.Solvers
{
    public class HamiltonSolver : BaseSolver
    {
        private readonly int[] path;
        private readonly AdjacencyMatrix matrix;
        private readonly NaiveSolver naiveSolver;

        public HamiltonSolver(Board board, Snake me)
            : base(board, me)
        {
            path = new int[board.Width * board.Height];
            matrix = new AdjacencyMatrix(board);
            naiveSolver = new NaiveSolver(board, me);
        }

        public override Direction GetNextDirection()
        {
            if (HasHamiltonianCycle())
            {
                var destination = matrix.GetPointAtIndex(path[1]);
                return Me.Head.GetDirectionTo(destination);
            }
            return naiveSolver.GetNextDirection();
        }

        private bool IsValid(int v, int k)
        {
            // If no edge, can't do
            if (!matrix.HasEdge(path[k-1], v))
            {
                return false;
            }

            for (var i = 0; i < k; i++)
            {
                if (path[i] == v)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsCycleFound(int k)
        {
            if (k == path.Length)
            {
                return matrix.HasEdge(path[k - 1], path[0]);
            }

            for (var v = 1; v < path.Length; v++)
            {
                if (IsValid(v,k))
                {
                    path[k] = v;
                    if (IsCycleFound(k+1))
                    {
                        return true;
                    }
                    path[k] = -1;
                }
            }

            return false;
        }

        private bool HasHamiltonianCycle()
        {
            Array.Fill(path, -1);
            path[0] = 0;

            return IsCycleFound(1);
        }
    }
}
