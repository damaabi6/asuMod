
using asuw.Content.Dusts;
using asuw.Content.Items.Weapons;
using asuw.Content.Projectiles.Filler;
using asuw.Content.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles
{
    public class TheEndBullet : ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        public ref float timer => ref Projectile.ai[1];
        public ref float tipe => ref Projectile.ai[0];

        public ref float crit => ref Projectile.ai[2];
        Player player => Main.player[Projectile.owner];


        public override void SetDefaults()
        {
            Projectile.width = 8; // The width of projectile hitbox
            Projectile.height = 8; // The height of projectile hitbox
            Projectile.aiStyle = ProjAIStyleID.Arrow; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 8; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            AIType = ProjectileID.Bullet; // Act exactly like default Bullet
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (tipe == 1)
            { SoundEngine.PlaySound(TheEnd.ShootB, player.Center); SoundEngine.PlaySound(TheEnd.Exhaust with { Volume = 0.4f }, player.Center); }
        }
        public override void AI()
        {

            NPC targetToHome = AsuUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 1000);

            

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            
            timer++;
            if (timer > 10)
            {
                if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is TheEnd te)
                {
                    if (targetToHome != null && targetToHome.CanBeChasedBy() && targetToHome.asuw().theEndTarget && Projectile.numHits == 0 && te.homeIn)
                    {
                        Projectile.velocity = AsuUtils.SmoothHomingBehavior(Projectile, targetToHome.Center, 1, 0.003f);
                    }
                }

                if (timer % 5 == 0)
                {
                    bool isSpark = Main.rand.NextBool(3);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, isSpark ? 278 : ModContent.DustType<SharpSparkDust>(), (Projectile.velocity * 2).RotatedByRandom(0.2f) * Main.rand.NextFloat(0.2f, 1f));
                    dust.noGravity = true;
                    dust.velocity *= (isSpark ? 0.5f : 1);
                    dust.scale = Main.rand.NextFloat(0.95f, 1.25f) * (isSpark ? 0.9f : 1);
                    dust.color = Main.rand.NextBool(5) ? Color.MediumPurple : Color.Indigo;
                }

                if (tipe == 0)
                {
                    
                }
                else
                {
                   
                   

                    
                }

                
            }

        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
          

            if (tipe != 0)
            {

                if (target.CanBeChasedBy())
                {
                    target.asuw().theEndTargetTick = 1200;
                }

                if (tipe == 1 || tipe == 4)
                {
                    Projectile.Kill();
                }

            }
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is TheEnd te)
            {
               if(!player.asuw().TheEndDestroyerSkill) te.healeddAmount++;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (tipe == 1)
            {
              
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TheEndBoom>(), Projectile.damage * 5, 10f, Projectile.owner);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (crit == 1)modifiers.SetCrit();

            
        }




    }
}
