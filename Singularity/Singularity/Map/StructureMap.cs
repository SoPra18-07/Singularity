using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Property;

namespace Singularity.Map
{
    /// <summary>
    /// A Structure map holds all the structures currently in the game. Additionally the structure map holds a graph
    /// representation of all the platforms and roads in the game. These graphes are used by pathfinding algorithms etc.
    /// </summary>
    public sealed class StructureMap : IDraw, IUpdate, IMousePositionListener
    {
        /// <summary>
        /// A list of all the platforms currently in the game
        /// </summary>
        private readonly LinkedList<PlatformBlank> mPlatforms;

        /// <summary>
        /// A list of all the roads in the game, identified by a source and destination platform.
        /// </summary>
        private readonly LinkedList<Road> mRoads;

        /// <summary>
        /// A list of all the platformPlacements in the game (the platforms following the mouse when building).
        /// </summary>
        private readonly LinkedList<PlatformPlacement> mPlatformsToPlace;

        /// <summary>
        /// The director for the game
        /// </summary>
        private readonly Director mDirector;

        /// <summary>
        /// A dictionary mapping platforms to the ID of the graph they are currently on
        /// </summary>
        private readonly Dictionary<PlatformBlank, int> mPlatformToGraphId;

        /// <summary>
        /// A dictionary mapping graph IDs to the graph object they belong to
        /// </summary>
        private readonly Dictionary<int, Graph.Graph> mGraphIdToGraph;

        /// <summary>
        /// The Fog of war of the current game
        /// </summary>
        private readonly FogOfWar mFow;

        /// <summary>
        /// The x coordinate of the mouse in world space
        /// </summary>
        private float mMouseX;

        /// <summary>
        /// The y coordinate of the mouse in world space
        /// </summary>
        private float mMouseY;

        /// <summary>
        /// Creates a new structure map which holds all the structures currently in the game.
        /// </summary>
        public StructureMap(FogOfWar fow, ref Director director)
        {
            director.GetInputManager.AddMousePositionListener(iMouseListener: this);

            mFow = fow;

            mPlatformToGraphId = new Dictionary<PlatformBlank, int>();
            mGraphIdToGraph = new Dictionary<int, Graph.Graph>();

            mDirector = director;

            mPlatformsToPlace = new LinkedList<PlatformPlacement>();
            mPlatforms = new LinkedList<PlatformBlank>();
            mRoads = new LinkedList<Road>();
        }

        /// <summary>
        /// A method existing so the DistributionManager has access to all platforms.
        /// </summary>
        /// <returns>A list of all the platforms currently in the game</returns>
        public LinkedList<PlatformBlank> GetPlatformList()
        {
            return mPlatforms;
        }

        /// <summary>
        /// Adds the specified platform to this map. This also modifies the graph structures
        /// used internally
        /// </summary>
        /// <param name="platform">The platform to be added</param>
        public void AddPlatform(PlatformBlank platform)
        {
            mPlatforms.AddLast(value: platform);
            mFow.AddRevealingObject(revealingObject: platform);

            // first of all get the "connection graph" of the platform to add. The connection graph
            // describes the graph (nodes and edges) reachable from this platform
            var graph = Bfs(startingNode: platform);

            foreach (var node in graph.GetNodes())
            {
                // if the platform in the current reachability graph isn't known then we don't do anything
                if (!mPlatformToGraphId.ContainsKey(key: (PlatformBlank) node))
                {
                    continue;
                }

                //if its known, we add our platform to it, since obviously we're connected to the same graph now.
                var graphIndex = mPlatformToGraphId[key: (PlatformBlank)node];
                mPlatformToGraphId[key: platform] = graphIndex;
                mGraphIdToGraph[key: graphIndex].AddNode(node: platform);
                platform.SetGraphIndex(graphIndex: graphIndex);
                return;
            }

            // we failed, meaning that this platform creates a new graph on its own.
            // intuitively: "the reachability graph from this node is only this node"
            var index = mGraphIdToGraph.Count;

            mGraphIdToGraph[key: index] = graph;
            mPlatformToGraphId[key: platform] = index;
            platform.SetGraphIndex(graphIndex: index);
            mDirector.GetPathManager.AddGraph(id: index, graph: graph);
        }

        /// <summary>
        /// Removes the specified platform from this map. This also modifies the underlying graph structures.
        /// </summary>
        /// <param name="platform">The platform to be removed</param>
        public void RemovePlatform(PlatformBlank platform)
        {
            mPlatforms.Remove(value: platform);
            mFow.RemoveRevealingObject(revealingObject: platform);

            // first update the references to all the roads connected to this accordingly
            foreach (var roads in platform.GetOutwardsEdges())
            {
                ((Road) roads).SourceAsNode = null;
            }

            foreach (var roads in platform.GetInwardsEdges())
            {
                ((Road) roads).DestinationAsNode = null;
            }

            // possible outcomes, no new graphs, 1 new graph, ..., n new graphs.
            // TODO: no idea how to efficiently handle this, implementation needed


        }

        /// <summary>
        /// Adds the specified road to this map. This also modifies the underlying graph structures
        /// </summary>
        /// <param name="road"></param>
        public void AddRoad(Road road)
        {
            mRoads.AddLast(value: road);

            // first check if two graphs got connected.
            // because we need to differ between road connected two graphs, or road was added to one graph only

            if (mPlatformToGraphId[key: (PlatformBlank) road.GetChild()] !=
                mPlatformToGraphId[key: (PlatformBlank) road.GetParent()])
            {
                var childIndex = mPlatformToGraphId[key: (PlatformBlank) road.GetChild()];
                var parentIndex = mPlatformToGraphId[key: (PlatformBlank) road.GetParent()];

                // since the graphes will get connected by this road, we first 
                // get all the nodes and edges from both graphes
                var connectGraphNodes = mGraphIdToGraph[key: childIndex].GetNodes()
                    .Concat(second: mGraphIdToGraph[key: parentIndex].GetNodes()).ToList();

                var connectGraphEdges = mGraphIdToGraph[key: childIndex].GetEdges()
                    .Concat(second: mGraphIdToGraph[key: parentIndex].GetEdges()).ToList();

                // don't forget to add the current road to the graph aswell
                connectGraphEdges.Add(item: road);

                foreach (var node in connectGraphNodes)
                {
                    // now we update our dictionary, such that all nodes formerly in the graph
                    // of the node road.GetChild() are now in the parent nodes graph.
                    // this is "arbitrary", we could also do it the other way around
                    mPlatformToGraphId[key: (PlatformBlank) node] = parentIndex;
                    ((PlatformBlank)node).SetGraphIndex(graphIndex: parentIndex);
                }

                // now we create the actual connected graph
                var graph = new Graph.Graph(nodes: connectGraphNodes, edges: connectGraphEdges);

                // the only thing left is to update the two former graph references
                // and add the new graph
                mGraphIdToGraph[key: childIndex] = null;
                mGraphIdToGraph[key: parentIndex] = graph;
                mDirector.GetPathManager.RemoveGraph(id: childIndex);
                mDirector.GetPathManager.AddGraph(id: parentIndex, graph: graph);
                return;
            }
            // the road was in the same graph, thus just add it to the graph
            mGraphIdToGraph[key: mPlatformToGraphId[key: (PlatformBlank) road.GetChild()]].AddEdge(edge: road);

        }

        /// <summary>
        /// Removes the specified road from this map. This also modifies the underlying graph structures.
        /// </summary>
        /// <param name="road">The road to be added to the game</param>
        public void RemoveRoad(Road road)
        {
            mRoads.Remove(value: road);

            var child = road.GetChild();
            var parent = road.GetParent();

            ((PlatformBlank)child).RemoveEdge(edge: road);
            ((PlatformBlank)parent).RemoveEdge(edge: road);

            //TODO: adjust underlying graph structures
            // more accurately: we have two cases:
            // 1. road gets destroyed -> two new seperate graphs get created
            // because they only were connected by the road to be removed.
            //
            // 1.1 road gets destroyed -> graph doesn't change, since child reachability graph
            // and parent reachability graph are the same even without the road. Just remove
            // the road from the graph
            // 
            // 2. road gets destroyed -> the graph stays the same, just
            // this road removed, this can be the case if either the child
            // or parent of the road is not existent anymore.
            //
            // 3. road gets destroyed -> both parent and child were non existent
            // not sure right now if this should be supportable, but lets
            // just do it. This does nothing, but remove the road from
            // the internal list of roads, since there should be no
            // graph just containing one edge

            // 1st check the 3rd case.
            if (child == null && parent == null)
            {
                return;
            }

            // now check the 2nd case.
            if (child == null || parent == null)
            {
                INode existent = null;

                if (child != null)
                {
                    existent = child;
                }

                if (parent != null)
                {
                    existent = parent;
                }

                // this is all we have to do, refer to the 2nd case above if more explanation needed
                mGraphIdToGraph[key: mPlatformToGraphId[key: (PlatformBlank) existent]].RemoveEdge(edge: road);
                return;
            }

            // now we're in the 1st case. But child and parent do exist. This is the main
            // case in the game

            var childReachableGraph = Bfs(startingNode: child);
            var parentReachableGraph = Bfs(startingNode: parent);

            // check whether both are still the same
            if (childReachableGraph.Equals(obj: parentReachableGraph))
            {
                mGraphIdToGraph[key: mPlatformToGraphId[key: (PlatformBlank) parent]].RemoveEdge(edge: road);
                return;
            }

            var newChildIndex = mGraphIdToGraph.Count;

            // update the values for the child nodes, the parent nodes reuse their values.
            foreach (var childNode in childReachableGraph.GetNodes())
            {
                mPlatformToGraphId[key: (PlatformBlank)childNode] = newChildIndex;
                ((PlatformBlank)childNode).SetGraphIndex(graphIndex: newChildIndex);
            }

            mGraphIdToGraph[key: newChildIndex] = childReachableGraph;
            mGraphIdToGraph[key: mPlatformToGraphId[key: (PlatformBlank) parent]] = parentReachableGraph;

            mDirector.GetPathManager.AddGraph(id: newChildIndex, graph: childReachableGraph);
            mDirector.GetPathManager.AddGraph(id: mPlatformToGraphId[key: (PlatformBlank)parent], graph: parentReachableGraph);
        }

        /// <summary>
        /// Implementation of a BFS algorithm for a given starting node.
        /// This doesn't work with a destination node, but rather returns
        /// the whole "reachability graph".
        /// </summary>
        /// <param name="startingNode">The node from which to get the reachability graph</param>
        /// <returns>The reachability graph from the node given</returns>
        private Graph.Graph Bfs(INode startingNode)
        {
            var visited = new Dictionary<INode, bool>();
            var queue = new Queue<INode>();
            visited[key: startingNode] = true;

            var graph = new Graph.Graph();

            queue.Enqueue(item: startingNode);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                graph.AddNode(node: node);
                graph.AddEdges(edges: node.GetOutwardsEdges());

                foreach (var child in node.GetChilds())
                {
                    if (visited.ContainsKey(key: child))
                    {
                        continue;
                    }
                    queue.Enqueue(item: child);
                    visited[key: child] = true;
                }
            }

            return graph;
        }

        /// <summary>
        /// This draws everything that should be drawn ABOVE the fog of war. This has to be satisfied
        /// by the one calling this method.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch on which drawing gets performed</param>
        public void DrawAboveFow(SpriteBatch spriteBatch)
        {
            foreach (var platformToAdd in mPlatformsToPlace)
            {
                platformToAdd.Draw(spriteBatch: spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(var platform in mPlatforms)
            {
                platform.Draw(spritebatch: spriteBatch);
            }

            foreach (var road in mRoads)
            {
                road.Draw(spriteBatch: spriteBatch);
            }
        }

        public void Update(GameTime gametime)
        {
            // the platform which currently gets hovered. This only gets set if we actually need it
            // e.g. when we have a "to be placed platform" currently in the game. This also 
            // fulfills multiple purposes.
            // First is when the platform to be placed is in its
            // first state, then the hovering refers to the platform which is under the platform
            // to place. This is needed to make sure that the platform to be placed doesn't get
            // placed ontop of another. 
            // The second is when the platform to be placed is in its second state,
            // then the hovering refers to the platform the road snaps to.
            PlatformBlank hovering = null;

            foreach (var platform in mPlatforms)
            {
                platform.Update(t: gametime);

                // we only want to continue if we have platforms to place.
                // the reason this is in this for loop is simply due to me
                // not having to iterate the same list twice.
                if (mPlatformsToPlace.Count <= 0)
                {
                    continue;
                }

                // this for loop is needed to fulfill the first hovering purpose mentioned.
                foreach(var platformToAdd in mPlatformsToPlace)
                {
                    // first make sure to update the bounds, etc., since our platforms normally don't move
                    // these don't get updated automatically.
                    platformToAdd.GetPlatform().UpdateValues();

                    // if our current platform doesn't intersect with the platform to be placed we 
                    // aren't hovering it, thus we continue to the next platform.
                    if (!platformToAdd.GetPlatform().AbsBounds.Intersects(value: platform.AbsBounds))
                    {
                        continue;
                    }
                    // this means, we're currently hovering a platform with the platform to place
                    // thus set it accordingly, so it can be handled in the respective class.
                    hovering = platform;

                }

                // this is needed to fulfill the second hovering purpose mentioned.
                if (!platform.AbsBounds.Intersects(value: new Rectangle(x: (int) mMouseX, y: (int) mMouseY, width: 1, height: 1)))
                {
                    continue;
                }

                hovering = platform;

            }

            foreach (var road in mRoads)
            {
                road.Update(gametime: gametime);
            }

            // we use this list to "mark" all the platformplacements to remove from the actual list. Since
            // we need to ensure that the actual list doesn't change size while iterating.
            var toRemove = new LinkedList<PlatformPlacement>();

            foreach (var platformToAdd in mPlatformsToPlace)
            {
                // finished means, that the platform either got set, or got canceled, where canceled means
                // that the building process got canceled
                if (!platformToAdd.IsFinished())
                {
                    platformToAdd.SetHovering(hovering: hovering);
                    platformToAdd.Update(gametime: gametime);
                    continue;
                }
                // the platform is finished AND canceled. Make sure to remove it, and update it a last time so it can clean up all its references
                // to other classes.
                if (platformToAdd.IsCanceled())
                {
                    platformToAdd.Update(gametime: gametime);
                    toRemove.AddLast(value: platformToAdd);
                    continue;
                }

                //platform is finished
                toRemove.AddLast(value: platformToAdd);

                AddPlatform(platform: platformToAdd.GetPlatform());
                platformToAdd.GetPlatform().Register();
                platformToAdd.GetRoad().Place(source: platformToAdd.GetPlatform(), dest: hovering);
                AddRoad(road: platformToAdd.GetRoad());

            }

            //finally make sure to remove the "marked" platformplacements to be removed
            foreach(var platformToRemove in toRemove)
            {
                mPlatformsToPlace.Remove(value: platformToRemove);
            }
        }

        /// <summary>
        /// Adds a platform to place to this map. This should solely be used building, which is also the only
        /// method to add platforms to our game dynamically.
        /// </summary>
        /// <param name="platformPlacement">The platformplacement to place</param>
        public void AddPlatformToPlace(PlatformPlacement platformPlacement)
        {
            mPlatformsToPlace.AddLast(value: platformPlacement);
        }

        public int GetGraphCount()
        {
            var graphs = 0;

            for (var i = 0; i < mGraphIdToGraph.Count; i++)
            {
                if (mGraphIdToGraph[i] != null)
                {
                    graphs++;
                }
            }

            return graphs;
        }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            mMouseX = worldX;
            mMouseY = worldY;
        }
    }
}
