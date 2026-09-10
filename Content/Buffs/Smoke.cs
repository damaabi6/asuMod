using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using asuw.Content.Global;

namespace asuw.Content.Buffs
{
    public class Smoke : ModBuff
    {

        
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<AsuGlobalNPC>().Smoke = true;
            

        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AsuPlayer>().Smoke = true;
        }

    }
}
