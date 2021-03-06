﻿using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Property;

namespace Singularity.Platforms
{
    [DataContract]
    public sealed class Road : ADie, ISpatial, IEdge
    {
        [DataMember]
        public Vector2 Source { get; set; }
        [DataMember]
        public Vector2 Destination { get; set; }
        [DataMember]
        public INode SourceAsNode { get; set; }
        [DataMember]
        public INode DestinationAsNode { get; set; }
        [DataMember]
        private bool mBlueprint;
        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }
        [DataMember]
        public Vector2 RelativePosition { get; set; }
        [DataMember]
        public Vector2 RelativeSize { get; set; }

        [DataMember]
        private BuildBluePrint mBlueprintAction;

        /// <summary>
        /// If it is a blueprint, then set this value to true. Once the road changes from a blueprint to a real road,
        /// this should automatically also add the road to the graph
        /// </summary>
        [DataMember]
        public bool Blueprint
        {
            get { return mBlueprint; }
            set { mBlueprint = value; }
        }

        /// <summary>
        /// Road is simply an edge between two platforms.
        /// </summary>
        /// <param name="source">The source IRevealing object from which this road gets drawn</param>
        /// <param name="destination">The destinaion IRevealing object to which this road gets drawn</param>
        /// <param name="director">The Director of all Managers</param>
        /// <param name="blueprint">Whether this road is a blueprint or not</param>
        public Road(PlatformBlank source, PlatformBlank destination, ref Director director, bool blueprint = false) : base(ref director)
        {

            // the hardcoded values need some changes for different platforms, ill wait until those are implemented to find a good solution.
            if(source == null && destination == null)
            {
                throw new Exception("Source and Destination can't both be null");
            }
            if(source == null)
            {
                Destination = destination.Center;
                Source = destination.Center;
            }else if(destination == null)
            {
                Source = source.Center;
                Destination = source.Center;
            }else
            {
                Place(source, destination);
            }
            Blueprint = blueprint;
        }

        public void SetBluePrint(BuildBluePrint bp)
        {
            mBlueprintAction = bp;
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
            spriteBatch.DrawLine(Source, Destination, Blueprint ?  new Color(new Vector4(.1803922f, 0.2078431f, .3803922f, .5f)) : new Color(new Vector3(.75f, .75f, .75f)), 5f, LayerConstants.RoadLayer);
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

        public override bool Die()
        {
            mBlueprintAction?.Die();
            if (Blueprint)
            {
                // needs to change if you can build blueprints on blueprints !!!
                if (((PlatformBlank) DestinationAsNode).mBlueprint)
                {
                    ((PlatformBlank) DestinationAsNode).FlagForDeath();
                }

                if (((PlatformBlank) SourceAsNode).mBlueprint)
                {
                    ((PlatformBlank) SourceAsNode).FlagForDeath();
                }
            }

            mDirector.GetStoryManager.Level.GameScreen.RemoveObject(this);
            HasDieded = true;
            return true;
        }

        public bool HasDieded { get; private set; }

        public new void ReloadContent(ref Director director)
        {
            base.ReloadContent(ref director);
            mDirector = director;
        }
    }
}
