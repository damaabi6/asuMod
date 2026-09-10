
using asuw.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.LootSimulation.LootSimulatorConditionSetterTypes;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles
{
    public class KirinoProj : ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        ref float timer => ref Projectile.ai[1];
        Player player => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 12; 
            Projectile.height = 12; 
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true; 
            Projectile.hostile = false; 
            Projectile.DamageType = DamageClass.Ranged; 
            Projectile.penetrate = 10; 
            Projectile.timeLeft = 300; 
            Projectile.alpha = 0; 
            Projectile.light = 0.5f; 
            Projectile.ignoreWater = true; 
            Projectile.tileCollide = true; 
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            AIType = ProjectileID.Bullet; 
        }
        public override void OnSpawn(IEntitySource source)
        {
            timer = 50;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (timer == 1)
            {
                Projectile.ResetLocalNPCHitImmunity();
                bounce(true);
            }

            if (timer > 0) timer--;
           
        
           Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpSparkDust>(), (Projectile.velocity * -0.2f).RotatedByRandom(0.4f).RotatedByRandom(0.6f) * Main.rand.NextFloat(0.5f, 2f), 0, Main.rand.NextBool() ? Color.LightSalmon : Color.BurlyWood, Main.rand.NextFloat(0.2f, 0.6f));
            dust.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            timer = 30;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.ResetLocalNPCHitImmunity();
                bounce(false);
                timer = 30;
            }

            return false;
        }

        void bounce(bool ToCursor)
        {
          

            if (ToCursor)
            {
                Projectile.velocity = (player.mouseWorld() - Projectile.Center).normalize() * Projectile.oldVelocity.Length();
            }
            else
            {
                if (Math.Abs(Projectile.velocity.X - Projectile.oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -Projectile.oldVelocity.X;
                }

                // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
                if (Math.Abs(Projectile.velocity.Y - Projectile.oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -Projectile.oldVelocity.Y;
                }
            }

            Projectile.timeLeft = 250;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= (1.3f - ((float)Projectile.numHits / 10f));
        }
     
    }
}
