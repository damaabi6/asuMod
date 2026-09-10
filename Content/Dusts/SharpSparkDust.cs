using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;


namespace asuw.Content.Dusts
{
    public class SharpSparkDust : ModDust
    {
        public static Asset<Texture2D> Outer { get; private set; }
        public static Asset<Texture2D> Inner { get; private set; }
        public override string Texture => "asuw/Assets/Blank";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Outer = ModContent.Request<Texture2D>("asuw/Assets/SharpSparkOuter");
                Inner = ModContent.Request<Texture2D>("asuw/Assets/SharpSparkInner");
            }
        }

        public override void OnSpawn(Dust dust)
        {
            dust.scale *= Main.rand.NextFloat(0.8f, 1f);
        }

        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
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

            Main.spriteBatch.Draw(Outer.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), Outer.Size() * 0.5f, dust.scale * 0.5f, SpriteEffects.None, 0f);
            if (!dust.noLight)
            {
                for (int i = 0; i < 2; i++)
                    Main.spriteBatch.Draw(Inner.Value, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.White, 0.3f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), Inner.Size() * 0.5f, new Vector2(1.45f, 1f) * dust.scale * 0.25f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
