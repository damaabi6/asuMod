using asuw.Content.Buffs;
using asuw.Content.Dusts;
using asuw.Content.Global;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Items.Weapons.Ranged;
using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;


namespace asuw.Content
{
    public partial class AsuGlobalNPC : GlobalNPC
    {
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
           

            return true;
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            if (npc.type != NPCID.BrainofCthulhu && (npc.type != NPCID.DukeFishron || npc.ai[0] <= 9f) && npc.active)
            {
                List<Texture2D> currentDebuffs = new List<Texture2D>() { };
                AsuGlobalNPC anpc = npc.asuw();

                if (anpc.DestinedDeath)
                    currentDebuffs.Add(ModContent.Request<Texture2D>("asuw/Content/Buffs/DestinedDeath").Value);
                if (anpc.pyroAffected)
                    currentDebuffs.Add(ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/Pyro").Value);

                // Total amount of elements in the buff list
                int buffTextureListLength = currentDebuffs.Count;
                // Total length of a single row in the buff display
                int totalLength = buffTextureListLength * 18;
                // Max amount of buffs per row
                int buffDisplayRowLimit = 4;
                // The maximum length of a single row in the buff display
                // Limited to 80 units, because every buff drawn here is half the size of a normal buff, 16 x 16, 16 * 4 = 64 units
                float drawPosX = totalLength >= 72f ? 36f : (float)(totalLength / 2);
                // The height of a single frame of the npc
                float npcHeight = (npc.height * npc.scale) / 2;
                // Offset the debuff display based on the npc's graphical offset, and 16 units, to create some space between the sprite and the display
                float scale = AsuConfig.Instance.DisableEnemyDebuffIconScaling ? 1f : MathF.Max(1, MathF.Min((npc.height * npc.scale) / 48f, 3.5f));
                float drawPosY = npcHeight + npc.gfxOffY + 25f;
                // Iterate through the buff texture list
                for (int i = 0; i < currentDebuffs.Count; i++)
                {
                    // Reset the X position of the display every 4th and non-zero iteration, otherwise decrease the X draw position by 16 units
                    if (i != 0)
                    {
                        if (i % buffDisplayRowLimit == 0)
                            drawPosX = 36f;
                        else
                            drawPosX -= 18f;
                    }

                    // Offset the Y position every row after 5 iterations to limit each displayed row to 5 debuffs
                    float additionalYOffset = 18f * (float)Math.Floor(i * 0.25);
                    var tex = currentDebuffs[i];
                    Vector2 pos = npc.Center - screenPos - new Vector2(drawPosX * scale, -drawPosY * (1 + scale * 0.15f) - (additionalYOffset * scale));
                    if (currentDebuffs[i] == ModContent.Request<Texture2D>("asuw/Content/Buffs/DestinedDeath").Value)
                    {
                        var outline = ModContent.Request<Texture2D>("asuw/Assets/WhiteBuff").Value;
                        Color col = Color.Lerp(Color.Red, Color.Black, MathHelper.Lerp(0.25f, 0.75f, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f));
                        spriteBatch.UseBlendState(BlendState.NonPremultiplied);
                        spriteBatch.Draw(outline, pos - Vector2.One * 2.5f * scale, null, col * npc.Opacity, 0f, default, 0.55f * scale, SpriteEffects.None, 0f);
                        spriteBatch.ExitShaderRegion();
                    }
                    spriteBatch.Draw(tex, pos, null, Color.White * npc.Opacity, 0f, default, 0.5f * scale, SpriteEffects.None, 0f);

                }
                
            }

            if (SinkingStack > 0 && Main.LocalPlayer.HeldItem != null && Main.LocalPlayer.HeldItem.type == ModContent.ItemType<SolemnLament>())
            {
                Color colorNum = Color.Lerp(Color.White, Color.Black, MathHelper.SmoothStep(0, 1, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f));
                float scale = generalScale(npc.Size) > 1 ? 1 + (generalScale(npc.Size).AbsDelta(1f) / 2f) : generalScale(npc.Size);
                ShaderFunctions.DrawFancyNumbers(npc.Center, SinkingStack, colorNum, 0.55f * generalScale(npc.Size) / scale);
            }
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {

            if (!npc.canDisplayBuffs)
                return;

            if (Hemorrhage)
            {
                for (int i = 0; i < 3; i++)
                {
                    float speedx = Main.rand.NextFloat(-10f, 10f);
                    float speedy = Main.rand.NextFloat(-20f, 3f);
                    var dust2 = Dust.NewDustDirect(npc.position + new Vector2(npc.width / 2, npc.height / 2), 10, 10, DustID.Blood);
                    dust2.noGravity = true;
                    dust2.velocity = new Vector2(speedx, speedy) + npc.velocity;
                    dust2.scale = 1.4f;

                }
                Lighting.AddLight(npc.position, 1f, 0f, 0f);
            }

            if (BlackBlood)
            {
                for (int i = 0; i < 3; i++)
                {
                    float speedx = Main.rand.NextFloat(-10f, 10f);
                    float speedy = Main.rand.NextFloat(-20f, 3f);
                    var dust2 = Dust.NewDustDirect(npc.position + new Vector2(npc.width / 2, npc.height / 2), 10, 10, 54);
                    dust2.noGravity = true;
                    dust2.velocity = new Vector2(speedx, speedy) + npc.velocity;
                    dust2.scale = 1.4f;

                }
                Lighting.AddLight(npc.position, 1f, 0f, 0f);
            }

            if (Smoke)
            {
                for (int i = 0; i < 2; i++)
                {
                    float speedx = Main.rand.NextFloat(-10f, 10f);
                    float speedy = Main.rand.NextFloat(-10f, -5f);
                    var dust2 = Dust.NewDustDirect(npc.position + new Vector2(npc.width / 2, npc.height / 2), 25, 25, DustID.Smoke);
                    dust2.noGravity = true;
                    dust2.velocity = new Vector2(speedx, speedy) + npc.velocity;
                    dust2.velocity *= 0.5f;
                    dust2.scale = 1.4f;

                }
                Lighting.AddLight(npc.position, 0.3f, 0.3f, 0.3f);
            }

            if (Shi)
            {
                Dust dust = Dust.NewDustPerfect(npc.Center, ModContent.DustType<SharpSparkDust>(), new Vector2(0, -12).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(0.8f, 2.3f);
                dust.color = Color.DarkRed;
                Dust dust2 = Dust.NewDustPerfect(npc.Center, ModContent.DustType<SharpSparkDust>(), new Vector2(0, -5).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust2.noGravity = false;
                dust2.scale = Main.rand.NextFloat(0.8f, 2.3f);
                dust2.color = Color.DarkRed;


                Lighting.AddLight(npc.position, 1f, 0f, 0f);
            }
            if (IronFlame)
            {
                drawColor = Color.OrangeRed;
                Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
                Dust dust2 = Dust.NewDustPerfect(npcSize, ModContent.DustType<SharpSparkDust>(), new Vector2(0, -5).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.7f, 1.1f);
                dust2.color = Main.rand.NextBool() ? Color.OrangeRed : Color.Firebrick;

               

            }
            if (projected && npctick120 % 5 == 0)
            {

            }
            if(theEndTarget)
            {
                drawColor = Color.MediumPurple;
                
            }
            if (pyroAffected)
            {
                

            }
            if (DestinedDeath)
            {
                
            }
            if (SinkingStack > 0)
            {
                int butterflyType = Main.rand.NextBool() ? ModContent.DustType<ButterflyWhite>() : ModContent.DustType<ButterflyBlack>();
                Vector2 butterflyPos = npc.Center + Utils.NextVector2Circular(Main.rand, npc.width / 5, npc.height / 5);
                Vector2 butterflyVel = (Vector2.UnitY * Main.rand.NextFloat(-4, -6)).RotatedByRandom(1f);
                if (Main.rand.NextBool(15 - SinkingStack))
                {
                    Dust butterflies = Dust.NewDustPerfect(butterflyPos, butterflyType, butterflyVel, 0, Color.White, Main.rand.NextFloat(0.5f, 1));
                    butterflies.noGravity = true;
                }
            }
        }

        // i configure visuals with the dummy
        public static float generalScale(Vector2 npcSize) 
        {
            NPC dummy = new NPC();
            dummy.SetDefaults(NPCID.TargetDummy);
            float dummySize = dummy.Size.Length();
            return npcSize.Length() / dummySize;
        }
    }

}
