using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Resources;

namespace Singularity.Platforms
{
    [DataContract]
    class Factory: PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;

        public Factory(Vector2 position,
            Texture2D spritesheet,
            Texture2D basesprite,
            SpriteFont libSans12,
            ref Director director,
            bool friendly = true)
            : base(position,
                spritesheet,
                basesprite,
                libSans12,
                ref director,
                EStructureType.Factory,
                -50,
                friendly: friendly)
        {
            //mIPlatformActions[0] = new ProduceFactoryResource(this);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            mCost = GetResourceCosts(EStructureType.Factory);
            mType = EStructureType.Factory;
            mSpritename = "Dome";
            SetPlatfromParameters();

            mIPlatformActions.Add(new RefineResourceAction(this, ref mDirector, new Dictionary<EResourceType, int> { { EResourceType.Metal, 1 } }, EResourceType.Steel));
            mIPlatformActions.Add(new RefineResourceAction(this, ref mDirector, new Dictionary<EResourceType, int> { { EResourceType.Metal, 1 } }, EResourceType.Copper));
            mIPlatformActions.Add(new RefineResourceAction(this, ref mDirector, new Dictionary<EResourceType, int> { { EResourceType.Sand, 1 } }, EResourceType.Silicon));
            mIPlatformActions.Add(new RefineResourceAction(this, ref mDirector, new Dictionary<EResourceType, int> { { EResourceType.Oil, 1 } }, EResourceType.Fuel));
            mIPlatformActions.Add(new RefineResourceAction(this, ref mDirector, new Dictionary<EResourceType, int> { { EResourceType.Oil, 1 } }, EResourceType.Plastic));
            mIPlatformActions.Add(new RefineResourceAction(this, ref mDirector, new Dictionary<EResourceType, int> { { EResourceType.Water, 1 }, { EResourceType.Sand, 1 } }, EResourceType.Concrete));
            mIPlatformActions.Add(new RefineResourceAction(this, ref mDirector, new Dictionary<EResourceType, int> { { EResourceType.Plastic, 1 }, {EResourceType.Silicon, 1}, { EResourceType.Copper, 1 } }, EResourceType.Chip));
        }

        public override void Produce()
        {
            mIPlatformActions.ForEach(a => a.Execute());
        }

        public void UiToggleAll()
        {
            mIPlatformActions.ForEach(a => a.UiToggleState());
        }
    }
}
