using asuw.Content.Items.Accesories.Wings;
using asuw.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace asuw.Content.UI
{
    public static class BulletsUI
    {
        //TODO addd more weapons, add second UI element function
        public static void Draw()
        {
            Player player = Main.LocalPlayer;

            if (player.active && !player.dead && player.HeldItem.ModItem != null)
            {
                if (player.HeldItem.ModItem is Hotshot hot)
                {
                    drawDatTing(hot.currentMag.ToString());
                    Vector2 bodyguardCountPos = new Vector2(Main.screenWidth * 0.5f - Main.UIScale * 40f, Main.screenHeight * 0.5f + Main.UIScale * 40f);
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, hot.perfectBodyguard.ToString(), bodyguardCountPos, Color.DodgerBlue, 0f, Vector2.Zero, Vector2.One * Main.UIScale);
                }
            }
        }

        static void drawDatTing(string Ammo) 
        {
            Texture2D bullets = ModContent.Request<Texture2D>("asuw/Content/Textures/bullets", AssetRequestMode.AsyncLoad).Value;
            //text Value
            string ammo = Ammo;
            //icon Pos
            Vector2 bulletIconCenter = new Vector2(Main.screenWidth * 0.5f - Main.UIScale * 60f,
            Main.screenHeight * 0.5f + Main.UIScale * 45f) + bullets.Size() * 0.5f;
            //text Pos
            Vector2 bulletsTextArea = FontAssets.MouseText.Value.MeasureString(ammo);
            Vector2 bulletsTextDrawPosition = bulletIconCenter + new Vector2(6f, 16f) - bulletsTextArea * 0.5f;

            // Draw the icon.
            Main.spriteBatch.Draw(bullets, bulletIconCenter, null, Color.White, 0f, bullets.Size() * 0.5f, Main.UIScale * 0.9f, 0, 0f);

            // Draw the current ammo
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, ammo, bulletsTextDrawPosition, Color.DarkOrange, 0f, Vector2.Zero, Vector2.One * Main.UIScale);
        }
    }
}
