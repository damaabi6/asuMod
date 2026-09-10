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
    public class Fear : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 5;
        }



        public override void SetDefaults()
        {
            Projectile.width = 63; // The width of projectile hitbox
            Projectile.height = 63; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
             // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 30; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            
            
            

        }
        int width = Main.rand.Next(8);
        int height = Main.rand.Next(8);

        
        public override void AI()
        {
            
            Lighting.AddLight(Projectile.Center, 0.87f, 0.27f, 0.33f);

            Player player = Main.player[Projectile.owner];

            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }


            for (int i = 0; i < 30; i++)
            {
                int dust3 = Dust.NewDust(Projectile.Center, width, height, DustID.Shadowflame);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].velocity = new Vector2(90f);
                Main.dust[dust3].scale = Main.rand.Next(1, 2);
                Main.dust[dust3].fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                Main.dust[dust3].velocity = Main.dust[dust3].velocity.RotatedByRandom(MathHelper.ToRadians(360));
                Main.dust[dust3].velocity *= 0.6f;
            }

            

        }


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale * 4, spriteEffects, 0);

            return false;

        }
        public override void OnSpawn(IEntitySource source)
        {
            Vector2 launchVelocity = new Vector2(0, 0);
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVelocity, ModContent.ProjectileType<Projectiles.Boo>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
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

