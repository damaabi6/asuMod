
using asuw.Content.Projectiles.Healing;
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
    public class Mikafist : ModProjectile
    {




        public override void SetDefaults()
        {
            Projectile.width = 35; // The width of projectile hitbox
            Projectile.height = 35; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 2; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 24; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
           // Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }




        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (0.815f / 2), (0.749f / 2), (1f / 2)); // R G B values from 0 to 1f. This is the red from the Crimson Heart pet
            float vel =  Main.rand.NextFloat(0.9f, 0.98f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // projectile sprite faces up
            Projectile.spriteDirection = Projectile.direction;
            Projectile.velocity *= vel;

            Projectile.alpha -= 50;

        }

        int hmm = Main.rand.Next(15);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            int width = Main.rand.Next(8);
            int height = Main.rand.Next(8);
            int dust3 = Dust.NewDust(Projectile.Center, width, height, 177);
            Main.dust[dust3].noGravity = true;
            Main.dust[dust3].velocity = Projectile.velocity * -2;
            Main.dust[dust3].scale = Main.rand.Next(1, 3);

            Main.dust[dust3].velocity = Main.dust[dust3].velocity.RotatedByRandom(MathHelper.ToRadians(30));
            Main.dust[dust3].velocity *= 0.2f;



            Vector2 launchVelocity = new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
            if (hmm == 0)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), target.Center, launchVelocity, ModContent.ProjectileType<HealingOrbMika>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }



          

        }
    }

}
           
           


