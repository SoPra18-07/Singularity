using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Exceptions;
using Singularity.Libraries;
using Singularity.Map.Properties;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Screen;
using Singularity.Units;

namespace Singularity.Map
{
    /// <summary>
    /// The idea for the MiniMap is to show the whole map at all times, but with simplified means. For example:
    /// platforms are represented with dots, as are military units (in the future). The original idea was to put
    /// EVERYTHING on the minimap, roads, resources, enemy units, enemy buildings, etc. The
    /// "true" purpose of a minimap is to be able to see everything at once at first glance without having to
    /// move the games camera to that exact location. 
    /// </summary>
    public sealed class MiniMap : IWindowItem
    {
        /// <summary>
        /// The interval in which the references to all the ingame objects get updated
        /// </summary>
        private const int UpdateMilliInterval = 1000;

        /// <summary>
        /// The map of the current game
        /// </summary>
        private readonly Map mMap;

        /// <summary>
        /// The camera of the current game
        /// </summary>
        private readonly Camera mCamera;

        /// <summary>
        /// The factor at which to downscale game objects as a whole.
        /// </summary>
        private readonly int mDownscaleFactor;

        /// <summary>
        /// The texture for the minimap
        /// </summary>
        private readonly Texture2D mTexture;

        /// <summary>
        /// A list of all revealing objects in the game
        /// </summary>
        private LinkedList<IRevealing> mRevealing;

        /// <summary>
        /// A list of all the platforms in the game
        /// </summary>
        private LinkedList<PlatformBlank> mPlatforms;

        /// <summary>
        /// A list of all the military units in the game
        /// </summary>

        /// <summary>
        /// A list of all the resources in the game
        /// </summary>
        private List<MapResource> mMapResources;

        public Vector2 Position { get; set; }

        public Vector2 Size { get; private set; }

        public bool ActiveInWindow { get; set; }
        public bool InactiveInSelectedPlatformWindow { get; set; }
        public bool OutOfScissorRectangle { get; set; }

        public MiniMap(Map map, Camera camera, Texture2D minimapTexture)
        {
            // note, currently the exception is always false, since the values do work together, but in the future, when
            // the map size gets changed this might occur.
            if (MapConstants.MapWidth / MapConstants.MiniMapWidth !=
                MapConstants.MapHeight / MapConstants.MiniMapHeight)
            {
                throw new MiniMapProportionsOffException("The map width and/or height is not compatible with the minimap width/height");
            }
            mMap = map;
            mCamera = camera;
            mDownscaleFactor = MapConstants.MapWidth / MapConstants.MiniMapWidth;
            mTexture = minimapTexture;

            mRevealing = new LinkedList<IRevealing>();
            mPlatforms = new LinkedList<PlatformBlank>();
            mMapResources = new List<MapResource>();

            Size = new Vector2(MapConstants.MiniMapWidth, MapConstants.MiniMapHeight);
            Position = new Vector2(0, 0);
            ActiveInWindow = true;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // note: since we don't use layers on the UI screen we have to draw in the order we want to objects to be layered onto another

            // first draw the background texture for the minimap. + 10, since we don't automatically get a padding for the top in the windowObject class.
            spriteBatch.Draw(mTexture, new Rectangle((int)Position.X, (int)Position.Y + 10, (int)Size.X, (int)Size.Y), Color.White);

            // first draw the revelation circles
            foreach (var revealing in mRevealing)
            {
                var newCenter = revealing.Center / mDownscaleFactor;

                var centerInMiniMap = new Vector2(Position.X + newCenter.X, Position.Y + 10 + newCenter.Y);

                // ReSharper disable once PossibleLossOfFraction
                spriteBatch.FillCircle(centerInMiniMap, revealing.RevelationRadius / mDownscaleFactor, 40, Color.White);
            }

            // now draw the platforms. Currently platforms are resembled with a 1 pixel green dot
            foreach (var platform in mPlatforms)
            {
                var newCenter = platform.Center / mDownscaleFactor;

                var centerInMiniMap = new Vector2(Position.X + newCenter.X, Position.Y + 10 + newCenter.Y);

                spriteBatch.FillCircle(centerInMiniMap, 1, 2, Color.Green);
            }

            //TODO: maybe draw resources on the mini map, currently the list of all resources is null, which prevents me from trying it
            /*
            foreach (var resource in mMapResources)
            {
                var resourceInMiniMap = new Rectangle((int) (Position.X + resource.AbsolutePosition.X / mDownscaleFactor), (int) (Position.Y + 10 + resource.AbsolutePosition.Y / mDownscaleFactor), (int) (resource.AbsoluteSize.X / mDownscaleFactor), (int) (resource.AbsoluteSize.Y / mDownscaleFactor));

                spriteBatch.DrawEllipse(resourceInMiniMap, ResourceHelper.GetColor(resource.Type), 0f);
            }
            */

            // draw the cameras viewport on the minimap
            spriteBatch.StrokedRectangle(new Vector2(Position.X + mCamera.GetRelativePosition().X / mDownscaleFactor, Position.Y + 10 + mCamera.GetRelativePosition().Y / mDownscaleFactor), mCamera.GetSize() / mDownscaleFactor, Color.Red, Color.Transparent, 1f, 0f);
        }

        public void Update(GameTime gametime)
        {
            // only update UpdateMilliInterval milliseconds. The basic idea was to refetch all the gameobjects every second
            // but I decided to just pass the reference which makes things alot easier. 

            // TODO: this could be replaced with an initialize method, that sets the reference as soons as all of them
            // TODO: are actually in the memory. For now this is fine.
            if (((int)gametime.TotalGameTime.TotalMilliseconds) % UpdateMilliInterval == 0)
            {
                return;
            }

            mPlatforms = mMap.GetStructureMap().GetPlatformList();
            mMapResources = mMap.GetResourceMap().GetAllResources();
            mRevealing = mMap.GetFogOfWar().GetRevealingObjects();

        }
    }
}

