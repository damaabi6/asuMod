using asuw.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace asuw.Content.Projectiles
{
    public class Rudal : ModProjectile
    {



        public ref float DelayTimer => ref Projectile.ai[1];
        public int DelayTimer2 = 0;
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 16; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1;  // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
           // Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }




        public override void AI()
        {
            int width = Main.rand.Next(8);
            int height = Main.rand.Next(8);
            int dust3 = Dust.NewDust(Projectile.Center, width, height, DustID.SolarFlare);
            Main.dust[dust3].noGravity = true;
            Main.dust[dust3].velocity = Projectile.velocity * -1;
            Main.dust[dust3].scale = Main.rand.Next(1, 2);
            Main.dust[dust3].velocity = Main.dust[dust3].velocity.RotatedByRandom(MathHelper.ToRadians(30));
            Main.dust[dust3].velocity *= 0.3f;

            DelayTimer2++;
            Player player = Main.player[Projectile.owner];
            float maxDetectRadius = 400f;

            Lighting.AddLight(Projectile.Center, (1f), (1f), (1f)); // R G B values from 0 to 1f. This is the red from the Crimson Heart pet

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // projectile sprite faces up
            Projectile.spriteDirection = Projectile.direction;

            if (Projectile.position.Y > player.position.Y - 20f)
            {
                Projectile.tileCollide = true;
            }

          

            if (DelayTimer < 18)
            {
                DelayTimer += 1;
                return;
            }
            
            // First, we find a homing target if we don't have one
            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            // If we don't have a target, don't adjust trajectory
            if (HomingTarget == null)
                return;

            // If found, we rotate the projectile velocity in the direction of the target.
            // We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(7)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; ;

           
            
           
                
            


        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC able to be targeted.
                if (IsValidTarget(target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target)
        {
            // This method checks that the NPC is:
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
            return target.CanBeChasedBy();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            modifiers.SourceDamage *= player.GetModPlayer<AsuPlayer>().butterflyHairpin ? 1.20f : 1f;

        }
       
        public override void OnKill(int timeLeft)
        {
          
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Vector2 launchVelocity = new Vector2(0, 0);
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVelocity, ModContent.ProjectileType<Projectiles.BoomS>(), Projectile.damage , Projectile.knockBack, Projectile.owner);
        }
    }
           
           
}

