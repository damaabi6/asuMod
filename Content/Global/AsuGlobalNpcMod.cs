using asuw.Content.Buffs;
using asuw.Content.Global;
using asuw.Content.Items.Accesories;
using asuw.Content.Items.Weapons;
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
            //if(projectile.type == ModContent.ProjectileType<coronachtArrow>())
            //{
            //    modifiers.SourceDamage *= 1 + (concentratedVolleyed / 100);
            //}
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
