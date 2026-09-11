using asuw.Content.Items.Accesories.Wings;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace asuw.Content.DrawLayers.UI
{
    public static class MoonveilFPUI
    {
        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (!player.active || player.dead || player.HeldItem.type != ModContent.ItemType<Moonveil>())
                return;

            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Moonveil moon)
            {
                if (moon.FP < 100)
                {
                    var barBG = ModContent.Request<Texture2D>("asuw/Assets/UIElements/GenericBarBack").Value;
                    var barFG = ModContent.Request<Texture2D>("asuw/Assets/UIElements/GenericBarFront").Value;

                    float lerp = moon.FP / 100f;
                    Vector2 drawPos = player.Center - Main.screenPosition + new Vector2(0, -36) - barBG.Size() / 2;
                    Rectangle frame = new Rectangle(0, 0, (int)(lerp * barFG.Width), barFG.Height);

                    float opacity = 1f;
                    Color color = Color.Lerp(Color.MidnightBlue, Color.DodgerBlue, lerp);

                    Main.spriteBatch.Draw(barBG, drawPos, color * opacity);
                    Main.spriteBatch.Draw(barFG, drawPos, frame, color * opacity * 0.8f);
                    
                }
            }
        }
    }
}
