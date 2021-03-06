﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map.Properties;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Resources;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Content;
using Singularity.Screen.ScreenClasses;
using Singularity.Utils;
using Singularity.Levels;

namespace Singularity.Map
{
    [DataContract]
    public sealed class Map : IDraw, IUpdate
    {
        [DataMember]
        private readonly CollisionMap mCollisionMap;
        [DataMember]
        private readonly StructureMap mStructureMap;
        [DataMember]
        private readonly ResourceMap mResourceMap;
        [DataMember]
        private readonly int mWidth;
        [DataMember]
        private readonly int mHeight;

        private Camera mCamera;

        private Texture2D mBackgroundTexture;

        private FogOfWar mFow;

        [DataMember]
        private int mXPosMin;
        [DataMember]
        private int mXPosMax;
        [DataMember]
        private int mYPosMin;
        [DataMember]
        private int mYPosMax;


        /// <summary>
        /// Creates a new Map object, which solely draws its background
        /// texture and if debugLine is given the grid. If initialResources isn't
        /// further specified there will be no resources on the map.
        /// </summary>
        /// <param name="backgroundTexture">The background texture of the map</param>
        /// <param name="width">The width of the map in number of tiles</param>
        /// <param name="height">The height of the map in number of tiles</param>
        /// <param name="fow">The FoW of the Map</param>
        /// <param name="camera">The camera of the window</param>
        /// <param name="director">A reference to the Director</param>
        public Map(Texture2D backgroundTexture,
            int width,
            int height,
            FogOfWar fow,
            Camera camera,
            ref Director director)
        {
            mWidth = width;
            mHeight = height;

            mCamera = camera;

            mBackgroundTexture = backgroundTexture;

            mFow = fow;

            var initialResources = director.GetStoryManager.Level is TechDemo ? null : ResourceHelper.GetRandomlyDistributedResources(50, ref director);


            mCollisionMap = new CollisionMap();
            mStructureMap = new StructureMap(fow, ref director);
            mResourceMap = new ResourceMap(initialResources, director);

            director.GetStoryManager.StructureMap = mStructureMap;
        }

        public void ReloadContent(Texture2D background, Camera camera, FogOfWar fow, ref Director dir, ContentManager content, UserInterfaceScreen ui)
        {
            mBackgroundTexture = background;
            mCamera = camera;
            mFow = fow;
            //ADD ALL THE THINGS TO THE CAMERA AND THE FOW
            mStructureMap.ReloadContent(content, mFow, ref dir, mCamera, this, ui);
            mCollisionMap.ReloadContent();
            mResourceMap.ReloadContent(ref dir);
        }

        /// <see cref="CollisionMap.UpdateCollider(ICollider)"/>
        internal void UpdateCollider(ICollider collider)
        {
            mCollisionMap.UpdateCollider(collider);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            var x = 0;
            var y = 0;
            //draw the background texture
            for (var column = 0; column < mWidth; column++)
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

                    var xpos = Math.Abs(row - column - (mWidth - 1));
                    var ypos = column + row;

                    if (xpos < mYPosMin || xpos > mYPosMax || ypos < mXPosMin || ypos > mXPosMax)
                    {
                        continue;
                    }

                    spriteBatch.Draw(mBackgroundTexture,
                        new Vector2(xpos * 100, ypos * 50),
                        new Rectangle(x * MapConstants.TileWidth, y * MapConstants.TileHeight, MapConstants.TileWidth, MapConstants.TileHeight),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        Vector2.One,
                        SpriteEffects.None,
                        LayerConstants.MapLayer);
                }
            }


            //make sure to only draw the grid if a texture is given.
            if (!GlobalVariables.DebugState)
            {
                return;

            }

            var colMap = mCollisionMap.GetCollisionMap();
            var walkabilityGrid = mCollisionMap.GetWalkabilityGrid();

            for (var columnCount = 0; columnCount <= colMap.GetLength(0); columnCount++)
            {
                spriteBatch.DrawLine(
                    new Vector2(columnCount * MapConstants.GridWidth, 0), MapConstants.MapHeight, MathHelper.Pi / 2f, Color.Blue, 1, LayerConstants.GridDebugLayer);
            }

            for (var rowCount = 0; rowCount <= colMap.GetLength(1); rowCount++)
            {
                spriteBatch.DrawLine(
                    new Vector2(0, rowCount * MapConstants.GridHeight), MapConstants.MapWidth, 0, Color.Yellow, 1, LayerConstants.GridDebugLayer);
            }



            for(var i = (int) (mCamera.GetRelativePosition().X / MapConstants.GridWidth); i < (mCamera.GetRelativePosition().X + mCamera.GetSize().X) / MapConstants.GridWidth; i++)
            {
                for (var j = (int) (mCamera.GetRelativePosition().Y / MapConstants.GridHeight); j < (mCamera.GetRelativePosition().Y + mCamera.GetSize().Y) / MapConstants.GridHeight; j ++)
                {
                    if (!walkabilityGrid.IsWalkableAt(i, j))
                    {
                        spriteBatch.FillRectangle(rect: new Rectangle(x: (i * MapConstants.GridWidth), y: j * MapConstants.GridHeight, width: MapConstants.GridWidth, height: MapConstants.GridHeight),
                            color: new Color(color: new Vector4(x: 1, y: 0, z: 0, w: 0.2f)), angle: 0f, layer: LayerConstants.CollisionDebugLayer);
                    }
                }
            }


        }

        public void Update(GameTime gametime)
        {
            // -1 for the left/top limits since we want fluid transitions and not the effect of tiles "lagging behind".
            mXPosMin = (int)(mCamera.GetRelativePosition().Y / 50) - 1;
            mXPosMax = (int)(mCamera.GetRelativePosition().Y / 50 + mCamera.GetSize().Y / 50);

            mYPosMin = (int)(mCamera.GetRelativePosition().X / 100) - 1;
            mYPosMax = (int)(mCamera.GetRelativePosition().X / 100 +
                                mCamera.GetSize().X / 100);
        }


        internal FogOfWar GetFogOfWar()
        {
            return mFow;
        }

        /// <see cref="StructureMap.AddPlatform(PlatformBlank)"/>
        internal void AddPlatform(PlatformBlank platform)
        {
            mStructureMap.AddPlatform(platform);
        }

        /// <see cref="StructureMap.RemovePlatform(PlatformBlank)"/>
        internal void RemovePlatform(PlatformBlank platform)
        {
            mStructureMap.RemovePlatform(platform);
        }

        internal void AddRoad(Road road)
        {
            mStructureMap.AddRoad(road);
        }

        internal void RemoveRoad(Road road)
        {
            mStructureMap.RemoveRoad(road);
        }

        internal StructureMap GetStructureMap()
        {
            return mStructureMap;
        }

        internal CollisionMap GetCollisionMap()
        {
            return mCollisionMap;
        }

        internal ResourceMap GetResourceMap()
        {
            return mResourceMap;
        }

        internal Vector2 GetMeasurements()
        {
            return new Vector2(mWidth, mHeight);
        }

        /// <summary>
        /// Checks whether the given vector is on the map.
        /// </summary>
        /// <param name="position">The position of which to check whether it is on the map</param>
        /// <param name="camera">The camera is needed to translate relative coordinates into absolute ones, if null then the given coordinates are treated as absolute ones</param>
        /// <returns>True if the position is on the map, false otherwise</returns>
        internal static bool IsOnTop(Vector2 position, Camera camera = null)
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
                camera == null ? position : Vector2.Transform(position, Matrix.Invert(camera.GetTransform()));


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

            return sign == 1 && sign2 == 1 && sign3 == 1 && sign4 == 1;

        }

        /// <summary>
        /// Checks whether the given rectangle is on the map.
        /// </summary>
        /// <param name="rect">The rectangle which should be checked whether its on the map</param>
        /// <param name="camera">The camera is needed to translate relative coordinates into absolute ones, if null then the given coordinates are treated as absolute ones</param>
        /// <returns>True if the rectangle is on the map, false otherwise</returns>
        internal static bool IsOnTop(Rectangle rect, Camera camera = null)
        {

            // simple logic, this yields true if all of them are true and false if one is false. One can easily convince himself,
            // that if all the "edge" points of the rectangle are on the map then the rectangle is on the map.

            return IsOnTop(new Vector2(rect.X, rect.Y), camera) &&
                   IsOnTop(new Vector2(rect.X + rect.Width, rect.Y), camera) &&
                   IsOnTop(new Vector2(rect.X, rect.Y + rect.Height), camera) &&
                   IsOnTop(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), camera);

        }

        public bool IsInVision(Rectangle rect)
        {
           // if one of the edge points of the given rectangle is already in view make sure to return true.

            return (IsInVision(new Vector2(rect.X, rect.Y)) ||
                    IsInVision(new Vector2(rect.X + rect.Width, rect.Y)) ||
                    IsInVision(new Vector2(rect.X, rect.Y + rect.Height)) ||
                    IsInVision(new Vector2(rect.X + rect.Width, rect.Y + rect.Height)) ||
                    IsInVision(new Vector2(rect.Center.X, rect.Center.Y)));
        }

        private bool IsInVision(Vector2 position)
        {
            var inVision = false;

            foreach (var revealing in mFow.GetRevealingObjects())
            {
                inVision = inVision || Geometry.Contains(revealing.Center, revealing.RevelationRadius, position);
            }

            return inVision;
        }


        /// <summary>
        /// Gets a valid random position on the current map
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetRandomPositionOnMap()
        {
            var random = new Random();

            var isOnMap = false;
            var pos = Vector2.Zero;

            while (!isOnMap)
            {
                pos = new Vector2(random.Next(MapConstants.MapWidth), random.Next(MapConstants.MapHeight));
                if (IsOnTop(pos))
                {
                    isOnMap = true;
                }

            }
            return pos;

        }

    }
}
