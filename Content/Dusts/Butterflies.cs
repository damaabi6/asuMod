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
        public static Asset<Texture2D> texF { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                tex = ModContent.Request<Texture2D>("asuw/Assets/Particles/ButterflyBlack");
                texF = ModContent.Request<Texture2D>("asuw/Assets/Particles/ButterflyBlackFlap");
            }
        }
        public override void OnSpawn(Dust dust)
        {
            dust.customData = Main.rand.NextBool() ? 1 : 0;
        }
        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
            dust.alpha += (int)(10 * fadeSpeed);

            if (dust.alpha >= 255)
                dust.active = false;

            dust.velocity *= 0.96f;
            dust.position += dust.velocity;

            if((int)dust.customData == 0)
            {
                if (Main.rand.NextBool(6))
                    dust.customData = 1;
            }
            else
            {
                if (Main.rand.NextBool(8))
                    dust.customData = 0;
            }

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Texture2D texture = (int)dust.customData == 0 ? texF.Value : tex.Value;
            Main.spriteBatch.Draw(texture, dust.position - Main.screenPosition, null, dust.color * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), texture.Size() * 0.5f, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class ButterflyWhite : ModDust
    {
        public override string Texture => "asuw/Assets/Blank";

        public static Asset<Texture2D> tex { get; private set; }
        public static Asset<Texture2D> texF { get; private set; }
        public override void Load()
        {
            if (!Main.dedServ)
            {
                tex = ModContent.Request<Texture2D>("asuw/Assets/Particles/ButterflyWhite");
                texF = ModContent.Request<Texture2D>("asuw/Assets/Particles/ButterflyWhiteFlap");
            }
        }
        public override void OnSpawn(Dust dust)
        {
            dust.customData = Main.rand.NextBool() ? 1 : 0;
        }
        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
            dust.alpha += (int)(10 * fadeSpeed);

            if (dust.alpha >= 255)
                dust.active = false;

            dust.velocity *= 0.96f;
            dust.position += dust.velocity;

            if ((int)dust.customData == 0)
            {
                if (Main.rand.NextBool(6))
                    dust.customData = 1;
            }
            else
            {
                if (Main.rand.NextBool(8))
                    dust.customData = 0;
            }

            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Texture2D texture = (int)dust.customData == 0 ? texF.Value : tex.Value;
            Main.spriteBatch.Draw(texture, dust.position - Main.screenPosition, null, dust.color * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), texture.Size() * 0.5f, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

}
