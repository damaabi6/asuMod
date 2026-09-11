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
    public static class HorusUI
    {
        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (!player.active || player.dead || player.HeldItem.type != ModContent.ItemType<Horus>())
                return;

            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Horus ho)
            {
                //textures
                Texture2D PistolCharge = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/HorusPistolCharged", AssetRequestMode.AsyncLoad).Value;
                Texture2D ShotgunCharge = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/HorusShotgunShell", AssetRequestMode.AsyncLoad).Value;
                Texture2D ShotgunCharged = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/HorusShotgunCharged", AssetRequestMode.AsyncLoad).Value;
                //text Value
                string pistolChargeText = ho.sideCharge.ToString();
                string shotgunChargeText = ho.mainCharge.ToString();
                //icon Pos
                Vector2 pistolChargeIconCenter = new Vector2(Main.screenWidth * 0.5f - Main.UIScale * 60f,
                Main.screenHeight * 0.5f + Main.UIScale * 40f) + PistolCharge.Size() * 0.5f;
                Vector2 shotgunChargeIconCenter = new Vector2(Main.screenWidth * 0.5f - Main.UIScale * 40f,
               Main.screenHeight * 0.5f + Main.UIScale * 40f) + ShotgunCharge.Size() * 0.5f;
                //text Pos
                Vector2 pistolChargeTextArea = FontAssets.MouseText.Value.MeasureString(pistolChargeText);
                Vector2 shotgunChargeTextArea = FontAssets.MouseText.Value.MeasureString(shotgunChargeText);
                Vector2 pistolChargeTextDrawPosition = pistolChargeIconCenter + new Vector2(6f, 16f) - pistolChargeTextArea * 0.5f;
                Vector2 shotgunChargeTextDrawPosition = shotgunChargeIconCenter + new Vector2(6f, 16f) - shotgunChargeTextArea * 0.5f;
                //shotgunChargedFrame
                Rectangle frameShotgun = ShotgunCharged.Frame(verticalFrames: 5, frameY: ho.ShotgunGlowFrame);
                //butterflyHairpin
                Color BH = AsuPlayer.SpecialMoveColorBH;

                // Draw the icon.
                spriteBatch.Draw(PistolCharge, pistolChargeIconCenter, null, Color.White, 0f,PistolCharge.Size() * 0.5f, Main.UIScale, 0, 0f);
                if (ho.mainCharge >= (player.asuw().butterflyHairpin ? 24 : 20)) spriteBatch.Draw(ShotgunCharged, shotgunChargeIconCenter, frameShotgun, Color.Pink, 0f, frameShotgun.Size() * 0.5f, Main.UIScale, 0, 0f);
                spriteBatch.Draw(ShotgunCharge, shotgunChargeIconCenter, null, Color.White, 0f, ShotgunCharge.Size() * 0.5f, Main.UIScale, 0, 0f);

                // Draw the charge amount 
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, pistolChargeText, pistolChargeTextDrawPosition, ho.sideCharge > 20 ? BH : Color.LightPink, 0f, Vector2.Zero, Vector2.One * Main.UIScale * 0.9f);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, shotgunChargeText, shotgunChargeTextDrawPosition,ho.mainCharge >= 20 && ho.mainCharge < 24 && player.asuw().butterflyHairpin ? BH : Color.DeepPink, 0f, Vector2.Zero, Vector2.One * Main.UIScale * 0.9f);
            }
        }
    }
}
