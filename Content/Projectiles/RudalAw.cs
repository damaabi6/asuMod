using asuw.Content.Buffs;
using asuw.Content.Global;
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
    public class RudalAw : ModProjectile
    {


        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 16; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
           // Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }




        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            int width = Main.rand.Next(8);
            int height = Main.rand.Next(8);
            int dust3 = Dust.NewDust(Projectile.Center, width, height, DustID.SolarFlare);
            Main.dust[dust3].noGravity = true;
            Main.dust[dust3].velocity = Projectile.velocity * -1;
            Main.dust[dust3].scale = Main.rand.Next(1, 2);
            Main.dust[dust3].velocity = Main.dust[dust3].velocity.RotatedByRandom(MathHelper.ToRadians(30));
            Main.dust[dust3].velocity *= 0.3f;

            


            Lighting.AddLight(Projectile.Center, (0.815f), (0.749f), (1f)); // R G B values from 0 to 1f. This is the red from the Crimson Heart pet

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // projectile sprite faces up
            Projectile.spriteDirection = Projectile.direction;
            Projectile.velocity = (player.Center - Projectile.Center).SafeNormalize(Vector2.One) * 60;

         

         

            



            

        }


       

      
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage *= 0f;
            if (Main.masterMode) modifiers.SourceDamage.Flat += 70f;
            else if (Main.expertMode) modifiers.SourceDamage.Flat += 40f;
            else modifiers.SourceDamage.Flat += 20f;

            target.AddBuff(ModContent.BuffType<Smoke>(), 80);
            

            //SoundEngine.PlaySound(new SoundStyle("asuw/Content/Sounds/ouch") { Volume = 0.7f, MaxInstances = 1 });

            int width = Main.rand.Next(8);
            int height = Main.rand.Next(8);
            int dust3 = Dust.NewDust(Projectile.Center, width, height, DustID.SolarFlare);
            Main.dust[dust3].noGravity = true;
            Main.dust[dust3].velocity = Projectile.velocity * -2;
            Main.dust[dust3].scale = Main.rand.Next(3, 9);

            Main.dust[dust3].velocity = Main.dust[dust3].velocity.RotatedByRandom(MathHelper.ToRadians(30));
            Main.dust[dust3].velocity *= 0.2f;

            Projectile.Kill();
        }


        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                int width = Main.rand.Next(30);
                int height = Main.rand.Next(30);
                int dust3 = Dust.NewDust(Projectile.Center, width, height, DustID.Smoke);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].velocity = Projectile.velocity * 1;
                Main.dust[dust3].scale = Main.rand.Next(1, 2);
                Main.dust[dust3].fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                Main.dust[dust3].velocity = Main.dust[dust3].velocity.RotatedByRandom(MathHelper.ToRadians(360)) * 3;
                Main.dust[dust3].velocity *= 0.4f;


                int dust2 = Dust.NewDust(Projectile.Center, width / 3, height / 3, DustID.SolarFlare);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity = Projectile.velocity * 1;
                Main.dust[dust2].scale = Main.rand.NextFloat(1f, 1.5f);
                Main.dust[dust2].fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                Main.dust[dust2].velocity = Main.dust[dust2].velocity.RotatedByRandom(MathHelper.ToRadians(360));
                Main.dust[dust2].velocity *= 0.4f;
                
            }
            SoundEngine.PlaySound(SoundID.Item14 , Projectile.Center);
           
        }
    }
           
           
}

