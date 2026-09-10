using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;


namespace asuw.Content
{
    public static partial class AsuUtils
    {
        public static void EnterShaderRegion(this SpriteBatch spriteBatch, BlendState newBlendState = null, Effect effect = null, Matrix? matrix = null)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, newBlendState ?? BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, matrix ?? Main.GameViewMatrix.TransformationMatrix);
        }

        public static void ExitShaderRegion(this SpriteBatch spriteBatch, Matrix? matrix = null)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, matrix ?? Main.GameViewMatrix.TransformationMatrix);
        }

        public static MiscShaderData SetShaderTexture(this MiscShaderData shader, string texture, int index = 1)
        {
            switch (index)
            {
                case 0:
                    shader.UseImage0(ModContent.Request<Texture2D>(texture));
                    break;
                case 1:
                    shader.UseImage1(ModContent.Request<Texture2D>(texture));
                    break;
            }
            return shader;
        }

    }

    public struct ColoredVertex : IVertexType
    {
        public Vector2 Position;
        public Vector3 TexCoord; // XY = UV, Z = custom (e.g. opacity/weight)
        public Color Color;

        public static readonly VertexDeclaration VertexDeclaration = new(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
        public ColoredVertex(Vector2 position, Vector3 texCoord, Color color)
        {
            Position = position;
            TexCoord = texCoord;
            Color = color;
        }
    }
}
