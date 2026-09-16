using asuw.Content.Buffs;
using asuw.Content.Global;
using asuw.Content.Items.Accesories;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Ranged;
using asuw.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


namespace asuw.Content
{
    public partial class AsuGlobalNPC : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.asuw().applySinking)
            {
                if (SinkingStack < 10)
                {
                    SinkingStack++;
                    SinkingTimer += Math.Min(150, SinkingTimer.AbsDelta(300)); // add half duration (2.5s~) every apply
                }
                else
                    SinkingTimer += Math.Min(120, SinkingTimer.AbsDelta(300)); // add 2 second for each overstack
            }

            if (SinkingStack > 0 && (projectile.asuw().benefitsFromSinking || projectile.asuw().applySinking))
                modifiers.FinalDamage += (0.02f * SinkingStack);
                
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            Player owner = projectile.GetOwner();

           
        }


        public override void OnKill(NPC npc)
        {
            foreach(var player in Main.ActivePlayers)
            {
                float dis = Vector2.Distance(npc.Center, player.MountedCenter);
                if (!player.dead && player.HeldItem.ModItem != null && player.HeldItem.ModItem is Tsurugi && dis < 2000) player.HealPlayer(Main.rand.Next(10,16));
            }
        }
    }
}
