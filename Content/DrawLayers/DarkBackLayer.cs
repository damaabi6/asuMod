using asuw.Content.Items.Accesories.Wings;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace asuw.Content.DrawLayers
{
    public class DarkBackLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => PlayerDrawLayers.BeforeFirstVanillaLayer;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.asuw().DarkBackLayerOP > 0;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            if (player.dead)
                return;

            Texture2D tex = ModContent.Request<Texture2D>("asuw/Assets/Black", AssetRequestMode.AsyncLoad).Value;

            DrawData layerdata = new DrawData(tex, player.MountedCenter - Main.screenPosition, null, Color.White * player.asuw().DarkBackLayerOP, 0, tex.Size() / 2f, 1000f, SpriteEffects.None, 0);

            if(Main.myPlayer == player.whoAmI)
            drawInfo.DrawDataCache.Add(layerdata);

        }
    }
}

