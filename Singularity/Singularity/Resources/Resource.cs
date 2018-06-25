using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;
using Singularity.Libraries;

namespace Singularity.Resources
{
    public class Resource : ISpatial
    {
        // TODO: fkarg implement


        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public EResourceType Type { get; internal set; }

        public Resource(EResourceType type, Vector2 position, int width)
        {
            Type = type;
            AbsolutePosition = position;
            AbsoluteSize = new Vector2(width, width * 0.6f);
        }

        public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawCircle(AbsolutePosition, 10, 20, ResourceHelper.GetColor(Type), 10, LayerConstants.GeneralUnitLayer);
        }

        public void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }
    }
}
