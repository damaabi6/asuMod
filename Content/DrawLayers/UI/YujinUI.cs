using asuw.Content.Items.Accesories.Wings;
using asuw.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace asuw.Content.DrawLayers.UI
{
    public static class YujinUI
    {
        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (!player.active || player.dead || player.HeldItem.type != ModContent.ItemType<Yujin>())
                return;

            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Yujin yu)
            {
                //textures
                Texture2D Chill = ModContent.Request<Texture2D>("asuw/Content/Textures/Chargedowns/EyeOfDeathChill", AssetRequestMode.AsyncLoad).Value;
                Texture2D ChillGlow = ModContent.Request<Texture2D>("asuw/Content/Textures/Chargedowns/EyeOfDeathChillGlow", AssetRequestMode.AsyncLoad).Value;
                Texture2D Mad = ModContent.Request<Texture2D>("asuw/Content/Textures/Chargedowns/EyeOfDeathMad", AssetRequestMode.AsyncLoad).Value;
                Texture2D MadGlow = ModContent.Request<Texture2D>("asuw/Content/Textures/Chargedowns/EyeOfDeathMadGlow", AssetRequestMode.AsyncLoad).Value;
                Texture2D TextBG = ModContent.Request<Texture2D>("asuw/Content/Textures/Chargedowns/EyeOfDeathTextBG", AssetRequestMode.AsyncLoad).Value;

                //text Value
                string eyeOfDeathText = Math.Abs((int)yu.ShiDMGInc).ToString();
                string healText = yu.heal.ToString();
                //icon Pos
                Vector2 eyeOfDeathIconCenter = new Vector2(Main.screenWidth * 0.5f - 40f,
               Main.screenHeight * 0.5f + Main.UIScale * 40f) + Chill.Size() * 0.5f;
                //text Pos
                Vector2 eyeOfDeathTextArea = FontAssets.MouseText.Value.MeasureString(eyeOfDeathText);
                Vector2 eyeOfDeathTextDrawPosition = eyeOfDeathIconCenter + new Vector2(0f, 20f * Main.UIScale);

                //Color
                Color textCol = yu.ShiDMGInc > 0 ? Color.Red : Color.IndianRed;

                //spriteBatch.Draw(TextBG, eyeOfDeathTextDrawPosition + (Vector2.UnitY * -2), null, Color.White * 0.2f, 0f, TextBG.Size() * 0.5f, Main.UIScale, 0, 0f);
                // Draw the icon.
                if (yu.ShiDMGInc <= 0)//chill af
                {
                    spriteBatch.Draw(ChillGlow, eyeOfDeathIconCenter, null, textCol, 0f, ChillGlow.Size() * 0.5f, Main.UIScale, 0, 0f);
                    spriteBatch.Draw(Chill, eyeOfDeathIconCenter, null, Color.White, 0f, Chill.Size() * 0.5f, Main.UIScale, 0, 0f);
                }
                else //she mad
                {
                    spriteBatch.Draw(MadGlow, eyeOfDeathIconCenter, null, textCol, 0f, MadGlow.Size() * 0.5f, Main.UIScale, 0, 0f);
                    spriteBatch.Draw(Mad, eyeOfDeathIconCenter, null, Color.White, 0f, Mad.Size() * 0.5f, Main.UIScale, 0, 0f);
                }
                // Draw the amount 
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, eyeOfDeathText, eyeOfDeathTextDrawPosition - eyeOfDeathTextArea * 0.5f + new Vector2(-1.5f,1), textCol, 0f, Vector2.Zero, Vector2.One * Main.UIScale * 1.2f);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, healText, eyeOfDeathTextDrawPosition - eyeOfDeathTextArea * 0.5f + new Vector2(-90f, 1), Color.Red, 0f, Vector2.Zero, Vector2.One * Main.UIScale * 1.2f);
            }
        }
    }
}
