using System;
using System.Collections.Generic;
using System.Text;

namespace Graph
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Based upon work by Alexander Chernosvitov and Ivan Fedorov https://www.codeproject.com/Members/Alexander-Chernosvitov</remarks>
    public class Edge
    {
        public int V1 { get; private set; }
        public int V2 { get; private set; }
        public bool Selected { get; set; }
        public float Weight { get; set; }

        public Edge(int n1, int n2, float w)
        {
            V1 = Math.Min(n1, n2);
            V2 = Math.Max(n1, n2);
            Weight = w;
            Selected = false;
        }

        public string Save()
        {
            return $"{V1} {V2} {Weight:f2}, ";
        }

        internal void DecreaseID(int id)
        {
            if (V1 > id)
                V1--;
            if (V2 > id)
                V2--;
        }

        public override bool Equals(object o)
        {
            var e = o as Edge;
            return e != null && (V1 == e.V1 && V2 == e.V2 || V1 == e.V2 && V2 == e.V1);
        }

        public override int GetHashCode()
        {
            return V1.GetHashCode() + V2.GetHashCode();
        }
    }
}
