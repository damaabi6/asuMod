using asuw.Content.Buffs;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Projectiles.Filler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles
{
    public class ClawEx : ModProjectile
    {
        
        int timer = 0;
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 22;

            // Prevents jitter when stepping up and down blocks and half blocks
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public bool Slashing = false;
        public bool Slash1 => Projectile.frame == 4;

        public bool Slash2 => Projectile.frame == 9;

        public bool Drill => Projectile.frame == 13 || Projectile.frame == 14 || Projectile.frame == 15 || Projectile.frame == 16 || Projectile.frame == 17 || Projectile.frame == 18;


        public ref int hitCooldown => ref Main.player[Projectile.owner].asuw().HitCooldown;

        public int time = 0;
        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 200; 
            Projectile.height = 200; 
            
            Projectile.friendly = true; 
            Projectile.hostile = false; 
            Projectile.DamageType = DamageClass.MeleeNoSpeed; 
            Projectile.penetrate = -1;  
            Projectile.timeLeft = 28; 
            Projectile.alpha = 0;  
            Projectile.light = 0.5f; 
            Projectile.ownerHitCheck = false;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true; 
            Projectile.tileCollide = false; 
            Projectile.extraUpdates = 0;                                       
            Projectile.localNPCHitCooldown = 5;
            Projectile.usesLocalNPCImmunity = true;
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
            int ran = Main.rand.Next(4);

            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 0)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Type];

            }

            if (Slash1)
            {
                // SoundEngine.PlaySound(Murasama.Swing with { Pitch = -0.1f }, Projectile.Center);
                if (Projectile.frameCounter % 5 == 0)
                {  SoundEngine.PlaySound(Claw_W.Swing1, Projectile.Center); 
                     SoundEngine.PlaySound(Claw_W.Swing2, Projectile.Center); 





                }
                   
                if (hitCooldown == 0)
                    Slashing = true;
                timer++;
                if (timer == 3)
                {
                  
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(0, -50), Projectile.velocity *1.38f, ModContent.ProjectileType<FillerClawEx>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    

                    
                }
                Projectile.numHits = 0;
            }
            else if (Slash2)
            {
                // SoundEngine.PlaySound(Murasama.Swing with { Pitch = -0.1f }, Projectile.Center);
                if (Projectile.frameCounter % 5 == 0)
                {
                    
                         SoundEngine.PlaySound(Claw_W.Swing1, Projectile.Center); 
                         SoundEngine.PlaySound(Claw_W.Swing2, Projectile.Center); 





                    
                }

                if (hitCooldown == 0)
                    Slashing = true;
                
                if (Projectile.frameCounter % 5 == 0)
                {

                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(0, -10), Projectile.velocity * 1.38f, ModContent.ProjectileType<FillerClawEx>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                  


                }
                Projectile.numHits = 0;
            }
            else if (Drill)
            {
                // SoundEngine.PlaySound(Murasama.Swing with { Pitch = -0.1f }, Projectile.Center);
                if (Projectile.frameCounter % 5 == 0)
                { SoundEngine.PlaySound(Claw_W.Drill, Projectile.Center); }

                if (hitCooldown == 0)
                    Slashing = true;



                if (Projectile.frameCounter % 5 == 0)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(0, -10), Projectile.velocity * 1.38f, ModContent.ProjectileType<FillerClawEx>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(0, -10), Projectile.velocity * 1.2f, ModContent.ProjectileType<FillerClawEx>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }



                Projectile.numHits = 0;
            }
            else
            {
                timer = 0;
                Slashing = false;
            }

            Vector2 origin = Projectile.Center + Projectile.velocity;
            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                if (!player.CantUseHoldout())
                    HandleChannelMovement(player, playerRotatedPoint);
                else
                {
                    hitCooldown = 14;
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
            Projectile.timeLeft = 4;

            // Player item-based field manipulation.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

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
            target.AddBuff(ModContent.BuffType<BlackBlood>(), 300);
            int ran = Main.rand.Next(4);
            if (ran == 0) { SoundEngine.PlaySound(Claw_W.Hit1, Projectile.Center); }
            else if (ran == 1) { SoundEngine.PlaySound(Claw_W.Hit2, Projectile.Center); }
            else if (ran == 2) { SoundEngine.PlaySound(Claw_W.Hit3, Projectile.Center); }
            else if (ran == 3) { SoundEngine.PlaySound(Claw_W.Hit4, Projectile.Center); }
        }
        
        public override bool? CanDamage() => Slashing == false ? false : null;
    }

    }

    

