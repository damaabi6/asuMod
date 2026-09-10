using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace asuw.Content.Projectiles.Filler
{
    public class AmiyaBack : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 28;
            // Prevents jitter when stepping up and down blocks and half blocks
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }


        public override void SetDefaults()
        {
            Projectile.width = 56; // The width of projectile hitbox
            Projectile.height = 56; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ArmorPenetration = 50;
                                       
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 84; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 10; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.direction = player.direction;
            Projectile.spriteDirection = Projectile.direction;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }

            if (player.direction > 0)
            {
                Projectile.Center = player.Center + (player.velocity * -1) + new Vector2(-40, -30);
            }
            else if (player.direction < 0)
            {
                Projectile.Center = player.Center + (player.velocity * -1) + new Vector2(40, -30);
            }
            Projectile.velocity = player.velocity;

            Vector2 eks = new Vector2(Main.rand.NextFloat(-18f, 18f), Main.rand.NextFloat(-2f, 18f));
            Vector2 vel =  new Vector2(0, -4f);




            
                
            
            Projectile.netUpdate = true;

        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];

            if (player.direction > 0) { Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(11,-4), Vector2.Zero, ModContent.ProjectileType<AmiyaCaster>(), Projectile.damage * 10, Projectile.knockBack, Projectile.owner, Projectile.ArmorPenetration); }

            if (player.direction < 0) { Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(-11, -4), Vector2.Zero, ModContent.ProjectileType<AmiyaCaster>(), Projectile.damage * 10, Projectile.knockBack, Projectile.owner, Projectile.ArmorPenetration); }


        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale * 0.25f, spriteEffects, 0);
            return false;
        }
    }
           
           
}

