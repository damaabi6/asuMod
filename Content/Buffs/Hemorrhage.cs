using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using asuw.Content.Global;

namespace asuw.Content.Buffs
{
    public class Hemorrhage : ModBuff
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
            npc.GetGlobalNPC<AsuGlobalNPC>().Hemorrhage = true;
            npc.GetGlobalNPC<AsuGlobalNPC>().Hemmorhage -= 1;

        }
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            npc.GetGlobalNPC<AsuGlobalNPC>().Hemmorhage += 20;
            return false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AsuPlayer>().Hemorrhage = true;
        }
    }
}
