using asuw.Content.Items.Accesories.Wings;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace asuw.Content.UI
{
    public static class RallyUI
    {
        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (!player.active || player.dead || player.HeldItem.type != ModContent.ItemType<SawCleaver>())
                return;

            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SawCleaver saw && saw.rallyHeal > 0)
            {
                //text Value
                string healText = saw.rallyHeal.ToString();
                //textures
                Texture2D icon = ModContent.Request<Texture2D>("asuw/Assets/Rally", AssetRequestMode.AsyncLoad).Value;
                //Pos
                Vector2 pos = new Vector2(Main.screenWidth - Main.UIScale * (328f), Main.UIScale * 17f) + icon.Size() * 0.5f;
                Vector2 textArea = FontAssets.MouseText.Value.MeasureString(healText);
                Vector2 textDrawPosition = pos + new Vector2(6f, 16f) - textArea * 0.5f;

                // Draw
                spriteBatch.Draw(icon, pos, null, Color.White, 0f, icon.Size() * 0.5f, Main.UIScale, 0, 0f);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, healText, textDrawPosition, Color.Red, 0f, Vector2.Zero, Vector2.One * Main.UIScale * 0.7f);
              
            }
        }
    }
}
