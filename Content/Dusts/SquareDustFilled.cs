using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace asuw.Content.Dusts
{
    public class SquareDustFilled : ModDust
    {
        public static Asset<Texture2D> GlowSquareInner { get; private set; }

        public override string Texture => "asuw/Assets/Blank";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                GlowSquareInner = ModContent.Request<Texture2D>("asuw/Content/Textures/GlowSquareParticleThickFillInnerPart");
                
            }
        }

        public override void OnSpawn(Dust dust)
        {
            dust.scale *= Main.rand.NextFloat(0.8f, 1f);
            dust.rotation = Main.rand.NextFloat(-5, 5);
        }

        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
            float rotDir = Math.Sign(dust.rotation);
            dust.rotation += 0.04f * dust.scale * rotDir;
            dust.velocity *= 0.96f * fadeSpeed;
            if (dust.noGravity)
                dust.scale -= 0.045f * fadeSpeed;
            else
            {
                dust.scale -= 0.03f * fadeSpeed;
                dust.velocity.Y += Main.rand.NextFloat(0.1f, 0.35f) * fadeSpeed;
            }

            float light = MathHelper.Clamp(dust.scale * 0.8f, 0f, 1f);
            if (!dust.noLightEmittence)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * light);

            if (dust.scale <= 0)
                dust.active = false;

            dust.position += dust.velocity;

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Vector2 squash = Vector2.One;

            Main.spriteBatch.Draw(GlowSquareInner.Value, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.DimGray, 0.5f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, GlowSquareInner.Size() * 0.5f, squash * dust.scale * 0.1f, SpriteEffects.None, 0f);

            return false;
        }
    }

   
}
