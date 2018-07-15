using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Singularity.Manager;
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
            // for reference: https://sashat.me/2017/01/11/list-of-20-simple-distinct-colors/
            switch (type)
            {
                case EResourceType.Chip:
                    return Color.Lime; // updated

                case EResourceType.Concrete:
                    return Color.Lavender; // updated

                case EResourceType.Copper:
                    return Color.Orange; // updated

                case EResourceType.Fuel:
                    return Color.Magenta; // updated

                case EResourceType.Metal:
                    return Color.Teal; // updated

                case EResourceType.Oil:
                    return Color.Brown; // updated

                case EResourceType.Plastic:
                    return Color.Purple; // updated

                case EResourceType.Sand:
                    return Color.Yellow; // updated

                case EResourceType.Silicon:
                    return Color.Cyan; // updated

                case EResourceType.Steel:
                    return Color.Navy; // updated

                case EResourceType.Stone:
                    return Color.Gray; // updated

                case EResourceType.Water:
                    return Color.Blue; // updated

                case EResourceType.Trash:
                    return Color.Red; // updated

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Randomly distributes basic resources on the map, specified by the amount.
        /// </summary>
        /// <param name="amount">The amount of resources to distribute onto the map</param>
        /// <returns>A list of the new created resources</returns>
        public static List<MapResource> GetRandomlyDistributedResources(int amount, ref Director director)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            // TODO: one could use a normal distribution for this value and make it dynamic to the map size. This would actually yield a nice dynamic solution.
            const int defaultWidth = 200;

            // make sure to only distribute basic resources
            EResourceType[] basicResources = {EResourceType.Water, EResourceType.Oil, EResourceType.Metal};

            var resources = new List<MapResource>(amount);
            var rnd = new Random();

            for (var i = 0; i < amount; i++)
            {
                var xPos = rnd.Next(MapConstants.MapWidth - defaultWidth);
                var yPos = rnd.Next(MapConstants.MapHeight - (int) (defaultWidth * 0.6f));

                //check whether the rectangle enclosing the ellipse is on the map. Then the ellipse has to be on the map aswell
                if (!Map.Map.IsOnTop(new Rectangle(xPos, yPos, defaultWidth, (int) (defaultWidth * 0.6f))))
                {
                    amount++;
                    continue;
                }
                resources.Add(new MapResource(basicResources[rnd.Next(basicResources.Length)], new Vector2(xPos, yPos), defaultWidth, ref director));
                Debug.WriteLine("Resources " + basicResources[rnd.Next(basicResources.Length)] + " can be found at " + xPos + ", " + yPos);
            }
            return resources;

        }
    }
}
