using asuw.Netcode;
using asuw.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {
        internal const int GlobalSyncPacketTimer = 15;
        public int packetTimer = 0;
        internal void MouseRightClickSync()
        {
            RightClickSyncPacket.Send(this);
        }
        internal void MouseRotationSync()
        {
            MouseRotationSyncPacket.Send(this);
        }
        internal void MousePositionSync()
        {
            MousePositionSyncPacket.Send(this);
        }
        internal void ProjOwnerDataSync()
        {
            ProjOwnerDataPacket.Send(this);
        }
    }
}
