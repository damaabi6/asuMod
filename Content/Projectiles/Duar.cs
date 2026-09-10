
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace asuw.Content.Projectiles
{
    public class Duar : ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        public ref float timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 24; 
            Projectile.height = 24; 
            Projectile.friendly = true; 
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged; 
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.alpha = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = 8;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

       
           

            timer++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center + (Projectile.rotation.ToRotationVector2() * -64);
            Vector2 end = start + (Projectile.rotation.ToRotationVector2() * 64);
            float collisionPoint = 0f;
            float collisionWidth = 24f;
           

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth * Projectile.scale, ref collisionPoint);
        }


    }
}
