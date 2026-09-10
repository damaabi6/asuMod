using asuw.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace asuw.Packets
{
    internal sealed class RightClickSyncPacket : AsuPackets
    {
        public static RightClickSyncPacket Instance { get; private set; }

        public static void Send(AsuPlayer player, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write(player.mouseRight);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadAsuPlayer();
            var rightClick = packet.ReadBoolean();

            if (player is null)
                return;

            player.mouseRight = rightClick;

            if (Main.dedServ)
                Send(player, ignoreClient: sender);
        }
    }
}
