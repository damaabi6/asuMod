using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace asuw.Effects
{
    public sealed class asuwShaders : ModSystem
    {
        public override void PostSetupContent()
        {
            Asset<Effect> trailForward = Mod.Assets.Request<Effect>("Effects/TrailForward");
            Asset<Effect> trailBackward = Mod.Assets.Request<Effect>("Effects/TrailBackward");
            Asset<Effect> arisuBlast = Mod.Assets.Request<Effect>("Effects/ArisuBlast");
            Asset<Effect> arisuBlastEdge = Mod.Assets.Request<Effect>("Effects/ArisuBlastEdge");
            Asset<Effect> slashForward = Mod.Assets.Request<Effect>("Effects/SlashForward");


            GameShaders.Misc["asuw:TrailForward"] = new MiscShaderData(trailForward, "TrailPass");
            GameShaders.Misc["asuw:TrailBackward"] = new MiscShaderData(trailBackward, "TrailPass");
            GameShaders.Misc["asuw:ArisuBlast"] = new MiscShaderData(arisuBlast, "TrailPass");
            GameShaders.Misc["asuw:ArisuBlastEdge"] = new MiscShaderData(arisuBlastEdge, "TrailPass");
            GameShaders.Misc["asuw:SlashForward"] = new MiscShaderData(slashForward, "TrailPass");


        }
    }
}
