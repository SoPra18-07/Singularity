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

        private readonly Camera mCamera;

        private readonly Texture2D mBackgroundTexture;

        private readonly bool mDebug;

        /// <summary>
        /// Creates a new Map object, which solely draws its background
        /// texture and if debugLine is given the grid. If initialResources isn't
        /// further specified there will be no resources on the map.
        /// </summary>
        /// <param name="backgroundTexture">The background texture of the map</param>
        /// <param name="viewport">The viewport of the window</param>
        /// <param name="debug">Whether the debug grid lines are drawn or not</param>
        /// <param name="initialResources">The initial resources of this map, if not specified there will not be any on the map</param>
        /// <param name="fow">The fog of war for this map</param>
        public Map(Texture2D backgroundTexture, Viewport viewport, InputManager inputManager, PathManager pathManager, bool debug = false, IEnumerable<Resource> initialResources = null)
        {

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

            //draw the background texture
            spriteBatch.Draw(mBackgroundTexture,
                new Rectangle(0, 0, MapConstants.MapWidth, MapConstants.MapHeight),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                LayerConstants.MapLayer);
            


            //make sure to only draw the grid if a texture is given.
            if (!mDebug)
            {
                return;

            }
            //draw the collision map grid.
            var collisonMap = mCollisionMap.GetCollisionMap();

            for (var columnCount = 0; columnCount <= collisonMap.GetLength(0); columnCount++)
            {

                spriteBatch.DrawLine(
                    new Vector2(columnCount * MapConstants.GridWidth, 0), MapConstants.MapHeight, MathHelper.Pi/2f, Color.Blue, 1, LayerConstants.GridDebugLayer);
            }

            for (var rowCount = 0; rowCount <= collisonMap.GetLength(0); rowCount++)
            {
                spriteBatch.DrawLine(
                    new Vector2(0, rowCount * MapConstants.GridHeight), MapConstants.MapWidth, 0, Color.Yellow, 1, LayerConstants.GridDebugLayer);
            }

            var colMap = mCollisionMap.GetCollisionMap();

            for(var i = 0; i < colMap.GetLength(0); i++)
            {
                for (var j = 0; j < colMap.GetLength(1); j ++)
                {
                    if (colMap[i, j].IsPresent())
                    {

                        spriteBatch.FillRectangle(new Rectangle(i * MapConstants.GridWidth, j * MapConstants.GridHeight, MapConstants.GridWidth, MapConstants.GridHeight), 
                            new Color(new Vector4(1, 0, 0, 0.2f)), 0f, LayerConstants.CollisionDebugLayer);
                    }
                }
            }
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

