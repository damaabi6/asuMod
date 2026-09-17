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

namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {
        public float genericBarValue = 0;
        public Color colorBarF = Color.White;
        public Color ColorBarB = Color.White;
        public float barOP = 1;

        public void DrawBar()
        {
            if (!Player.active || Player.dead)
                return;

            var barBG = ModContent.Request<Texture2D>("asuw/Assets/UIElements/GenericBarBack").Value;
            var barFG = ModContent.Request<Texture2D>("asuw/Assets/UIElements/GenericBarFront").Value;

            genericBarValue = Math.Clamp(genericBarValue, 0, 1f);
            Vector2 drawPos = Player.MountedCenter - Main.screenPosition + new Vector2(0, -36) - barBG.Size() / 2;

            Rectangle frame = new Rectangle(0, 0, (int)(genericBarValue * barFG.Width), barFG.Height);

            Main.spriteBatch.EnterShaderRegion();
            Main.spriteBatch.Draw(barBG, drawPos, null, ColorBarB * barOP, 0, default, 1, 0, 0);
            Main.spriteBatch.Draw(barFG, drawPos, frame, colorBarF * barOP, 0, default, 1, 0, 0);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
