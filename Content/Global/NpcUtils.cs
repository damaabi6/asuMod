using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace asuw.Content
{
    public static partial class AsuUtils
    {
        public static int TryGetNPC(int type)
        {
            foreach (var index in Main.ActiveNPCs)
            {
                if (index.type == type)
                    return index.whoAmI;
            }
            return -1;
        }
        public static bool IsAnEnemy(this NPC npc, bool allowStatues = true, bool checkDead = true, bool checkDamage = true)
        {
            if (npc == null || (!npc.active && (!checkDead || npc.life > 0)) || npc.townNPC || npc.friendly)
            {
                return false;
            }

            if (!allowStatues && npc.SpawnedFromStatue)
            {
                return false;
            }

            if (npc.lifeMax <= 5 || (npc.defDamage <= 5 && checkDamage && npc.lifeMax <= 3000))
            {
                return false;
            }

            return true;
        }
        public static bool IsABoss(this NPC npc)
        {
            if (npc == null || !npc.active)
            {
                return false;
            }

            if (npc.boss && npc.type != 395)
            {
                return true;
            }

            if (npc.type == 14 || npc.type == 13 || npc.type == 15)
            {
                return true;
            }

            return true;
        }
    }
}
