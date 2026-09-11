using System.Collections.Generic;
using asuw.Content.DrawLayers.UI;
using asuw.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace asuw.Content.DrawLayers
{
    public class ModifyInterfacelayers : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            int entityMarkIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Entity Markers");
            //BulletsUI
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("BulletsUI", () =>
            {
                BulletsUI.Draw();
                return true;
            }, InterfaceScaleType.None));
            //Eye Of Horus
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("HorusUI", () =>
            {
                HorusUI.Draw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.None));
            //Saw Cleaver Rally
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("RallyUI", () =>
            {
                RallyUI.Draw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.None));
            //Eye Of Death yujin
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("YujinUI", () =>
            {
                YujinUI.Draw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.None));
            //CorruptionEye
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("CorruptionEyeUI", () =>
            {
                if (Main.EquipPage != 1 && Main.EquipPage != 2)
                    CorruptionEyeUI.Draw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.None));
            //StaffOfHoma
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("HomaUI", () =>
            {
                HomaUI.Draw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.None));
            //Moonveil
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("MoonveilFPUI", () =>
            {
                MoonveilFPUI.Draw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.None));
            //Homa Bloom Blossom
            foreach (var npcs in Main.ActiveNPCs)
            {
                if(npcs.HasBuff(ModContent.BuffType<Pyro>()))
                {
                    layers.Insert(entityMarkIndex, new LegacyGameInterfaceLayer("BloodBlossom", () =>
                    {
                        Pyro.drawBloom(npcs, npcs.asuw());
                        return true;
                    }, InterfaceScaleType.None));
                }
            }

        }
        
    }
}
