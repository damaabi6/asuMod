using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Items.Accesories;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Projectiles;
using asuw.Content.Projectiles.InfoAndChargeBar;
using asuw.Effects;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;
using Terraria.WorldBuilding;


namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {
        //TODO organize this fckin mess
        public bool rant;
        public bool butterflyHairpin;
        public bool WeaponCooldown;
        public bool SubaruFullAuto;
        public bool ShiBlade;
        public bool ShiDash;
        public bool FuneralShot;
        public bool FuneralTimerTickDown;
        public bool ProjectionDashing;
        public bool ProjectionImmun;
        public bool TheEndDestroyerOverheated;
        public bool TheEndDestroyerSkill;
        public bool HorusBarrierUp;
        public bool CorruptionEyeEquipped;
        public bool CorruptionEyeActivated;
        public bool CorruptionEyeZero;
        public bool PyroEnchanted;


        public float AdditiveCritDamageBonus;
        public int HitCooldown = 0;
        public int UseCooldown = 0;
        public int CoolTick = 0;
        public int CoolTickDown = 0;
        public double CoolNum = 0;
        public int SubaruSpeed = 3;
        public int StoneEnchantmentType = 0;
        public int CopperEnchantmentType = 0;
        public int GoldenEnchantmentType = 0;
        public int IronEnchantmentType = 0;
        public int FuneralDMGPenalty = 0;
        public int FuneralType = 0;
        public int FuneralTimerTick = 0;
        public int TungTungTung = 0;
        public int Timer120 = 0;
        public int Timer1200 = 0;
        public int ProjectionImmuntick = 0;

        public float CorruptionEyeChargeAmnt = 100;
        public float CorruptionEyeShaderIntensity = 0;

        public int oldTracerBulletColor = 0;
        public int tracerBulletColor = 0;

        public bool SOLChargeUpActivated = false;

        public float GeneralScreenShakePower = 0f;
        public float ScreenAngle = 0f;
        public float ScreenAngleFade = 0f;
        public float ScreenZoomTo = 0f;
        public float ScreenZoomToFade = 0f;
        public Vector2 ScreenZoomToPos = Vector2.Zero;
        public float DarkBackLayerOP = 0;
        public float DarkBackLayerFade = 0;

        public bool LungingDown;
        public override void ResetEffects()
        {
            
            AdditiveCritDamageBonus = 0f;
            Smoke = false;
            WeaponCooldown = false;
            butterflyHairpin = false;
            SubaruFullAuto = false;
            rant = false;
            ShiDash = false;
            CorruptionEyeEquipped = false;
            CorruptionEyeActivated = false;
            CorruptionEyeZero = false;
            PyroEnchanted = false;


        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (AdditiveCritDamageBonus > 0)
            {
                modifiers.CritDamage += AdditiveCritDamageBonus;
            }
        }

        public override void ModifyScreenPosition()
        {
            float screenShakePower = 1f;

            if (GeneralScreenShakePower > 0f)
                Main.screenPosition += Main.rand.NextVector2Circular(GeneralScreenShakePower * screenShakePower, GeneralScreenShakePower * screenShakePower);

            GeneralScreenShakePower = MathHelper.Clamp(GeneralScreenShakePower - 0.185f, 0f, 20f * screenShakePower);

        }

        public override void PreUpdate()
        {

            if (Timer120 < 120)
                Timer120++;
            else
                Timer120 = 0;

            if (Timer1200 < 1200)
                Timer1200++;
            else
                Timer1200 = 0;

            if (Main.myPlayer == Player.whoAmI)
            {
                mouseRight = PlayerInput.Triggers.Current.MouseRight;
                var worldPos = LockOnHelper.Enabled ? LockOnHelper.PredictedPosition : Main.MouseWorld;
                mouseWorldDeltaFromPlayer = worldPos - Player.MountedCenter;
                mouseRotationFromPlayer = mouseWorldDeltaFromPlayer.ToRotation();

                if (rightClickListener && mouseRight != oldMouseRight)
                {
                    oldMouseRight = mouseRight;
                    syncMouseRightClick = true;
                    rightClickListener = false;
                }
                if (mouseWorldListener && Vector2.Distance(mouseWorldDeltaFromPlayer, oldMouseWorldDeltaFromPlayer) > 5f)
                {
                    oldMouseWorldDeltaFromPlayer = mouseWorldDeltaFromPlayer;
                    syncMousePosition = true;
                    mouseWorldListener = false;
                }
                if (mouseRotationListener && Math.Abs((mouseWorldDeltaFromPlayer).ToRotation() - (oldMouseWorldDeltaFromPlayer).ToRotation()) > 0.15f)
                {
                    oldMouseWorldDeltaFromPlayer = mouseWorldDeltaFromPlayer;
                    syncMouseRotation = true;
                    mouseRotationListener = false;
                }
            }

            if (WeaponCooldown)
            {
                if (CoolTick < 0)
                {
                    CoolNum = 999;
                }
                else
                {
                    Math.Floor(CoolNum);
                    CoolNum = (CoolTick / 60) + 1;
                }
            }
            else
            {
                CoolNum = 0;
                CoolTick = 0;
                CoolTickDown = 0;
            }
          
            if (rant)
            { ran++; }
            else { ran = 0; }

         

            if (FuneralDMGPenalty > 45)
            { FuneralDMGPenalty = 45; }
            if (FuneralDMGPenalty < 0)
            { FuneralDMGPenalty = 0; }

            if (FuneralType <= 1)
            { FuneralShot = true; }
            else
            { FuneralShot = false; }

            if (FuneralTimerTickDown)
            { FuneralTimerTick--; }

            if (FuneralTimerTick < 0)
            {
                FuneralShot = true;
                FuneralType = 0;
                FuneralTimerTick = 0;
                FuneralTimerTickDown = false;
            }

            if (ProjectionImmuntick > 0)
            {
                ProjectionImmun = true;
                ProjectionImmuntick--;
            }
            else
            {
                ProjectionImmun = false;
            }


        }

        
        public override void PostUpdate()
        {
            if (CorruptionEyeZero && (CorruptionEyeStoredY > 0.2f || CorruptionEyeStoredY < -0.2f) && (Player.velocity.Y > 0.2f || Player.velocity.Y < -0.2f))
            {
                Player.velocity.Y = CorruptionEyeStoredY;
            }

            if (Player.whoAmI == Main.myPlayer)
            {
                float fadeAngleAmnt = Math.Max(ScreenAngle.AbsDelta(0) * ScreenAngleFade, float.Epsilon);
                if (MathF.Abs(ScreenAngle) > 0)
                    ScreenAngle = ScreenAngle.Towards(0, fadeAngleAmnt);
                ShaderFunctions.AngleScreen(ScreenAngle);

                float fadeZoomToAmnt = Math.Max(ScreenZoomTo.AbsDelta(0) * ScreenZoomToFade, float.Epsilon);
                if (ScreenZoomTo > 0)
                {
                    ScreenZoomTo = ScreenZoomTo.Towards(0, fadeZoomToAmnt);
                }
                ShaderFunctions.ZoomScreen(ScreenZoomTo, ScreenZoomToPos);

                float darkLayerFadeAmnt = Math.Max(DarkBackLayerOP.AbsDelta(0) * DarkBackLayerFade, float.Epsilon);
                if(DarkBackLayerOP > 0)
                    DarkBackLayerOP = DarkBackLayerOP.Towards(0, darkLayerFadeAmnt);
                

            }
        }

        public override void UpdateDead()
        {
            if (WeaponCooldown)
            {
                ShiDash = false;
                CoolNum = 0;
                CoolTick = 0;
                CoolTickDown = 0;
                WeaponCooldown = false;
            }

            if (Player.HeldItem.ModItem != null && Player.HeldItem.ModItem is IronLotus il)
            {
                il.IronLotusCharge = 0f;
                il.IronLotusChargeNum = 0;
                il.IronLotusFlameCharge = 0;
                il.IronFlameTick = 0;
                il.IronFlameLevel = 5;
                il.IronLotusFlame = false;
            }

        }

     

    }

}
