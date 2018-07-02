﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Platforms
{
    public sealed class Road : ISpatial, IEdge
    {
        public Vector2 Source { get; set; }

        public Vector2 Destination { get; set; }

        private INode SourceAsNode { get; set; }

        private INode DestinationAsNode { get; set; }

        private bool mBlueprint;

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        /// <summary>
        /// If it is a blueprint, then set this value to true. Once the road changes from a blueprint to a real road,
        /// this should automatically also add the road to the graph
        /// </summary>
        public bool Blueprint
        {
            get { return mBlueprint; }
            set
            {
                mBlueprint = value;
                if (!value) { // todo: add road to graph - done?
                           }
            }
        }

        /// <summary>
        /// Road is simply an edge between two platforms.
        /// </summary>
        /// <param name="source">The source IRevealing object from which this road gets drawn</param>
        /// <param name="destination">The destinaion IRevealing object to which this road gets drawn</param>
        /// <param name="blueprint">Whether this road is a blueprint or not</param>
        public Road(PlatformBlank source, PlatformBlank destination , bool blueprint)
        {

            // the hardcoded values need some changes for different platforms, ill wait until those are implemented to find a good solution.
            if(source == null && destination == null)
            {
                throw new System.Exception("Source and Destination can't both be null");
            }
            if(source == null && destination != null)
            {
                Destination = destination.Center;
                Source = destination.Center;
            }else if(source != null && destination == null)
            {
                Source = source.Center;
                Destination = source.Center;
            }else
            {
                Place(source, destination);
            }
            Blueprint = blueprint;

        }

        public void Place(PlatformBlank source, PlatformBlank dest)
        {
            if(source == null || dest == null)
            {
                return;
            }

            SourceAsNode = source;
            DestinationAsNode = dest;

            Source = source.Center;
            Destination = dest.Center;

            source.AddEdge(this, EEdgeFacing.Outwards);
            dest.AddEdge(this, EEdgeFacing.Inwards);

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(Source, Destination, mBlueprint ?  new Color(new Vector4(.1803922f, 0.2078431f, .3803922f, .5f)) : new Color(new Vector3(.75f, .75f, .75f)), 5f, LayerConstants.RoadLayer);
        }

        public void Update(GameTime gametime)
        {

        }

        public INode GetParent()
        {
            return SourceAsNode;
        }

        public INode GetChild()
        {
            return DestinationAsNode;
        }

        public float GetCost()
        {
            return Vector2.Distance(Source, Destination);
        }

        public bool Die()
        {
            ((PlatformBlank) SourceAsNode).Kill(this);
            ((PlatformBlank) DestinationAsNode).Kill(this);
            return true;
        }
    }
}