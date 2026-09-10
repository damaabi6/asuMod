using asuw.Content.Dusts;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Projectiles;
using asuw.Content.Projectiles.GBFstuff;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles.GBFstuff
{
    public class RedSaw : ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        Player player => Projectile.GetOwner();
        float duration = 1280; //total duration
        float rotVariance = 0; //for random
        float randRot = 1;
        int hitcooldown = 4;
        ref float rotation => ref Projectile.ai[2];
        ref float time => ref Projectile.ai[1];



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
            Projectile.timeLeft = 30;
            Projectile.alpha = 0;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = 5;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;

        }
        public override void OnSpawn(IEntitySource source)
        {
            rotVariance = ((MathHelper.TwoPi * 18f) + Main.rand.NextFloat(MathHelper.TwoPi * 2f));
            randRot = Main.rand.NextBool() ? 1 : -1;
            player.itemAnimation = player.itemAnimationMax / 2;
            player.itemTime = player.itemTimeMax / 2;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)randRot);
            writer.Write(rotVariance);
            writer.Write(Projectile.Opacity);
            writer.Write(duration);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            randRot = reader.ReadInt32();
            rotVariance = reader.ReadSingle();
            Projectile.Opacity = reader.ReadSingle();
            duration = reader.ReadSingle();
        }
        public override void AI()
        {
            if (Projectile.Opacity < 0.04f || player.dead) Projectile.Kill();
            Texture2D texture = ModContent.Request<Texture2D>("asuw/Content/Projectiles/GBFstuff/Lusilly2", AssetRequestMode.AsyncLoad).Value;

            float dis = Vector2.Distance(player.mouseWorld(), Projectile.Center);
            float speeed = dis > texture.Width * Projectile.scale ? dis > texture.Width * Projectile.scale * 2 ? 12f : 5f : 1.5f;
            Vector2 vectortomouse = (player.mouseWorld() - Projectile.Center).normalize();
            Projectile.velocity = vectortomouse;
            Projectile.Center = Projectile.Center.MoveTowards(player.mouseWorld(), speeed);

            float t = Utils.GetLerpValue(0f, 1f, time / duration, true);
            float v = AsuUtils.ExpoOut(t);
            float t2 = Utils.GetLerpValue(1, 0, (time - (duration / 2)) / (duration / 2), true);
            float v2 = AsuUtils.ExpoIn(t2);

            Projectile.scale = MathHelper.Lerp(1f, 1.7f, AsuUtils.CubicOut(t));
            Projectile.Opacity = time <= (duration / 2) ? 1 : v2 ;
            rotation = (rotVariance) * t;
            Projectile.rotation = (rotation * randRot);

            if (Projectile.Opacity > 0.1f)
            {
                Vector2 vel1 = (Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * randRot) * Main.rand.NextFloat(6f, 9f)).RotatedByRandom(MathHelper.ToRadians(30f));
                Vector2 vel2 = (Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2 * randRot) * Main.rand.NextFloat(6f, 9f)).RotatedByRandom(MathHelper.ToRadians(30f));

                Dust dust1 = Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * (texture.Width * Projectile.scale) * -1) * Main.rand.NextFloat(0f, 1f), ModContent.DustType<SharpSparkDust>(), vel1.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.8f), 0, Color.Red, Main.rand.NextFloat(0.3f, 0.8f));
                dust1.noGravity = true;

                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * (texture.Width * Projectile.scale) * 1) * Main.rand.NextFloat(0f, 1f), ModContent.DustType<SharpSparkDust>(), vel2.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.8f), 0, Color.Red, Main.rand.NextFloat(0.3f, 0.8f));
                dust2.noGravity = true;
            }

            odr.Add(Projectile.rotation);
            ods.Add(Projectile.scale);
            odp.Add(Projectile.Center);
            if (odr.Count > 35)
            {
                odr.RemoveAt(0);
                ods.RemoveAt(0);
                odp.RemoveAt(0);
            }
            if (time <= duration) Projectile.timeLeft = 2;
            if ((time / Projectile.MaxUpdates) % hitcooldown == 0)
            {
                Projectile.ResetLocalNPCHitImmunity();
                Projectile.numHits = 0;
            }
            if (time == Projectile.MaxUpdates) Projectile.ForceNetUpdate();
            time++;
        }

        public override bool? CanDamage() => Projectile.Opacity > 0.065f ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits == 0)
            {
                SoundEngine.PlaySound(Lucilus.hit4 with { Volume = 0.75f, PitchVariance = 0.4f, MaxInstances = 6 }, Projectile.Center);
            }

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1f;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = ModContent.Request<Texture2D>("asuw/Content/Projectiles/GBFstuff/Lusilly2", AssetRequestMode.AsyncLoad).Value;
            Vector2 start = Projectile.Center + (Vector2.UnitX * (texture.Width * -Projectile.scale));
            Vector2 end = Projectile.Center + (Vector2.UnitX * (texture.Width * Projectile.scale));
            float collisionPoint = 0f;
            float collisionWidth = (texture.Width * 2) * Projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
           
            Texture2D texture = ModContent.Request<Texture2D>("asuw/Content/Projectiles/GBFstuff/Lusilly3", AssetRequestMode.AsyncLoad).Value;
            Texture2D texture2 = ModContent.Request<Texture2D>("asuw/Content/Projectiles/GBFstuff/Lusilly2Flip", AssetRequestMode.AsyncLoad).Value;
            Texture2D textureGlow = ModContent.Request<Texture2D>("asuw/Content/Projectiles/GBFstuff/Lusilly3Glow", AssetRequestMode.AsyncLoad).Value;
            Texture2D texture2Glow = ModContent.Request<Texture2D>("asuw/Content/Projectiles/GBFstuff/Lusilly2GlowFlip", AssetRequestMode.AsyncLoad).Value;

            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/SplitTrail", AssetRequestMode.ImmediateLoad).Value;

            //inner1
            List<ColoredVertex> ve1 = new List<ColoredVertex>(); //2
            List<ColoredVertex> ve2 = new List<ColoredVertex>(); //3

            ////inner2
            //List<ColoredVertex> ve12 = new List<ColoredVertex>(); //2
            //List<ColoredVertex> ve22 = new List<ColoredVertex>(); //3

            ////outer
            //List<ColoredVertex> ve3 = new List<ColoredVertex>(); //2
            //List<ColoredVertex> ve4 = new List<ColoredVertex>(); //3



            for (int i = 0; i < odr.Count; i++)
            {
                Color a = Color.Snow;
                //inner
                ve1.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture.Width) * ods[i])),
                      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                      a));
                ve1.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture.Width / 9f) * ods[i])),
                      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                      a));

                ve2.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * (texture2.Width * ods[i]) * -1f),
                      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                      a));
                ve2.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture2.Width / 16f) * ods[i]) * -1),
                      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                      a));

                //ve12.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture.Width) * ods[i])),
                //      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                //      a));
                //ve12.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture.Width / 9f) * ods[i])),
                //      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                //      a));

                //ve22.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * (texture2.Width * ods[i]) * -1f),
                //      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                //      a));
                //ve22.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture2.Width / 16f) * ods[i]) * -1),
                //      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                //      a));

                ////outer
                //ve3.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture.Width * 1.07f) * ods[i])),
                //      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                //      a));
                //ve3.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture2.Width * 0.92f) * ods[i])),
                //      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                //      a));

                //ve4.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture2.Width * 1.07f) * ods[i]) * -1f),
                //      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                //      a));
                //ve4.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2() * ((texture2.Width * 0.92f) * ods[i]) * -1),
                //      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                //      a));
            }

            if (ve1.Count >= 3 && ve2.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                Effect shader = ModContent.Request<Effect>("asuw/Effects/ColorizeBloom",  AssetRequestMode.ImmediateLoad).Value; // Used To Be Colorize
                //Effect shader2 = ModContent.Request<Effect>("asuw/Effects/ColorizeVoid", AssetRequestMode.ImmediateLoad).Value;
                //Effect shader3 = ModContent.Request<Effect>("asuw/Effects/ColorizeBloom", AssetRequestMode.ImmediateLoad).Value;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue((Color.Red).ToVector4());
                shader.Parameters["color1"].SetValue((Color.Maroon).ToVector4());
                shader.Parameters["alpha"].SetValue(Projectile.Opacity);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();

                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve1.ToArray(), 0, ve1.Count - 2);
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve2.ToArray(), 0, ve2.Count - 2);

                //shader2.Parameters["color2"].SetValue((Color.Red).ToVector4());
                //shader2.Parameters["color1"].SetValue((Color.Lerp(Color.Brown, Color.Tomato, 0.25f)).ToVector4());
                //shader2.Parameters["alpha"].SetValue(Projectile.Opacity);
                //shader2.CurrentTechnique.Passes["EffectPass"].Apply();

                //trail = ModContent.Request<Texture2D>("asuw/Assets/MotionTrail2", AssetRequestMode.ImmediateLoad).Value;
                //gd.Textures[0] = trail;
                //gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve12.ToArray(), 0, ve12.Count - 2);
                //gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve22.ToArray(), 0, ve22.Count - 2);

                //shader3.Parameters["color2"].SetValue((Color.Red).ToVector4());
                //shader3.Parameters["color1"].SetValue((Color.Lerp(Color.Brown, Color.Tomato, 0.25f)).ToVector4());
                //shader3.Parameters["alpha"].SetValue(Projectile.Opacity);
                //shader3.CurrentTechnique.Passes["EffectPass"].Apply();

                //trail = ModContent.Request<Texture2D>("asuw/Assets/Trail", AssetRequestMode.ImmediateLoad).Value;
                //gd.Textures[0] = trail;
                //gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve3.ToArray(), 0, ve3.Count - 2);
                //gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve4.ToArray(), 0, ve4.Count - 2);

                Main.spriteBatch.ExitShaderRegion();
            }

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(textureGlow, Projectile.Center - Main.screenPosition, default, Color.Red * Projectile.Opacity, Projectile.rotation, new Vector2(0, textureGlow.Height / 2f), Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture2Glow, Projectile.Center - Main.screenPosition, default, Color.Red * Projectile.Opacity, Projectile.rotation, new Vector2(texture2Glow.Width, texture2Glow.Height / 2f), Projectile.scale, randRot == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity, Projectile.rotation, new Vector2(0, texture.Height / 2f), Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture2, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity, Projectile.rotation, new Vector2(texture2.Width, texture2.Height / 2f), Projectile.scale, randRot == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);


            return false;
        }
    }
}
