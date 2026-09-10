using asuw.Content;
using asuw.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace asuw.Netcode
{
    internal sealed class ProjOwnerDataPacket : AsuPackets
    {
        public static ProjOwnerDataPacket Instance { get; private set; }

        public static void Send(AsuPlayer player, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write((byte)player.tracerBulletColor);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadAsuPlayer();
            var color = (int)packet.ReadByte();

            if (player is null)
                return;

            player.tracerBulletColor = color;

            if (Main.dedServ)
                Send(player, ignoreClient: sender);
        }
    }
}
