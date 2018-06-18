using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;

namespace Singularity.Resources
{
	public class Resource : ISpatial
    {
        public Resource()
        {
			
        }

		public Vector2 RelativePosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Vector2 RelativeSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Vector2 AbsolutePosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Vector2 AbsoluteSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Draw(SpriteBatch spriteBatch)
		{
			throw new NotImplementedException();
		}

		public void Update(GameTime gametime)
		{
			throw new NotImplementedException();
		}
	}
}
