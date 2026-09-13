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
                    SinkingCounter += Math.Min(150, SinkingCounter.AbsDelta(300)); // add half duration (2.5s~) every apply
                }
                else
                    SinkingCounter += Math.Min(120, SinkingCounter.AbsDelta(300)); // add 2 second for each overstack
            }

            if (npc.asuw().SinkingStack > 0 && projectile.asuw().benefitsFromSinking)
                modifiers.FinalDamage += (0.02f * npc.asuw().SinkingStack);
                
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            Player owner = projectile.GetOwner();

            if(owner.HeldItem.ModItem != null && owner.HeldItem.ModItem is SolemnLament sl)
            {
                if (projectile.asuw().benefitsFromSinking && !projectile.asuw().applySinking && projectile.numHits == 0)
                {
                    if (Main.rand.NextFloat() < 0.073f / (sl.AltAmmo + 1 + (SinkingStack / 2)))
                        sl.AltAmmo++;
                }

            }
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
