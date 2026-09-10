using asuw.Content.Global;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Buffs
{
    public class DestinedDeath : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            int treshold = (int)((float)npc.lifeMax * (npc.IsABoss() ? 0.96f : 0.9f)); //4% to bosses, 10% everything else
            int HPdeletion = npc.life - treshold;
            if (npc.life > treshold && AsuUtils.sync())
            {
                npc.DamageEffectColored(HPdeletion, Color.Maroon);
                npc.life = treshold;    
            }
            npc.asuw().DestinedDeath = true;
        }
    }

    public class MalikethsDestinedDeath : ModBuff
    {
        public override string Texture => "asuw/Content/Buffs/DestinedDeath";

        public override void Update(NPC npc, ref int buffIndex)
        {
            int treshold = (int)((float)npc.lifeMax * (npc.IsABoss() ? 0.92f : 0.8f)); //8% to bosses, 20% everything else
            int HPdeletion = npc.life - treshold;
            if (npc.life > treshold && AsuUtils.sync())
            {
                npc.DamageEffectColored(HPdeletion, Color.Maroon);
                npc.life = treshold;
            }
            npc.GetGlobalNPC<AsuGlobalNPC>().DestinedDeath = true;
        }
    }
}
