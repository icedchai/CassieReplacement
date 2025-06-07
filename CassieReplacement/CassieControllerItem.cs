#if EXILED
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassieReplacement
{
    [CustomItem(ItemType.Radio)]
    public class CassieControllerItem : CustomItem
    {
        public static CassieControllerItem Singleton { get; private set; }

        public override void Init()
        {
            base.Init();
            Singleton = this;
        }

        public override uint Id { get; set; } = 1100;

        public override string Name { get; set; } = "CASSIE Remote Control";

        public override string Description { get; set; } = "Use .cassie in client console to use";

        public override float Weight { get; set; }

        public override SpawnProperties SpawnProperties { get; set; }

        public override bool ShouldMessageOnGban => true;
    }
}
#endif