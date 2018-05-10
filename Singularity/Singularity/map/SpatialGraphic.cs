using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.map
{
    internal sealed class SpatialGraphic : ILayerable
    {

        public Texture2D Graphic { get; }

        public Vector2 Position { get; }

        public int Layer { get; set; }


        public SpatialGraphic(Texture2D graphic, Vector2 position, int layer)
        {
            Graphic = graphic;
            Position = position;
            Layer = layer;

        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (SpatialGraphic) obj;
            return Graphic.Equals(other.Graphic) && Position.Equals(other.Position) && Layer.Equals(other.Layer);
        }

        public override int GetHashCode()
        {
            return Graphic.GetHashCode() ^ Position.GetHashCode();
        }
    }
}
