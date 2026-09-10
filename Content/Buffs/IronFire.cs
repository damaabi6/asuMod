using asuw.Content.Global;
using asuw.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Buffs
{
    public class IronFire : ModBuff
    {

        public float dmgstack;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;



        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.asuw().IronFlame = true;

        }

    }
}
