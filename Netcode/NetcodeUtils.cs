using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace asuw.Content
{
    public static partial class AsuUtils
    {
        public static bool IsNullOrInactive(this Entity entity)
        {
            if (entity is null) return true;
            if (!entity.active) return true;

            return false;
        }
        public static AsuPlayer ReadAsuPlayer(this BinaryReader reader, bool nullOnInactive = true)
        {
            return reader.ReadPlayer(nullOnInactive)?.asuw() ?? null;
        }

        public static void WriteWhoAmI(this BinaryWriter writer, ModPlayer player) => WriteWhoAmI(writer, player?.Player);
        public static void WriteWhoAmI(this BinaryWriter writer, Player player)
        {
            byte whoAmI = (byte)(player?.whoAmI ?? Main.maxPlayers);
            writer.Write(whoAmI);
        }

        public static void WriteWhoAmI(this BinaryWriter writer, ModNPC npc) => WriteWhoAmI(writer, npc?.NPC);
        public static void WriteWhoAmI(this BinaryWriter writer, NPC npc)
        {
            byte whoAmI = (byte)(npc?.whoAmI ?? Main.maxNPCs);
            writer.Write(whoAmI);
        }

        public static Player ReadPlayer(this BinaryReader reader, bool nullOnInactive = true)
        {
            int num = reader.ReadByte();
            if (num >= 255)
            {
                return null;
            }

            Player player = Main.player[num];
            if (nullOnInactive && player.IsNullOrInactive())
            {
                return null;
            }

            return player;
        }

        public static NPCType ReadModNPC<NPCType>(this BinaryReader reader, bool nullOnInactive = true) where NPCType : ModNPC => ReadNPC(reader, nullOnInactive)?.ModNPC as NPCType;
        public static ModNPC ReadModNPC(this BinaryReader reader, bool nullOnInactive = true) => ReadNPC(reader, nullOnInactive)?.ModNPC ?? null;
        public static NPC ReadNPC(this BinaryReader reader, bool nullOnInactive = true)
        {
            int index = reader.ReadByte();

            if (index >= Main.maxNPCs)
                return null;

            var npc = Main.npc[index];

            if (nullOnInactive && npc.IsNullOrInactive())
                return null;

            return npc;
        }
    }
}
