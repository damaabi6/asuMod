using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Cooldown
{
    public abstract class CoolDown
    {
        public int id;

        public string texture;

        public int tick;

        public int duration;

        public Color color;

        public abstract void Update();
        public abstract bool DrawCondition();

    }

    public static class CoolDownID
    {
        public static readonly int SolemnLamentCD = 1;
        public static readonly int BlackBladeCD = 2;
    }
}
