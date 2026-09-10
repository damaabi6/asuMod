using asuw.Content.Items.Accesories;
using asuw.Content.Items.Ammos;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {
        public static Color SpecialMoveColorCE =>  Color.Lerp(Color.Maroon, Color.Indigo, MathHelper.SmoothStep(0, 1, (MathF.Sin(Main.GlobalTimeWrappedHourly* 2) + 1) * 0.5f));

        public static Color SpecialMoveColorBH => Color.Lerp(Color.Lerp(Color.Gold, Color.NavajoWhite, 0.5f), Color.Lerp(Color.DeepSkyBlue, Color.RoyalBlue, 0.5f), MathHelper.SmoothStep(0, 1, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f));

        public static Color SpecialMoveColorWS1 => Color.Lerp(Color.Pink, Color.Violet, MathHelper.SmoothStep(0, 1, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f));

        public static Color WeaponSkillColor => Color.Lerp(Color.Pink, Color.Violet, MathHelper.SmoothStep(0, 1, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f));

        public static Color ColorWSP => Color.ForestGreen;

        public static Color ColorWSA => Color.PaleVioletRed;

        public static Color SpecialMoveColorWSC => Color.Lerp(Color.Red, Color.DarkRed, MathHelper.SmoothStep(0, 1, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f));

        public static int[] syncProjs = [ModContent.ProjectileType<TracerBulletProj>()];

        public override void PostUpdateMiscEffects()
        {
            if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                packetTimer++;
                if (packetTimer == GlobalSyncPacketTimer)
                {
                    packetTimer = 0;
                }

                if (syncMouseRightClick)
                {
                    syncMouseRightClick = false;
                    MouseRightClickSync();
                }

                mouseWorldPacketTimer = Math.Min(mouseWorldPacketTimer + 1, MouseWorldPacketInterval);
                if (mouseWorldPacketTimer >= MouseWorldPacketInterval)
                {
                    if (syncMousePosition)
                    {
                        mouseWorldPacketTimer = 0;
                        syncMousePosition = false;
                        syncMouseRotation = false; // Rotation also get update on position packet
                        MousePositionSync();
                    }

                    if (syncMouseRotation)
                    {
                        mouseWorldPacketTimer = 0;
                        syncMouseRotation = false;
                        MouseRotationSync();
                    }
                }

                if (packetTimer == 0)
                {
                    //Projectile owner data sync
                    bool needToSyncProj = false;
                    foreach (var proj in syncProjs)
                    {
                        if (Player.ownedProjectileCounts[proj] > 0)
                        {
                            needToSyncProj = true;
                            break;
                        }
                    }
                    if (needToSyncProj)
                        ProjOwnerDataSync();
                }
            }

            if (!base.Player.mount.Active)
            {
                if (LungingDown)
                {
                    base.Player.maxFallSpeed = 80f;
                    base.Player.noFallDmg = true;
                }
            }
                if (HitCooldown > 0)
                HitCooldown--;

            if (UseCooldown > 0)
                UseCooldown--;

            if (Player.HeldItem.type == ModContent.ItemType<ProjectionSorcery>())
            {
                Player.moveSpeed += 2f;
                Player.jumpSpeedBoost += 2f;
                Player.wingAccRunSpeed += 2f;


                if (ProjectionImmun)
                {
                    Player.immune = true;
                    Player.immuneTime = ProjectionImmuntick;
                    Player.immuneNoBlink = true;
                    for (int k = 0; k < Player.hurtCooldowns.Length; k++)
                        Player.hurtCooldowns[k] = Player.immuneTime;
                }

            }

            if(CorruptionEyeZero)
            {
                Player.moveSpeed += CorruptionEye.MoveSpeedBoost;
                Player.manaRegenCount += CorruptionEye.ManaRegenBoost;
            }

            //if (SOLcoreUpdate && Player.HeldItem.type != ModContent.ItemType<SOL>())
            //    Player.wingTime = 0;

            

           




            
        }

       

        
    }
}
