using asuw.Content.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace asuw.Content.Items.Accesories
{
    public class ButterflyHairpin : ModItem
    {
        //TODO total application to all weapon skills, currently sucks
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplaceAll("ff00ff", Utils.Hex3(AsuPlayer.SpecialMoveColorBH));
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<ButterflyHairpinRare>();
            Item.maxStack = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AsuPlayer>().butterflyHairpin = true;
        }
    }
}