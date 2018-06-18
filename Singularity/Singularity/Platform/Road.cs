using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Platform
{
    public sealed class Road : ISpatial, IEdge
    {
        private PlatformBlank Source { get; }

        private PlatformBlank Destination { get; }

        private bool _mBlueprint;

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
            get { return _mBlueprint; }
            set
            {
                _mBlueprint = value;
                if (!value) { // add road to graph
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

            source.AddEdge(this, EEdgeFacing.Outwards);
            destination.AddEdge(this, EEdgeFacing.Inwards);

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(((IRevealing)Source).Center, ((IRevealing)Destination).Center, _mBlueprint ? new Color(new Vector3(46, 53, 97)) : new Color(new Vector4(0, 40, 40, 255)), 5f, LayerConstants.RoadLayer);
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
            return Vector2.Distance(((IRevealing) Source).Center, ((IRevealing) Destination).Center);
        }
    }
}
