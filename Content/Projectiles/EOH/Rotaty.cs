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
    public class Rotaty : ModProjectile
    {
        float hitbox = 100;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center + (Vector2.UnitX * (hitbox * -1));
            Vector2 end = Projectile.Center + (Vector2.UnitX * (hitbox));
            float collisionPoint = 0f;
            float collisionWidth = hitbox * 2;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override string Texture => "asuw/Assets/Blank";

        float rotAmnt = 0;
        float length = 0;
        float odlength = 10;
        float lengthAmnt = 0;
        float width = 10;
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
            odlength = Main.rand.Next(18, 25);
            length = Projectile.velocity.Length();
            lengthAmnt = Main.rand.NextFloat(0.84f, 0.92f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            rotAmnt = Main.rand.NextFloat(0.001f, 0.03f) * (Main.rand.NextBool() ? 1 : -1);
            width += Main.rand.NextFloat(-4f, 4f);
            colorBright = Color.Lerp(Color.Yellow, Color.Goldenrod, Main.rand.NextFloat(0f, 1f));
            Projectile.timeLeft = (int)duration;
        }
        public override void AI()
        {
            Projectile.rotation += rotAmnt;
            Projectile.velocity = Projectile.rotation.ToRotationVector2() * length;
            length *= lengthAmnt;

            //float half = (duration / 2);
            //Projectile.Opacity = time <= half ? 1 : MathHelper.Lerp(1f, 0f, AsuUtils.ExpoIn((time - half) / half));

            odr.Add(Projectile.rotation);
            odp.Add(Projectile.Center);
            if (odr.Count > odlength)
            {
                odr.RemoveAt(0);
                odp.RemoveAt(0);
            }

            time++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<ColoredVertex> ve = new List<ColoredVertex>();
            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/Cone", AssetRequestMode.ImmediateLoad).Value;

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
                Effect shader = ModContent.Request<Effect>("asuw/Effects/ColorizeBloom", AssetRequestMode.ImmediateLoad).Value; 
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
