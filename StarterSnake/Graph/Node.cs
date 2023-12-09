namespace Graph
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Based upon work by Alexander Chernosvitov and Ivan Fedorov https://www.codeproject.com/Members/Alexander-Chernosvitov</remarks>
    public class Node
    {
        public int ID;              // Vertex ID
        public List<int> Adj; // Adjacency list
        public Node(int id)
        {
            ID = id;
            Adj = new List<int>();
        }
        public Node(int id, List<int> adj)
        {
            ID = id;
            Adj = new List<int>(adj); //TODO: clone necessary?
            Adj.Sort(); //TODO: Needed?
        }

        //TODO: Used/needed?
        public override string ToString()
        {
            var s = "";
            foreach (int n in Adj)
                s += $"{n} ";
            s = s.TrimEnd(' ');
            return s;
        }
    }
}