using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace asuw.Content.Dusts
{
    public class StarBurst : ModDust
    {
        public static Asset<Texture2D> Star { get; private set; }
        public override string Texture => "asuw/Assets/Blank";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Star = ModContent.Request<Texture2D>("asuw/Assets/Particles/Star");
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
            for (int i = 0; i < 2; i++)
                Main.spriteBatch.Draw(Star.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, Star.Size() * 0.5f, dust.scale * 0.5f, SpriteEffects.None, 0f);
            if (!dust.noLight)
            {
                for (int i = 0; i < 2; i++)
                    Main.spriteBatch.Draw(Star.Value, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.White, 0.67f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, Star.Size() * 0.5f, dust.scale * 0.3f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }

 
}
