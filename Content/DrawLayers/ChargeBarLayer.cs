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
    public class ChargeBarLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => PlayerDrawLayers.AfterLastVanillaLayer;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem != null && drawInfo.shadow == 0;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            if (player.dead || player.HeldItem == null)
                return;

            Color lightColor = Lighting.GetColor((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), Color.White);
            Color color =  lightColor * (1f - drawInfo.shadow);

            if (player.HeldItem.type == ModContent.ItemType<SOL>())
            {
                SOL.DrawBar(ref drawInfo);
            }

        }
    }
}

