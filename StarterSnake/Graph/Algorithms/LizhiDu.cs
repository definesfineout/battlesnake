using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Algorithms
{
    /// <summary>
    /// Decoupled implementation of Lizhi Du's algorithm for Hamiltonian cycle, modified
    /// from the work of Alexander Chernosvitov and Ivan Fedorov.
    /// UI-related code has been removed. Original comment annotations retained.
    /// 
    /// Original heading comment:
    /// Illustrates the ingenious algorithm for searching a Hamiltonian cycle in undirected graphs
    /// The algorithm was created by prof. Lizhi Du (Wuhan University of Science and Technology. China)
    /// https://www.researchgate.net/publication/272158931_A_polynomian_time_algorithm_for_Hamilton_cycle_and_its_detailed_proof_newest_version
    /// This implementation of the algorithm is done by Alexander Chernosvitov and Ivan Fedorov (Russia)
    /// https://www.codeproject.com/Members/Alexander-Chernosvitov
    /// </summary>
    public class LizhiDu
    {
        #region Data
        readonly List<int> hc;  // Hamiltonian cycle
        //GraphView gv;
        readonly Graph graph;
        readonly int[,]
            adj,    // Adjacency matrix
            breaks; // Breakpoint matrix
        readonly int nodeCount;
        int
            breakCount,     // Breakpoints counter
            n0, n1, n2, n3; // Search positions within the cycle 
        #endregion

        //public LizhiDu(GraphView g) { gv = g; graph = g.Graph; nodeCount = g.Count; }
        public LizhiDu(Graph g) {
            graph = g;
            nodeCount = g.NodeCount;
            breakCount = 0;
            adj = graph.GetAjacencyMatrix();
            hc = graph.Hc = new List<int>(nodeCount);  // Hamilton cycle 
            breaks = new int[nodeCount, nodeCount]; // Breakpoints matrix
        }

        #region Algo
        public void Search() // Search Hamiltonian cycle with Lizhi Du Algorithm
        {
            Init(); // Count breakpoins, mark adjacency matrix and prepare the list of Hamiltonian cycle nodes
            graph.Steps = 0;
            n0 = 0;
            while (breakCount != 0)
            {
                if (adj[hc[n0], hc[(n0 + 1) % nodeCount]] != 2) // Two neighbors connected ?
                {
                    n0 = (n0 + 1) % nodeCount; // Not a breakpoint, go to next pair
                    continue;
                }
                graph.Steps++;
                if (!TryToMend()) // Breakpoint found. Try to mend it
                {
                    graph.Hc = null;
                    return;
                }
                // This operation might not be needed ??
                for (int i = 0; i < nodeCount; i++) // Next Try. Clear breakpoints matrix.
                    for (int j = 0; j < nodeCount; j++)
                        breaks[i, j] = 0;
            }
            //DrawResult();
        }

        void Init()
        {
            for (int i = 0; i < nodeCount; i++) // Broad cycle (by Lizhi Du)
                hc.Add(i);

            // A pair of nodes is considered a breakpoint if some node is not adjacent to its consecutive neighbor
            for (int i = 0; i < nodeCount; i++) // Mark all breakpoint node pairs in adjacency matrix.
            {
                int j = (i + 1) % nodeCount; // a consecutive neighbor
                if (adj[i, j] == 0)  // We found a breakpoint
                {
                    adj[i, j] = adj[j, i] = 2; // Mark both nodes in adjacency matrix
                    breakCount++;
                }
                //else
                //    gv.OnEdgeStateChanged(true, i, j); // Show in GraphView that this edge is a candidate to Hamilton cycle
            }
        }

        bool TryToMend()
        {
            while (true) // While breakpoint not mended
            {
                if (graph.ShouldStop)
                    return false;
                n1 = hc[n0];
                n2 = hc[(n0 + 1) % nodeCount];
                n3 = (n0 + 2) % nodeCount;
                breaks[n1, n2] = breaks[n2, n1] = 1; // There is a break

                if (!TryFirst())
                {
                    if (!TrySecond())
                        if (!TryThird())
                            return false; // Graph has no Hamilton cycles
                }
                else if (BreakMended()) // No more breakpoints from this point of view
                    return true;
            }
        }

        bool BreakMended()
        {
            for (int i = 0; i < nodeCount; i++)
            {
                if (breaks[hc[i], hc[(i + 1) % nodeCount]] == 2)
                {
                    n0 = i; // Move to next breakpoint
                    return false;
                }
            }
            return true;
        }

        bool TryFirst()
        {
            for (int i = n3; i != n0; i = (i + 1) % nodeCount) // go from n3 to n0
            {
                if (adj[hc[i], n2] != 0)
                {
                    int a = hc[(i + 1) % nodeCount];
                    for (int j = n3; j != i; j = (j + 1) % nodeCount)
                    {
                        int
                            b = hc[j],
                            c = hc[(j + 1) % nodeCount];
                        if (adj[a, b] != 0 && adj[n1, c] != 0)
                        {
                            CutAndInsert(i, j, false); // No rotation. --- CutAndInsert modifies adj and hc
                            return true;
                        }
                        if (adj[a, c] != 0 && adj[n1, b] != 0) // Rotation
                        {
                            CutAndInsert(i, j, true);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        bool TrySecond()
        {
            for (int i = n3; i != n0; i = (i + 1) % nodeCount)
            {
                int
                    a = hc[i],
                    b = hc[(i + 1) % nodeCount];
                if (adj[a, n2] != 0)
                {
                    for (int j = n3; j != i; j = (j + 1) % nodeCount)
                    {
                        int
                            c = hc[j],
                            d = hc[(j + 1) % nodeCount];
                        if (adj[b, c] != 0 && adj[n1, d] == 0 && breaks[n1, d] != 1)
                        {
                            CutAndInsert(i, j, false);
                            n0 = hc.IndexOf(n1);
                            return true;
                        }
                        if (adj[b, c] == 0 && adj[n1, d] != 0 && breaks[b, c] != 1)
                        {
                            CutAndInsert(i, j, false);
                            n0 = hc.IndexOf(c);
                            return true;
                        }
                        if (adj[b, d] != 0 && adj[n1, c] == 0 && breaks[n1, c] != 1)
                        {
                            CutAndInsert(i, j, true);
                            n0 = hc.IndexOf(c);
                            return true;
                        }
                        if (adj[b, d] == 0 && adj[n1, c] != 0 && breaks[b, d] != 1)
                        {
                            CutAndInsert(i, j, true);
                            n0 = hc.IndexOf(b);
                            return true;
                        }
                    }
                }
                else if (breaks[a, n2] != 1)
                {
                    for (int j = n3; j != i; j = (j + 1) % nodeCount)
                    {
                        int
                            c = hc[j],
                            d = hc[(j + 1) % nodeCount];
                        if (adj[b, c] != 0 && adj[n1, d] != 0)
                        {
                            CutAndInsert(i, j, false);
                            n0 = hc.IndexOf(a);
                            return true;
                        }
                        if (adj[b, d] != 0 && adj[n1, c] != 0)
                        {
                            CutAndInsert(i, j, true);
                            n0 = hc.IndexOf(a);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        bool TryThird()
        {
            for (int i = n3; i != n0; i = (i + 1) % nodeCount)
            {
                int
                    a = hc[i],
                    b = hc[(i + 1) % nodeCount];
                if (adj[a, n2] == 0)
                {
                    for (int j = n3; j != i; j = (j + 1) % nodeCount)
                    {
                        int
                            c = hc[j],
                            d = hc[(j + 1) % nodeCount];
                        if (adj[b, c] != 0 && adj[n1, d] == 0 && breaks[n1, d] != 1 && breaks[a, n2] != 1)
                        {
                            breaks[a, n2] = breaks[n2, a] = 2;
                            CutAndInsert(i, j, false);
                            n0 = hc.IndexOf(n1);
                            return true;
                        }
                        if (adj[b, c] == 0 && adj[n1, d] != 0 && breaks[b, c] != 1 && breaks[a, n2] != 1)
                        {
                            breaks[a, n2] = breaks[n2, a] = 2;
                            CutAndInsert(i, j, false);
                            n0 = hc.IndexOf(c);
                            return true;
                        }
                        if (adj[b, d] != 0 && adj[n1, c] == 0 && breaks[n1, c] != 1 && breaks[a, n2] != 1)
                        {
                            breaks[a, n2] = breaks[n2, a] = 2;
                            CutAndInsert(i, j, true);
                            n0 = hc.IndexOf(c);
                            return true;
                        }
                        if (adj[b, d] == 0 && adj[n1, c] != 0 && breaks[b, d] != 1 && breaks[a, n2] != 1)
                        {
                            breaks[a, n2] = breaks[n2, a] = 2;
                            CutAndInsert(i, j, true);
                            n0 = hc.IndexOf(b);
                            return true;
                        }
                    }
                }
                else
                {
                    for (int j = n3; j != i; j = (j + 1) % nodeCount)
                    {
                        int
                            c = hc[j],
                            d = hc[(j + 1) % nodeCount];
                        if (adj[b, c] == 0 && adj[n1, d] == 0 && breaks[n1, d] != 1 && breaks[b, c] != 1)
                        {
                            breaks[c, b] = breaks[b, c] = 2;
                            CutAndInsert(i, j, false);
                            n0 = hc.IndexOf(n1);
                            return true;
                        }
                        if (adj[b, d] == 0 && adj[n1, c] == 0 && breaks[b, d] != 1 && breaks[n1, c] != 1)
                        {
                            breaks[c, n1] = breaks[n1, c] = 2;
                            CutAndInsert(i, j, true);
                            n0 = hc.IndexOf(b);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void CutAndInsert(int i, int j, bool rotate)
        {
            int
                k = (i + 1) % nodeCount,
                m = (j + 1) % nodeCount,
                a = hc[n0],
                b = hc[(n0 + 1) % nodeCount],
                c = hc[k],
                d = hc[j],
                e = hc[m],
                f = hc[i];
            //if (adj[d, e] == 1)
            //	gv.OnEdgeStateChanged(false, d, e);
            //if (adj[f, c] == 1)
            //	gv.OnEdgeStateChanged(false, f, c);
            //if (adj[f, b] == 1)
            //	gv.OnEdgeStateChanged(true, f, b);
            //if (rotate)
            //{
            //	if (adj[d, a] == 1)
            //		gv.OnEdgeStateChanged(true, d, a);
            //	if (adj[c, e] == 1)
            //		gv.OnEdgeStateChanged(true, c, e);
            //}
            //else
            //{
            //	if (adj[d, c] == 1)
            //		gv.OnEdgeStateChanged(true, d, c);
            //	if (adj[a, e] == 1)
            //		gv.OnEdgeStateChanged(true, a, e);
            //}

            graph.Steps++;
            //Application.OpenForms[0].Invalidate();
            if (adj[a, b] == 2)
            {
                adj[a, b] = adj[b, a] = 0;
                breakCount--;
            }
            if (adj[f, c] == 2 && (i != m || !rotate))
            {
                adj[f, c] = adj[c, f] = 0;
                breakCount--;
            }
            if (adj[d, e] == 2)
            {
                adj[d, e] = adj[e, d] = 0;
                breakCount--;
            }
            List<int> copy;
            if (k <= n0)
            {
                copy = hc.GetRange(k, n0 - k + 1);
                if (m > n0)
                    m -= copy.Count;
                hc.RemoveRange(k, n0 - k + 1);
            }
            else
            {
                copy = hc.GetRange(k, hc.Count - k);
                copy.AddRange(hc.GetRange(0, n0 + 1));
                hc.RemoveRange(k, hc.Count - k);
                if (m > n0)
                    m -= n0 + 1;
                hc.RemoveRange(0, n0 + 1);
            }
            if (rotate)
                copy.Reverse();
            hc.InsertRange(m, copy);
        }
        //void DrawResult()
        //{
        //    gv.UnselectEadges();
        //    var last = graph.Hc.Count - 1;
        //    for (int i = 0; i < last; i++)
        //        gv.OnEdgeStateChanged(true, graph.Hc[i], graph.Hc[i + 1]);
        //    gv.OnEdgeStateChanged(true, graph.Hc[last], graph.Hc[0]);
        //    Application.OpenForms[0].Invalidate();
        //}
        #endregion
    }
}
