using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
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

namespace asuw.Content.Projectiles.Filler
{
    public class IronLotusHeavySlash: ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        public ref float timer => ref Projectile.ai[0];

        public ref float FadeIn => ref Projectile.ai[1];

        Player player => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 800; // The width of projectile hitbox
            Projectile.height = 800; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 20; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
           
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;

            
        }
        
        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            Projectile.velocity *= 0.8f;
            timer++;
            FadeIn = fadeinop(timer);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            
            SpriteEffects effects;

            Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("asuw/Assets/Blank");

            if (Projectile.spriteDirection > 0)
            {
                effects = SpriteEffects.None;
            }
            else
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Goldenrod, Color.DarkOrange, 0.25f) with { A = 0 } * FadeIn * 0.35f, Projectile.rotation, swoosh.Size() * 0.5f, Projectile.scale * 1.36f, effects);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is IronLotus il)
            {
                if (Projectile.numHits == 0)
                {
                    il.IronLotusCharge += 10;
                }
            }


        }
        float fadeinop(float tick)
        {
            if (tick < 10)
                return MathHelper.SmoothStep(0f, 0.7f, tick / 10f);
            else return MathHelper.SmoothStep(0.7f, 0f, (tick - 10) / 10f); 
        }

    }
           
           
}

