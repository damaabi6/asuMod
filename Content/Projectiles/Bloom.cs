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
    public class Bloom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 4;
        }



        public override void SetDefaults()
        {
            Projectile.width = 128; // The width of projectile hitbox
            Projectile.height = 128; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
             // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 100; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 12; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = 4;
            Projectile.usesLocalNPCImmunity = true;


        }
        int width = Main.rand.Next(8);
        int height = Main.rand.Next(8);

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.87f, 0.27f, 0.33f);

            Player player = Main.player[Projectile.owner];


            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }

            
                int dust3 = Dust.NewDust(Projectile.Center, width, height, DustID.Shadowflame);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].velocity = new Vector2((Main.rand.NextFloat(20f))); 
                Main.dust[dust3].scale = Main.rand.Next(1, 2);
                Main.dust[dust3].fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                Main.dust[dust3].velocity = Main.dust[dust3].velocity.RotatedByRandom(MathHelper.ToRadians(360));
                Main.dust[dust3].velocity *= 0.4f;
            
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (ModLoader.TryGetMod("CalamityMod", out Mod CalamityMod) && CalamityMod.TryFind<ModBuff>("DemonicFlames", out ModBuff DemonicFlames))
            {
                target.AddBuff(DemonicFlames.Type, 600);
            }
        }
        
    }
           
           
}

