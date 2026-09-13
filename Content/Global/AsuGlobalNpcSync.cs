using asuw.Content.Buffs;
using asuw.Content.Global;
using asuw.Content.Items.Accesories;
using asuw.Content.Items.Weapons;
using asuw.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace asuw.Content
{
    public partial class AsuGlobalNPC : GlobalNPC
    {
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((short)npc.asuw().SinkingCounter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            npc.asuw().SinkingCounter = binaryReader.ReadInt16();
        }
    }
}
