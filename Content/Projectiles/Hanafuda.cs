using asuw.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace asuw.Content.Projectiles
{
    public class Hanafuda : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 10;

            // Prevents jitter when stepping up and down blocks and half blocks
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        int timer = 0;
        

        public bool Slashing = false;
        public bool Slash1 => Projectile.frame == 0;

        public bool Stop => Projectile.frame == 3;
        public bool Slash2 => Projectile.frame == 5;
        public ref int hitCooldown => ref Main.player[Projectile.owner].asuw().HitCooldown;

        public int time = 0;
        public ref float DelayTimer => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 640; // The width of projectile hitbox
            Projectile.height = 640; // The height of projectile hitbox
            
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Default; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 40; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ownerHitCheck = false;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
                                         //Projectile.hide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 ;



            Projectile.hide = true;
            

        }


     
            

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + (Projectile.velocity * 0f) + new Vector2(0, 0).RotatedBy(Projectile.rotation), frame, Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];


            Projectile.frameCounter++;
            if (Projectile.frameCounter % 4 == 0)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }


            if (Slash1)
            {
               
                // SoundEngine.PlaySound(Murasama.Swing with { Pitch = -0.1f }, Projectile.Center);
                if (Projectile.frameCounter % 4 == 0 || Projectile.frameCounter == 1 )
                { 
                    int ran = Main.rand.Next(2);
                if (ran == 0)
                {
                    SoundEngine.PlaySound(Sakura.Swing1 , Projectile.Center);

                } else if (ran == 1)
                {
                    SoundEngine.PlaySound(Sakura.Swing2 , Projectile.Center);
                }
                }

          
                
                if (hitCooldown == 0)
                    Slashing = true;
                Projectile.numHits = 0;
            }

            else if (Slash2)
            {
                
                if (Projectile.frameCounter % 4 == 0)
                {
                    int ran = Main.rand.Next(2);
                    if (ran == 0)
                    {
                        SoundEngine.PlaySound(Sakura.Swing1, Projectile.Center);

                    }
                    else if (ran == 1)
                    {
                        SoundEngine.PlaySound(Sakura.Swing2, Projectile.Center);
                    }
                }

                if (hitCooldown == 0)
                    Slashing = true;
                Projectile.numHits = 0;
            }
            else
                Slashing = false;

          

            Vector2 origin = Projectile.Center + Projectile.velocity;
            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                if (!player.CantUseHoldout())
                    HandleChannelMovement(player, playerRotatedPoint);
                else
                {
                    hitCooldown = 8;
                    Projectile.Kill();
                }
            }

            if (Slashing)
            {
                float velocityAngle = Projectile.velocity.ToRotation();
                Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
            }
            float velocityAngle2 = Projectile.velocity.ToRotation();
            Projectile.direction = (Math.Cos(velocityAngle2) > 0).ToDirectionInt();

            // Positioning close to the end of the player's arm.

            Projectile.Center = playerRotatedPoint + velocityAngle2.ToRotationVector2();

            // Sprite and player directioning.
            player.ChangeDir(Projectile.direction);

            // Prevents the projectile from dying
            Projectile.timeLeft = 2;

            // Player item-based field manipulation.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            Lighting.AddLight(origin, Color.Purple.ToVector3() * (Slashing == true ? 3.5f : 2f));
        }
            
        

        public void HandleChannelMovement(Player player, Vector2 playerRotatedPoint)
        {
            float speed = 1f;
            if (player.HeldItem.shoot == Projectile.type)
            {
                speed = player.HeldItem.shootSpeed * Projectile.scale;
            }
            // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;

            // Sync if a velocity component changes.
            if (Slashing)
            {
                if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = newVelocity;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 launchVelocity = new Vector2(0, 0);
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), target.Center, launchVelocity, ModContent.ProjectileType<Projectiles.Bloom>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);


            int ran = Main.rand.Next(2);
            if (ran == 0)
            {
                SoundEngine.PlaySound(Sakura.Hit1 , Projectile.Center);

            }
            else if (ran == 1)
            {
                SoundEngine.PlaySound(Sakura.Hit2 , Projectile.Center);
            }





        }
        public override bool? CanDamage() => Slashing == false ? false : null;
       
    }

}

    

