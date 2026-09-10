
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
    public class TheEndSkillNotes: ModProjectile
    {

        Player player => Main.player[Projectile.owner];

        public ref float tipe => ref Projectile.ai[0];

        public ref float posx => ref Projectile.ai[1];

        public ref float timer => ref Projectile.ai[2];
        public override string Texture => "asuw/Content/Textures/FuneralButterflyBlack";

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

            Projectile.spriteDirection = 1;
            if(Projectile.timeLeft > 20)
            { Projectile.alpha -= 30; }
            else {
                timer++;
                Projectile.alpha = 0 + (int)(30 * timer);
            }
         
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = ModContent.Request<Texture2D>($"asuw/Content/Textures/Extra/TheEndSkillNote{tipe}", AssetRequestMode.AsyncLoad).Value; 
            Vector2 origin = texture.Size() / 2f;

            Color color = Color.Lerp(Color.MediumPurple,Color.Violet,Main.rand.NextFloat(0.4f,0.8f));

            Main.EntitySpriteDraw(texture,
                  Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                  null, Projectile.GetAlpha(color), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }



    }
           
           
}

