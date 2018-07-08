﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;

namespace Singularity.Platforms
{
    /// <summary>
    /// The basic idea for this was to provide an easy way to get a platform via the platformtype.
    /// </summary>
    static class PlatformFactory
    {
        private static Texture2D sConeSheet;

        private static Texture2D sCylinderSheet;

        private static Texture2D sDomeSheet;

        private static Texture2D sBlankSheet;

        private static SpriteFont sLibSans12;

        /// <summary>
        /// Initializes the platform factory with the sprite sheets needed.
        /// </summary>
        /// <param name="coneSheet">The cone sprite sheet for the facilities</param>
        /// <param name="cylinderSheet">The cylinder sprite sheet for the facilities</param>
        /// <param name="domeSheet">The dome sprite sheet for the facilities</param>
        /// <param name="blankSheet">The blank sprite sheet for the facilities</param>
        public static void Init(Texture2D coneSheet, Texture2D cylinderSheet, Texture2D domeSheet, Texture2D blankSheet, SpriteFont libSans12)
        {
            sConeSheet = coneSheet;
            sCylinderSheet = cylinderSheet;
            sDomeSheet = domeSheet;
            sBlankSheet = blankSheet;
            sLibSans12 = libSans12;
        }

        /// <summary>
        /// Returns the platform specified by the given platform type.
        /// </summary>
        /// <param name="type">The type of the platform to get</param>
        /// <param name="director">A reference to the director needed to initializes some platforms</param>
        /// <param name="x">The initial x coordinate of the platform</param>
        /// <param name="y">The initial y coordinate of the platform</param>
        /// <param name="resourceMap">The resource map needed to initialize some platforms</param>
        /// <returns></returns>
        public static PlatformBlank Get(EPlatformType type, ref Director director, float x = 0, float y = 0, ResourceMap resourceMap = null, bool autoRegister = true)
        {
            //TODO: add conesheet to this query. Its not included right now since it doesn't exists at this point in time
            if (sCylinderSheet == null || sDomeSheet == null || sBlankSheet == null)
            {
                throw new Exception("Init needs to be called before.");
            }

            var position = new Vector2(x, y);

            switch (type)
            {
                case EPlatformType.Quarry:
                    return new Quarry(position, sDomeSheet, sBlankSheet, libSans12: sLibSans12, resource: resourceMap, director: ref director, autoRegister: autoRegister);

                case EPlatformType.Barracks:
                    return new Barracks(position, sCylinderSheet, sBlankSheet, sLibSans12, ref director);

                case EPlatformType.Blank:
                    return new PlatformBlank(position, sBlankSheet, sBlankSheet, sLibSans12, ref director);

                case EPlatformType.Command:
                    return new CommandCenter(position, sCylinderSheet, sBlankSheet, sLibSans12, ref director);

                case EPlatformType.Energy:
                    return new EnergyFacility(position, sDomeSheet, sBlankSheet, sLibSans12, ref director);

                case EPlatformType.Factory:
                    return new Factory(position, sDomeSheet, sBlankSheet, sLibSans12, ref director);

                case EPlatformType.Junkyard:
                    return new Junkyard(position, sDomeSheet, sBlankSheet, sLibSans12, ref director);

                case EPlatformType.Kinetic:
                    return new DefenseKinetic(position, sConeSheet, sBlankSheet, ref director);

                case EPlatformType.Mine:
                    return new Mine(position, sDomeSheet, sBlankSheet, libSans12: sLibSans12, resource: resourceMap, director: ref director, autoRegister: autoRegister);

                case EPlatformType.Packaging:
                    throw new NotImplementedException("Packaging facilities have not yet been implemented");

                case EPlatformType.Storage:
                    return new Storage(position, sDomeSheet, sBlankSheet, sLibSans12, ref director);

                case EPlatformType.Well:
                    return new Well(position, sDomeSheet, sBlankSheet, libSans12: sLibSans12, resource: resourceMap, director: ref director, autoRegister: autoRegister);

                case EPlatformType.Laser:
                    return new DefenseLaser(position, sConeSheet, sBlankSheet, ref director);

                default:
                    break;
            }
            throw new ArgumentOutOfRangeException(nameof(type), type, "The given type is not supported");

        }
    }
}
