using asuw.Content.Dusts;
using asuw.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles
{
    public class AmiyaPew : ModProjectile
    {

        public int Time = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36; // The width of projectile hitbox
                                                         // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 1200; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 30; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 24; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }


        public override void AI()
        {
            Time++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(-3, 3), ModContent.DustType<SharpSparkDust>());
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.9f, 1.3f);
            dust.color = Amiya.mainColor;
            dust.fadeIn = -0.4f;
            if (Time < 120)
            {
                
                    Vector2 trailPos = Projectile.Center + Main.rand.NextVector2Circular(10, 10);
                    float trailScale = Main.rand.NextFloat(0.6f, 0.75f);
                    Color trailColor = Main.rand.NextBool(3) ? Color.PaleVioletRed : Amiya.mainColor;
                
                
                
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Amiya.mainColor with { A = 0 };
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

          
        }
        
    }
           
           
}

