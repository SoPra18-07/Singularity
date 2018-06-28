using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;

namespace Singularity.Platform
{
    static class PlatformFactory
    {
        private static Texture2D mConeSheet;

        private static Texture2D mCylinderSheet;

        private static Texture2D mDomeSheet;

        private static Texture2D mBlankSheet;

        private static Director mDirector;


        public static void Init(Texture2D coneSheet, Texture2D cylinderSheet, Texture2D domeSheet, Texture2D blankSheet)
        {
            mConeSheet = coneSheet;
            mCylinderSheet = cylinderSheet;
            mDomeSheet = domeSheet;
            mBlankSheet = blankSheet;
        }

        public static PlatformBlank Get(EPlatformType type, ref Director director, int x = 0, int y = 0, ResourceMap resourceMap = null)
        {
            //TODO: add conesheet to this query. Its not included right now since it doesn't exists at this point in time
            if (mCylinderSheet == null || mDomeSheet == null || mBlankSheet == null)
            {
                throw new Exception("Init needs to be called before.");
            }

            var position = new Vector2(x, y);

            switch (type)
            {
                case EPlatformType.Quarry:
                    return new Quarry(position, mDomeSheet, mBlankSheet, resourceMap, ref director);

                case EPlatformType.Barracks:
                    throw new NotImplementedException("Barracks have not yet been implemented");

                case EPlatformType.Blank:
                    return new PlatformBlank(position, mBlankSheet, mBlankSheet);

                case EPlatformType.Command:
                    return new CommandCenter(position, mCylinderSheet, mBlankSheet, ref director);

                case EPlatformType.Energy:
                    return new EnergyFacility(position, mDomeSheet, mBlankSheet);

                case EPlatformType.Factory:
                    return new Factory(position, mDomeSheet, mBlankSheet);

                case EPlatformType.Junkyard:
                    return new Junkyard(position, mDomeSheet, mBlankSheet, ref director);

                case EPlatformType.Kinetic:
                    throw new NotImplementedException("Kinetic Facilities have not yet been implemented");

                case EPlatformType.Mine:
                    return new Mine(position, mDomeSheet, mBlankSheet, resourceMap, ref director);

                case EPlatformType.Packaging:
                    throw new NotImplementedException("Packaging facilities have not yet been implemented");

                case EPlatformType.Storage:
                    return new Storage(position, mDomeSheet, mBlankSheet);

                case EPlatformType.Well:
                    return new Well(position, mDomeSheet, mBlankSheet, resourceMap, ref director);

                case EPlatformType.Laser:
                    throw new NotImplementedException("Laser facilites have not yet been implemented");

                default:
                    break;
            }
            throw new ArgumentOutOfRangeException(nameof(type), type, "The given type is not supported");

        }
    }
}
