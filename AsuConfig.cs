
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace asuw
{
    [BackgroundColor(49, 32, 36, 216)]
    public class AsuConfig : ModConfig
    {
        public static AsuConfig Instance;

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        public bool DisableEnemyDebuffIcon { get; set; }
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        public bool DisableEnemyDebuffIconScaling { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 2f)]
        [DefaultValue(1f)]
        public float ScreenEffectsPower { get; set; }
    }
}
