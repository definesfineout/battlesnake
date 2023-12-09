using Graph.Algorithms;
using Starter.Core;
using System.Diagnostics;

namespace TestSnakeStuff
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // empty 10x10
            //var board = new Board
            //{
            //    Height = 10,
            //    Width = 10
            //};

            // 11x11 (standard) with 2 snakes and 1 hazard
            //var board = new Board
            //{
            //    Height = 11,
            //    Width = 11,
            //    Hazards = new[] {new Point(0,0)},
            //    Snakes = new[] {
            //        new Snake { Body = new List<Point> { new Point(2,2), new Point(2, 3), new Point(2, 4), new Point(2, 5) } },
            //        new Snake { Body = new List<Point> { new Point(5,2), new Point(6, 2), new Point(7, 2), new Point(7, 3) } }
            //    }
            //};

            // 19x19 (large) with 2 snakes and 1 hazard
            // TODO: reproduce realistic 4-snake game
            var board = new Board
            {
                Height = 19,
                Width = 19,
                Hazards = new[] { new Point(0, 0) },
                Snakes = new[] {
                    new Snake { Body = new List<Point> { new Point(2,2), new Point(2, 3), new Point(2, 4), new Point(2, 5) } },
                    new Snake { Body = new List<Point> { new Point(5,2), new Point(6, 2), new Point(7, 2), new Point(7, 3) } }
                }
            };

            var graph = board.ToGraph();

            Console.WriteLine($"{board.Width} x {board.Height} Populated Board as Graph:\r\n{graph}\r\n");

            var ld = new LizhiDu(graph);
            var sw = Stopwatch.StartNew();
            ld.Search();
            sw.Stop();

            if (graph.Hc == null)
            {
                Console.WriteLine("No Hamilton cycle found.");
            }
            else
            {
                Console.WriteLine($"Hamilton cycle found: {string.Join(' ', graph.Hc)}\r\n");
                Console.WriteLine($"Steps: {graph.Steps}");
                Console.WriteLine($"Milliseconds: {sw.ElapsedMilliseconds}");
            }

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }
}