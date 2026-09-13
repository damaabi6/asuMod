using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;


namespace asuw.Content.Dusts
{
    public class Explosion1 : ModDust
    {
        public static Asset<Texture2D> tex { get; private set; }
        public override string Texture => "asuw/Assets/Blank";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                tex = ModContent.Request<Texture2D>("asuw/Assets/Particles/Explosion1");
            }
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
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, dust.scale * 0.5f, SpriteEffects.None, 0f);
            if (!dust.noLight)
            {
                for (int i = 0; i < 2; i++)
                    Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.White, 0.3f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, dust.scale * 0.4f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }

    public class ShatteredExplosion : ModDust
    {
        public static Asset<Texture2D> tex { get; private set; }
        public override string Texture => "asuw/Assets/Blank";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                tex = ModContent.Request<Texture2D>("asuw/Assets/Particles/ShatteredExplosion");
            }
        }
        public override void OnSpawn(Dust dust)
        {
            dust.rotation = AsuUtils.randomRot();
        }
        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);

            dust.scale *= 1f + (0.15f * Utils.GetLerpValue(1, 0, AsuUtils.QuadOut((float)dust.alpha / 255f)));
            dust.alpha += (int)(15 * fadeSpeed);

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, tex.Size() * 0.5f, dust.scale * 0.5f, SpriteEffects.None, 0f);
            if (!dust.noLight)
            {
                for (int i = 0; i < 2; i++)
                    Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.White, 0.3f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, tex.Size() * 0.5f, dust.scale * 0.4f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }

    public class ShatteredExplosionNP : ModDust
    {
        public static Asset<Texture2D> tex { get; private set; }
        public override string Texture => "asuw/Assets/Blank";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                tex = ModContent.Request<Texture2D>("asuw/Assets/Particles/ShatteredExplosion");
            }
        }
        public override void OnSpawn(Dust dust)
        {
            dust.rotation = AsuUtils.randomRot();
        }
        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);

            dust.scale *= 1f + (0.15f * Utils.GetLerpValue(1, 0, AsuUtils.QuadOut((float)dust.alpha / 255f)));
            dust.alpha += (int)(15 * fadeSpeed);

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            ShaderFunctions.vertexColored(Main.spriteBatch, dust.color, dust.color, Utils.GetLerpValue(255, 0, dust.alpha));
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, null, Color.White * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, tex.Size() * 0.5f, dust.scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
