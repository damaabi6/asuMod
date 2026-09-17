using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Items.Weapons.Ranged;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public class BlackBladeCD : CoolDown
    {
        public BlackBladeCD()
        {
            id = CoolDownID.BlackBladeCD;
            texture = "asuw/Content/Cooldown/WeaponCooldowns/WeaponCooldown";
            tick = 0;
            duration = 0;
            color = Color.Black;
        }
        public override void Update()
        {
            if (tick > 0)
                tick--;
        }
        public override bool DrawCondition() => Main.LocalPlayer.IsHoldingItem(ModContent.ItemType<BlackBlade>()) || Main.LocalPlayer.IsHoldingItem(ModContent.ItemType<MalikethBlackBlade>());

    }

}
