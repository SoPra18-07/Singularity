using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.platform
{
    class Road : IDraw
    {
        public Vector2 Origin { get; }
        public Vector2 Destination { get; }
        private bool mBlueprint;

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
                if (!value) { // add road to graph
                           }
            } 
        }

        /// <summary>
        /// Road is simply an edge between two platforms. 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="blueprint"></param>
        public Road(Vector2 origin, Vector2 destination, bool blueprint)
        {
            Origin = origin;
            Destination = destination;
            Blueprint = blueprint;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(Origin, Destination, mBlueprint? new Color(new Vector3(46, 53, 97)) : new Color(new Vector4(0, 40, 40, 255)), 5f, LayerConstants.RoadLayer);
        }
    }
}
