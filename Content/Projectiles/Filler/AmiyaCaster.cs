
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles.Filler
{
    public class AmiyaCaster : ModProjectile
    {

        public ref float DelayTimer => ref Projectile.ai[1];

        public override string Texture => "asuw/Content/Projectiles/CasterBall";
        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 16; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 30; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.ArmorPenetration = 20;

        }


        public override void AI()
        {
             Player player = Main.player[Projectile.owner];

            //float down = player.Bottom.Y;
            //float up = (player.Top.Y - 2);
            //if (player.direction > 0)
            //{
            //    float front = (player.Right.X + 2);

            //    if (Projectile.Center.X >= front)
            //        Projectile.Center = new Vector2(front, Projectile.Center.Y) + player.velocity;



            //}
            //else if (player.direction < 0)
            //{
            //    float front = (player.Left.X -2);
            //    if (Projectile.Center.X <= front)
            //        Projectile.Center = new Vector2(front, Projectile.Center.Y) + player.velocity;


            //}

            //if (Projectile.Center.Y >= down)
            //{

            //        Projectile.Center = new Vector2(Projectile.Center.X, down) + player.velocity;

            //}
            //else if (Projectile.Center.Y <= up)
            //{

            //        Projectile.Center = new Vector2(Projectile.Center.X, up) + player.velocity;

            //}
            //Projectile.netUpdate = true;

           
        

                Projectile.velocity =  player.velocity;
            
            
            
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {

                //Particle spark = new CustomSpark(Projectile.position, vectorToCursor.SafeNormalize(Vector2.One) * 4, "CalamityMod/Particles/BloomRing", false, 12, 0.7f, Color.DarkRed, new Vector2(0.5f, 1.5f), true, false, extraRotation: -MathHelper.PiOver2, noShrink: true);
                //GeneralParticleHandler.SpawnParticle(spark);

                //Particle spark2 = new CustomSpark(Projectile.position, vectorToCursor.SafeNormalize(Vector2.One) * 8 * 2, "CalamityMod/Particles/BloomRing", false, 12, 0.4f, Color.DarkRed, new Vector2(0.5f, 1.5f), true, false, extraRotation: -MathHelper.PiOver2, noShrink: true);
                //GeneralParticleHandler.SpawnParticle(spark2);
            }

            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = new Vector2(Main.rand.NextFloat(-10f, 10f));
                
                var dust2 = Dust.NewDustDirect(Projectile.position, 10, 10, 54);
                dust2.noGravity = true;
                dust2.velocity = speed; 
                dust2.scale = 1.4f;
                dust2.velocity = dust2.velocity.RotatedByRandom(MathHelper.ToRadians(360));

                var dust = Dust.NewDustDirect(Projectile.position, 10, 10, DustID.RedTorch);
                dust.noGravity = true;
                dust.velocity = speed;
                dust.scale = 1.4f;
                dust.velocity = dust.velocity.RotatedByRandom(MathHelper.ToRadians(360));

                
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            
        }
        
    }
           
           
}

