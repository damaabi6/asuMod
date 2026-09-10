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
    public class OffhandLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.OffhandAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem != null;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            if (player.dead || player.HeldItem == null)
                return;

            Color lightColor = Lighting.GetColor((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), Color.White);
            Color color =  lightColor * (1f - drawInfo.shadow);

            if (player.HeldItem.type == ModContent.ItemType<SolemnLament>() && player.ownedProjectileCounts[ModContent.ProjectileType<Solemn>()] > 0)
                Solemn.drawOffhand(ref drawInfo, player, color);

            if (player.HeldItem.type == ModContent.ItemType<Tsurugi>() && player.ownedProjectileCounts[ModContent.ProjectileType<GunpowderTsu>()] > 0)
                GunpowderTsu.drawOffhand(ref drawInfo, player, color);


        }
    }
}

