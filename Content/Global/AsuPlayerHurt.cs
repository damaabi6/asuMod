using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;



namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {
        public double dmgModifier = 0;
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            double damageMult = 1D;
            if (Player.HeldItem.ModItem != null)
            {   if (Player.HeldItem.ModItem is Yujin yu)
                { damageMult += (yu.ShiDMGInc * 0.01); }
                if (Player.HeldItem.ModItem is IronLotus il && il.IronLotusFlame)
                { damageMult -= 0.5; }
                if (Player.HeldItem.ModItem is Hotshot hot && hot.reloadParry)
                { 
                    if(hot.reloadParry)
                    modifiers.Cancel();
                    if (hot.perfectBodyguard > 0)
                    damageMult += ((float)hot.perfectBodyguard * 0.01f);
                }
                if (Player.HeldItem.ModItem is Tsurugi tsu && Player.controlUseItem)
                { damageMult -= 0.3; }
            }
            if (Player.HeldItem.type == ModContent.ItemType<ProjectionSorcery>())
            { damageMult += 0.15; }
            if (Player.HeldItem.type == ModContent.ItemType<TheEnd>())
            { damageMult -= 0.3; }
            if (Player.HeldItem.type == ModContent.ItemType<Horus>() && HorusBarrierUp)
            { damageMult -= 0.4; }

            if (SOLChargeUpActivated)
                damageMult -= 0.7;

            damageMult += dmgModifier;
            if(damageMult < double.Epsilon) damageMult = double.Epsilon;
            modifiers.SourceDamage *= (float)damageMult;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.HeldItem.ModItem != null)
            {
                if (Player.HeldItem.ModItem is Yujin yu)
                {
                    yu.ShiDMGInc -= 4f;
                    yu.heal -= (int)((float)yu.heal * 0.3f);
                }
                if (Player.HeldItem.ModItem is Hotshot hot && hot.perfectBodyguard > 0)
                { hot.perfectBodyguard--; }
                if (Player.HeldItem.ModItem is SawCleaver saw)
                {

                    saw.rallyHeal += (int)((float)info.Damage * 0.85f);
                    saw.rallytimer = 300;
                    
                }
            }

            if(SOLChargeUpActivated)
                SOLChargeUpActivated = false;
        }
    }
}
