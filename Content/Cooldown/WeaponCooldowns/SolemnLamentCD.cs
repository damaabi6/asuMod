using asuw.Content.Items.Weapons.Ranged;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Cooldown.WeaponCooldowns
{
    public class SolemnLamentCD : CoolDown
    {
        public SolemnLamentCD()
        {
            id = CoolDownID.SolemnLamentCD;
            tick = 0;
            duration = 0;
            color = Color.White;
            texture = "asuw/Content/Cooldown/WeaponCooldowns/SLCD";
        }
        public override void Update()
        {
            if (tick > 0)
                tick--;
        }
        public override bool DrawCondition() => Main.LocalPlayer.IsHoldingItem(ModContent.ItemType<SolemnLament>());

    }

}
