using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.GameContent.NetModules;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.Net;
using Terraria.Utilities;


namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {
        public Vector2 mouseWorldDeltaFromPlayer;
        private Vector2 oldMouseWorldDeltaFromPlayer;
        public Vector2 mouseWorld => base.Player.MountedCenter + mouseWorldDeltaFromPlayer;
        public bool mouseWorldListener = false;

        public float mouseRotationFromPlayer;
        public bool mouseRotationListener = false;

        public bool mouseRight;
        private bool oldMouseRight = false;
        public bool rightClickListener = false;
        public Vector2 mouseNormalFromPlayer => mouseRotationFromPlayer.ToRotationVector2();

        public bool syncMousePosition = false;
        public bool syncMouseRotation = false;
        public bool syncMouseRightClick = false;

        private int mouseWorldPacketTimer = 0;
        private const int MouseWorldPacketInterval = 2;
       
    }
}
