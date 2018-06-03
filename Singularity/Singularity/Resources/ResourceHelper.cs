﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map.Properties;

namespace Singularity.Resources
{

    /// <summary>
    /// Provides a helper class to ease access to the corresponding color for a given resource type and provide random resources distribution.
    /// </summary>
    internal static class ResourceHelper
    {

        /// <summary>
        /// Gets the fitting color for the given resource type.
        /// </summary>
        /// <param name="type">The resource type of the resource to get the color for</param>
        /// <returns>The color for the given type</returns>
        public static Color GetColor(EResourceType type)
        {
            // TODO: use a more fitting color distribution, literally just chose anything here.
            switch (type)
            {
                case EResourceType.Chip:
                    return Color.LavenderBlush;

                case EResourceType.Concrete:
                    return Color.LightBlue;

                case EResourceType.Copper:
                    return Color.IndianRed;

                case EResourceType.Fuel:
                    return Color.DarkSlateGray;

                case EResourceType.Metal:
                    return Color.DarkMagenta;

                case EResourceType.Oil:
                    return Color.RosyBrown;

                case EResourceType.Plastic:
                    return Color.BlueViolet;

                case EResourceType.Sand:
                    return Color.Yellow;

                case EResourceType.Silicon:
                    return Color.Beige;

                case EResourceType.Steel:
                    return Color.Black;

                case EResourceType.Stone:
                    return Color.LightGray;

                case EResourceType.Water:
                    return Color.Aqua;

                case EResourceType.Trash:
                    return Color.Gray;

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// TODO: add docu
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static List<Resource> GetRandomlyDistributedResources(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            // one could use a normal distribution for this value and make it dynamic to the map size. This would actually yield a nice dynamic solution.
            const int defaultWidth = 200;

            EResourceType[] basicResources = {EResourceType.Water, EResourceType.Sand, EResourceType.Oil, EResourceType.Metal, EResourceType.Stone};

            var resources = new List<Resource>(amount);
            var rnd = new Random();

            for (var i = 0; i < amount; i++)
            {
                var xPos = rnd.Next(MapConstants.MapWidth - defaultWidth);
                var yPos = rnd.Next(MapConstants.MapHeight - (int) (defaultWidth * 0.6f));

                if (!Map.Map.IsOnTop(new Vector2(xPos, yPos)))
                {
                    amount++;
                    continue;
                }
                resources.Add(new Resource(basicResources[rnd.Next(basicResources.Length - 1)], new Vector2(xPos, yPos), defaultWidth));

            }
            return resources;

        }
    }
}
