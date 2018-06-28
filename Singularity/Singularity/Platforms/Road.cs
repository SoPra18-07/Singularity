using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Platforms
{
    public sealed class Road : ISpatial, IEdge
    {
        private PlatformBlank Source { get; }

        private PlatformBlank Destination { get; }

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
                if (!value) { // todo: add road to graph
                           }
            }
        }

        /// <summary>
        /// Road is simply an edge between two platforms.
        /// </summary>
        /// <param name="source">The source IRevealing object from which this road gets drawn</param>
        /// <param name="destination">The destinaion IRevealing object to which this road gets drawn</param>
        /// <param name="blueprint">Whether this road is a blueprint or not</param>
        public Road(PlatformBlank source, PlatformBlank destination, bool blueprint)
        {
            // the hardcoded values need some changes for different platforms, ill wait until those are implemented to find a good solution.
            Source = source;
            Destination = destination;
            Blueprint = blueprint;

            source.AddEdge(edge: this, facing: EEdgeFacing.Outwards);
            destination.AddEdge(edge: this, facing: EEdgeFacing.Inwards);

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(point1: ((IRevealing)Source).Center, point2: ((IRevealing)Destination).Center, color: mBlueprint ?  new Color(color: new Vector4(x: .1803922f, y: 0.2078431f, z: .3803922f, w: .5f)) : new Color(color: new Vector3(x: .75f, y: .75f, z: .75f)), thickness: 5f, layerDepth: LayerConstants.RoadLayer);
        }

        public void Update(GameTime gametime)
        {

        }

        public INode GetParent()
        {
            return Source;
        }

        public INode GetChild()
        {
            return Destination;
        }

        public float GetCost()
        {
            return Vector2.Distance(value1: ((IRevealing) Source).Center, value2: ((IRevealing) Destination).Center);
        }
    }
}
