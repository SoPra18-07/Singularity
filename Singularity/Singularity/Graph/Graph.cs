using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Graph
{

    public class Graph
    {
        private readonly List<INode> mNodes;

        private readonly List<IEdge> mEdges;

        public Graph(List<INode> nodes = null, List<IEdge> edges = null)
        {
            mNodes = new List<INode>();
            mEdges = new List<IEdge>();

            if (nodes != null)
            {
                mNodes = nodes;
            }

            if (edges != null)
            {
                mEdges = edges;
            }
            
        }

        public void AddNode(INode node)
        {
            mNodes.Add(node);
        }

        public void AddEdge(IEdge edge)
        {
            mEdges.Remove(edge);
        }

        public void AddNodes(IEnumerable<INode> nodes)
        {
            foreach(var node in nodes)
            {
                AddNode(node);
            }
        }

        public void AddEdges(IEnumerable<IEdge> edges)
        {
            foreach (var edge in edges)
            {
                AddEdge(edge);
            }
        }

        public void RemoveNode(INode node)
        {
            mNodes.Remove(node);
        }

        public void RemoveEdge(IEdge edge)
        {
            mEdges.Remove(edge);
        }

        public void RemoveNodes(IEnumerable<INode> nodes)
        {
            foreach (var node in nodes)
            {
                RemoveNode(node);
            }
        }

        public void RemoveEdges(IEnumerable<IEdge> edges)
        {
            foreach (var edge in edges)
            {
                RemoveEdge(edge);
            }
        }

        public List<INode> GetNodes()
        {
            return mNodes;
        }

        public List<IEdge> GetEdges()
        {
            return mEdges;
        }

    }
}
