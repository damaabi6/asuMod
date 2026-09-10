using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;


namespace asuw.Content.Dusts
{
    public class SquishDust : ModDust
    {
        public static Asset<Texture2D> Light { get; private set; }
        public override string Texture => "asuw/Assets/Blank";

        public override void Load()
        {
            if (!Main.dedServ)
            { 
                Light = ModContent.Request<Texture2D>("asuw/Assets/Particles/circle_05");
            }
        }

        Vector2 squish = Vector2.One;
        public override void OnSpawn(Dust dust)
        {
            dust.scale *= Main.rand.NextFloat(0.1f, 0.2f);
            squish.X = Main.rand.NextFloat(1.75f, 2f);
        }
        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
            dust.velocity *= 0.96f * fadeSpeed;
            dust.scale *= 0.96f * fadeSpeed;

            if (!dust.noGravity)
                dust.velocity.Y += Main.rand.NextFloat(0.1f, 0.35f) * fadeSpeed;


            if (squish.X > 1) squish.X *= 0.96f * fadeSpeed;
            else
                squish.X = 1;

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
            Vector2 DefSquish = new Vector2(2f, 0.3f);
            Main.spriteBatch.Draw(Light.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), Light.Size() * 0.5f, dust.scale * DefSquish * 0.5f * squish, SpriteEffects.None, 0f);
            if (!dust.noLight)
            {
                for (int i = 0; i < 2; i++)
                    Main.spriteBatch.Draw(Light.Value, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.White, 0.3f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.velocity.ToRotation(), Light.Size() * 0.5f, dust.scale * DefSquish * 0.3f * squish, SpriteEffects.None, 0f);
            }
            return false;
        }

        public static void drawBehindWings(ref PlayerDrawSet drawInfo)
        {

        }
    }
}
