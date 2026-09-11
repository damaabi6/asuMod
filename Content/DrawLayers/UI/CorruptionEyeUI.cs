using asuw.Content.Items.Accesories;
using asuw.Content.Items.Accesories.Wings;
using asuw.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace asuw.Content.DrawLayers.UI
{
    public static class CorruptionEyeUI
    {
        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (!player.active || player.dead || !player.asuw().CorruptionEyeEquipped)
                return;

            //textures
            Texture2D frame = ModContent.Request<Texture2D>("asuw/Content/Textures/Chargedowns/CorruptionBarBorder", AssetRequestMode.AsyncLoad).Value;
            Texture2D bar = ModContent.Request<Texture2D>("asuw/Content/Textures/Chargedowns/CorruptionBar", AssetRequestMode.AsyncLoad).Value;

            float barAmount = player.asuw().CorruptionEyeChargeAmnt / CorruptionEye.maxCharge;

            //bar amount display
            Vector2 barStretch = new Vector2(barAmount, 1f);
            //Pos
            Vector2 frameCenter = new Vector2(Main.screenWidth - Main.UIScale * 330f, Main.screenHeight - Main.UIScale * 180f) + frame.Size() * 0.5f;
            Vector2 barCenter = frameCenter + new Vector2(60 * Main.UIScale,0);
            Vector2 barOrigin = new Vector2(bar.Width, bar.Height);
            //Vector2 barCenter = frameCenter - (Vector2.UnitX * (100f + (player.asuw().CorruptionEyeChargeAmnt * 0.5f)));
            //bar color
            Color barcol = AsuPlayer.SpecialMoveColorCE;
            Color barcol2 = Color.Lerp(Color.Indigo,Color.Black, 0.68f);

            //barBG
            spriteBatch.Draw(bar, barCenter, null, barcol2, 0f, barOrigin, Main.UIScale, 0, 0f);
            //bar
            spriteBatch.Draw(bar, barCenter, null, barcol, 0f, barOrigin, Main.UIScale * barStretch, 0, 0f);
            //frame
            spriteBatch.Draw(frame, frameCenter, null, Color.White, 0f, frame.Size() * 0.5f, Main.UIScale, 0, 0f);


        }
    }
}
