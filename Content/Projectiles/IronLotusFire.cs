using asuw.Content.Buffs;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Projectiles.Filler;
using asuw.Content.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Mono.Cecil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
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
    public class IronLotusFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 24;

            // Prevents jitter when stepping up and down blocks and half blocks
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        Player player => Main.player[Projectile.owner];

        public bool Slashing = false;

        public bool FrameUpdate = false;
        public bool Slash1 =>  Projectile.frame == 2 || Projectile.frame == 3;

        public bool Slash2 => Projectile.frame == 10 || Projectile.frame == 11;

        public bool Slash3 => Projectile.frame == 17 || Projectile.frame == 18;

        public ref float Timer => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 640; // The width of projectile hitbox
            Projectile.height = 640; // The height of projectile hitbox
            
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.MeleeNoSpeed; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 144; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ownerHitCheck = false;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time 
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1 ;
            Projectile.ArmorPenetration = 99;

            Projectile.scale = 1.5f;

        }

        public override void OnSpawn(IEntitySource source)
        {
            float velocityAngle = Projectile.velocity.ToRotation();
            Projectile.rotation = velocityAngle + (Projectile.ai[0] == -1).ToInt() * MathHelper.Pi;
        }
            

       

        public override void AI()
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is IronLotus il)
            {

                Timer++;

                Projectile.frameCounter++;
                if (Projectile.frameCounter % 6 == 0)
                {
                    Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                    FrameUpdate = true;

                }
                else { FrameUpdate = false; }




                if (Slash1 || Slash2 || Slash3)
                {
                    if (Projectile.frameCounter % 6 == 0)
                    { il.IronLotusFlameCharge += 5; }
                    Slashing = true;
       
                    if (Projectile.frame == 2 || Projectile.frame == 10 || Projectile.frame == 17 )
                    {
                        if (Projectile.frameCounter % 6 == 0)
                        { Projectile.numHits = 0; }
                    }
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
                        Projectile.Kill();
                    }
                }

                if (FrameUpdate || Timer <= 1)
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
                player.itemTime = 50;
                player.itemAnimation = 50;

               

                Lighting.AddLight(origin, Color.OrangeRed.ToVector3() * (Slashing == true ? 3.5f : 2f));

               
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            //Texture2D smear = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/IronLotusSmear", AssetRequestMode.ImmediateLoad).Value;
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            //Main.EntitySpriteDraw(smear, Projectile.Center - Main.screenPosition + (Projectile.velocity * 0f) + new Vector2(0, 0).RotatedBy(Projectile.rotation), frame, Color.Lerp(Color.Goldenrod, Color.DarkOrange, 0.5f) * 0.3f, Projectile.rotation, origin, Projectile.scale + 0.5f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + (Projectile.velocity * 0f) + new Vector2(0, 0).RotatedBy(Projectile.rotation), frame, Color.White, Projectile.rotation, origin, Projectile.scale + 0.5f, spriteEffects, 0);

           
            return false;
        }

       
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is IronLotus il)
            {
                if (Projectile.numHits <= 2)
                { il.IronLotusFlameCharge++; }

                if (Projectile.numHits <= 11)
                { 
                il.IronFlameDMG += 3;
                il.IronFlameDMGLevel += 0.2f;
                }
            }
            target.AddBuff(ModContent.BuffType<IronFire>(), 1000);
           

            if (target.HasBuff<IronFire>())
            {
                if (Projectile.numHits == 0)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), player.Center, Vector2.Zero, ModContent.ProjectileType<IronLotusHeal>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public void HandleChannelMovement(Player player, Vector2 playerRotatedPoint)
        {
            float speed = 1f;
            if (player.HeldItem.shoot == Projectile.type)
            {
                speed = (player.HeldItem.shootSpeed) * Projectile.scale;
            }
            // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;

            // Sync if a velocity component changes.
            if (FrameUpdate || Timer <=1 )
            {
                if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = newVelocity;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 vectorToCursor = Main.MouseWorld - Projectile.Center;
            Vector2 start = Projectile.Center + (vectorToCursor.SafeNormalize(Vector2.One) * -570);
            Vector2 end = start + (vectorToCursor.SafeNormalize(Vector2.One) * 1180);
            float collisionPoint = 0f;
            if(Slash1)
            { return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 430f * Projectile.scale, ref collisionPoint); }
            else if(Slash2)
            { return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 300f * Projectile.scale, ref collisionPoint); }
            else
            { return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 740f * Projectile.scale, ref collisionPoint); }
          
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1f;
        }
        public override bool? CanDamage() => Slashing == false ? false : null;
       
    }

}

    

