using asuw.Content.Global;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Buffs
{
    public class BlackBlood : ModBuff
    {

        //public static void BlackBloodNPCLifeRegen(NPC npc, int buffType, ref int buffIndex, ref int damage)
        //{
        //    int baseBanishingFireDoTValue = (int)npc.Calamity().ActiveHeatDebuffMultiplier.ApplyTo(npc.lifeMax / 100);
        //    npc.Calamity().ApplyDPSDebuff(baseBanishingFireDoTValue, baseBanishingFireDoTValue / 5, ref npc.lifeRegen, ref damage);
        //}
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<AsuGlobalNPC>().BlackBlood = true;
            npc.GetGlobalNPC<AsuGlobalNPC>().BBlood -= 5;

        }
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            npc.GetGlobalNPC<AsuGlobalNPC>().BBlood += 100;
            return false;
        }
        
    }
}
