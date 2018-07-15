using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;

namespace Singularity.Platforms
{
    /// <inheritdoc cref="PlatformBlank"/>
    [DataContract]
    internal class Barracks : PlatformBlank
    {
        private List<AMakeUnit> ProducableUnits = new List<AMakeUnit>();

        public Barracks(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            SpriteFont libSans12,
            ref Director director,
            bool friendly = true)
            : base(position: position,
                platformSpriteSheet: platformSpriteSheet,
                baseSprite: baseSprite,
                libsans12: libSans12,
                director: ref director,
                type: EStructureType.Barracks,
                friendly: friendly)
        {
            ProducableUnits.Add(new MakeFastMilitaryUnit(this, ref director));
            ProducableUnits.Add(new MakeHeavyMilitaryUnit(this, ref director));
            ProducableUnits.Add(new MakeStandardMilitaryUnit(this, ref director));

            // Todo: add cost if you have it.
            // mCost = new Dictionary<EResourceType, int>();
            
        }
        
        public override List<IPlatformAction> GetIPlatformActions()
        {
            var list = new List<IPlatformAction>(ProducableUnits);
            list.AddRange(base.GetIPlatformActions().AsEnumerable());
            // Debug.WriteLine("List is " + list.Count + " long. ?");
            return list;
        }

        public override void Update(GameTime t)
        {
            ProducableUnits.ForEach(a => a.Update(t));
            base.Update(t);
        }
    }
}
