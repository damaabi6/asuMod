using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;


namespace asuw.Content.Dusts
{
    public class LightRing : ModDust
    {
        public static Asset<Texture2D> tex { get; private set; }

        public override string Texture => "asuw/Assets/Blank";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                tex = ModContent.Request<Texture2D>("asuw/Assets/Particles/LightRing");
            }
        }

        public override void OnSpawn(Dust dust)
        {
            dust.scale *= 0.5f;
        }

        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
            dust.velocity *= 0.96f * (1f - dust.fadeIn);
            if (dust.noGravity)
                dust.scale += 0.045f * fadeSpeed;
            else
            {
                dust.scale += 0.03f * fadeSpeed;
                dust.velocity.Y += Main.rand.NextFloat(0.1f, 0.35f) * fadeSpeed;
            }
            dust.alpha += 17;
            float light = MathHelper.Clamp(dust.scale * 0.8f, 0f, 1f);
            if (!dust.noLightEmittence)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * light);

            if (dust.alpha >= 255)
                dust.active = false;

            dust.position += dust.velocity;

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Vector2 squish = new Vector2(0.3f, 1f);
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, dust.scale * 0.1f * squish, SpriteEffects.None, 0f);
            if (!dust.noLight)
            {
                for (int i = 0; i < 2; i++)
                    Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.White, 0.4f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, new Vector2(1.45f, 1f) * dust.scale * 0.09f * squish, SpriteEffects.None, 0f);
            }
            return false;
        }
    }

    public class LightRingNP : ModDust
    {
        public static Asset<Texture2D> tex { get; private set; }

        public override string Texture => "asuw/Assets/Blank";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                tex = ModContent.Request<Texture2D>("asuw/Assets/Particles/LightRing");
            }
        }

        public override void OnSpawn(Dust dust)
        {
            dust.scale *= 0.5f;
        }

        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
            dust.velocity *= 0.96f * (1f - dust.fadeIn);
            if (dust.noGravity)
                dust.scale += 0.045f * fadeSpeed;
            else
            {
                dust.scale += 0.03f * fadeSpeed;
                dust.velocity.Y += Main.rand.NextFloat(0.1f, 0.35f) * fadeSpeed;
            }
            dust.alpha += 17;
            float light = MathHelper.Clamp(dust.scale * 0.8f, 0f, 1f);
            if (!dust.noLightEmittence)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * light);

            if (dust.alpha >= 255)
                dust.active = false;

            dust.position += dust.velocity;

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Vector2 squish = new Vector2(0.3f, 1f);
            ShaderFunctions.vertexColored(Main.spriteBatch, dust.color, dust.color, Utils.GetLerpValue(255, 0, dust.alpha));
            for (int i = 0; i < 2; i++)
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, null, Color.White * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, dust.scale * 0.1f * squish, SpriteEffects.None, 0f);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
