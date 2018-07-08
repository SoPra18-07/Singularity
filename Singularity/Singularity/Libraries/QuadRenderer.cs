
#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Singularity.Libraries
{
    /// <summary>
    /// Renders a simple quad to the screen. Uncomment the Vertex / Index buffers to make it a static fullscreen quad.
    /// The performance effect is barely measurable though and you need to dispose of the buffers when finished!
    /// </summary>
    public class QuadRenderer
    {
        //buffers for rendering the quad
        private readonly VertexPositionTexture[] mVertexBuffer;
        private readonly short[] mIndexBuffer;

        //private VertexBuffer _vBuffer;
        //private IndexBuffer _iBuffer;

        public QuadRenderer(GraphicsDevice graphicsDevice)
        {
            mVertexBuffer = new VertexPositionTexture[4];
            mVertexBuffer[0] = new VertexPositionTexture(position: new Vector3(x: -1, y: 1, z: 1), textureCoordinate: new Vector2(x: 0, y: 0));
            mVertexBuffer[1] = new VertexPositionTexture(position: new Vector3(x: 1, y: 1, z: 1), textureCoordinate: new Vector2(x: 1, y: 0));
            mVertexBuffer[2] = new VertexPositionTexture(position: new Vector3(x: -1, y: -1, z: 1), textureCoordinate: new Vector2(x: 0, y: 1));
            mVertexBuffer[3] = new VertexPositionTexture(position: new Vector3(x: 1, y: -1, z: 1), textureCoordinate: new Vector2(x: 1, y: 1));

            mIndexBuffer = new short[] { 0, 3, 2, 0, 1, 3 };

            //_vBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            //_iBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);

            //_vBuffer.SetData(_vertexBuffer);
            //_iBuffer.SetData(_indexBuffer);

        }

        public void RenderQuad(GraphicsDevice graphicsDevice, Vector2 v1, Vector2 v2)
        {
            mVertexBuffer[0].Position.X = v1.X;
            mVertexBuffer[0].Position.Y = v2.Y;

            mVertexBuffer[1].Position.X = v2.X;
            mVertexBuffer[1].Position.Y = v2.Y;

            mVertexBuffer[2].Position.X = v1.X;
            mVertexBuffer[2].Position.Y = v1.Y;

            mVertexBuffer[3].Position.X = v2.X;
            mVertexBuffer[3].Position.Y = v1.Y;

            graphicsDevice.DrawUserIndexedPrimitives
                (primitiveType: PrimitiveType.TriangleList, vertexData: mVertexBuffer, vertexOffset: 0, numVertices: 4, indexData: mIndexBuffer, indexOffset: 0, primitiveCount: 2);

            //graphicsDevice.SetVertexBuffer(_vBuffer);
            //graphicsDevice.Indices = (_iBuffer);

            //graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
            //    0, 2);
        }
    }
}
