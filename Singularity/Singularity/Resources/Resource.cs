using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;

namespace Singularity.Resources
{
    public class Resource : ISpatial
    {
        // TODO: fkarg implement


        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }


        private EResourceType mType;
        private Vector2 mAbsolutePosition;
        private int mV; // velocity

        public Resource()
        {
        }

        public Resource(EResourceType type, Vector2 position, int width)
        {

            Type = type;
            AbsolutePosition = position;
            AbsoluteSize = new Vector2(width, width * 0.6f);
        }

        public EResourceType Type { get; internal set; }

        public void Draw(SpriteBatch spriteBatch)

            spriteBatch.DrawCircle(AbsolutePosition, 10, 20, Color.Black, 10, LayerConstants.GeneralUnitLayer);
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }
    }
}
