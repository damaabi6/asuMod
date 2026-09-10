using asuw.Content.Items.Weapons;
using asuw.Content.Projectiles.Filler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles
{
    public class AmiyaBall : ModProjectile
    {

        public int Time = 0;

        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32; // The width of projectile hitbox
                                                      // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 60; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.

            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(Amiya.Cast, Projectile.Center);
        }
        public override void AI()
        {

            Projectile.velocity *= 0.95f;
            Projectile.frameCounter++;
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }

         
            if (Projectile.timeLeft < 10)
            { Projectile.scale *= 0.9f; }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 vectorToCursor = Main.MouseWorld - Projectile.Center;
                
                for (int i = 0; i < 3; i++)
                {
                    int ran = Main.rand.Next(20,40);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vectorToCursor.RotatedByRandom(0.5f).SafeNormalize(Vector2.One) * ran, ModContent.ProjectileType<AmiyaBallBullet>(), Projectile.damage , Projectile.knockBack, Projectile.owner, Projectile.ArmorPenetration);
                }

            

               
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * -1;
            return false;
        }
    }
           
           
}

