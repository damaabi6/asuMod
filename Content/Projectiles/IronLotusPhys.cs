using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Projectiles.Filler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;
using static tModPorter.ProgressUpdate;

namespace asuw.Content.Projectiles
{
    public class IronLotusPhys : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        public bool flip = false;
        public bool demeg = false;
        public int alep = 0;
        public float FadeIn = 0;

        public ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 452; // The width of projectile hitbox
            Projectile.height = 404; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 140; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.scale = 0.8f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((float)player.asuw().mouseRotationFromPlayer);
            writer.Write((int)player.direction);
            writer.Write((int)Projectile.spriteDirection);




        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            player.asuw().mouseRotationFromPlayer = reader.ReadSingle();
            player.direction = reader.ReadInt32();
            Projectile.spriteDirection = reader.ReadInt32();



        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(player.asuw().mouseWorld.X > player.MountedCenter.X ? 1 : -1);
            }
        }
        public override void AI()
        {
            
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is IronLotus il)
            {
                if (Timer >= 40 && Timer < 90)
            { flip = true; }
            else
            { flip = false; }
            
           
              
                Projectile.Center = player.Center;
                Projectile.direction = player.direction;
                Projectile.spriteDirection = Projectile.direction * (flip ? 1 : -1);
                player.heldProj = Projectile.whoAmI;

                Vector2 vectorToCursor = player.asuw().mouseRotationFromPlayer.ToRotationVector2();

            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(player.asuw().mouseWorld.X > player.MountedCenter.X ? 1 : -1);
            }
            if (Timer == 10 || Timer == 50 || Timer == 70 || Timer == 110)
                {
                    Projectile.ResetLocalNPCHitImmunity();
                    Projectile.numHits = 0;
                }

                Projectile.scale = WeaponScale(Timer);
                FadeIn = fadeinop(Timer);

                float angle = WeaponAngle(Timer);
                Projectile.rotation = vectorToCursor.ToRotation() + (MathHelper.ToRadians(angle) * player.direction);

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, vectorToCursor.ToRotation() - MathHelper.ToRadians(90f));

                Projectile.netUpdate = true;
                Timer++;

                if (Timer < 125)
                { Projectile.alpha -= 30; }
                else
                {
                    alep++;
                    Projectile.alpha = 0 + (alep * 18);
                }
          
                if (player.CantUseHoldout())
                {
                    if (Timer == 40 || Timer == 70 || Timer == 90)
                    {
                        Projectile.Kill();
                    }

                }
               

                Vector2 posrand = Projectile.Center + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length() * Main.rand.NextFloat(0.6f,1f)) * Projectile.scale);
                Vector2 vel = Projectile.rotation.ToRotationVector2().SafeNormalize(Vector2.Zero);
                int size = Main.rand.Next(5, 19);
                Dust idx = Dust.NewDustPerfect(posrand, DustID.GoldCoin, vel * Main.rand.NextFloat(5,12), 100, default, 1f);
                idx.noGravity = true;
                idx.velocity *= 0.9f;

               

                player.itemTime = 2;
                player.itemAnimation = 2;

            
                if (il.IronLotusFlame)
                { Projectile.Kill(); }
            }
            
        }
        
       
        public override bool PreDraw(ref Color lightColor)
        {
            
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;
            float smearoffset;

   

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(Projectile.width * 0.22f, Projectile.height);
                rotationOffset = MathHelper.ToRadians(47f);
                effects = SpriteEffects.None;
                smearoffset = 2.5f;
            }
            else
            {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(140f);
                effects = SpriteEffects.FlipHorizontally;
                smearoffset = 0.5f;
            }

            Texture2D texture = TextureAssets.Projectile[Type].Value;





            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

           
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
         
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 140f * Projectile.scale, ref collisionPoint);


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is IronLotus il)
            {
                if (Projectile.numHits == 0)
                {
                    il.IronLotusCharge += 5;
                }
            }

        
        }

        public override bool? CanDamage() => Timer >= 10 ? null : false;
       

        float WeaponAngle(float tick)
        {
           
                if(tick < 10)
                    return MathHelper.SmoothStep(-200f, -160f, tick / 10f); 
                else if (tick < 40 && tick >= 10)
                return MathHelper.SmoothStep(-160f, 160f, (tick - 10) / 30f);
            else if (tick < 50 && tick >= 40)
                return MathHelper.SmoothStep(160f, 180f, (tick - 40) / 10f);
            else if (tick < 90 && tick >= 50)
                return MathHelper.SmoothStep(180f, -540f, (tick - 50) / 40f);
            else if (tick < 110 && tick >= 90)
                return MathHelper.SmoothStep(-540f, -560f, (tick - 90) / 20f);
            else 
                return MathHelper.SmoothStep(-560f, -200f, (tick - 110) / 30f);

        }
        float fadeinop(float tick)
        {
            if (tick < 10)
                return 0f;
            else if (tick < 25 && tick >= 10)
                return MathHelper.SmoothStep(0f, 0.7f, (tick - 10) / 15f);
            else if (tick < 40 && tick >= 25)
                return MathHelper.SmoothStep(0.7f, 0f, (tick - 25) / 15f);
            else if (tick < 50 && tick >= 40)
                return 0f;
            else if (tick < 70 && tick >= 50)
                return MathHelper.SmoothStep(0f, 0.7f, (tick - 50) / 20f);
            else if (tick < 90 && tick >= 70)
                return MathHelper.SmoothStep(0.7f, 0f, (tick - 70) / 20f);
            else if (tick < 110 && tick >= 90)
                return 0f;
            else if (tick < 125 && tick >= 110)
                return MathHelper.SmoothStep(0f, 0.7f, (tick - 110) / 15f);
            else 
                return MathHelper.SmoothStep(0.7f, 0f, (tick - 125) / 15f);

        }
        float WeaponScale(float tick)
        {
            if (tick < 10)
                return MathHelper.SmoothStep(0.8f, 1f, tick / 10f);
            else if (tick < 40 && tick >= 10)
                return MathHelper.SmoothStep(1f, 0.8f, (tick - 10) / 30f);
            else if (tick < 50 && tick >= 40)
                return MathHelper.SmoothStep(0.8f, 1f, (tick - 40) / 10f);
            else if (tick < 90 && tick >= 50)
                return MathHelper.SmoothStep(1f, 0.8f, (tick - 50) / 40f);
            else if (tick < 110 && tick >= 90)
                return MathHelper.SmoothStep(0.8f, 1f, (tick - 90) / 20f);
            else
                return MathHelper.SmoothStep(1f, 0.8f, (tick - 110) / 30f);
        }

       

    }
           
           
}

