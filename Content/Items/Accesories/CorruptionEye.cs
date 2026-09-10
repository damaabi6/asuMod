using asuw.Content.Dusts;
using asuw.Content.Global;
using asuw.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;


namespace asuw.Content.Items.Accesories
{
    public class CorruptionEye : ModItem
    {
        //TODO black corruption eyes screen fx
        public const float maxCharge = 100;
        public bool activated = false;

        public static float MoveSpeedBoost = 0.3f;
        public static float VerticalSpeedBoost = 0.15f;
        public static int ManaRegenBoost = 50;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<ButterflyHairpinRare>();
            Item.maxStack = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.asuw().CorruptionEyeEquipped = true;

            if (player.asuw().CorruptionEyeShaderIntensity < 0) player.asuw().CorruptionEyeShaderIntensity = 0;
            if (player.asuw().CorruptionEyeShaderIntensity > 1) player.asuw().CorruptionEyeShaderIntensity = 1;

            if (player.asuw().CorruptionEyeChargeAmnt < 0) player.asuw().CorruptionEyeChargeAmnt = 0;
            if (player.asuw().CorruptionEyeChargeAmnt > maxCharge) player.asuw().CorruptionEyeChargeAmnt = maxCharge;

            if (AsuKeybinds.CorruptionEyeActivation.JustPressed)
            {
                if (activated)
                { activated = false; }
                else
                {
                    if (player.asuw().CorruptionEyeChargeAmnt >= 50)
                    {
                        activated = true;

                        for (int i = 0; i < 7; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(player.MountedCenter + Main.rand.NextVector2Circular(15, 15), ModContent.DustType<SharpSparkDust>(), (Vector2.UnitX * 20).RotatedByRandom(MathHelper.TwoPi), 0, Main.rand.NextBool() ? Color.IndianRed : Color.Red, Main.rand.NextFloat(1f, 2f));
                            dust.noGravity = true;
                        }
                    }
                }
            }

            if (!activated) //unactivated
            {
                if (player.asuw().CorruptionEyeShaderIntensity > 0.03f)
                    player.asuw().CorruptionEyeShaderIntensity -= 0.01f;
                else player.asuw().CorruptionEyeShaderIntensity = 0f;

                if (player.asuw().CorruptionEyeChargeAmnt < maxCharge)
                {
                    if (player.asuw().Timer120 % 30 == 0) player.asuw().CorruptionEyeChargeAmnt++;
                }
                player.asuw().CorruptionEyeActivated = false;
                player.GetDamage(DamageClass.Magic) *= 1 + 0.15f + (player.asuw().CorruptionEyeShaderIntensity);
            }
            else // activated
            {
                player.asuw().CorruptionEyeShaderIntensity = 1f - (player.asuw().CorruptionEyeChargeAmnt / 100f);
                if (player.asuw().CorruptionEyeChargeAmnt > 0)
                {
                    if (player.asuw().Timer120 % 20 == 0) player.asuw().CorruptionEyeChargeAmnt--; 
                }
                player.asuw().CorruptionEyeActivated = true;
                player.GetDamage(DamageClass.Magic) *= 1 + 0.15f + (1 - (player.asuw().CorruptionEyeChargeAmnt / 100f));
            }

            player.GetArmorPenetration(DamageClass.Magic) += 15;
            player.statManaMax2 += 100;

            if (player.asuw().CorruptionEyeChargeAmnt <= 0)
            {
                player.asuw().CorruptionEyeZero = true;
                //player.AddBuff(ModContent.BuffType<Laceration>(), 2);
                //player.AddBuff(ModContent.BuffType<ArmorCrunch>(), 2);

                Dust dust = Dust.NewDustPerfect(player.MountedCenter + Main.rand.NextVector2Circular(15, 15), ModContent.DustType<SharpSparkDust>(), (Vector2.UnitY * -15 * player.gravDir).RotatedByRandom(0.4f) * Main.rand.NextFloat(0.3f,1.2f), 0, Main.rand.NextBool() ? Color.IndianRed : Color.Red, Main.rand.NextFloat(0.5f, 1.8f));
                dust.noGravity = true;
            }

            if (player.asuw().CorruptionEyeShaderIntensity > 0.001f)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    //tint
                    if (!Filters.Scene["asuw:CorruptedTint"].IsActive())
                        Filters.Scene.Activate("asuw:CorruptedTint", player.Center).GetShader()
                                .UseOpacity(1f)
                                .UseIntensity(player.asuw().CorruptionEyeShaderIntensity * 0.85f);
                    else
                        Filters.Scene["asuw:CorruptedTint"].GetShader().UseIntensity(player.asuw().CorruptionEyeShaderIntensity * 0.85f);

                    //vignette
                    if (!Filters.Scene["asuw:VignetteEffect"].IsActive())
                        Filters.Scene.Activate("asuw:VignetteEffect", player.Center).GetShader()
                                .UseOpacity(1f)
                                .UseProgress(0.7f)
                                .UseIntensity(player.asuw().CorruptionEyeShaderIntensity * 1.1f);
                    else
                        Filters.Scene["asuw:VignetteEffect"].GetShader().UseIntensity(player.asuw().CorruptionEyeShaderIntensity * 1.1f);

                }
            }
            else
            {
                RemoveShaders();
            }
            
        }
        public static void RemoveShaders()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (Filters.Scene["asuw:VignetteEffect"].IsActive())
                    Filters.Scene["asuw:VignetteEffect"].Deactivate();
                if (Filters.Scene["asuw:CorruptedTint"].IsActive())
                    Filters.Scene["asuw:CorruptedTint"].Deactivate();
            }
        }
    }

   
    
}