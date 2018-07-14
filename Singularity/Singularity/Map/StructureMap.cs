using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Map
{
    /// <summary>
    /// A Structure map holds all the structures currently in the game. Additionally the structure map holds a graph
    /// representation of all the platforms and roads in the game. These graphes are used by pathfinding algorithms etc.
    /// </summary>
    [DataContract]
    public sealed class StructureMap : IDraw, IUpdate, IMousePositionListener
    {
        /// <summary>
        /// A list of all the platforms currently in the game
        /// </summary>
        [DataMember]
        private readonly LinkedList<PlatformBlank> mPlatforms;

        /// <summary>
        /// A list of all the roads in the game, identified by a source and destination platform.
        /// </summary>
        [DataMember]
        private readonly LinkedList<Road> mRoads;

        /// <summary>
        /// A list of all the platformPlacements in the game (the platforms following the mouse when building).
        /// </summary>
        [DataMember]
        private readonly LinkedList<StructurePlacer> mStructuresToPlace;

        /// <summary>
        /// The director for the game
        /// </summary>
        private Director mDirector;

        /// <summary>
        /// A dictionary mapping platforms to the ID of the graph they are currently on
        /// </summary>
        [DataMember]
        private readonly Dictionary<PlatformBlank, int> mPlatformToGraphId;

        /// <summary>
        /// A dictionary mapping graph IDs to the graph object they belong to
        /// </summary>
        [DataMember]
        private readonly Dictionary<int, Graph.Graph> mGraphIdToGraph;

        /// <summary>
        /// A dictioanry mapping graph IDs to the energy level of the graph
        /// </summary>
        [DataMember]
        private readonly Dictionary<int, int> mGraphIdToEnergyLevel;

        /// <summary>
        /// The Fog of war of the current game
        /// </summary>
        private FogOfWar mFow;

        /// <summary>
        /// The x coordinate of the mouse in world space
        /// </summary>
        [DataMember]
        private float mMouseX;

        /// <summary>
        /// The y coordinate of the mouse in world space
        /// </summary>
        [DataMember]
        private float mMouseY;

        /// <summary>
        /// Creates a new structure map which holds all the structures currently in the game.
        /// </summary>
        public StructureMap(FogOfWar fow, ref Director director)
        {
            director.InputManager.AddMousePositionListener(this);

            mFow = fow;

            mPlatformToGraphId = new Dictionary<PlatformBlank, int>();
            mGraphIdToGraph = new Dictionary<int, Graph.Graph>();
            mGraphIdToEnergyLevel = new Dictionary<int, int>();

            mDirector = director;

            mStructuresToPlace = new LinkedList<StructurePlacer>();
            mPlatforms = new LinkedList<PlatformBlank>();
            mRoads = new LinkedList<Road>();
        }


        public void ReloadContent(ContentManager content, FogOfWar fow, ref Director dir, Camera camera, Map map, UserInterfaceScreen ui)
        {
            mFow = fow;
            mDirector = dir;
            dir.InputManager.AddMousePositionListener(this);
            foreach (var placement in mStructuresToPlace)
            {
                placement.ReloadContent(camera, ref dir, map);
            }

            foreach (var platform in mPlatforms)
            {
                platform.ReloadContent(content, ref dir);
            }
            //Update uis graphid dictionary
            ui.CallingAllGraphs(mGraphIdToGraph);

            foreach(var roads in mRoads)
            {
                roads.ReloadContent(ref dir);
            }
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

            mPlatforms.AddLast(platform);
            mFow.AddRevealingObject(platform);

            // first of all get the "connection graph" of the platform to add. The connection graph
            // describes the graph (nodes and edges) reachable from this platform
            var graph = Bfs(platform);

            foreach (var node in graph.GetNodes())
            {
                // if the platform in the current reachability graph isn't known then we don't do anything
                if (!mPlatformToGraphId.ContainsKey((PlatformBlank) node))
                {
                    continue;
                }

                //if its known, we add our platform to it, since obviously we're connected to the same graph now.
                var graphIndex = mPlatformToGraphId[(PlatformBlank)node];
                mPlatformToGraphId[platform] = graphIndex;
                mGraphIdToGraph[graphIndex].AddNode(platform);
                platform.SetGraphIndex(graphIndex);
                UpdateGenUnitsGraphIndex(mGraphIdToGraph[graphIndex], graphIndex);
                return;
            }

            // we failed, meaning that this platform creates a new graph on its own.
            // intuitively: "the reachability graph from this node is only this node"
            var index = mGraphIdToGraph.Count;

            mGraphIdToEnergyLevel[index] = 0;
            mGraphIdToGraph[index] = graph;
            mPlatformToGraphId[platform] = index;
            platform.SetGraphIndex(index);
            
            UpdateGenUnitsGraphIndex(mGraphIdToGraph[index], index);

            mDirector.DistributionDirector.AddManager(index);
            mDirector.PathManager.AddGraph(index, graph);
        }

        /// <summary>
        /// Removes the specified platform from this map. This also modifies the underlying graph structures.
        /// </summary>
        /// <param name="platform">The platform to be removed</param>
        public void RemovePlatform(PlatformBlank platform)
        {
            mPlatforms.Remove(platform);
            mFow.RemoveRevealingObject(platform);

            var index = mPlatformToGraphId[platform];

            mGraphIdToGraph[index] = null;

            mDirector.DistributionDirector.RemoveManager(index, mGraphIdToGraph);
            mDirector.PathManager.RemoveGraph(index);
        }

        /// <summary>
        /// Adds the specified road to this map. This also modifies the underlying graph structures
        /// </summary>
        /// <param name="road"></param>
        public void AddRoad(Road road)
        {
            mRoads.AddLast(road);

            // first check if two graphs got connected.
            // because we need to differ between road connected two graphs, or road was added to one graph only

            if (mPlatformToGraphId[(PlatformBlank) road.GetChild()] !=
                mPlatformToGraphId[(PlatformBlank) road.GetParent()])
            {
                var childIndex = mPlatformToGraphId[(PlatformBlank) road.GetChild()];
                var parentIndex = mPlatformToGraphId[(PlatformBlank) road.GetParent()];

                // since the graphes will get connected by this road, we first
                // get all the nodes and edges from both graphes
                var connectGraphNodes = mGraphIdToGraph[childIndex].GetNodes()
                    .Concat(mGraphIdToGraph[parentIndex].GetNodes()).ToList();

                var connectGraphEdges = mGraphIdToGraph[childIndex].GetEdges()
                    .Concat(mGraphIdToGraph[parentIndex].GetEdges()).ToList();

                // don't forget to add the current road to the graph aswell
                connectGraphEdges.Add(road);

                foreach (var node in connectGraphNodes)
                {
                    // now we update our dictionary, such that all nodes formerly in the graph
                    // of the node road.GetChild() are now in the parent nodes graph.
                    // this is "arbitrary", we could also do it the other way around
                    mPlatformToGraphId[(PlatformBlank) node] = parentIndex;
                    ((PlatformBlank)node).SetGraphIndex(parentIndex);
                }

                // now we create the actual connected graph
                var graph = new Graph.Graph(connectGraphNodes, connectGraphEdges);

                // the only thing left is to update the two former graph references
                // and add the new graph
                mGraphIdToGraph[childIndex] = null;
                mGraphIdToGraph[parentIndex] = graph;

                UpdateGenUnitsGraphIndex(mGraphIdToGraph[parentIndex], parentIndex);

                mGraphIdToEnergyLevel[parentIndex] =
                    mGraphIdToEnergyLevel[parentIndex] + mGraphIdToEnergyLevel[childIndex];
                mGraphIdToEnergyLevel[childIndex] = 0;

                mDirector.DistributionDirector.MergeManagers(childIndex, parentIndex, parentIndex);
                mDirector.PathManager.RemoveGraph(childIndex);
                mDirector.PathManager.AddGraph(parentIndex, graph);
                return;
            }
            // the road was in the same graph, thus just add it to the graph
            mGraphIdToGraph[mPlatformToGraphId[(PlatformBlank) road.GetChild()]].AddEdge(road);

        }

        /// <summary>
        /// Removes the specified road from this map. This also modifies the underlying graph structures.
        /// </summary>
        /// <param name="road">The road to be added to the game</param>
        public void RemoveRoad(Road road)
        {
            mRoads.Remove(road);

            var child = road.GetChild();
            var parent = road.GetParent();

            ((PlatformBlank)child)?.RemoveEdge(road);
            ((PlatformBlank)parent)?.RemoveEdge(road);

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
                mGraphIdToGraph[mPlatformToGraphId[(PlatformBlank) existent]].RemoveEdge(road);
                return;
            }

            // now we're in the 1st case. But child and parent do exist. This is the main
            // case in the game

            var childReachableGraph = Bfs(child);
            var parentReachableGraph = Bfs(parent);

            // check whether both are still the same
            if (childReachableGraph.Equals(parentReachableGraph))
            {
                mGraphIdToGraph[mPlatformToGraphId[(PlatformBlank) parent]].RemoveEdge(road);
                return;
            }

            var newChildIndex = mGraphIdToGraph.Count;

            var platforms = new List<PlatformBlank>();
            var units = new List<GeneralUnit>();

            // update the values for the child nodes, the parent nodes reuse their values.
            foreach (var childNode in childReachableGraph.GetNodes())
            {
                mPlatformToGraphId[(PlatformBlank)childNode] = newChildIndex;
                ((PlatformBlank)childNode).SetGraphIndex(newChildIndex);
                platforms.Add((PlatformBlank) childNode);
                foreach (var unit in ((PlatformBlank)childNode).GetGeneralUnitsOnPlatform())
                {
                    units.Add(unit);
                }
            }

            mGraphIdToGraph[newChildIndex] = childReachableGraph;
            mGraphIdToGraph[mPlatformToGraphId[(PlatformBlank) parent]] = parentReachableGraph;

            UpdateGenUnitsGraphIndex(mGraphIdToGraph[newChildIndex], newChildIndex);

            mGraphIdToEnergyLevel[newChildIndex] = 0;
            mGraphIdToEnergyLevel[mPlatformToGraphId[(PlatformBlank) parent]] = 0;
            UpdateEnergyLevel(newChildIndex);
            UpdateEnergyLevel(mPlatformToGraphId[(PlatformBlank)parent]);

            mDirector.DistributionDirector.SplitManagers(mPlatformToGraphId[(PlatformBlank)parent], newChildIndex, platforms, units, mGraphIdToGraph);
            mDirector.PathManager.AddGraph(newChildIndex, childReachableGraph);
            mDirector.PathManager.AddGraph(mPlatformToGraphId[(PlatformBlank)parent], parentReachableGraph);
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
            visited[startingNode] = true;

            var graph = new Graph.Graph();

            queue.Enqueue(startingNode);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                graph.AddNode(node);
                graph.AddEdges(node.GetOutwardsEdges());

                foreach (var child in node.GetChilds())
                {
                    if (child == null)
                    {
                        continue;
                    }

                    if (visited.ContainsKey(child))
                    {
                        continue;
                    }
                    queue.Enqueue(child);
                    visited[child] = true;
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
            foreach (var platformToAdd in mStructuresToPlace)
            {
                platformToAdd.Draw(spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(var platform in mPlatforms)
            {
                platform.Draw(spriteBatch);
            }

            foreach (var road in mRoads)
            {
                road.Draw(spriteBatch);
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
                platform.Update(gametime);

                // we only want to continue if we have platforms to place.
                // the reason this is in this for loop is simply due to me
                // not having to iterate the same list twice.
                if (mStructuresToPlace.Count <= 0)
                {
                    continue;
                }

                // this for loop is needed to fulfill the first hovering purpose mentioned.
                foreach(var structureToAdd in mStructuresToPlace)
                {
                    if (structureToAdd.GetPlatform() == null)
                    {
                        continue;
                    }

                    // first make sure to update the bounds, etc., since our platforms normally don't move
                    // these don't get updated automatically.
                    structureToAdd.GetPlatform().UpdateValues();

                    // if our current platform doesn't intersect with the platform to be placed we
                    // aren't hovering it, thus we continue to the next platform.
                    if (!structureToAdd.GetPlatform().AbsBounds.Intersects(platform.AbsBounds))
                    {
                        continue;
                    }
                    // this means, we're currently hovering a platform with the platform to place
                    // thus set it accordingly, so it can be handled in the respective class.
                    hovering = platform;

                }

                // this is needed to fulfill the second hovering purpose mentioned.
                if (!platform.AbsBounds.Intersects(new Rectangle((int) mMouseX, (int) mMouseY, 1, 1)))
                {
                    continue;
                }

                hovering = platform;

            }

            foreach (var road in mRoads)
            {
                road.Update(gametime);
            }

            // we use this list to "mark" all the platformplacements to remove from the actual list. Since
            // we need to ensure that the actual list doesn't change size while iterating.
            var toRemove = new LinkedList<StructurePlacer>();

            foreach (var structureToAdd in mStructuresToPlace)
            {
                // finished means, that the platform either got set, or got canceled, where canceled means
                // that the building process got canceled
                if (!structureToAdd.IsFinished())
                {
                    structureToAdd.SetHovering(hovering);
                    structureToAdd.Update(gametime);
                    continue;
                }
                // the platform is finished AND canceled. Make sure to remove it, and update it a last time so it can clean up all its references
                // to other classes.
                if (structureToAdd.IsCanceled())
                {
                    structureToAdd.Update(gametime);
                    toRemove.AddLast(structureToAdd);
                    continue;
                }

                //platform is finished
                toRemove.AddLast(structureToAdd);
                if (structureToAdd.GetPlatform() != null)
                {
                    AddPlatform(structureToAdd.GetPlatform());
                    structureToAdd.GetPlatform().Register();
                    structureToAdd.GetConnectionRoad().Place(structureToAdd.GetPlatform(), hovering);
                    AddRoad(structureToAdd.GetConnectionRoad());
                }
                else
                {
                    AddRoad(structureToAdd.GetRoad());
                }

            }

            //finally make sure to remove the "marked" platformplacements to be removed
            foreach(var platformToRemove in toRemove)
            {
                mStructuresToPlace.Remove(platformToRemove);
            }

            // now update the energy level of all graphs
            foreach (var graphId in mGraphIdToGraph.Keys)
            {
                if (mGraphIdToGraph[graphId] == null)
                {
                    continue;
                }

                UpdateEnergyLevel(graphId);
            }
        }

        private void UpdateEnergyLevel(int graphId)
        {
            var wasNegative = mGraphIdToEnergyLevel[graphId] < 0;

            mGraphIdToEnergyLevel[graphId] = 0;

            foreach (var node in mGraphIdToGraph[graphId].GetNodes())
            {
                if (((PlatformBlank) node).IsManuallyDeactivated())
                {
                    continue;
                }

                mGraphIdToEnergyLevel[graphId] = mGraphIdToEnergyLevel[graphId] + ((PlatformBlank) node).GetProvidingEnergy();
                mGraphIdToEnergyLevel[graphId] = mGraphIdToEnergyLevel[graphId] - ((PlatformBlank) node).GetDrainingEnergy();
            }

            CheckEnergyLevel(graphId, wasNegative);
        }

        /// <summary>
        /// Adds a platform to place to this map. This should solely be used building, which is also the only
        /// method to add platforms to our game dynamically.
        /// </summary>
        /// <param name="structurePlacer">The platformplacement to place</param>
        public void AddPlatformToPlace(StructurePlacer structurePlacer)
        {
            mStructuresToPlace.AddLast(structurePlacer);
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

        private void CheckEnergyLevel(int graphId, bool wasNegative)
        {
            // energy level was positive and still is
            if (!wasNegative && mGraphIdToEnergyLevel[graphId] >= 0)
            {
                return;
            }

            // energy level was negative and is positive considering all the platforms that weren't manually deactivated
            // -> reactivate all the platforms which weren't manually deactivated
            if (wasNegative && mGraphIdToEnergyLevel[graphId] >= 0)
            {
                foreach (var node in mGraphIdToGraph[graphId].GetNodes())
                {
                    if (((PlatformBlank) node).IsManuallyDeactivated())
                    {
                        continue;
                    }
                    ((PlatformBlank)node).Activate(false);
                }
                return;
            }

            // energy level was something and is now negative
            foreach (var node in mGraphIdToGraph[graphId].GetNodes())
            {
                ((PlatformBlank)node).Deactivate(false);
            }
        }

        private List<GeneralUnit> GetGenUnitsOnGraph(Graph.Graph graph)
        {
            var genUnits = new List<GeneralUnit>();

            foreach (var node in graph.GetNodes())
            {
                var nodeAsPlat = (PlatformBlank) node;

                foreach (var unit in nodeAsPlat.GetGeneralUnitsOnPlatform())
                {
                    genUnits.Add(unit);
                }
            }

            return genUnits;
        }

        private void UpdateGenUnitsGraphIndex(Graph.Graph graph, int newId)
        {
            var list = GetGenUnitsOnGraph(graph);

            foreach (var genUnit in list)
            {
                genUnit.Graphid = newId;
            }
        }
    }
}
