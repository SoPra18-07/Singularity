using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Singularity.Graph
{
    /// <summary>
    /// Provides a basic Graph object, holding a list of nodes and edges. Self explanatory
    /// </summary>
    public sealed class Graph
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
            mNodes.Add(item: node);
        }

        public void AddEdge(IEdge edge)
        {
            mEdges.Remove(item: edge);
        }

        public void AddNodes(IEnumerable<INode> nodes)
        {
            foreach(var node in nodes)
            {
                AddNode(node: node);
            }
        }

        public void AddEdges(IEnumerable<IEdge> edges)
        {
            foreach (var edge in edges)
            {
                AddEdge(edge: edge);
            }
        }

        public void RemoveNode(INode node)
        {
            mNodes.Remove(item: node);
        }

        public void RemoveEdge(IEdge edge)
        {
            mEdges.Remove(item: edge);
        }

        public void RemoveNodes(IEnumerable<INode> nodes)
        {
            foreach (var node in nodes)
            {
                RemoveNode(node: node);
            }
        }

        public void RemoveEdges(IEnumerable<IEdge> edges)
        {
            foreach (var edge in edges)
            {
                RemoveEdge(edge: edge);
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

        public override bool Equals(object obj)
        {
            var graph = obj as Graph;

            if (graph == null)
            {
                return false;
            }

            if (!(graph.GetNodes().TrueForAll(match: GetNodes().Contains) && graph.GetNodes().Count == GetNodes().Count))
            {
                return false;
            }

            if (!(graph.GetEdges().TrueForAll(match: GetEdges().Contains) && graph.GetEdges().Count == GetEdges().Count))
            {
                return false;
            }
            return true;

        }

        [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
        public override int GetHashCode()
        {
            return base.GetHashCode() + mNodes.GetHashCode() * mEdges.GetHashCode();
        }
    }
}
