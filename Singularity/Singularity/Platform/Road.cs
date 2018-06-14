using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Map.Properties;
using Singularity.Property;

namespace Singularity.Platform
{
    public sealed class Road : IDraw, ISpatial, IStructure, IUpdate
    {
        private float mLayer;

        private Vector2 Source { get; }
        private Vector2 Destination { get; }

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
                if (!value)
                { // add road to graph
                }
            }
        }

        public bool IsPlaced { get; set; }

        public bool IsAdded { get; set; }

        public bool IsSemiPlaced { get; set; }

        /// <summary>
        /// Road is simply an edge between two platforms.
        /// </summary>
        /// <param name="source">The source ISpatial object from which this road gets drawn</param>
        /// <param name="destination">The destinaion ISpatial object to which this road gets drawn</param>
        /// <param name="blueprint">Whether this road is a blueprint or not</param>
        public Road(PlatformBlank source, PlatformBlank destination, bool blueprint, bool isPlaced = true)
        {
            IsPlaced = isPlaced;
            IsSemiPlaced = isPlaced;
            IsAdded = false;

            // the hardcoded values need some changes for different platforms, ill wait until those are implemented to find a good solution.
            Source = new Vector2(source.AbsolutePosition.X + source.AbsoluteSize.X / 2, source.AbsolutePosition.Y + 109);
            Destination = new Vector2(destination.AbsolutePosition.X + destination.AbsoluteSize.X / 2, destination.AbsolutePosition.Y + 109);
            Blueprint = blueprint;

            AbsolutePosition = destination.Center;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(Source, Destination, mBlueprint ? new Color(new Vector3(.1804f, .2078f, .3804f)) : new Color(new Vector4(.776f, .776f, .776f, 255)), 5f, LayerConstants.RoadLayer);
        }

        public void Update(GameTime gametime)
        {
            mLayer = (IsPlaced ? LayerConstants.RoadLayer : LayerConstants.RoadBuildingLayer);
        }
    }
}