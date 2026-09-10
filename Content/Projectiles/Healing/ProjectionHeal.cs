using asuw.Content.Global;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles.Healing
{
    public class ProjectionHeal : ModProjectile
    {
        
        Player player => Main.player[Projectile.owner];
        public override string Texture => "asuw/Assets/Blank";
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            
            Projectile.HealingProjectile(player.statLifeMax2 / 20, Projectile.owner, 6f, 15f);

        }
    }
}