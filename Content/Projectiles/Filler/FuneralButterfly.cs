
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles
{
    public class FuneralButterfly: ModProjectile
    {

        Player player => Main.player[Projectile.owner];

        public ref float tipe => ref Projectile.ai[0];

        public ref float ding => ref Projectile.ai[1];
        public override string Texture => "asuw/Content/Textures/FuneralButterflyBlack";

        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 2;

        }

        public override void SetDefaults()
        {
            Projectile.width = 26; // The width of projectile hitbox
            Projectile.height = 26; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 60; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
           
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.scale = Main.rand.NextFloat(0.8f, 1.2f);
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;

            Projectile.frameCounter++;

            if (Projectile.frameCounter % 5 == 0)
            {
                if(Projectile.frame == 1)
                { Projectile.frame = 0; }
                else { Projectile.frame = 1; }

            }

            if (ding == 1)
            {
                Projectile.velocity *= 0.9f;
                Projectile.alpha = 0 + (Projectile.frameCounter * 3);
            }
            else
            {

                if (Projectile.timeLeft > 30)
                { Projectile.alpha -= 10; }
                else
                { Projectile.alpha += 10; }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            SpriteEffects effects;
            Texture2D texture;



            if (Projectile.spriteDirection > 0)
            {
                effects = SpriteEffects.None;
            }
            else
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            if (tipe == 0)
            { texture = TextureAssets.Projectile[Type].Value; }
            else { texture = ModContent.Request<Texture2D>("asuw/Content/Textures/FuneralButterflyWhite", AssetRequestMode.ImmediateLoad).Value; }

            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;


            Main.EntitySpriteDraw(texture,
                  Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                  sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);

            return false;
        }



    }
           
           
}

