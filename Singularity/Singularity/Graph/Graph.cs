using System.Collections.Generic;

namespace Singularity.Graph
{
    /// <summary>
    /// Provides a basic Graph object, holding a list of nodes and edges. Self explanatory
    /// </summary>
    public class Graph
    {
        private readonly List<INode> _mNodes;

        private readonly List<IEdge> _mEdges;

        public Graph(List<INode> nodes = null, List<IEdge> edges = null)
        {
            _mNodes = new List<INode>();
            _mEdges = new List<IEdge>();

            if (nodes != null)
            {
                _mNodes = nodes;
            }

            if (edges != null)
            {
                _mEdges = edges;
            }

        }

        public void AddNode(INode node)
        {
            _mNodes.Add(node);
        }

        public void AddEdge(IEdge edge)
        {
            _mEdges.Remove(edge);
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
            _mNodes.Remove(node);
        }

        public void RemoveEdge(IEdge edge)
        {
            _mEdges.Remove(edge);
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
            return _mNodes;
        }

        public List<IEdge> GetEdges()
        {
            return _mEdges;
        }

    }
}
