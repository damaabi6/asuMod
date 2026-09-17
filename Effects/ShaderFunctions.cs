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

        public static ColoredVertex[] Rectanglevertex(Vector2 center, float width, float height, float originOffsetX = 0.5f, float originOffsetY = 0.5f, bool adjustToScreenPos = true, bool useTransfromationMatrix = false)
        {
            originOffsetX = Math.Clamp(originOffsetX, 0, 1);
            originOffsetY = Math.Clamp(originOffsetY, 0, 1);
            Vector2 zoom = useTransfromationMatrix ? Main.GameViewMatrix.Zoom : Vector2.One;
            Vector2 screenCenter = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            List<ColoredVertex> rect = new List<ColoredVertex>();
            for (int j = 0; j < 20; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    Vector2 posX = (center - (adjustToScreenPos ? Main.screenPosition : Vector2.Zero))
                    - Vector2.UnitX * (width * originOffsetX)
                    + Vector2.UnitX * (width / 19f) * j;

                    float vOffset = (k == 0) ? -height * originOffsetY : height * (1f - originOffsetY);
                    Vector2 posY = posX + Vector2.UnitY * vOffset;

                    Vector2 finalPos = screenCenter + (posY - screenCenter) * zoom;

                    Vector3 coords = new Vector3(j / (20f - 1f), k, 1);
                    rect.Add(new ColoredVertex(finalPos, coords, Color.AliceBlue));
                }
            }
            return rect.ToArray();
        }
        public static void DrawRectangle(Vector2 center, float width, float height, Color color, float opacity, float originOffsetX = 0.5f, float originOffsetY = 0.5f, string overrideTexture = "", bool applyColoring = true, bool adjustToScreenPos = true, bool useTransfromationMatrix = false)
        {
            ColoredVertex[] rect = Rectanglevertex(center, width, height, originOffsetX, originOffsetY, adjustToScreenPos, useTransfromationMatrix);
            string texPath = "asuw/Assets/White";
            if (!string.IsNullOrEmpty(overrideTexture))
                texPath = overrideTexture;
            Texture2D tex = ModContent.Request<Texture2D>(texPath,AssetRequestMode.AsyncLoad).Value;
            if (applyColoring)
                vertexColored(Main.spriteBatch, color, color, opacity);
            else
                SimpleDraw(Main.spriteBatch, opacity);
            Main.graphics.GraphicsDevice.Textures[0] = tex;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, rect.ToArray(), 0, rect.Length - 2);
            Main.spriteBatch.ExitShaderRegion();
        }

        public static void DrawFancyNumbers(Vector2 pos, int num, Color color, float scale, float opacity = 1)
        {
            //font is evantic by sign studio on dafont
            num = Math.Abs(num);
       
            int[] digits = num.ToString()
                        .Select(c => c - '0')
                        .ToArray();

            for (int i = 0; i < digits.Length; i++)
            {
                Texture2D numTex = ModContent.Request<Texture2D>($"asuw/Assets/UIElements/FancyNumbers/FancyNumber{digits[i]}", AssetRequestMode.ImmediateLoad).Value;
                float digitWidth = numTex.Width * scale;
                float xOffset = (i - (digits.Length - 1) / 2f) * digitWidth;
                Vector2 finalPos = pos + Vector2.UnitX * xOffset;
                Vector2 firstDigitIsOneOffset = (digits[0] == 1 && digits.Length > 1) ? Vector2.UnitX * -(digitWidth / (6 + digits.Length)) : Vector2.Zero; // i hate this
                vertexColored(Main.spriteBatch, color, color, 1);
                Main.spriteBatch.Draw(numTex, finalPos - Main.screenPosition + firstDigitIsOneOffset, null, Color.White * opacity, 0, numTex.Size() / 2f, scale, SpriteEffects.None, 0);
                Main.spriteBatch.ExitShaderRegion();
            }
        }
        public static void SimpleDraw(SpriteBatch spriteBatch, float opacity)
        {
            spriteBatch.EnterShaderRegion();
            Effect shader = ModContent.Request<Effect>("asuw/Effects/SimplyDraw", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            shader.Parameters["alpha"].SetValue(opacity);
            shader.CurrentTechnique.Passes["EffectPass"].Apply();
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
