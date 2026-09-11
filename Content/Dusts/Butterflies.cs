using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;


namespace asuw.Content.Dusts
{
    public class ButterflyBlack : ModDust
    {
        public override string Texture => "asuw/Assets/Blank";

        public static Asset<Texture2D> tex { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                tex = ModContent.Request<Texture2D>("asuw/Assets/Particles/ButterflyBlack");
            }
        }

        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
            dust.alpha += (int)(15 * fadeSpeed);

            if (dust.alpha >= 255)
                dust.active = false;

            dust.velocity *= 0.96f;
            dust.position += dust.velocity;

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, null, dust.color * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class ButterflyWhite : ModDust
    {
        public override string Texture => "asuw/Assets/Blank";

        public static Asset<Texture2D> tex { get; private set; }
        public override void Load()
        {
            if (!Main.dedServ)
            {
                tex = ModContent.Request<Texture2D>("asuw/Assets/Particles/ButterflyWhite");
            }
        }

        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
            dust.alpha += (int)(15 * fadeSpeed);

            if (dust.alpha >= 255)
                dust.active = false;

            dust.velocity *= 0.96f;
            dust.position += dust.velocity;

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, null, dust.color * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), tex.Size() * 0.5f, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

}
