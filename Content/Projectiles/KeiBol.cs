using asuw.Content.Dusts;
using asuw.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles
{
    public class KeiBol : ModProjectile
    {

        Player player => Main.player[Projectile.owner];
        public override string Texture => "asuw/Assets/Blank";
        public bool blasted = false;
        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 85;
            Projectile.height = 85;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.timeLeft = 160;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

        }
        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Timer > 3 && !blasted)
            {
               

  
            }
            Timer++;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(Projectile.numHits == 0)blast();
        }

        void blast()
        {
            blasted = true;
            Projectile.velocity *= 0f;
            Projectile.Resize(750, 750);
            Projectile.timeLeft = 20;
         
            SoundEngine.PlaySound(new SoundStyle("asuw/Content/Sounds/SwordOfLight/KeiBallBlast") with { Volume = 1.1f }, Projectile.Center);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10),  ModContent.DustType<SharpSparkDust>(), Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(9f, 18f), 0, Color.MediumVioletRed, Main.rand.NextFloat(1.5f, 3.85f));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<SquareDustFilled>(), Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4f, 11f), 0, Color.MediumVioletRed, Main.rand.NextFloat(1.2f, 3.4f));
            }
        }

    
    }
}
