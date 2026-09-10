
using asuw.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;

namespace asuw.Content.Projectiles
{
    public class SupernovaRocket : ModProjectile
    {

        Player player => Main.player[Projectile.owner];
        public ref float Timer => ref Projectile.ai[2];
        public ref float barrage => ref Projectile.ai[1];
        public ref float Ray => ref Projectile.ai[0];
        public Color mainColor = Color.CornflowerBlue;
        public NPC target;
        public bool hide = false;
        public float rotAmnt = 0.14f;
        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundStyle sound = new("asuw/Content/Sounds/SwordOfLight/SupernovaRocketLaunch");
            if(barrage != 1)SoundEngine.PlaySound(sound with { Volume = 0.69f , PitchVariance = 0.4f , MaxInstances = 30}, Projectile.Center);

            if (Ray == 0)
                mainColor = Color.CornflowerBlue;
            else if (Ray == 1)
                mainColor = Color.Lerp(Color.DeepSkyBlue, Color.MediumOrchid, 0.85f);
            else
                mainColor = Color.Lerp(Color.Lerp(Color.DeepPink, Color.SteelBlue, 0.5f), Color.Lerp(Color.DodgerBlue, Color.DarkOrchid, 0.5f), MathHelper.SmoothStep(0, 1, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f));

            for (int i = 0; i < 5; i++)
            {
               

                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpSparkDust>(), (Projectile.velocity.normalize() * Main.rand.NextFloat(6f, 18f)).RotatedByRandom(0.4f), 0, mainColor, Main.rand.NextFloat(0.9f, 1.5f));
                dust.noGravity = true;
            }
        }
       
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (barrage == 1) mainColor = mainColor with { A = 0 };

            target = AsuUtils.FindTarget_HomingProj(Projectile, player.mouseWorld(), 1000);
            if (Timer > 5)
            {
                if (target != null) 
                    Projectile.velocity = AsuUtils.SmoothHomingBehavior(Projectile, target.Center, 1f, rotAmnt);
                else
                    Projectile.velocity = AsuUtils.SmoothHomingBehavior(Projectile, player.mouseWorld(), 1f, rotAmnt);

                if (Timer % 2 == 0 && target!= null) rotAmnt *= 1.5f;
            }

        

            if (target == null)
            {
                if (AsuUtils.getDistance(Projectile.Center, player.mouseWorld()) <= 30)
                {
                    Projectile.Kill();
                }
            }
            

            
            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            blast();
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.numHits == 0) blast();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float value = 20f;
            Vector2 start = Projectile.Center + (Projectile.rotation.ToRotationVector2() * -(value / 2));
            Vector2 end = start + (Projectile.rotation.ToRotationVector2() * value); ;
            float collisionPoint = 0f;
            float collisionWidth = value;
           
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = AsuUtils.GetTexture(Projectile);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
        
             Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None);

            return false;

        }
        void blast()
        {


        }
    }
}
