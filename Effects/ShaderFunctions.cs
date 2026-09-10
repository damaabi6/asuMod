using asuw.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Effects
{
    public static class ShaderFunctions
    {
        public static void ZoomBlur(Vector2 pos, float strength, float intensity, float maxDist = 0.3f)
        {
            if(strength < float.Epsilon)
                DisableScreenShader("asuw:ZoomBlur");
            
            if (Main.netMode != NetmodeID.Server)
            {
                ActivateScreenShader("asuw:ZoomBlur", pos);

                Filters.Scene["asuw:ZoomBlur"].GetShader()
                    .UseTargetPosition(pos)
                    .UseOpacity(strength)
                    .UseIntensity(intensity);
                Filters.Scene["asuw:ZoomBlur"].GetShader().Shader.Parameters["uMaxDist"].SetValue(maxDist);
            }
        }
        public static void ActivateScreenShader(string shader, Vector2 pos)
        {
            if (Main.netMode != NetmodeID.Server && !Filters.Scene[shader].IsActive())
                Filters.Scene.Activate(shader, pos);
        }
        public static void DisableScreenShader(string shader)
        {
            if (Main.netMode != NetmodeID.Server && Filters.Scene[shader].IsActive())
                Filters.Scene[shader].Deactivate();
        }

        public static void vertexTrail(SpriteBatch spriteBatch, Color colorDark, Color colorBright, float opacity, float fadeOut)
        {
            spriteBatch.EnterShaderRegion();
            Effect shader = ModContent.Request<Effect>("asuw/Effects/VertexTrail", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
            shader.Parameters["color2"].SetValue((colorBright).ToVector4());
            shader.Parameters["color1"].SetValue((colorDark).ToVector4());
            shader.Parameters["alpha"].SetValue(opacity);
            shader.Parameters["trailFade"].SetValue(fadeOut);
            shader.CurrentTechnique.Passes["EffectPass"].Apply();
        }

        public static void vertexColorBloom(SpriteBatch spriteBatch, Color colorDark, Color colorBright, float opacity, bool fade = false, float fadeAmount = 1)
        {
            if (fade)
            {
                spriteBatch.EnterShaderRegion();
                Effect shader = ModContent.Request<Effect>("asuw/Effects/ColorizeBloomFade", AssetRequestMode.ImmediateLoad).Value;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["trailFade"].SetValue(fadeAmount);
                shader.Parameters["color2"].SetValue((colorBright).ToVector4());
                shader.Parameters["color1"].SetValue((colorDark).ToVector4());
                shader.Parameters["alpha"].SetValue(opacity);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();
            }
            else
            {
                spriteBatch.EnterShaderRegion();
                Effect shader = ModContent.Request<Effect>("asuw/Effects/ColorizeBloom", AssetRequestMode.ImmediateLoad).Value;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue((colorBright).ToVector4());
                shader.Parameters["color1"].SetValue((colorDark).ToVector4());
                shader.Parameters["alpha"].SetValue(opacity);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();
            }
        }
        public static void vertexColored(SpriteBatch spriteBatch, Color colorDark, Color colorBright, float opacity)
        {
            spriteBatch.EnterShaderRegion();
            Effect shader = ModContent.Request<Effect>("asuw/Effects/Colorize", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            shader.Parameters["color2"].SetValue((colorBright).ToVector4());
            shader.Parameters["color1"].SetValue((colorDark).ToVector4());
            shader.Parameters["alpha"].SetValue(opacity);
            shader.CurrentTechnique.Passes["EffectPass"].Apply();
        }

        public static void vertexScanUp(SpriteBatch spriteBatch, Color colorDark, Color colorBright, float opacity, float progress, float scanHeight, bool bloom = true)
        {
            spriteBatch.EnterShaderRegion();
            Effect shader = ModContent.Request<Effect>("asuw/Effects/ScanUp", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            shader.Parameters["color2"].SetValue((colorBright).ToVector4());
            shader.Parameters["color1"].SetValue((colorDark).ToVector4());
            shader.Parameters["alpha"].SetValue(opacity);
            shader.Parameters["uProgress"].SetValue(progress);
            shader.Parameters["uScanHeight"].SetValue(scanHeight);
            shader.Parameters["bloom"].SetValue(bloom);
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            shader.CurrentTechnique.Passes["EffectPass"].Apply();
        }

        public static void vertexScrollUp(SpriteBatch spriteBatch, Color colorDark, Color colorBright, float opacity, bool bloom = true)
        {
            spriteBatch.EnterShaderRegion();
            Effect shader = ModContent.Request<Effect>("asuw/Effects/ScrollUp", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            shader.Parameters["color2"].SetValue((colorBright).ToVector4());
            shader.Parameters["color1"].SetValue((colorDark).ToVector4());
            shader.Parameters["alpha"].SetValue(opacity);
            shader.Parameters["bloom"].SetValue(bloom);
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            shader.CurrentTechnique.Passes["EffectPass"].Apply();
        }
    }
}
