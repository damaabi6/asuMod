using asuw.Content.Cooldown.WeaponCooldowns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace asuw.Content
{
    public static partial class AsuUtils
    {
        //TODO organize ts
        public static AsuPlayer asuw(this Player player) => player.GetModPlayer<AsuPlayer>();
        public static AsuGlobalNPC asuw(this NPC npc) => npc.GetGlobalNPC<AsuGlobalNPC>();
        public static AsuGlobalProj asuw(this Projectile proj) => proj.GetGlobalProjectile<AsuGlobalProj>();

        public static int[] WeaponCooldowns = { ModContent.BuffType<HorusCooldown>(), ModContent.BuffType<MakotoCooldown>(), ModContent.BuffType<ProjectionDashCD>(), ModContent.BuffType<ShiCooldown>(), ModContent.BuffType<SubaruCooldown>() };
        public static bool CantUseHoldout(this Player player, bool needsToHold = true) => player == null || !player.active || player.dead || (!player.channel && needsToHold) || player.CCed || player.noItems;
   
        public static float AngleBetween(this float angle, float otherAngle)
        {
            return (otherAngle - angle + MathF.PI).Modulo(MathF.PI * 2f) - MathF.PI;
        }
        public static float AbsDelta(this float value, float otherValue) => MathF.Abs(otherValue - value);
        public static int AbsDelta(this int value, int otherValue) => Math.Abs(otherValue - value);

        public static float PingPong(float time, float dur)
        {
            float t = time / (int)(dur / 2f);
            return t <= 1f ? t : 2f - t;
        }
        public static float SawTooth(float time, float duration, int repeats)
        {
            return ((time / duration) * repeats) % 1f;
        }
        public static void UseBlendState(this SpriteBatch sb, BlendState blend, SamplerState s = null)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, blend, s == null ? Main.DefaultSamplerState : s, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
        }
     
        public static float SquaredF(this float amount)
        {
            return amount * amount;
        }
        public static int SquaredI(this int amount)
        {
            return amount * amount;
        }
        public static float Modulo(this float dividend, float divisor)
        {
            return dividend - (float)Math.Floor(dividend / divisor) * divisor;
        }
        public static Texture2D GetTexture(this Projectile p)
        {
            return TextureAssets.Projectile[p.type].Value;
        }
        public static float randomRot()
        {
            return (float)(Main.rand.NextDouble() * MathHelper.Pi * 2);
        }
        public static float getDistance(Vector2 v1, Vector2 v2)
        {
            return ((float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2)));
        }
        public static NPC FindTarget_HomingProj(Projectile proj, Vector2 center, float radians, Func<Projectile, int, bool> filter = null)
        {
            NPC npc = null;
            float dist = radians;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.CanBeChasedBy(proj) && !n.friendly)
                {
                    if (getDistance(n.Center, center) <= dist && (filter == null || filter.Invoke(proj, n.whoAmI)))
                    {
                        dist = getDistance(n.Center, center);
                        npc = n;
                    }
                }
            }
            return npc;
        }
        public static Vector2 randomVec(float max)
        {
            return new Vector2(Main.rand.NextFloat(-max, max), Main.rand.NextFloat(-max, max));
        }

        public static float RotTowards(this float curAngle, float targetAngle, float maxChange)
        {
            curAngle = MathHelper.WrapAngle(curAngle);
            targetAngle = MathHelper.WrapAngle(targetAngle);
            if (curAngle < targetAngle)
            {
                if (targetAngle - curAngle > (float)Math.PI)
                {
                    curAngle += (float)Math.PI * 2f;
                }
            }
            else if (curAngle - targetAngle > (float)Math.PI)
            {
                curAngle -= (float)Math.PI * 2f;
            }

            curAngle += MathHelper.Clamp(targetAngle - curAngle, 0f - maxChange, maxChange);
            return MathHelper.WrapAngle(curAngle);
        }

        public static float Towards(this float current, float target, float maxChange)
        {
            if (current < target)
                return Math.Min(current + maxChange, target);
            else
                return Math.Max(current - maxChange, target);
        }
        public static Vector2 SmoothHomingBehavior(this Entity entity, Vector2 TargetCenter, float SpeedUpdates = 1, float HomingStrength = 0.1f)
        {
            float targetAngle = (TargetCenter - entity.Center).ToRotation();
            float f = entity.velocity.ToRotation().RotTowards(targetAngle, HomingStrength);
            Vector2 speed = f.ToRotationVector2() * entity.velocity.Length() * SpeedUpdates;
            entity.velocity = speed;
            return speed;
        }
        public static float RotateTowardsAngle(float currentRadians, float targetRadians, float rotateSpeed, bool useFixedSpeed = true)
        {
            currentRadians = MathHelper.WrapAngle(currentRadians);
            targetRadians = MathHelper.WrapAngle(targetRadians);

            float difference = targetRadians - currentRadians;
            float turnAmount = MathHelper.WrapAngle(difference);

            if (useFixedSpeed)
            {
                turnAmount = MathHelper.Clamp(turnAmount, -rotateSpeed, rotateSpeed);
            }
            else
            {
                turnAmount *= MathHelper.Clamp(rotateSpeed, 0f, 1f);
            }

            return currentRadians + turnAmount;
        }
        public static bool sync() => Main.netMode != NetmodeID.MultiplayerClient;
        public static void DamageEffectColored(this NPC npc, int damageAmount, Color color)
        {
            Rectangle location = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
            if (Main.dedServ)
            {
                NetMessage.SendData(81, -1, -1, null, (int)color.PackedValue, location.Center.X, location.Center.Y, damageAmount);
            }
            else
            {
                CombatText.NewText(location, color, damageAmount);
            }
        }

        public static Vector2 normalize(this Vector2 v)
        {
            return v.SafeNormalize(Vector2.Zero);
        }
        public static Vector2 mouseWorld(this Player player)
        {
            player.asuw().mouseWorldListener = true;
            return player.asuw().mouseWorld;
        }
        public static void FriendlySetDefaults(this Projectile Projectile, DamageClass dmgClass, bool tileCollide = false, int penetrate = 1)
        {
            Projectile.DamageType = dmgClass;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = penetrate;
            Projectile.tileCollide = tileCollide;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }
        //i need to remember to use this
        public static void SetHandRotFront(this Player owner, float r)
        {
              owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, r - (float)(Math.PI * 0.5f));
        }
        public static void SetHandRotBack(this Player owner, float r)
        {
            owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, r - (float)(Math.PI * 0.5f));
        }
       
  

     
    }
}
