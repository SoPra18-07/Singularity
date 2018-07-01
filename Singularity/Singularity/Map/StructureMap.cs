using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Property;
using System.Diagnostics;
using System.Linq;
using Singularity.Graph;
using Singularity.Input;

namespace Singularity.Map
{
    /// <summary>
    /// A Structure map holds all the structures currently in the game.
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

        private readonly LinkedList<PlatformPlacement> mPlatformsToPlace;

        private readonly Director mDirector;

        private readonly Dictionary<PlatformBlank, int> mPlatformToGraphId;

        private readonly Dictionary<int, Graph.Graph> mGraphIdToGraph;

        private readonly FogOfWar mFow;

        private float mMouseX;

        private float mMouseY;

        /// <summary>
        /// Creates a new structure map which holds all the structures currently in the game.
        /// </summary>
        public StructureMap(FogOfWar fow, ref Director director)
        {
            director.GetInputManager.AddMousePositionListener(this);

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
        /// <returns></returns>
        public LinkedList<PlatformBlank> GetPlatformList()
        {
            return mPlatforms;
        }

        /// <summary>
        /// Adds the specified platform to this map.
        /// </summary>
        /// <param name="platform">The platform to be added</param>
        public void AddPlatform(PlatformBlank platform)
        {
            mPlatforms.AddLast(platform);
            mFow.AddRevealingObject(platform);

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
                return;
            }

            // we failed, meaning that this platform creates a new graph on its own.
            var index = mGraphIdToGraph.Count;

            mGraphIdToGraph[index] = graph;
            mPlatformToGraphId[platform] = index;
            platform.SetGraphIndex(index);
            mDirector.GetPathManager.AddGraph(index, graph);
        }

        /// <summary>
        /// Removes the specified platform from this map.
        /// </summary>
        /// <param name="platform">The platform to be removed</param>
        public void RemovePlatform(PlatformBlank platform)
        {
            mPlatforms.Remove(platform);
            mFow.RemoveRevealingObject(platform);

            //TODO: update graphs on removal.


        }
        public void AddRoad(Road road)
        {
            mRoads.AddLast(road);

            //First check if two graphs got connected.

            if (mPlatformToGraphId[(PlatformBlank) road.GetChild()] !=
                mPlatformToGraphId[(PlatformBlank) road.GetParent()])
            {
                var childIndex = mPlatformToGraphId[(PlatformBlank) road.GetChild()];
                var parentIndex = mPlatformToGraphId[(PlatformBlank) road.GetParent()];

                //TODO: concat one graph with another, delete one and add the road to it
                var connectGraphNodes = mGraphIdToGraph[childIndex].GetNodes()
                    .Concat(mGraphIdToGraph[parentIndex].GetNodes()).ToList();

                var connectGraphEdges = mGraphIdToGraph[childIndex].GetEdges()
                    .Concat(mGraphIdToGraph[parentIndex].GetEdges()).ToList();

                connectGraphEdges.Add(road);

                foreach (var node in connectGraphNodes)
                {
                    mPlatformToGraphId[(PlatformBlank) node] = parentIndex;
                    ((PlatformBlank)node).SetGraphIndex(parentIndex);
                }

                var graph = new Graph.Graph(connectGraphNodes, connectGraphEdges);

                mGraphIdToGraph[childIndex] = null;
                mGraphIdToGraph[parentIndex] = graph;
                mDirector.GetPathManager.RemoveGraph(childIndex);
                mDirector.GetPathManager.AddGraph(parentIndex, graph);
                return;
            }
            // the road was in the same graph, thus just add it to the graph
            mGraphIdToGraph[mPlatformToGraphId[(PlatformBlank) road.GetChild()]].AddEdge(road);

        }

        public void RemoveRoad(Road road)
        {
            mRoads.Remove(road);


        }

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

        public void DrawAboveFow(SpriteBatch spriteBatch)
        {
            foreach (var platformToAdd in mPlatformsToPlace)
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
            PlatformBlank hovering = null;

            foreach (var platform in mPlatforms)
            {
                platform.Update(gametime);

                if (mPlatformsToPlace.Count <= 0)
                {
                    continue;
                }

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

            var toRemove = new LinkedList<PlatformPlacement>();

            foreach (var platformToAdd in mPlatformsToPlace)
            {
                if (!platformToAdd.IsFinished())
                {
                    platformToAdd.SetHovering(hovering);
                    platformToAdd.Update(gametime);
                    return;
                }
                //platform is finished
                AddPlatform(platformToAdd.GetPlatform());
                platformToAdd.GetPlatform().Register();
                platformToAdd.GetRoad().Place(platformToAdd.GetPlatform(), hovering);
                AddRoad(platformToAdd.GetRoad());
                toRemove.AddLast(platformToAdd);
            }

            foreach(var platformToRemove in toRemove)
            {
                mPlatformsToPlace.Remove(platformToRemove);
            }
        }

        public void AddPlatformToPlace(PlatformPlacement platformPlacement)
        {
            mPlatformsToPlace.AddLast(platformPlacement);
        }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            mMouseX = worldX;
            mMouseY = worldY;
        }
    }
}
