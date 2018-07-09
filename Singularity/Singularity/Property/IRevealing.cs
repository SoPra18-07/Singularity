using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Utils;

namespace Singularity.Property
{
    public interface IRevealing
    {
        int RevelationRadius { get; }

        Vector2 Center { get; }

        Texture2D RevealingTexture { get; }
        void LoadContent(GraphicsDevice device);
        void UnLoadContent();
    }

    public abstract class ARevealing : IRevealing
    {
        public int RevelationRadius { get; protected set; }

        public Vector2 Center { get; protected set; }

        public Texture2D RevealingTexture { get; private set; }

        public virtual void LoadContent(GraphicsDevice device)
        {
            RevealingTexture = new Texture2D(device, this.RevelationRadius, this.RevelationRadius / 2);
            Color[] data = new Color[RevelationRadius * RevelationRadius / 2];
            var count = 0;
            for (int i = 0; i < RevelationRadius; i++) {
                for (int j = 0; j < RevelationRadius / 2; j++) {
                    /*
                    if (Geometry.Length(new Vector2(RevelationRadius - i, RevelationRadius / 2 - j)) < RevelationRadius) {
                        data[i * RevelationRadius / 2 + j] = Color.Transparent;
                    } else {
                        data[i * RevelationRadius / 2 + j] = new Color(new Vector3(255, 255, 255));
                    }
                    */
                    if (i > j)
                    {
                        data[count] = Color.Transparent;
                    }
                    else
                    {
                        // data[count] = Color.Black;
                    }
                    count++;
                }
            }
            RevealingTexture.SetData(data);
        }

        public void UnLoadContent()
        {
            RevealingTexture.Dispose();
        }

    }
}
