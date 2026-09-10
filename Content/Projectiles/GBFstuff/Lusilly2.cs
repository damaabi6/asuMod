using asuw.Content.Dusts;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles.GBFstuff
{
    public class Lusilly2 : ModProjectile
    {
        //refute
        Player player => Projectile.GetOwner();
        ref float time => ref Projectile.ai[1];
        ref float flipDir => ref Projectile.ai[0];
        ref float mode => ref Projectile.ai[2];
        float direction = 0;
        float duration = 0;
        float easedTime = 0;
        float slashGlowOP = 0;
        bool canDMG = false;
        float directionDis = 280f;
        float distanceBonusScale = 0;
        Vector2 destination;
        float destinationRot = 0;

        //trail parameters
        List<float> odr = new List<float>();
        List<float> ods = new List<float>();
        List<Vector2> odp = new List<Vector2>();

        public override void SetDefaults()
        {
            Projectile.width = 167; 
            Projectile.height = 167; 
            Projectile.friendly = true; 
            Projectile.hostile = false; 
            Projectile.DamageType = DamageClass.Melee; 
            Projectile.penetrate = -1; 
            Projectile.timeLeft = 35; 
            Projectile.alpha = 0; 
            Projectile.light = 0.5f; 
            Projectile.ignoreWater = true; 
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = 10;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;

        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.spriteDirection);
            writer.WriteVector2(destination);
            writer.Write(destinationRot);
            writer.Write(duration);
            writer.Write(direction);
            writer.Write(directionDis);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadInt32();
            destination = reader.ReadVector2();
            destinationRot = reader.ReadSingle();
            duration = reader.ReadSingle();
            direction = reader.ReadSingle();
            directionDis = reader.ReadSingle();
        }
        public override void OnSpawn(IEntitySource source)
        {
            direction = player.asuw().mouseRotationFromPlayer;
            duration = player.itemTimeMax * 2 * Projectile.MaxUpdates;
            if (duration < 300) duration = 300; //cap so anim not broke
            float dis = Vector2.Distance(player.MountedCenter, player.asuw().mouseWorld);
            if (dis > 280 && dis < 600) directionDis = dis;
            else if (dis <= 280) directionDis = 280f;
            else directionDis = 600f;
            distanceBonusScale = ((directionDis - 280f) / 320f);
        }


        public override void AI()
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            if (mode == 1)//normal Att
            {
                Projectile.velocity = direction.ToRotationVector2();
                Projectile.spriteDirection = Projectile.direction * (int)flipDir;

                float t = time / ((int)duration / 2f);            // 0.0 → 2.0
                float v = t <= 1f ? t : 2f - t;          // 0→1 then 1→0
                easedTime = MathHelper.Lerp(0f, 1f, time / (int)duration);

                float destinationHor = MathHelper.Lerp(70f, directionDis, AsuUtils.CircIn(v));
                destinationRot = MathHelper.ToRadians(MathHelper.Lerp(-160f, 160f, AsuUtils.ExpoInOut(easedTime)) * Projectile.direction * flipDir);
                destination = (Vector2.UnitX * destinationHor).RotatedBy(direction + destinationRot);

                Projectile.Center = player.MountedCenter + destination;
                Projectile.rotation = direction + destinationRot;
                Projectile.scale = MathHelper.Lerp(1f, 1.9f + distanceBonusScale * 1.1f, AsuUtils.CircIn(v));
                Projectile.Opacity = MathHelper.Lerp(0f, 1f, AsuUtils.CircOut(v));
                slashGlowOP = MathHelper.Lerp(0f, 1f, AsuUtils.CircIn(v));

                if (time > duration * 0.4f && time < duration * 0.6f)
                {
                    Vector2 edge = Projectile.Center + (Projectile.rotation.ToRotationVector2() * ((texture.Width / 3f) * Projectile.scale * 1.5f));
                    Vector2 vel = (Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2 * flipDir) * Main.rand.NextFloat(6f, 9f)).RotatedByRandom(MathHelper.ToRadians(30f));

                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * ((Projectile.width / 2f) * Projectile.scale) * -1) + (Projectile.rotation.ToRotationVector2() * (Projectile.width * Projectile.scale * Main.rand.NextFloat(0.35f, 1f))), ModContent.DustType<SharpSparkDust>(), vel * Main.rand.NextFloat(0.1f, 0.8f), 0, Color.Red, Main.rand.NextFloat(0.3f, 0.8f));
                        dust.noGravity = true;
                    }

                    if (time > duration * 0.45f && time < duration * 0.55f)
                    {
                        canDMG = true;
                    }
                    else canDMG = false;
                }
                else canDMG = false;

                if (time > duration * 0.35f)//trail
                {
                    odr.Add(Projectile.rotation);
                    ods.Add(Projectile.scale);
                    odp.Add(Projectile.Center);
                    if (odr.Count > 10)
                    {
                        odr.RemoveAt(0);
                        ods.RemoveAt(0);
                        odp.RemoveAt(0);
                    }
                }
                else
                {
                    if (odr.Count > 0)
                    {
                        odr.RemoveAt(0);
                        ods.RemoveAt(0);
                        odp.RemoveAt(0);
                    }
                }
            }
            else //idle
            {
                float easing = (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f;
                Vector2 pos = player.MountedCenter + Vector2.UnitX * player.direction * MathHelper.Lerp(-30f, -35f, easing) * 2.5f + Vector2.UnitY * MathHelper.Lerp(-80f, -90f, easing) * 0.2f;
                Projectile.Center = Projectile.Center.MoveTowards(pos, 3);
                Projectile.velocity = (player.Center - Projectile.Center).normalize();
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = -MathHelper.PiOver2 + MathHelper.ToRadians(MathHelper.Lerp(0f, -3f, easing) * Projectile.direction) + MathHelper.ToRadians(20 * Projectile.direction);
                Projectile.Opacity = time < 600 ? MathHelper.Lerp(0, 1, time / 600) : 1;
                Projectile.scale = 1;
                float glowSpawnOp = time < 600 ? MathHelper.Lerp(1, 0, time / 600) : 0;
                slashGlowOP = MathHelper.Lerp(0.4f, 1f, easing) + glowSpawnOp;
                canDMG = false;

                if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Lucilus lul)
                {
                    if (player.ownedProjectileCounts[Projectile.type] <= 1 && player.ownedProjectileCounts[ModContent.ProjectileType<RedSaw>()] < 1 && !player.dead) Projectile.timeLeft = 2;
                }
            }
            if (time <= duration) Projectile.timeLeft = 2;
            if (time == Projectile.MaxUpdates) Projectile.ForceNetUpdate();
            time++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {


            float randRot = Main.rand.NextFloat(-0.15f, 0.15f);
            Vector2 pos = Vector2.Lerp(Projectile.Center, target.Center, 0.9f);
            Vector2 vectortotarget = (target.Center - player.Center).normalize();
            Vector2 vel = vectortotarget.RotatedBy(MathHelper.ToRadians(50 * Projectile.spriteDirection)).RotatedBy(randRot);


            if (Projectile.numHits <= 1)
            {

                

                
            }

          
        }

        public override bool? CanDamage() => canDMG ? null : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = player.MountedCenter;
            Vector2 end = Projectile.Center + (Projectile.rotation.ToRotationVector2() * ((texture.Width / 3f) * Projectile.scale * 1.8f));
            float collisionPoint = 0f;
            float collisionWidth = 90f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D textureShine = ModContent.Request<Texture2D>("asuw/Content/Projectiles/GBFstuff/Lusilly2Shined", AssetRequestMode.AsyncLoad).Value;
            Texture2D slashglow = ModContent.Request<Texture2D>("asuw/Content/Projectiles/GBFstuff/Lusilly2Glow", AssetRequestMode.AsyncLoad).Value;

            Vector2 origin = texture.Size() / 2f;

            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/MotionTrail2", AssetRequestMode.ImmediateLoad).Value;

            List<ColoredVertex> ve = new List<ColoredVertex>();

            for (int i = 0; i < odr.Count; i++)
            {
                Color b = Color.MistyRose;
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture.Width / 3f) * ods[i]) * 1.5f),
                      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                      b));
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture.Width / 4f) * ods[i] * -1)),
                      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                      b));
            }

            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                Effect shader = ModContent.Request<Effect>("asuw/Effects/ColorizeBloom", AssetRequestMode.ImmediateLoad).Value;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue((Color.Red).ToVector4());
                shader.Parameters["color1"].SetValue((Color.Maroon).ToVector4());
                shader.Parameters["alpha"].SetValue(slashGlowOP);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();

                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                trail = ModContent.Request<Texture2D>("asuw/Assets/SplitTrail", AssetRequestMode.ImmediateLoad).Value;
                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                Main.spriteBatch.ExitShaderRegion();
            }
            

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(slashglow, Projectile.Center - Main.screenPosition, default, Color.Red * slashGlowOP, Projectile.rotation, slashglow.Size() / 2f, Projectile.scale, effects, 0);
            Main.spriteBatch.ExitShaderRegion();

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            if (mode != 0) Main.spriteBatch.Draw(textureShine, Projectile.Center - Main.screenPosition, default, Color.White * slashGlowOP, Projectile.rotation, origin, Projectile.scale, effects, 0);


            return false;
        }


    }
     
}

