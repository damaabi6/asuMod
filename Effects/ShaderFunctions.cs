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
            strength *= AsuConfig.Instance.ScreenEffectsPower;
            if (strength < float.Epsilon)
            {
                DisableScreenShader("asuw:ZoomBlur");
                return;
            }
            
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

        public static void AngleScreen(float angle)
        {
            angle *= AsuConfig.Instance.ScreenEffectsPower;
            if (MathF.Abs(angle) < float.Epsilon)
            {
                DisableScreenShader("asuw:AngleScreen");
                return;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                ActivateScreenShader("asuw:AngleScreen");
                var shader = Filters.Scene["asuw:AngleScreen"].GetShader();
                shader.Shader.Parameters["uRotation"].SetValue(angle);
            }
        }
        public static void ZoomScreen(float zoom, Vector2 worldPos)
        {
            zoom *= AsuConfig.Instance.ScreenEffectsPower;
            if (zoom < float.Epsilon)
            {
                DisableScreenShader("asuw:ZoomInto");
                return;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                ActivateScreenShader("asuw:ZoomInto");

                var shader = Filters.Scene["asuw:ZoomInto"].GetShader();

                // convert world position to normalized [0,1] screen UV
                Vector2 screenPos = worldPos - Main.screenPosition;
                Vector2 normalizedPos = new Vector2(
                    screenPos.X / Main.screenWidth,
                    screenPos.Y / Main.screenHeight
                );

                shader.Shader.Parameters["uZoomAmnt"].SetValue(zoom + 1);
                shader.Shader.Parameters["uZoomPoint"].SetValue(normalizedPos);
            }
        }
        public static void ActivateScreenShader(string shader, Vector2 pos)
        {
            if (Main.netMode != NetmodeID.Server && !Filters.Scene[shader].IsActive())
                Filters.Scene.Activate(shader, pos);
        }

        public static void ActivateScreenShader(string shader)
        {
            if (Main.netMode != NetmodeID.Server && !Filters.Scene[shader].IsActive())
                Filters.Scene.Activate(shader);
        }
        public static void DisableScreenShader(string shader)
        {
            if (Main.netMode != NetmodeID.Server && Filters.Scene[shader].IsActive())
                Filters.Scene[shader].Deactivate();
        }

        public static void DrawRectangle(Vector2 center, float width, float height, Color color, float opacity)
        {
            List<ColoredVertex> rect = new List<ColoredVertex>();
            for (int j = 0; j < 20; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    Vector2 posX = center - Main.screenPosition - Vector2.UnitX * (width / 2) + Vector2.UnitX * (width / 19f) * j;
                    Vector2 posY = posX - Vector2.UnitY * (height / 2) * (k == 0 ? 1 : -1);
                    Vector3 coords = new Vector3(j / (20f - 1f), k, 1);
                    rect.Add(new ColoredVertex(posY, coords, Color.AliceBlue));
                }
            }
            Texture2D tex = ModContent.Request<Texture2D>("asuw/Assets/White",AssetRequestMode.AsyncLoad).Value;
            vertexColored(Main.spriteBatch, color, color, opacity);
            Main.graphics.GraphicsDevice.Textures[0] = tex;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, rect.ToArray(), 0, rect.Count - 2);
            Main.spriteBatch.ExitShaderRegion();
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
