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
    public class IronLotusPhys2 : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        public int alphaTimer = 0;
        public ref float Timer => ref Projectile.ai[0];
        public bool yesdmg = false;
        public float shineop = 0;
        public float FadeIn = 0;

        public override string Texture => "asuw/Content/Projectiles/IronLotusPhys";
        public override void SetDefaults()
        {
            Projectile.width = 452; // The width of projectile hitbox
            Projectile.height = 404; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 130; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
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

            Vector2 vectorToCursor = player.asuw().mouseRotationFromPlayer.ToRotationVector2();
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Main.MouseWorld.X > player.MountedCenter.X ? 1 : -1);
            }


            Projectile.Center = player.Center;
                Projectile.direction = player.direction;
                Projectile.spriteDirection = Projectile.direction * -1;
                player.heldProj = Projectile.whoAmI;

            if (Timer < 90)
            {
                Vector2 posrand = Projectile.Center + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length() * Main.rand.NextFloat(0.6f, 1f)) * Projectile.scale);
                Vector2 vel = Projectile.rotation.ToRotationVector2().SafeNormalize(Vector2.Zero);
                int size = Main.rand.Next(8, 26);
                Dust idx = Dust.NewDustPerfect(posrand + new Vector2(Main.rand.NextFloat(15), Main.rand.NextFloat(15)), DustID.GoldCoin, vel * Main.rand.NextFloat(5, 12), 100, default, Main.rand.NextFloat(2f,4f));
                idx.noGravity = true;
            }

            if(Timer == 100)
            {

                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, (vectorToCursor * 120), ModContent.ProjectileType<IronLotusHeavySlash>(), Projectile.damage /2, 2, player.whoAmI);
            }
            if (Timer < 114)
            { Projectile.alpha -= 30; }
            else
            {
                alphaTimer++;
                Projectile.alpha = 0 + (alphaTimer * 30);
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));

            if (Timer > 90 && Timer < 110)
            { yesdmg = true; }



            float angle = WeaponAngle(Timer);
            Projectile.rotation = vectorToCursor.ToRotation() + (MathHelper.ToRadians(angle) * player.direction);

            Projectile.netUpdate = true;
            Timer++;
          
            shineop = shineopacity(Timer);
            FadeIn = fadeinop(Timer);

            player.itemTime = 10;
            player.itemAnimation = 10;



        }
        public override void OnKill(int timeLeft)
        {
           
        }  
       
        public override bool PreDraw(ref Color lightColor)
        {

            Vector2 origin;
            float rotationOffset;
            float smearoffset;
            SpriteEffects effects;


            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(Projectile.width * 0.24f, Projectile.height);
                rotationOffset = MathHelper.ToRadians(47f);
                effects = SpriteEffects.None;
                smearoffset = 2.5f;
            }
            else
            {
                origin = new Vector2(Projectile.width * 1.02f, Projectile.height);
                rotationOffset = MathHelper.ToRadians(140f);
                effects = SpriteEffects.FlipHorizontally;
                smearoffset = 0.5f;
            }


            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D shinetext = ModContent.Request<Texture2D>("asuw/Content/Projectiles/IronLotusPhysShine", AssetRequestMode.ImmediateLoad).Value;

        

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(shinetext, Projectile.Center - Main.screenPosition, default, Color.White * shineop, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);


            Projectile.scale = WeaponScale(Timer);



                return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 200f * Projectile.scale, ref collisionPoint);


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is IronLotus il)
            {
                if (Projectile.numHits == 0)
                {
                    il.IronLotusCharge += 10;
                }
            }


        }

        public override bool? CanDamage() => yesdmg ? null : false;
       

        float WeaponAngle(float tick)
        {
            Player player = Main.player[Projectile.owner];
           
                if(tick < 70)
                    return MathHelper.SmoothStep(-70f, -175f, tick / 70f);
            else if (tick < 90 && tick >= 70)
                return MathHelper.SmoothStep(-175f, -190f, (tick - 70) / 20f);
            else if (tick < 110 && tick >= 90)
                return MathHelper.SmoothStep(-190f, 160f, (tick - 90) / 20f);
            else 
                return MathHelper.SmoothStep(160f, 130f, (tick - 110) / 20f);

        }
        float WeaponScale(float tick)
        {
            if (tick < 70)
                return MathHelper.SmoothStep(0.8f, 1f, tick / 70f);
            else if (tick < 90 && tick >= 70)
                return MathHelper.SmoothStep(1f, 1.2f, (tick - 70) / 20f);
            else if (tick < 110 && tick >= 90)
                return MathHelper.SmoothStep(1.2f, 0.9f, (tick - 90) / 20f);
            else 
                return MathHelper.SmoothStep(0.9f, 0.8f, (tick - 110) / 20f);
           
        }

        float shineopacity(float tick)
        {
            if (tick < 70)
                return 0f;
            else if (tick < 80 && tick >= 70)
                return MathHelper.SmoothStep(0f, 0.7f, (tick - 70) / 10f);
            else if (tick < 90 && tick >= 80)
                return MathHelper.SmoothStep(0.7f, 0f, (tick - 80) / 10f);
            else return 0f;
        }

        float fadeinop(float tick)
        {
            if (tick < 90)
                return 0f;
            else if (tick < 100 && tick >= 90)
                return MathHelper.SmoothStep(0f, 0.7f, (tick - 90) / 10f);
            else if (tick < 110 && tick >= 100)
                return MathHelper.SmoothStep(0.7f, 0f, (tick - 100) / 10f);
            else return 0f;
        }


    }
           
           
}

