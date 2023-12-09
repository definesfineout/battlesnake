using Graph;

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
        public IEnumerable<Point>? Food { get; set; }


        /// <summary>
        /// Array of <see cref="Snake"/>s representing all Battlesnakes remaining on the game
        /// board (including yourself if you haven't been eliminated).
        /// Example: [{"id": "snake-one", ...}, ...]
        /// </summary>
        public IEnumerable<Snake>? Snakes { get; set; }

        /// <summary>
        /// Array of coordinates representing hazardous locations on the game board. These
        /// will only appear in some game modes.
        /// Example: [{"x": 0, "y": 0}, ..., {"x": 0, "y": 1}]
        /// </summary>
        public IEnumerable<Point>? Hazards { get; set; }

        public PointType GetPointType(Point point)
        {
            if (Food?.Any(f => f.X == point.X
                           && f.Y == point.Y) ?? false)
            {
                return PointType.Food;
            }

            if (Snakes?.Any(s =>
                    s.Body?.Any(b => b.X == point.X
                                 && b.Y == point.Y)
                    ?? false)
                ?? false)
            {
                return PointType.Snake;
            }

            if (Hazards?.Any(h => h.X == point.X
                              && h.Y == point.Y) ?? false)
            {
                return PointType.Hazard;
            }

            return PointType.Empty;
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
                && (type == PointType.Food
                    || type == PointType.Empty);
        }

        private Dictionary<int, Point>? GraphIdToPoint;
        public Graph.Graph ToGraph()
        {
            //TODO: floodfill to graph only connected area; allow searching cycle within smaller, bounded area

            GraphIdToPoint = new Dictionary<int, Point>();

            var graph = new Graph.Graph();
            int id = 0;
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var p = new Point(x, y);

                    // omit "unsafe" points on the board from the graph (hazards and snakes)
                    if (!IsPointSafe(p))
                        continue;

                    var a = new List<int>();

                    // Add adjacencies, checking w/ IsPointSafe to omit hazards, snakes, and OoB
                    // Note we only search left and down as those are the Ids we know so far;
                    // we also fill in the reciprocal adjacencies on those points moving right and up.

                    // Ex. if we can move left safely, we can also move right from the left adjacency
                    if (IsPointSafe(p.GetAdjacent(Enumerations.Direction.Left)!))
                    {
                        // observe that left is always the immediately previous index
                        var leftPointId = id - 1;

                        a.Add(leftPointId); // left from here
                        graph.nodes[leftPointId].Adj.Add(id); // right from the left adjacency
                    }
                    //if (IsPointSafe(p.GetAdjacent(Enumerations.Direction.Right)!)) a.Add(id + 1); // right

                    // And, if we ccan move down safely, we can also move up from the down adjacency
                    var downPoint = p.GetAdjacent(Enumerations.Direction.Down);
                    if (IsPointSafe(downPoint!))
                    {
                        // "look back" at the ID we gave the adjacent point (down from here)
                        var downPointId = GraphIdFromPoint(downPoint!);

                        a.Add(downPointId); // down from here
                        graph.nodes[downPointId].Adj.Add(id); // up from the down adjacency
                    }
                    //if (IsPointSafe(p.GetAdjacent(Enumerations.Direction.Up)!)) a.Add(id + Width); // up

                    graph.nodes.Add(new Node(id, a)); // observe/assert that graph.nodes[N] = node{id=N}
                    GraphIdToPoint[id] = p;
                    id++;
                }
            }

            return graph;
        }

        public Point PointFromGraphId(int id)
        {
            // Ensure graph has been produced (though not sure why we'd be here if not)
            if (GraphIdToPoint == null) _ = ToGraph();

            // Ensure Id exists in the graph
            if (!GraphIdToPoint!.ContainsKey(id))
                throw new ArgumentOutOfRangeException(nameof(id));

            return GraphIdToPoint[id];
        }

        public int GraphIdFromPoint(Point p)
        {
            // Ensure graph has been produced (though not sure why we'd be here if not)
            if (GraphIdToPoint == null) _ = ToGraph();

            // Ensure Point exists in the graph
            if (!GraphIdToPoint!.ContainsValue(p))
                throw new ArgumentOutOfRangeException(nameof(p));

            return GraphIdToPoint.First(kvp => kvp.Value.Equals(p)).Key;
        }
    }

    public enum PointType
    {
        Empty,
        Food,
        Snake,
        Hazard
    }
}