using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace asuw.Content.Projectiles
{
    public class IronLotusFireMass : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 6;

            // Prevents jitter when stepping up and down blocks and half blocks
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        

        public bool Slashing = false;

      
       

        public ref float Timer => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 662; // The width of projectile hitbox
            Projectile.height = 662; // The height of projectile hitbox
            
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Default; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 32; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ownerHitCheck = false;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
                                         //Projectile.hide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 31 ;



            

        }

        
            

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + (Projectile.velocity * 0f) + new Vector2(0, 0).RotatedBy(Projectile.rotation), frame, Color.White, Projectile.rotation, origin, Projectile.scale + 1.2f, spriteEffects, 0);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Timer++;

            Projectile.frameCounter++;
            if (Projectile.frameCounter % 6 == 0)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
               
            }
          


           

            if (Projectile.frame == 4)
            {
                        
                    Slashing = true;
                Projectile.numHits = 0;
            }
            else
                Slashing = false;

            if(Timer >= 32)
            { Projectile.Kill(); }



            player.ChangeDir(Main.MouseWorld.X > player.MountedCenter.X ? 1 : -1);
            Projectile.Center = player.Center;
            Projectile.direction = player.direction;
            Projectile.spriteDirection = Projectile.direction;
            player.heldProj = Projectile.whoAmI;



        }
            
        

       
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 vectorToCursor = Main.MouseWorld - Projectile.Center;
            Vector2 start = Projectile.Center + (vectorToCursor.SafeNormalize(Vector2.One) * -780);
            Vector2 end = start + (vectorToCursor.SafeNormalize(Vector2.One) * 1450);
            float collisionPoint = 0f;
            
             return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 1500f * Projectile.scale, ref collisionPoint); 
           


        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is IronLotus il)
            {
                il.IronLotusCharge = 0;
            }
        }
        public override bool? CanDamage() => Slashing == false ? false : null;
       
    }

}

    

