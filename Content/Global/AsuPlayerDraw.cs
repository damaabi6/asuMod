
using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Dusts;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Items.Weapons.Ranged;
using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;


namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {
        public int ran = 0;
        //TODO organize ts
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (!Player.active || Player.dead)
                return;

            if (Hemorrhage)
            {
                for (int i = 0; i < 3; i++)
                {
                    float speedx = Main.rand.NextFloat(-10f, 10f);
                    float speedy = Main.rand.NextFloat(-20f, 3f);
                    var dust2 = Dust.NewDustDirect(Player.position, 10, 10, DustID.Blood);
                    dust2.noGravity = true;
                    dust2.velocity = new Vector2(speedx, speedy) + Player.velocity;
                    dust2.scale = 1.4f;

                }
                Lighting.AddLight(Player.position, 1f, 0f, 0f);
            }
            if (Smoke)
            {


                float speedx = Main.rand.NextFloat(-10f, 10f);
                float speedy = Main.rand.NextFloat(-10f, -5f);
                var dust2 = Dust.NewDustDirect(Player.position, 25, 25, DustID.Smoke);
                dust2.noGravity = true;
                dust2.velocity = new Vector2(speedx, speedy) + Player.velocity;
                dust2.velocity *= 0.5f;
                dust2.scale = 1.4f;


                Lighting.AddLight(Player.position, 0.3f, 0.3f, 0.3f);
            }
            if(PyroEnchanted && Main.rand.NextBool(5))
            {
                //GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Player.Center + Utils.NextVector2Circular(Main.rand,15,15), (Vector2.UnitY).RotatedByRandom(0.4f) * Main.rand.NextFloat(-3, -6), (Main.rand.NextBool() ? Color.OrangeRed : Color.Lerp(Color.Firebrick, Color.Orange, 0.5f)), 20, Main.rand.NextFloat(0.1f, 0.3f), 2f, Main.rand.NextFloat(-0.05f, 0.05f)));
                //GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Player.Center + Utils.NextVector2Circular(Main.rand, 15, 15), (Vector2.UnitY).RotatedByRandom(0.4f) * Main.rand.NextFloat(-3,-6), (Main.rand.NextBool() ? Color.Lerp(Color.Maroon, Color.Black, 0.5f) : Color.Lerp(Color.Firebrick, Color.Black, 0.5f)), 20, Main.rand.NextFloat(0.1f, 0.3f) / 2f, 2f, Main.rand.NextFloat(-0.05f, 0.05f)));
            }
            if (ShiDash)
            {
                
                    Dust dust = Dust.NewDustPerfect(Player.Center, ModContent.DustType<SharpSparkDust>(), new Vector2(0, -12).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                    dust.noGravity = false;
                    dust.scale = Main.rand.NextFloat(0.8f, 2.3f);
                    dust.color = Color.DarkRed;
                    Dust dust2 = Dust.NewDustPerfect(Player.Center, ModContent.DustType<SharpSparkDust>(), new Vector2(0, -5).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                    dust2.noGravity = false;
                    dust2.scale = Main.rand.NextFloat(0.8f, 2.3f);
                    dust2.color = Color.DarkRed;

                
                Lighting.AddLight(Player.position, 1f, 0f, 0f);
            }
            if (Player.HeldItem.type == ModContent.ItemType<ProjectionSorcery>() && Player.immune)
            {

                Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), ModContent.DustType<SharpSparkDust>(), new Vector2(0, -18).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 0.4f));
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(0.2f, 0.3f);
                dust.color = Color.Blue;
                Dust dust2 = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), ModContent.DustType<SharpSparkDust>(), new Vector2(0, -7).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 0.4f));
                dust2.noGravity = false;
                dust2.scale = Main.rand.NextFloat(0.2f, 0.3f);
                dust2.color = Color.DeepSkyBlue;
            }
            //ui stuff
            if (Main.myPlayer == Player.whoAmI)
            {
                if (WeaponCooldown && Player.HeldItem.type == ModContent.ItemType<Horus>())
                {

                    Texture2D texture = ModContent.Request<Texture2D>($"asuw/Content/Textures/Chargedowns/HorusCooldown{CoolNum}").Value;
                    Vector2 drawPosition = (Player.Center + new Vector2(0, -100)) - Main.screenPosition;
                    Color drawColor = Color.White;
                    Vector2 origin = texture.Size() * 0.5f;
                    Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, 0, origin, 2, SpriteEffects.None);
                }

                if (WeaponCooldown && Player.HeldItem.type == ModContent.ItemType<Honor>())
                {

                    Texture2D texture = ModContent.Request<Texture2D>($"asuw/Content/Textures/Chargedowns/MakotoBoomCooldown{CoolNum}").Value;
                    Vector2 drawPosition = (Player.Center + new Vector2(0, -100)) - Main.screenPosition;
                    Color drawColor = Color.White;
                    Vector2 origin = texture.Size() * 0.5f;
                    Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, 0, origin, 2, SpriteEffects.None);
                }
             
             
                if (FuneralType > 0 && Player.HeldItem.type == ModContent.ItemType<SolemnLament>())
                {
                    Texture2D texture = ModContent.Request<Texture2D>($"asuw/Content/Textures/Chargedowns/FuneralShot{FuneralType}").Value;
                    Vector2 drawPosition = (Player.Center + new Vector2(0, -100)) - Main.screenPosition;
                    Color drawColor = Color.White;
                    Vector2 origin = texture.Size() * 0.5f;
                    Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, 0, origin, 0.5f, SpriteEffects.None);
                }
                if (Player.HeldItem.ModItem != null && Player.HeldItem.ModItem is TheEnd te)
                {
                   
                }
            }
        }
      
        
    }
}

