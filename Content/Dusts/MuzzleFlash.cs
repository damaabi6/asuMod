using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;


namespace asuw.Content.Dusts
{
    public class MuzzleFlash : ModDust
    {
        public override string Texture => "asuw/Assets/Blank";
        float spawnScale = 0;
        public override void OnSpawn(Dust dust)
        {
            spawnScale = dust.scale;
        }
        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);

            dust.scale += 0.03f * fadeSpeed;
            dust.alpha += (int)(15 * fadeSpeed);

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            int texIndex = Main.rand.Next(1, 3);
            Texture2D tex = ModContent.Request<Texture2D>($"asuw/Assets/Particles/Muzzle{texIndex}").Value;
            float scaleLerp = MathHelper.Lerp(1, 0, dust.scale / spawnScale);
            Vector2 squish = new Vector2(1, scaleLerp);
            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, spawnScale * squish, SpriteEffects.None, 0f);
            if (!dust.noLight)
            {
                for (int i = 0; i < 2; i++)
                    Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.White, 0.3f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, spawnScale * squish, SpriteEffects.None, 0f);
            }
            return false;
        }
    }

    public class MuzzleFlashNP : ModDust
    {
        public override string Texture => "asuw/Assets/Blank";
        float spawnScale = 0;
        int dustIndex = 1;
        bool flip = false;
        public override void OnSpawn(Dust dust)
        {
            dustIndex = Main.rand.Next(1, 3);
            spawnScale = dust.scale;
            flip = Main.rand.NextBool();
        }
        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
            dust.velocity *= 0.975f;
            dust.scale -= 0.067f * fadeSpeed;

            if (dust.scale < 0)
                dust.active = false;

            dust.position += dust.velocity;

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = ModContent.Request<Texture2D>($"asuw/Assets/Particles/Muzzle{dustIndex}").Value;
            float scaleLerp = MathHelper.Lerp(0, 1, dust.scale / spawnScale);
            Vector2 squish = new Vector2(1 - (scaleLerp * 0.4f), scaleLerp);
            Vector2 origin = new Vector2(0, tex.Height / 2f);
            ShaderFunctions.vertexColored(Main.spriteBatch, dust.color, dust.color, Utils.GetLerpValue(255, 0, dust.alpha));
            for (int i = 0; i < 2; i++)
                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.White * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), origin, spawnScale * squish, flip ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
