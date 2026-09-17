
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class asuw : Mod
	{
        internal static asuw Instance => _Instance ??= ModContent.GetInstance<asuw>();
        private static asuw _Instance;
        internal static ILog Log => Instance.Logger;

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                //progress = smoothness , intensity is... intensity
                Asset<Effect> vignetteShader = this.Assets.Request<Effect>("Effects/VignetteEffect");
                //intensityOnly
                Asset<Effect> corruptionTintShader = this.Assets.Request<Effect>("Effects/CorruptedTintEffect");
                Asset<Effect> ZoomBlur = this.Assets.Request<Effect>("Effects/ZoomBlur");
                Asset<Effect> AngleScreen = this.Assets.Request<Effect>("Effects/AngleScreen");
                Asset<Effect> ZoomTo = this.Assets.Request<Effect>("Effects/ZoomInto");

                Filters.Scene["asuw:VignetteEffect"] = new Filter(new ScreenShaderData(vignetteShader, "Pass1"), EffectPriority.Medium);
                Filters.Scene["asuw:CorruptedTint"] = new Filter(new ScreenShaderData(corruptionTintShader, "Pass1"), EffectPriority.High);
                Filters.Scene["asuw:ZoomBlur"] = new Filter(new ScreenShaderData(ZoomBlur, "EffectPass"), EffectPriority.Medium);
                Filters.Scene["asuw:AngleScreen"] = new Filter(new ScreenShaderData(AngleScreen, "EffectPass").UseColor(Color.White)
                .UseOpacity(1f), EffectPriority.VeryHigh);
                Filters.Scene["asuw:ZoomInto"] = new Filter(new ScreenShaderData(ZoomTo, "EffectPass").UseColor(Color.White)
                .UseOpacity(1f), EffectPriority.VeryHigh);

                Filters.Scene["asuw:VignetteEffect"].Load();
                Filters.Scene["asuw:CorruptedTint"].Load();
                Filters.Scene["asuw:ZoomBlur"].Load();
                Filters.Scene["asuw:AngleScreen"].Load();
                Filters.Scene["asuw:ZoomInto"].Load();

            }

        }
        
        public override void Unload()
        {
            
        }

        public override void PostSetupContent()
        {
           
        }
    }
}
