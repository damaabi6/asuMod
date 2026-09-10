using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;


namespace asuw.Content.Dusts
{
    public class ExplosionSpark : ModDust
    {
        public static Asset<Texture2D> tex1 { get; private set; }
        public static Asset<Texture2D> tex2 { get; private set; }
        int texture = 0;
        public override string Texture => "asuw/Assets/Blank";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                tex1 = ModContent.Request<Texture2D>("asuw/Assets/Boom1");
                tex2 = ModContent.Request<Texture2D>("asuw/Assets/Boom2");
            }
        }

        public override void OnSpawn(Dust dust)
        {
            texture = Main.rand.NextBool() ? 0 : 1;
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
            Texture2D tex = (texture == 0 ? tex1.Value : tex2.Value);
            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, dust.scale * 0.5f, SpriteEffects.None, 0f);
            if (!dust.noLight)
            {
                for (int i = 0; i < 2; i++)
                    Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.White, 0.3f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, new Vector2(1.45f, 1f) * dust.scale * 0.25f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
