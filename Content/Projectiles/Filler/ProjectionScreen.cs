using asuw.Content.Buffs;
using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Items.Weapons;
using asuw.Content.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace asuw.Content.Projectiles.Filler
{
    public class ProjectionScreen : ModProjectile
    {
       

        public Player player => Main.player[Projectile.owner];

        public bool tickdown = false;

     
        private NPC target
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float timer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 88; // The width of projectile hitbox
            Projectile.height = 167; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged;                             // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 100; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = 10;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Vector2 vectortoplayer = player.Center - Projectile.Center;
            if (target == null)
            {
                target = AsuUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 400);
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.rotation = vectortoplayer.ToRotation();
            }


            if (((float)target.height / 100) < 2)
            {
                Projectile.scale = (float)target.height / 100;
            }
            else
            {
                Projectile.scale = 2;
            }
        }
        public override void AI()
        {
            float sqrDistanceToPlayer = Vector2.DistanceSquared(player.Center, Projectile.Center);
            if (sqrDistanceToPlayer < (88 * 167) *( Projectile.scale + 0.5f) && player.asuw().ProjectionDashing)
            {
                Projectile.ai[2] = 1;
               
                Projectile.Kill(); 
            }
           
            Vector2 vectortoplayer = player.Center - Projectile.Center;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.rotation = vectortoplayer.ToRotation();
            }

            Projectile.spriteDirection = 1;
            if (((float)target.height / 100) < 2)
            {
                Projectile.scale = (float)target.height / 100;
            }
            else
            {
                Projectile.scale = 2;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 0)
            {
                if (Projectile.frame < 7)
                {
                    Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                }
                else
                {
                    Projectile.frame = 7;
                }
            }
            

            if(!target.active || target == null)
            {
                Projectile.Kill();
            }

          
                Vector2 launchVel = Utils.DirectionTo(target.Center, Projectile.Center);
                Projectile.alpha = 0;
                

                Projectile.velocity = target.Center - Projectile.Center;



            Projectile.netUpdate = true;




        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.asuw().projectedtick += 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + (Projectile.velocity * 0f) + new Vector2(0, 0).RotatedBy(Projectile.rotation), frame, Color.White, Projectile.rotation, origin, Projectile.scale + 0.5f, SpriteEffects.None, 0);

            Texture2D texture2 = ModContent.Request<Texture2D>($"asuw/Content/Textures/Extra/ProjectionFrameBack", AssetRequestMode.AsyncLoad).Value;

            Vector2 origin2 = texture2.Size() * 0.5f;
            Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, default, lightColor * 0.15f, Projectile.rotation, origin2, Projectile.scale + 0.5f, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[2] == 1)
            {
                player.ClearBuff(ModContent.BuffType<ProjectionDashCD>());
                player.asuw().ProjectionImmuntick = 120;

                SoundEngine.PlaySound(ProjectionSorcery.glass1, Projectile.Center);
                SoundEngine.PlaySound(ProjectionSorcery.glass2, Projectile.Center);
                SoundEngine.PlaySound(ProjectionSorcery.glass3, Projectile.Center);
                SoundEngine.PlaySound(ProjectionSorcery.glass4, Projectile.Center);
                SoundEngine.PlaySound(ProjectionSorcery.glass5, Projectile.Center);
                SoundEngine.PlaySound(ProjectionSorcery.glass6, Projectile.Center);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ProjectionFind>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ProjectionBoom>(), (int)((float)Projectile.damage * (player.asuw().butterflyHairpin ? 4.8f : 4f)), Projectile.knockBack, Projectile.owner, 1);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<ProjectionHeal>(), Projectile.damage, 0f, Main.myPlayer);
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, (player.velocity.SafeNormalize(Vector2.One) * Main.rand.Next(22, 35)).RotatedBy(MathHelper.ToRadians(-20 + i * 10) * player.direction), ModContent.ProjectileType<ShaterredProject>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, i, Projectile.scale);
                }

                for (int i = 0;i < 12; i++)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, (player.velocity.SafeNormalize(Vector2.One) * Main.rand.Next(14,40)).RotatedByRandom(0.5f), ModContent.ProjectileType<ShaterredProjectSparks>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Main.rand.Next(3),Projectile.scale);
                }

            }
        }
    }
}

