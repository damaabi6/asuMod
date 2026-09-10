using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles.EOH
{
    public class Straighty : ModProjectile
    {
        
        
        public override string Texture => "asuw/Assets/Blank";

        float length = 9;
        int tex = 1;
        int shaderTipe = 1;
        float speedDec = 1;
        float width = 3;
        float startAt = 0;
        float duration = 60;
        Color colorBright;

        //trail parameters 
        List<float> odr = new List<float>();
        List<Vector2> odp = new List<Vector2>();

        ref float time => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 30;
            Projectile.MaxUpdates = 3;
        }
        public override void OnSpawn(IEntitySource source)
        {
            length = Main.rand.Next(18, 31);
            width *= Main.rand.NextFloat(1f, 3f);
            if (width > (3f * 1.5f)) tex = Main.rand.Next(2,4);
            if (width > (3f * 2.5f)) shaderTipe = 2;
            if (tex == 3) length /= 2;
            startAt = Main.rand.Next(13);
            speedDec = Main.rand.NextFloat(0.85f, 0.92f);
            duration *= Main.rand.NextFloat(0.9f, 1.1f);
            colorBright = Color.Lerp(Color.Yellow, Color.Goldenrod, Main.rand.NextFloat(0f, 1f));
            Projectile.timeLeft = (int)duration;
        }
        public override void AI()
        {
            Projectile.velocity *= speedDec;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (time > startAt)
            {
                odr.Add(Projectile.rotation);
                odp.Add(Projectile.Center);
            }

            if (odr.Count > length)
            {
                odr.RemoveAt(0);
                odp.RemoveAt(0);
            }
            time++;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float disBonus = time / duration;
            modifiers.FinalDamage += disBonus;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float hitbox = width * 10;
            Vector2 start = Projectile.Center + (Vector2.UnitX * (hitbox * -1));
            Vector2 end = Projectile.Center + (Vector2.UnitX * (hitbox));
            float collisionPoint = 0f;
            float collisionWidth = hitbox * 2;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<ColoredVertex> ve = new List<ColoredVertex>();
            string text = tex == 1 ? "asuw/Assets/LineHard" : tex == 2 ? "asuw/Assets/Streaky" : "asuw/Assets/Smear";
            Texture2D trail = ModContent.Request<Texture2D>(text, AssetRequestMode.ImmediateLoad).Value;

            for (int i = 0; i < odr.Count; i++)
            {
                Color a = Color.Snow;
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * width),
                      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                      a));
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odr[i].ToRotationVector2().RotatedBy(MathHelper.PiOver2) * width),
                      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                      a));
            }

            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                string textSH = shaderTipe == 1 ? "asuw/Effects/Colorize" : "asuw/Effects/ColorizeBloom";
                Effect shader = ModContent.Request<Effect>(textSH, AssetRequestMode.ImmediateLoad).Value; 
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue((colorBright).ToVector4());
                shader.Parameters["color1"].SetValue((Color.DarkOrange).ToVector4());
                shader.Parameters["alpha"].SetValue(Projectile.Opacity);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();

                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);


                Main.spriteBatch.ExitShaderRegion();
            }



                return false;
        }
    }
}
