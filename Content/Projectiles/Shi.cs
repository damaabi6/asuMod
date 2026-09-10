
using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Items.Weapons;
using asuw.Content.Projectiles.Filler;
using asuw.Content.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Mono.Cecil.Cil;
using ReLogic.Content;
using System;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using asuw.Content.Dusts;

namespace asuw.Content.Projectiles
{

    public class Shi : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        public override string Texture => "asuw/Content/Textures/shislash";
        public bool slashing;
        public float FadeIn = 0;
        public Vector2 scale;
        Vector2 SmearScale;
        Color smearCol;
        public float rotation = 0;
        public float rotAmnt = 0;
        float SmearOP = 0;
        public ref float dmg => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[0];
        float duration = 0f;
        bool forward = true;
        float rot = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 204;
            Projectile.height = 204;
            Projectile.friendly = true;
            Projectile.timeLeft = 80;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.MaxUpdates = 4;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            duration = player.HeldItem.useTime;
            duration *= Projectile.MaxUpdates;
            Timer = Projectile.ai[2] == 1? duration : 0f;
           //duration *= player.GetTotalAttackSpeed(Projectile.DamageType);
        }
        public override void AI()
        {
            //positioning and direction
            player.asuw().mouseRotationListener = true;
            Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
            //Projectile.Center = player.GetDrawCenter();
            player.heldProj = Projectile.whoAmI;
            Projectile.spriteDirection = forward ? Projectile.direction : -Projectile.direction;
            //player Dir and Arm rot
            player.ChangeDir(Projectile.direction);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation * player.gravDir) - MathHelper.ToRadians(90f));
            //timeleft
            if (!player.CantUseHoldout())
                Projectile.timeLeft = (int)duration;
            else
                if (Timer == duration - 1 || Timer == 1) Projectile.Kill();
            //immunity
            if (Timer == duration || Timer == 0)
            {
                Projectile.ResetLocalNPCHitImmunity();
                Projectile.numHits = 0;
                Projectile.velocity = player.asuw().mouseNormalFromPlayer;
                rot = player.asuw().mouseRotationFromPlayer;
            }
            //rotations
            rotAmnt = forward ? MathHelper.Lerp(0, 1, AsuUtils.ExpoOut(Timer / duration)) : MathHelper.Lerp(0, 1, AsuUtils.ExpoIn(Timer / duration));
            rotation = MathHelper.ToRadians((-160f + (320f * rotAmnt)) * Projectile.direction);
            Projectile.rotation = rot + rotation;
            //lerp amounts
            float t = Timer / (duration / 2f);            // 0.0 → 2.0
            float amount = t <= 1f ? t : 2f - t;          // 0→1 then 1→0
            float scaleAmnt = MathHelper.Lerp(-0.2f, 0.35f, AsuUtils.ExpoOut(amount));
            scale = new Vector2(1 + scaleAmnt, 1 + (scaleAmnt / 2f));
            SmearScale = new Vector2(1 + (scaleAmnt * 2), 1 + scaleAmnt);
            SmearOP = MathHelper.Lerp(0, 0.6f, amount);
            //smears
            if (Timer == (forward ? duration * 0.25f : duration * 0.75f))
            {
               
            }
            //sword Tip
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center;
            Vector2 end = start + (Projectile.rotation.ToRotationVector2() * ((texture.Width * scale.X) * 0.85f));
            //dusts
            if (forward ? Timer <= 30 : Timer >= 50)
            {
                Dust dust = Dust.NewDustPerfect(end, DustID.GemRuby, Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(forward ? -90 : 90)).RotatedByRandom(0.4f) * Main.rand.NextFloat(4f, 7f), 0, default, Main.rand.NextFloat(1f, 2f));
                dust.noGravity = true;
            }
            //mafh stuffs
            player.itemAnimation = 2;
            player.itemTime = 2;
            if (Timer >= duration) { Timer = duration; forward = false; }
            if (Timer <= 0f) { Timer = 0f; forward = true; }
            Timer += forward ? 1 : -1;
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Yujin yu)
            {
                dmg = (yu.ShiDMGInc * 0.01f);
            }

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center;
            Vector2 end = start + (Projectile.rotation.ToRotationVector2() * (texture.Width * scale.X + 10));
            float collisionPoint = 0f;
            float collisionWidth = texture.Height * scale.Y + 20f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override void OnKill(int timeLeft)
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Yujin yu)
            {
                yu.UpOrDown = forward ? 1 : 0;//0 is down 1 is up
            }


        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits == 0)
            {
                player.wingTime += (player.wingTimeMax / 10);
                if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Yujin yu)
                {
                    yu.ShiDMGInc += 1f;
                    if (yu.ShiDMGInc > 0)
                    {
                        yu.heal += 4;
                    }
                }

                SoundEngine.PlaySound(new SoundStyle("asuw/Content/Sounds/ShiClash") with { Volume = 0.7f, PitchVariance = 0.25f }, player.Center);

                SoundEngine.PlaySound(new SoundStyle("asuw/Content/Sounds/Thump") with { Volume = 0.85f }, player.Center);
            }

            Vector2 vectortotarget = (target.Center - Projectile.Center).normalize();
            Vector2 pos;
            float sqrDistanceTo = Vector2.DistanceSquared(target.Center, Projectile.Center);

            if (sqrDistanceTo > (Projectile.width * Projectile.height))
                pos = Projectile.Center + (vectortotarget * Projectile.width);
            else
                pos = target.Center;

            for (int i = 0; i < 7; i++)
            {
                Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(15, 15), ModContent.DustType<SharpSparkDust>(), vectortotarget.RotatedBy(MathHelper.ToRadians(forward ? 30 : -30) * Projectile.direction).RotatedByRandom(0.6f) * Main.rand.NextFloat(8f, 14f), 0, Main.rand.NextBool() ? Color.IndianRed : Color.Red, Main.rand.NextFloat(1f, 2f));
                dust.noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *=  (1f + dmg + (Projectile.numHits <= 4 ? 1 - (Projectile.numHits / 4f) : 0));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D slashglow = ModContent.Request<Texture2D>("asuw/Content/Textures/shislashglow", AssetRequestMode.AsyncLoad).Value;
         
            Vector2 origin = new Vector2(0, texture.Height / 2f);

          
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation, origin, 0.8f * scale, effects, 0);
            Main.spriteBatch.Draw(slashglow, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity, Projectile.rotation, origin, 0.8f * scale, effects, 0);

            return false;
        }


        
    }
}