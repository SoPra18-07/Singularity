using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.platform
{
    class Road : IDraw, ISpatial
    {
        public Vector2 Origin { get; }
        public Vector2 Destination { get; }
        private bool mBlueprint;

        /*
         TODO: The size and position needs to be integrated in this object in some way. It doesn't
         TODO: really matter if its a bad representation since you can easily transform the position and size
         TODO: in your own queries. For example the position could be used as the origin and the width of the size
         TODO: as the length and the height of the size as the thickness. Now you can transform the size rectangle
         TODO: to perfectly fit your line with an angle.
        */

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
            spriteBatch.DrawLine(Origin, Destination, mBlueprint? new Color(new Vector3(46, 53, 97)) : new Color(new Vector4(0, 40, 40, 255)), 5f);
        }
    }
}
