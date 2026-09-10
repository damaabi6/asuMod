using asuw.Content.Buffs;
using asuw.Content.Projectiles.EOH;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;


namespace asuw.Content.Projectiles.Filler
{
    public class HoshinoUlt : ModProjectile
    {

        Player Owner => Main.player[Projectile.owner];

        public ref float tipe => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 6; 
            Projectile.height = 6; 
            Projectile.friendly = true; 
            Projectile.hostile = false; 
            Projectile.DamageType = DamageClass.Ranged;                   
            Projectile.penetrate = 1; 
            Projectile.timeLeft = 360;
            Projectile.alpha = 0; 
            Projectile.light = 0.5f; 
            Projectile.ignoreWater = true; 
            Projectile.extraUpdates = 7; 
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }
      
        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            

            if (Projectile.timeLeft > 359 && tipe == 0)
            {
                for (int i = 1; i < 50; i++)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 vectorToCursor = Main.MouseWorld - Projectile.Center;
                        Vector2 velocity = ((vectorToCursor.RotatedByRandom(0.55f).SafeNormalize(Vector2.One)) * ((-1 - i) + ((float)i / 2f)));
                        float rand = Main.rand.NextFloat(-10, 10);
                        Dust dust3 = Dust.NewDustPerfect(Owner.Center, 31, velocity, 0, default, Main.rand.NextFloat(2, 3));
                    }

                }

            }



        }
      
        public override void OnSpawn(IEntitySource source)
        {

            
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 vectorToCursor = Main.MouseWorld - Projectile.Center;

                if (tipe == 0)
                {

                }

             

                
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= Owner.GetModPlayer<AsuPlayer>().butterflyHairpin ? 1.20f : 1f;
        }

        //bigger hitbox cuz missing this would be annoying af
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
        {
            Vector2 start = Projectile.Center + (Projectile.rotation.ToRotationVector2() * -50f);
            Vector2 end = start + (Projectile.rotation.ToRotationVector2() * 100f);
            float collisionPoint = 0f;
            float collisionWidth = 100f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth * Projectile.scale, ref collisionPoint);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (tipe != 1) Projectile.Kill();
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (tipe != 1) return true;
            else return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HorusBoomVis>(), Projectile.damage * 5, Projectile.knockBack, Projectile.owner, 1);

        }
    }
}

