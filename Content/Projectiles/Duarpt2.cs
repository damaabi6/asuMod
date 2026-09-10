
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
    public class Duarpt2 : ModProjectile
    {
   
      
        int ran = Main.rand.Next(2);
        public ref float DelayTimer => ref Projectile.ai[1];
        
        public override void SetDefaults()
        {
            Projectile.width = 35; // The width of projectile hitbox
            Projectile.height = 35; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 90; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 60; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            

        }

        
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.87f, 0.27f, 0.33f);
            if (ran == 0)
            {
                Projectile.rotation += 0.4f * (float)Projectile.direction;
            }
            else
            {
                Projectile.rotation -= 0.4f * (float)Projectile.direction;
            }


            if (Projectile.timeLeft > 50)
            {
                Projectile.velocity *= 0.9f;
            }

            // A short delay to homing behavior after being fired
            if (DelayTimer < 30)
            {
                DelayTimer += 1;
                return;
            }
            AsuUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 1000f, 30f, 20f);
        }
        Vector2 vel = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
         {

           
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, new Vector2(20, 20).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1));
                dust2.scale = Main.rand.NextFloat(0.55f, 0.85f);
                dust2.noGravity = true;
                dust2.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;
            


        }

        public override bool? CanDamage() => Projectile.timeLeft >= 50? false : null;
    }
     
}

