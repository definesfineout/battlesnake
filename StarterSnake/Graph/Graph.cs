using System;
using System.Collections.Generic;
using System.Text;

namespace Graph
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Based upon work by Alexander Chernosvitov and Ivan Fedorov https://www.codeproject.com/Members/Alexander-Chernosvitov</remarks>
    public class Graph
    {
        #region Data
        public List<Node> nodes;  // Node collection. Each node has a collection of adjacent nodes.
        //int[] bk; // Recursive Backtracking Hamiltonian cycle
        public int Steps; // # of steps
        //public string path; // Connectivity path
        //internal event Action<bool, int, int> EdgeStateChanged;

        public List<int>? Hc { get; set; } // Hamiltonian cycle for other algorithms
        internal int NodeCount { get { return nodes.Count; } }
        public bool ShouldStop { get; set; }
        internal List<Edge> Edges
        {
            get
            {
                var edges = new List<Edge>();
                for (int i = 0; i < nodes.Count; i++)
                {
                    var n = nodes[i];
                    foreach (var a in n.Adj)
                    {
                        var e = new Edge(i, a, 1);
                        if (!edges.Contains(e))
                            edges.Add(e);
                    }
                }
                return edges;
            }
        }
        #endregion

        #region Init
        public Graph()
        {
            ShouldStop = false;
            nodes = new List<Node>();
        }
        public Graph(List<Node> nodes, List<Edge> edges)
        {
            this.nodes = new List<Node>(nodes.Count);
            if (nodes.Count == 0)
                return;
            for (int i = 0; i < nodes.Count; i++)
            {
                var a = new List<int>(); // Adjacent nodes
                foreach (Edge e in edges)
                {
                    if (i == e.V1 && !a.Contains(e.V2))
                        a.Add(e.V2);
                    if (i == e.V2 && !a.Contains(e.V1))
                        a.Add(e.V1);
                }
                this.nodes.Add(new Node(i, a));
            }
        }
        public Graph(string s)
        {
            var tokens = s.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            nodes = new List<Node>(tokens.Length);
            int id = 0;
            foreach (string to in tokens)
            {
                var a = new List<int>(); // Adjacent nodes
                foreach (string t in to.Split(new char[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries))
                    a.Add(int.Parse(t));
                nodes.Add(new Node(id++, a));
            }
        }
        public void Connect(int n1, int n2)
        {
            var a1 = nodes[n1].Adj;
            var a2 = nodes[n2].Adj;
            if (!a1.Contains(n2))
                a1.Add(n2);
            if (!a2.Contains(n1))
                a2.Add(n1);
        }
        #endregion

        //#region Algo
        //public void BackTrack()
        //{
        //    bk = new int[NodeCount];
        //    for (int i = 0; i < NodeCount; i++)
        //        bk[i] = -1;
        //    bk[0] = 0;
        //    Steps = 0;
        //    if (!Recurse(1))
        //    {
        //        Hc = null;
        //        return;
        //    }
        //    Hc = bk.ToList();
        //}
        //bool Recurse(int p) // Used by BackTrack
        //{
        //    int pm = p - 1;
        //    int nm = bk[pm];
        //    var a = nodes[nm].Adj;
        //    if (p == NodeCount)
        //    {
        //        if (a[0] == 0)
        //        {
        //            EdgeStateChanged?.Invoke(true, nm, 0);
        //            return true;
        //        }
        //        return false;
        //    }

        //    for (int i = 1; i < NodeCount; i++)
        //    {
        //        if (ShouldStop)
        //            break;
        //        if (a.Contains(i) && !bk.Contains(i))
        //        {
        //            EdgeStateChanged?.Invoke(true, nm, i);
        //            bk[p] = i;
        //            if (Recurse(p + 1))
        //                return true;
        //            Steps++;
        //            bk[p] = -1;
        //            EdgeStateChanged?.Invoke(false, nm, i);
        //        }
        //    }
        //    return false;
        //}

        //public void RobertsFlores() // Roberts and Flores Algorithm
        //{
        //    Hc = new List<int>(NodeCount);  // Hamilton cycle
        //    Hc.Add(0);         // Add the first node node (arbitrary)
        //    Steps = 0;

        //    for (int add = 0, del = 0; Hc.Count <= NodeCount;) // Last added and deleted nodes in a chain
        //    {
        //        if (ShouldStop)
        //            break;
        //        add = Hc[Hc.Count - 1];
        //        List<int> a = null; // Nodes adjacent to last added node
        //        do
        //        {
        //            foreach (int i in nodes[add].Adj)
        //            {
        //                if (!Hc.Contains(i) && i > del)
        //                {
        //                    Hc.Add(i);
        //                    Steps++;
        //                    EdgeStateChanged?.Invoke(true, i, add);
        //                    add = i;
        //                    del = -1;
        //                    break;
        //                }
        //            }
        //            a = nodes[add].Adj;
        //        }
        //        while (!Subset(a, Hc) && !Sup(Diff(a, Hc), del));

        //        int last = Hc.Count - 1;
        //        if (Hc.Count == NodeCount && nodes[add].Adj.Contains(0)) // Cycle found
        //        {
        //            EdgeStateChanged?.Invoke(true, Hc[last], 0);
        //            return; // Ham cycle is found 
        //        }

        //        if (Hc.Count > 1)
        //        {
        //            del = Hc[last];
        //            Hc.RemoveAt(last);
        //            add = Hc[Hc.Count - 1];
        //            //Steps++;
        //            EdgeStateChanged?.Invoke(false, del, add);
        //        }

        //        if (Hc.Count <= 1)
        //        {
        //            var a0 = nodes[0].Adj;
        //            if (del >= a0[a0.Count - 1])
        //                break;
        //        }
        //    }
        //    Hc = null;
        //}

        //public void Posa() // Algorithm Posa 
        //{
        //    Hc = new List<int>(NodeCount);
        //    int startId = 0;
        //    Hc.Add(startId);  // Start with 0 node
        //    int last = Hc[startId]; // Last added node in HC chain
        //    List<int> a = nodes[last].Adj, dif;
        //    var rand = new Random();
        //    Steps = 0;
        //    while (Hc.Count < NodeCount)
        //    {
        //        if (ShouldStop)
        //            break;
        //        bool ok = true;
        //        while (Hc.Count < NodeCount && ok)
        //        {
        //            a = nodes[last].Adj; // Nodes adjacent to the last node 
        //            dif = Diff(a, Hc); // Nodes in adjacent list but not in HC chain 
        //            if (ok = dif.Count > 0)
        //            {
        //                Steps++;
        //                int j = dif[rand.Next(0, dif.Count)]; // Take a random node from dif and put it in HC chain
        //                Hc.Add(j);
        //                EdgeStateChanged?.Invoke(true, j, last);
        //                last = j;
        //            }
        //        }

        //        if (Hc.Count == NodeCount && nodes[last].Adj.Contains(startId)) // HC was found
        //        {
        //            EdgeStateChanged?.Invoke(true, Hc[Hc.Count - 1], startId); // Show the last edge
        //            return; // Ham cycle is found
        //        }

        //        int k = a[rand.Next(0, a.Count)];
        //        int id = IndexOfVertex(Hc, k) + 1;
        //        for (int i = 0; i < Hc.Count - 1; i++)
        //            EdgeStateChanged?.Invoke(false, Hc[i], Hc[i + 1]);

        //        var temp = new List<int>(NodeCount);
        //        foreach (var node in Hc)
        //            temp.Add(node);
        //        Hc.RemoveRange(id, Hc.Count - id);
        //        Hc = RotateTransform(temp, id - 1);
        //        last = Hc[Hc.Count - 1];
        //        for (int i = 0; i < Hc.Count - 1; i++)
        //            EdgeStateChanged?.Invoke(true, Hc[i], Hc[i + 1]);
        //        if (IsEqual(Hc, temp))
        //            break;
        //    }
        //    Hc = null;
        //}
        //List<int> RotateTransform(List<int> list, int k) // Used by Posa
        //{
        //    List<int> rt = new List<int>(list.Count);
        //    for (int i = 0, p = 0; i < list.Count; i++)
        //    {
        //        Steps++;
        //        rt.Add(p = list[i <= k ? i : list.Count - i + k]);
        //    }
        //    return rt;
        //}
        //#endregion

        #region Misc
        internal int[,] GetAjacencyMatrix()
        {
            var a = new int[NodeCount, NodeCount];
            for (int i = 0; i < NodeCount; i++)
            {
                for (int j = i + 1; j < NodeCount; j++)
                    a[i, j] = a[j, i] = nodes[i].Adj.Contains(j) ? 1 : 0;
            }
            return a;
        }

        //List<int> Diff(List<int> list1, List<int> list2)
        //{
        //    var list = new List<int>();
        //    foreach (int o in list1)
        //        if (!list2.Contains(o))
        //            list.Add(o);
        //    return list;
        //}

        //bool Subset(List<int> list, List<int> l2)
        //{
        //    foreach (int o in list)
        //        if (!l2.Contains(o))
        //            return false;
        //    return true;
        //}

        //bool Sup(List<int> list, int n) // Is n supremum of the list
        //{
        //    foreach (int id in list)
        //        if (id > n)
        //            return false;
        //    return true;
        //}

        //public Node GetNodeById(int n)
        //{
        //    if (0 <= n && n < nodes.Count)
        //        return nodes[n] as Node;
        //    return null;
        //}

        //public bool HasEdges()
        //{
        //    for (int i = 0; i < nodes.Count; i++)
        //    {
        //        for (int j = 0; j < nodes[i].Adj.Count; j++)
        //        {
        //            int id = nodes[i].Adj[j];
        //            if (!nodes[id].Adj.Contains(i))
        //                return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool HasHangers()
        //{
        //    for (int i = 0; i < nodes.Count; i++)
        //        if (nodes[i].Adj.Count <= 1)
        //            return true;
        //    return false;
        //}

        //int IndexOfVertex(List<int> list, int k) // Find index of a vertex in a list
        //{
        //    for (int i = 0; i < list.Count; i++)
        //        if (list[i] == k)
        //            return i;
        //    return -1;
        //}

        //public List<Node> SortByDegree() // Descending order
        //{
        //    var a = new Node[nodes.Count];
        //    nodes.CopyTo(a);
        //    int n = a.Length;
        //    bool sorted = false;

        //    for (int i = 1; !sorted; i++)
        //    {
        //        sorted = true;
        //        for (int j = n - 1, k = n - 2; j >= i; j--, k--)
        //        {
        //            int
        //                d2 = a[j].Adj.Count,
        //                d1 = a[k].Adj.Count;
        //            if (d2 > d1)
        //            {
        //                var t = a[j];
        //                a[j] = a[k];
        //                a[k] = t;
        //                sorted = false;
        //            }
        //        }
        //    }
        //    return a.ToList();
        //}

        //bool IsEqual(List<int> l1, List<int> l2)
        //{
        //    if (l1.Count == l2.Count)
        //    {
        //        for (int i = 0; i < l1.Count; i++)
        //            if (l1[i] != l2[i])
        //                return false;
        //    }
        //    else
        //        return false;
        //    return true;
        //}

        public override string ToString()
        {
            var s = "";
            foreach (Node n in nodes)
                s += $"{n}\r\n";
            s = s.TrimEnd('\n').TrimEnd('\r') + ";\r\n";
            return s;
        }
        #endregion
    }
}
