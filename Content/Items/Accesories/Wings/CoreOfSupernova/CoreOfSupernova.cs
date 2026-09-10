using asuw.Content.Dusts;
using asuw.Content.Items.Accessories.Wings;
using asuw.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Accesories.Wings
{
    //damn

    [AutoloadEquip(EquipType.Wings)]
    public class CoreOfSupernova : BaseWings
    {
        public override float BonusAscentWhileFalling => 1f;
        public override float BonusAscentWhileRising => 0.17f;
        public override float RisingSpeedThreshold => 1.2f;
        public override float MaxAscentSpeed => 3.25f;
        public override float BaseAscent => 0.15f;

        public bool hide = false;

        public float wingRotTo = 0;
        public float wingRot = 0;
        public bool disableWingRot = false;
        public float thrusterStrength = 0;
        public float thrusterStrengthOverride = 0;
        public float purple = 0;
        public float curlRot = 0;
        public float bloomRotStrength = 0;
        public float bloomRot = 0;


        public override void SetStaticDefaults() => ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(300, 9.5f, 2f);

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.accessory = true;
            Item.width = 22;
            Item.height = 20;
            Item.value = 10000;
            Item.rare = ItemRarityID.Purple;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            hide = hideVisual && player.velocity.Y == 0f;

            player.DisableWingFlapSound();

            Vector2 pos = player.Center + player.velocity - (Vector2.UnitX * 8 * player.direction);
            Vector2 offset = (MathHelper.PiOver2 * (player.direction == 1 ? 1.05f : 0.95f) + MathHelper.PiOver4 * player.direction + MathHelper.PiOver4 * wingRot).ToRotationVector2() * 30;
            Vector2 frontPos = pos - (Vector2.UnitX * 25 * player.direction) - (Vector2.UnitY * 24 * player.gravDir) + offset;
            Vector2 backPos = pos + (Vector2.UnitX * 26 * player.direction) - (Vector2.UnitY * 24 * player.gravDir) + offset;

            if (purple > 0) purple *= 0.94f;
            if (purple < 0.02f) purple = 0;
            if (bloomRotStrength > 0) bloomRotStrength *= 0.92f;
            if (bloomRotStrength < 0.01f) bloomRotStrength = 0;
            if (thrusterStrengthOverride > 0) thrusterStrengthOverride -= 0.05f;
            if (thrusterStrengthOverride < 0) thrusterStrengthOverride = 0;

            if (player.velocity.Y != 0)
            {
                wingRotTo = player.velocity.normalize().X * 0.4f * player.gravDir;
                float defaultThrusterSrength = player.controlJump ? 1f : 0.75f;
                float thrusterStrengthTo = thrusterStrengthOverride > defaultThrusterSrength ? thrusterStrengthOverride : defaultThrusterSrength;
                float maxChange = thrusterStrength.AbsDelta(thrusterStrengthTo) / 15;
                thrusterStrength = thrusterStrength.Towards(thrusterStrengthTo, maxChange);


                //dusts maybe
            }
            else
            {
                thrusterStrength = thrusterStrength.Towards(thrusterStrengthOverride > 0 ? thrusterStrengthOverride / 1.3f : 0, 0.05f);
                wingRotTo = 0;
            }
            float finalWingRotTo = disableWingRot ? 0f : wingRotTo;
            wingRot = wingRot.RotTowards(finalWingRotTo, MathF.Abs(wingRot.AngleBetween(finalWingRotTo) / 10f));
            curlRot = (MathHelper.PiOver4 / 3f) * thrusterStrength;
            float bloomRotTo = (-MathHelper.PiOver4 / 1.3f) * bloomRotStrength;
            bloomRot = bloomRot.Towards(bloomRotTo, 0.1f);

            Color lightingColor = Color.Lerp(Color.DodgerBlue, Color.Fuchsia, purple);
            Lighting.AddLight(frontPos, lightingColor.ToVector3() * thrusterStrength);
            Lighting.AddLight(backPos, lightingColor.ToVector3() * thrusterStrength);
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(wingRot);
            writer.Write(bloomRot);
            writer.Write(curlRot);
            writer.Write(thrusterStrength);
            writer.Write(purple);
        }
        public override void NetReceive(BinaryReader reader)
        {
            wingRot = reader.ReadSingle();
            bloomRot = reader.ReadSingle();
            curlRot = reader.ReadSingle();
            thrusterStrength = reader.ReadSingle();
            purple = reader.ReadSingle();
        }
    }

    public class CoreOfSupernovaLayer : PlayerDrawLayer
    {
        public static Asset<Texture2D> coreTexture;
        float blasterRotAmnt = 1;
        public override void Load()
        {
            coreTexture = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/Core");
        }

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Wings);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.wings == EquipLoader.GetEquipSlot(Mod, "CoreOfSupernova", EquipType.Wings) || (drawInfo.drawPlayer.HeldItem.ModItem != null && drawInfo.drawPlayer.HeldItem.ModItem is SOL sol && sol.blasterTime > 0);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            if (player.dead || !player.active || player == null)
                return;

            SOL heldWeapon;
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SOL sol)
                heldWeapon = sol;
            else
                heldWeapon = null;

            //-------------------Core--------------------------||
            Texture2D CoreP = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/CoreP", AssetRequestMode.AsyncLoad).Value;
            Texture2D CoreB = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/CoreB", AssetRequestMode.AsyncLoad).Value;
            //-------------------------------------------------||


            //-------------------Front------------------------||
            Texture2D FrontCoreB = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontWingCoreB", AssetRequestMode.AsyncLoad).Value;
            Texture2D FrontCoreP = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontWingCoreP", AssetRequestMode.AsyncLoad).Value;

            Texture2D Front1P = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontWing1P", AssetRequestMode.AsyncLoad).Value;
            Texture2D Front1B = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontWing1B", AssetRequestMode.AsyncLoad).Value;
            Texture2D Front2P = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontWing2P", AssetRequestMode.AsyncLoad).Value;
            Texture2D Front2B = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontWing2B", AssetRequestMode.AsyncLoad).Value;
            Texture2D Front3P = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontWing3P", AssetRequestMode.AsyncLoad).Value;
            Texture2D Front3B = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontWing3B", AssetRequestMode.AsyncLoad).Value;
            Texture2D Front4P = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontWing4P", AssetRequestMode.AsyncLoad).Value;
            Texture2D Front4B = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontWing4B", AssetRequestMode.AsyncLoad).Value;
            //-------------------------------------------------||


            //-------------------Back------------------------- ||
            Texture2D BackCore = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/BackWingCore", AssetRequestMode.AsyncLoad).Value;

            Texture2D Back1P = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/BackWing1P", AssetRequestMode.AsyncLoad).Value;
            Texture2D Back1B = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/BackWing1B", AssetRequestMode.AsyncLoad).Value;
            Texture2D Back2P = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/BackWing2P", AssetRequestMode.AsyncLoad).Value;
            Texture2D Back2B = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/BackWing2B", AssetRequestMode.AsyncLoad).Value;
            Texture2D Back3P = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/BackWing3P", AssetRequestMode.AsyncLoad).Value;
            Texture2D Back3B = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/BackWing3B", AssetRequestMode.AsyncLoad).Value;
            Texture2D Back4P = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/BackWing4P", AssetRequestMode.AsyncLoad).Value;
            Texture2D Back4B = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/BackWing4B", AssetRequestMode.AsyncLoad).Value;
            //-------------------------------------------------||


            //-------------------Blaster-----------------------||
            Texture2D Blaster = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Laser_test").Value;
            //-------------------------------------------------||


            //-------------------Thruster----------------------||
            Texture2D BackFire = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/BackFire").Value;
            Texture2D FrontFire = ModContent.Request<Texture2D>("asuw/Content/Items/Accesories/Wings/CoreOfSupernova/FrontFire").Value;
            //-------------------------------------------------||

            int direction = player.direction * (int)player.gravDir;
            SpriteEffects effects = direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (player.equippedWings != null && player.equippedWings.ModItem is CoreOfSupernova cor)
            {

                Vector2 Position = drawInfo.Position;
                Vector2 pos = new Vector2((int)(Position.X - Main.screenPosition.X + (player.width / 2) - (2 * player.direction)), (int)(Position.Y - Main.screenPosition.Y + (player.height / 2 + player.HeightOffsetVisual / 2f) - 2f * player.gravDir)) - (Vector2.UnitX * 8 * player.direction);
                Color lightColor = Lighting.GetColor((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), Color.White);
                Color color = lightColor * (1f - drawInfo.shadow);

                float wingRotOffset1 = (cor.curlRot + cor.bloomRot) * direction;
                float wingRotOffset2 = ((cor.curlRot / 2) + (cor.bloomRot / 2)) * direction;
                float purple = cor.purple;


                //-------------------Core--------------------------||
                DrawData coreDataP = new DrawData(CoreP, pos, null, color * purple, 0f, CoreP.Size() / 2f, 1f * player.gravDir, effects, 0);
                DrawData coreDataB = new DrawData(CoreP, pos, null, color, 0f, CoreB.Size() / 2f, 1f * player.gravDir, effects, 0);
                //-------------------------------------------------||


                //-------------------Front-------------------------||
                Vector2 frontPos = pos - (Vector2.UnitX * 23 * player.direction) - (Vector2.UnitY * player.gravDir * 24);
                Vector2 frontOrigin = direction == 1 ? new Vector2(FrontCoreP.Width - 14, 12) : new Vector2(14, 12);

                //2-4

                DrawData frontDataCoreP = new DrawData(FrontCoreP, frontPos, null, color * purple, cor.wingRot, frontOrigin, 1.2f * player.gravDir, effects, 0);
                DrawData frontDataCoreB = new DrawData(FrontCoreB, frontPos, null, color, cor.wingRot, frontOrigin, 1.2f * player.gravDir, effects, 0);


                DrawData frontData1P = new DrawData(Front1P, frontPos, null, color * purple, cor.wingRot + wingRotOffset1, frontOrigin, 1.2f * player.gravDir, effects, 0);
                DrawData frontData1B = new DrawData(Front1B, frontPos, null, color, cor.wingRot + wingRotOffset1, frontOrigin, 1.2f * player.gravDir, effects, 0);


                DrawData frontData2P = new DrawData(Front2P, frontPos, null, color * purple, cor.wingRot + wingRotOffset2, frontOrigin, 1.2f * player.gravDir, effects, 0);
                DrawData frontData2B = new DrawData(Front2B, frontPos, null, color, cor.wingRot + wingRotOffset2, frontOrigin, 1.2f * player.gravDir, effects, 0);


                DrawData frontData3P = new DrawData(Front3P, frontPos, null, color * purple, cor.wingRot - wingRotOffset1, frontOrigin, 1.2f * player.gravDir, effects, 0);
                DrawData frontData3B = new DrawData(Front3B, frontPos, null, color, cor.wingRot - wingRotOffset1, frontOrigin, 1.2f * player.gravDir, effects, 0);


                DrawData frontData4P = new DrawData(Front4P, frontPos, null, color * purple, cor.wingRot - wingRotOffset2, frontOrigin, 1.2f * player.gravDir, effects, 0);
                DrawData frontData4B = new DrawData(Front4B, frontPos, null, color, cor.wingRot - wingRotOffset2, frontOrigin, 1.2f * player.gravDir, effects, 0);
                //-------------------------------------------------||


                //-------------------Back--------------------------||
                Vector2 backPos = pos + (Vector2.UnitX * 26 * player.direction) - (Vector2.UnitY * player.gravDir * 24);
                Vector2 backOrigin = direction == 1 ? new Vector2(BackCore.Width - 16, 8) : new Vector2(16, 8);

                //1-3

                DrawData backDataCore = new DrawData(BackCore, backPos, null, color, cor.wingRot, backOrigin, 1.2f * player.gravDir, effects, 0);

                DrawData backData1P = new DrawData(Back1P, backPos, null, color * purple, cor.wingRot + wingRotOffset2, backOrigin, 1.2f * player.gravDir, effects, 0);
                DrawData backData1B = new DrawData(Back1B, backPos, null, color, cor.wingRot + wingRotOffset2, backOrigin, 1.2f * player.gravDir, effects, 0);

                DrawData backData2P = new DrawData(Back2P, backPos, null, color * purple, cor.wingRot + wingRotOffset1, backOrigin, 1.2f * player.gravDir, effects, 0);
                DrawData backData2B = new DrawData(Back2B, backPos, null, color, cor.wingRot + wingRotOffset1, backOrigin, 1.2f * player.gravDir, effects, 0);

                DrawData backData3P = new DrawData(Back3P, backPos, null, color * purple, cor.wingRot - wingRotOffset2, backOrigin, 1.2f * player.gravDir, effects, 0);
                DrawData backData3B = new DrawData(Back3B, backPos, null, color, cor.wingRot - wingRotOffset2, backOrigin, 1.2f * player.gravDir, effects, 0);

                DrawData backData4P = new DrawData(Back4P, backPos, null, color * purple, cor.wingRot - wingRotOffset1, backOrigin, 1.2f * player.gravDir, effects, 0);
                DrawData backData4B = new DrawData(Back4B, backPos, null, color, cor.wingRot - wingRotOffset1, backOrigin, 1.2f * player.gravDir, effects, 0);
                //-------------------------------------------------||


                //-------------------Blaster-----------------------||
                Vector2 blasterPos = pos - Vector2.UnitY * (player.height / 2) * player.gravDir;
                Vector2 blasterOrigin = direction == 1 ? new Vector2(Blaster.Width * 0.9f, Blaster.Height * 0.1f) : new Vector2(Blaster.Width * 0.1f, Blaster.Height * 0.1f);
                //-------------------------------------------------||


                //-------------------Thruster----------------------||
                Color fireColor = Color.Lerp(Color.DodgerBlue, Color.Fuchsia, cor.bloomRotStrength * 0.7f) with { A = 0 } * cor.thrusterStrength;
                Color fireColorGlow = Color.White with { A = 0 } * 0.67f * cor.thrusterStrength;
                float fireRotOffset = MathHelper.ToRadians(Main.rand.NextFloat(-2, 2)) * cor.thrusterStrength;

                DrawData BackFireData = new DrawData(BackFire, backPos, null, fireColor, cor.wingRot * 0.6f + fireRotOffset * 0.7f, backOrigin, (1.33f * (0.1f + cor.thrusterStrength) + (cor.bloomRotStrength * 0.48f)) * player.gravDir, effects, 0);
                DrawData FrontFireData = new DrawData(FrontFire, frontPos, null, fireColor, cor.wingRot + fireRotOffset, frontOrigin, (1.33f * (0.1f + cor.thrusterStrength) + (cor.bloomRotStrength * 0.6f)) * player.gravDir, effects, 0);

                DrawData BackFireGlowData = new DrawData(BackFire, backPos, null, fireColorGlow, cor.wingRot * 0.6f + fireRotOffset * 0.7f * 0.8f, backOrigin, (1.15f * (0.1f + cor.thrusterStrength) + (cor.bloomRotStrength * 0.27f)) * player.gravDir, effects, 0);
                DrawData FrontFireGlowData = new DrawData(FrontFire, frontPos, null, fireColorGlow, cor.wingRot + fireRotOffset * 0.8f, frontOrigin, (1.15f * (0.1f + cor.thrusterStrength) + (cor.bloomRotStrength * 0.35f)) * player.gravDir, effects, 0);
                //-------------------------------------------------||


                //-----------------------------------------------------------------------------------------------------||
                //----------------------------------------------Draw---------------------------------------------------||
                //-----------------------------------------------------------------------------------------------------||


                //---------BackThruster--------------------------||
                drawInfo.DrawDataCache.Add(BackFireData);
                drawInfo.DrawDataCache.Add(BackFireGlowData);
                //-----------------------------------------------||


                //---------BackWing------------------------------||
                backDataCore.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(backDataCore);

                backData1B.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(backData1B);
                backData1P.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(backData1P);

                backData2B.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(backData2B);
                backData2P.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(backData2P);

                backData3B.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(backData3B);
                backData3P.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(backData3P);

                backData4B.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(backData4B);
                backData4P.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(backData4P);
                //-----------------------------------------------||



                //-------------------Core--------------------------||
                coreDataB.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(coreDataB);
                coreDataP.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(coreDataP);
                //-------------------------------------------------||


                //---------Blaster-------------------------------||
                if (heldWeapon != null)
                {
                    float RotOffset = MathHelper.ToRadians(210 * direction);
                    float maxChange = 0.1f - (blasterRotAmnt.AbsDelta(heldWeapon.blasterTime > 0 ? 0 : 1) / 13);
                    if (heldWeapon.blasterTime > 0) blasterRotAmnt = blasterRotAmnt.Towards(0, maxChange);
                    else blasterRotAmnt = blasterRotAmnt.Towards(1, maxChange);
                    float rotToAim = heldWeapon.blasterPhase == 0 ? (player.gravDir == -1 ? MathF.PI : 0) : heldWeapon.blasterRotTo + (direction == -1 ? MathF.PI : 0);
                    float finalRot = RotOffset - (RotOffset * blasterRotAmnt) + rotToAim;
                    Vector2 jitter = heldWeapon.blasterPhase == 2 ? Utils.NextVector2Circular(Main.rand, 2, 2) : Vector2.Zero;
                    DrawData blasterData = new DrawData(Blaster, blasterPos + jitter, null, color, finalRot, blasterOrigin, 1f, effects, 0);
                    drawInfo.DrawDataCache.Add(blasterData);
                }
                //-----------------------------------------------||


                //---------FrontThruster-------------------------||
                drawInfo.DrawDataCache.Add(FrontFireData);
                drawInfo.DrawDataCache.Add(FrontFireGlowData);
                //-----------------------------------------------||


                //---------FrontWing-----------------------------||
                frontData1B.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(frontData1B);
                frontData1P.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(frontData1P);

                frontData2B.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(frontData2B);
                frontData2P.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(frontData2P);

                frontData3B.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(frontData3B);
                frontData3P.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(frontData3P);

                frontData4B.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(frontData4B);
                frontData4P.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(frontData4P);

                frontDataCoreB.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(frontDataCoreB);
                frontDataCoreP.shader = drawInfo.drawPlayer.cWings;
                drawInfo.DrawDataCache.Add(frontDataCoreP);
                //-----------------------------------------------||

            }
        }
    }
}
