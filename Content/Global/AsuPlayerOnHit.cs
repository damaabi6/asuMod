using asuw.Content.Buffs;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.whoAmI != Main.myPlayer)
                return;


            var source = Player.GetSource_OnHit(target);
            AsuGlobalNPC agn = target.asuw();
            AsuGlobalProj agp = proj.asuw();

            if (proj.type == ModContent.ProjectileType<HomaStaff>() && Player.HasBuff(ModContent.BuffType<Pyro>()))
                agn.pyroApplicator = Player.whoAmI;


        }
       

        
    }
}
