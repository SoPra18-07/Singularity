using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Exceptions;
using Singularity.Graph.Paths;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Map.Properties;
using Singularity.Platform;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Utils;

namespace Singularity.Map
{   
    internal sealed class Map : IDraw, IUpdate
    {
        private readonly CollisionMap mCollisionMap;
        private readonly StructureMap mStructureMap;
        private readonly ResourceMap mResourceMap;

        private readonly int mWidth;
        private readonly int mHeight;

        private readonly Camera mCamera;

        private readonly Texture2D mBackgroundTexture;
        private readonly SpriteFont mLibSans12;

        private readonly bool mDebug;


        /// <summary>
        /// Creates a new Map object, which solely draws its background
        /// texture and if debugLine is given the grid. If initialResources isn't
        /// further specified there will be no resources on the map.
        /// </summary>
        /// <param name="backgroundTexture">The background texture of the map</param>
        /// <param name="width">The width of the map in number of tiles</param>
        /// <param name="viewport">The viewport of the window</param>
        /// <param name="inputManager">InputManager used by the game</param>
        /// <param name="pathManager">PathManager used by the game</param>
        /// <param name="debug">Whether the debug grid lines are drawn or not</param>
        /// <param name="initialResources">The initial resources of this map, if not specified there will not be any on the map</param>
        /// <param name="height">The height of the map in number of tiles</param>
        public Map(Texture2D backgroundTexture,
            int height,
            int width,
            Viewport viewport,
            InputManager inputManager,
            PathManager pathManager,
            SpriteFont debugFont,
            bool debug = false,
            IEnumerable<Resource> initialResources = null)
        {
            mWidth = width;
            mHeight = height;
            mLibSans12 = debugFont;

            mBackgroundTexture = backgroundTexture;
            mDebug = debug;

            mCamera = new Camera(viewport, inputManager, 0, 0);

            mCollisionMap = new CollisionMap();
            mStructureMap = new StructureMap(pathManager);
            mResourceMap = new ResourceMap(initialResources);
        }

        /// <see cref="CollisionMap.UpdateCollider(ICollider)"/>
        public void UpdateCollider(ICollider collider)
        {
            mCollisionMap.UpdateCollider(collider);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var x = 0;
            var y = 0;
            //draw the background texture
            for (int column = 0; column < mWidth; column++)
            {
                // variables are used to choose which tile to draw
                
                if (column == 0)
                {
                    y = 0;
                }
                if (column > 0)
                {
                    y = 1;
                }

                if (column == mWidth - 1)
                {
                    y = 2;
                }
                for (int row = 0; row < mHeight; row++)
                {
                    if (row == 0)
                    {
                        x = 2;
                    }
                    if (row > 0)
                    {
                        x = 1;
                    }

                    if (row == mHeight - 1)
                    {
                        x = 0;
                    }

                    var xpos = Math.Abs(value: row - column - (mWidth - 1));
                    var ypos = column + row;
                    spriteBatch.Draw(texture: mBackgroundTexture,
                        position: new Vector2(x: xpos * 100, y: ypos * 50),
                        sourceRectangle: new Rectangle(x: x * 200, y: y * 100, width: 200, height: 100),
                        color: Color.White,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: Vector2.One,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.MapLayer);
                }
            }


            //make sure to only draw the grid if a texture is given.
            if (!mDebug)
            {
                return;

            }
            //draw the collision map grid.
            for (int column = 0; column < mWidth; column++)
            {
                for (var i = 0; i < 5; i++)
                {
                    int xseparator = 20 * i;
                    int yseparator = 10 * i;
                    int xpos = column + mWidth;
                    int ypos = column;
                    int xpos2 = mWidth - column;
                    spriteBatch.DrawLine(
                        point: new Vector2(x: xpos * 100 + xseparator, y: ypos * 50 + yseparator),
                        length: 2236.0679775f,
                        angle: -0.463647609f + (float) Math.PI,
                        color: Color.Blue,
                        thickness: 1,
                        layerDepth: LayerConstants.GridDebugLayer);
                    spriteBatch.DrawLine(
                        point: new Vector2(x: xpos2 * 100 - xseparator, y: ypos * 50 + yseparator),
                        length: 2236.0679775f,
                        angle: 0.463647609f,
                        color: Color.Yellow,
                        thickness: 1,
                        layerDepth: LayerConstants.GridDebugLayer);
                }
            }
            /*
            var colMap = mCollisionMap.GetCollisionMap();

            for(var i = 0; i < colMap.GetLength(dimension: 0); i++)
            {
                for (var j = 0; j < colMap.GetLength(dimension: 1); j ++)
                {
                    if (!colMap[i, j].IsWalkable())
                    {

                        spriteBatch.FillRectangle(rect: new Rectangle(x: i * MapConstants.GridWidth, y: j * MapConstants.GridHeight, width: MapConstants.GridWidth, height: MapConstants.GridHeight), 
                            color: new Color(color: new Vector4(x: 1, y: 0, z: 0, w: 0.2f)), angle: 0f, layer: LayerConstants.CollisionDebugLayer);
                    }
                }
            }
            */
        }

        //TODO: remove if input manager is available since we only use this to pass an update to the camera.
        public void Update(GameTime gametime)
        {
            mCamera.Update(gametime);
        }

        /// <see cref="StructureMap.AddPlatform(PlatformBlank)"/>
        public void AddPlatform(PlatformBlank platform)
        {
            mStructureMap.AddPlatform(platform);
        }

        /// <see cref="StructureMap.RemovePlatform(PlatformBlank)"/>
        public void RemovePlatform(PlatformBlank platform)
        {
            mStructureMap.RemovePlatform(platform);
        }

        public void AddRoad(Road road)
        {
            mStructureMap.AddRoad(road);
        }

        public void RemoveRoad(Road road)
        {
            mStructureMap.RemoveRoad(road);
        }

        public StructureMap GetStructureMap()
        {
            return mStructureMap;
        }
        
        public CollisionMap GetCollisionMap()
        {
            return mCollisionMap;
        }

        public ResourceMap GetResourceMap()
        {
            return mResourceMap;
        }

        public Camera GetCamera()
        {
            return mCamera;
        }

        /// <summary>
        /// Checks whether the given vector is on the map.
        /// </summary>
        /// <param name="position">The position of which to check whether it is on the map</param>
        /// <param name="camera">The camera is needed to translate relative coordinates into absolute ones, if null then the given coordinates are treated as absolute ones</param>
        /// <returns>True if the position is on the map, false otherwise</returns>
        public static bool IsOnTop(Vector2 position, Camera camera = null)
        {
            //TODO: extend to rectangle, so we move away from whether the origin point is on the map.

            /*
             * For those who are interested, this is basically the sign of the determinant of the following matrix:
             *
             * [B.X - A.X   B.Y - A.Y]
             * [M.X - A.X   M.Y - A.Y]
             *
             * where a line is specified by the points B(X, Y) and A(X, Y). M(X, Y) is the specific point we want to check against.
             * obviously the sign of the determinant is either -1, 0 or 1. One could think of it as "-1 if its to your left when looking from A
             * to B, 0 if its on the line, and 1 if its to your right when looking from A to B".
             * For our particular case the respective sign's are 1 when the given point is "inwards" as in on the map. And if thats the case for all the
             * lines that specify our map the given position is also on the map.
             */

            var worldSpacePosition =
                (camera == null ? position : Vector2.Transform(position, Matrix.Invert(camera.GetTransform())));


            var sign = Math.Sign(
                (MapConstants.sTop.X - MapConstants.sLeft.X) * (worldSpacePosition.Y - MapConstants.sLeft.Y) -
                (MapConstants.sTop.Y - MapConstants.sLeft.Y) * (worldSpacePosition.X - MapConstants.sLeft.X)
            );

            var sign2 = Math.Sign(
                (MapConstants.sLeft.X - MapConstants.sBottom.X) * (worldSpacePosition.Y - MapConstants.sBottom.Y) -
                (MapConstants.sLeft.Y - MapConstants.sBottom.Y) * (worldSpacePosition.X - MapConstants.sBottom.X)
            );

            var sign3 = Math.Sign(
                (MapConstants.sRight.X - MapConstants.sTop.X) * (worldSpacePosition.Y - MapConstants.sTop.Y) -
                (MapConstants.sRight.Y - MapConstants.sTop.Y) * (worldSpacePosition.X - MapConstants.sTop.X)
            );

            var sign4 = Math.Sign(
                (MapConstants.sBottom.X - MapConstants.sRight.X) * (worldSpacePosition.Y - MapConstants.sRight.Y) -
                (MapConstants.sBottom.Y - MapConstants.sRight.Y) * (worldSpacePosition.X - MapConstants.sRight.X)
            );

            return (sign == 1 && sign2 == 1 && sign3 == 1 && sign4 == 1);

        }

        /// <summary>
        /// Checks whether the given rectangle is on the map.
        /// </summary>
        /// <param name="rect">The rectangle which should be checked whether its on the map</param>
        /// <param name="camera">The camera is needed to translate relative coordinates into absolute ones, if null then the given coordinates are treated as absolute ones</param>
        /// <returns>True if the rectangle is on the map, false otherwise</returns>
        public static bool IsOnTop(Rectangle rect, Camera camera = null)
        {

            // simple logic, this yields true if all of them are true and false if one is false. One can easily convince himself,
            // that if all the "edge" points of the rectangle are on the map then the rectangle is on the map.

            return (IsOnTop(new Vector2(rect.X, rect.Y), camera) && 
                    IsOnTop(new Vector2(rect.X + rect.Width, rect.Y), camera) &&
                    IsOnTop(new Vector2(rect.X, rect.Y + rect.Height), camera) &&
                    IsOnTop(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), camera));

        }

    }
}

