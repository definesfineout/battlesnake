using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starter.Core
{
    public class AdjacencyMatrix
    {
        private readonly Board board;
        private readonly bool[,] matrix;

        public AdjacencyMatrix(Board board)
        {
            this.board = board;
            var numberOfVertices = board.Width * board.Height;
            matrix = new bool[numberOfVertices,numberOfVertices];
            BuildMatrix();
        }

        private void BuildMatrix()
        {
            for (int i = 0; i < board.Height; i++)
            {
                for (int j = 0; j < board.Width; j++)
                {
                    var index = GetIndexOfPoint(new Point(j, i));
                    var point = new Point(j, i);
                    var adjacent = point
                        .GetAllAdjacent()
                        .Where(a => board.IsPointSafe(a));

                    foreach (var adj in adjacent)
                    {
                        var adjIndex = GetIndexOfPoint(adj);
                        matrix[index, adjIndex] = true;
                        matrix[adjIndex, index] = true;
                    }
                }
            }
        }

        public int GetIndexOfPoint(Point point)
        {
            return point.Y * board.Width + point.X;
        }

        public Point GetPointAtIndex(int index)
        {
            var x = index % board.Width;
            var y = index / board.Width;
            return new Point(x, y);
        }

        public bool HasEdge(int v, int k)
        {
            return matrix[v, k];
        }
    }
}
