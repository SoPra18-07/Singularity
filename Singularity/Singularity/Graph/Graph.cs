﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Singularity.Graph
{
    /// <summary>
    /// Provides a basic Graph object, holding a list of nodes and edges. Self explanatory
    /// </summary>
    [DataContract]
    public sealed class Graph
    {
        [DataMember]
        private readonly List<INode> mNodes;
        [DataMember]
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

        /* ReSharper
        public void AddNodes(IEnumerable<INode> nodes)
        {
            foreach(var node in nodes)
            {
                AddNode(node);
            }
        } */

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

        /* ReSharper
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
        // */

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

            if (!(graph.GetNodes().TrueForAll(GetNodes().Contains) && graph.GetNodes().Count == GetNodes().Count))
            {
                return false;
            }

            if (!(graph.GetEdges().TrueForAll(GetEdges().Contains) && graph.GetEdges().Count == GetEdges().Count))
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
