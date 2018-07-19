using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Map.Properties;
using Singularity.Property;

namespace Singularity.Map
{
    /// <inheritdoc cref="IUpdate"/>
    /// <inheritdoc cref="IDraw"/>
    /// <remarks>
    /// The FogOfWar grays out areas which are not "visible" from the current state of the game. Platforms and Units can
    /// discover new areas which is why they possess a light circle around them. The way this is accomplished is by using
    /// the stencil buffer and alpha effects. I can give a short but thorough explanation of whats going on:
    /// We basically add a completely transparent mask on the map (circle shape). Then we set every pixel
    /// thats not transparent to 1 in the stencil buffer (the inside of the circle is internally not handled
    /// transparent, thus we have on every pixel inside the circle a 1 in the stencil buffer, and the rest is 0)
    /// The reason everything else is 0 is due to the AlphaEffect. Then we start drawing all our (spatial) graphics
    /// if the value in the stencil buffer at that position is 1. So only on the inside of the circle. This creates
    /// a "cut off" effect of the circle on the inside. The only thing left is the "gray fog" everywhere where
    /// nothing is revealed. This we achieve by "inverting the mask", we don't actually do that but its the same
    /// effect. Internally we draw a rectangle over the whole screen (in semi transparent black) everywhere
    /// where the stencil is not 1. So on everything outside of the circle. This way we applied a gray fog
    /// out of the circle. We create multiple circle masks (for every object). This way we achieve exactly
    /// what we want. The performance is WAY better than the former one. I haven't looked into the
    /// monogames spriteBatch.Begin() method, but we call it 4 times, and always use a different stencil state for it.
    /// So it basically has to atleast traverse all the pixels on the screen 4 times per draw. This is done anyways for
    /// all the different buffer in the background (ColorBuffer, DepthBuffer, VertexBuffer, ...) so it shouldn't really
    /// add that much extra performance on the gpu.
    /// </remarks>
    [DataContract]
    public sealed class FogOfWar : IUpdate
    {

        /// <summary>
        /// A list of all the objects which are able to reveal the fog of war.
        /// </summary>
        [DataMember]
        private readonly LinkedList<IRevealing> mRevealingObjects;

        /// <summary>
        /// A stencil state which is used to initialize the stencil buffer
        /// with ones for every non transparent pixel, and 0 for every
        /// transparent pixel.
        /// </summary>
        private DepthStencilState mInitializeMaskStencilState;

        /// <summary>
        /// A stencil state which is used to draw outside of every mask
        /// currently applied in the stencil buffer.
        /// </summary>
        private DepthStencilState mApplyInvertedMaskStencilState;

        private DepthStencilState mApplyMaskStencilState;

        /// <summary>
        /// The AlphaTestEffect compares alpha values of pixels and sets them given certain restraints.
        /// </summary>
        private AlphaTestEffect mAlphaComparator;

        /// <summary>
        /// The camera object of the game used for screen coordinae calculation.
        /// </summary>
        private Camera mCamera;

        /// <summary>
        /// Creates a new FogOfWar object for the given mapTexture.
        /// </summary>
        /// <param name="camera">The Camera of the game</param>
        /// <param name="graphicsDevice">The graphical Device (System-util)</param>
        // @Ativolex: update your comments. :)
        public FogOfWar(Camera camera, GraphicsDevice graphicsDevice)
        {

            mCamera = camera;

            mRevealingObjects = new LinkedList<IRevealing>();

            mInitializeMaskStencilState = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };


            mApplyMaskStencilState = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.LessEqual,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };

            mApplyInvertedMaskStencilState = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Greater,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };



            mAlphaComparator = new AlphaTestEffect(graphicsDevice)
            {
                Projection = mCamera.GetStencilProjection(),
                VertexColorEnabled = true,
                DiffuseColor = Color.White.ToVector3(),
                AlphaFunction = CompareFunction.Always,
                ReferenceAlpha = 0
            };

        }

        public void ReloadContent(GraphicsDeviceManager graphics, Camera camera)
        {
            mCamera = camera;
            mInitializeMaskStencilState = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };


            mApplyMaskStencilState = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.LessEqual,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };

            mApplyInvertedMaskStencilState = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Greater,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };



            mAlphaComparator = new AlphaTestEffect(graphics.GraphicsDevice)
            {
                Projection = mCamera.GetStencilProjection(),
                VertexColorEnabled = true,
                DiffuseColor = Color.White.ToVector3(),
                AlphaFunction = CompareFunction.Always,
                ReferenceAlpha = 0
            };
        }

        /// <summary>
        /// Draws the revealing circles around the units in a transparent fashion.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawMasks(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, mInitializeMaskStencilState, null, mAlphaComparator, mCamera.GetTransform());

            foreach (var revealing in mRevealingObjects)
            {
                spriteBatch.DrawEllipse(new Rectangle((int) revealing.Center.X - revealing.RevelationRadius, (int) revealing.Center.Y - revealing.RevelationRadius / 2, revealing.RevelationRadius * 2, revealing.RevelationRadius), Color.Transparent, LayerConstants.FogOfWarLayer);
            }

            spriteBatch.End();

        }

        /// <summary>
        /// Fills the inverted masks with a semi transparent color to make the illusion of fog of war
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void FillInvertedMask(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, mApplyInvertedMaskStencilState, null, null, mCamera.GetTransform());

            spriteBatch.FillRectangle(new Rectangle(0, 0, MapConstants.MapWidth, MapConstants.MapHeight), new Color(Color.Black, 0.5f));

            spriteBatch.End();
        }

        /// <summary>
        /// Adds a revealing object to the fog of war.
        /// </summary>
        /// <param name="revealingObject">The object which can reveal the fog of war.</param>
        public void AddRevealingObject(IRevealing revealingObject)
        {
            if (!revealingObject.Friendly)
            {
                return;
            }

            mRevealingObjects.AddLast(revealingObject);
        }

        /// <summary>
        /// Removes a revealing object from the fog of war.
        /// </summary>
        /// <param name="revealingObject">The object which can reveal the fog of war.</param>
        public void RemoveRevealingObject(IRevealing revealingObject)
        {
            mRevealingObjects.Remove(revealingObject);
        }

        public void Update(GameTime gametime)
        {
            mAlphaComparator.Projection = mCamera.GetStencilProjection();
        }

        public DepthStencilState GetApplyMaskStencilState()
        {
            return mApplyMaskStencilState;
        }

        /// <summary>
        /// Gets all the revealing objects currently in the game.
        /// </summary>
        /// <returns>A list of all the revealing objects in the game</returns>
        public LinkedList<IRevealing> GetRevealingObjects()
        {
            return mRevealingObjects;
        }
    }
}

