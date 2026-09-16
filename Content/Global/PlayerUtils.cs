using asuw.Content.Items.Accesories.Wings;
using asuw.Content.Items.Accessories.Wings;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content
{
    public static partial class AsuUtils
    {
        public static int[] debuffIDS = new int[] { BuffID.Bleeding, BuffID.Poisoned, BuffID.Venom, BuffID.OnFire, BuffID.OnFire3, BuffID.CursedInferno, BuffID.Frostburn, BuffID.Frostburn2, BuffID.BrokenArmor, BuffID.Confused, BuffID.Darkness, BuffID.Blackout, BuffID.Obstructed, BuffID.Silenced, BuffID.Cursed, BuffID.Chilled, BuffID.Slow, BuffID.Burning, BuffID.Frozen, BuffID.Webbed, BuffID.Stoned, BuffID.WindPushed, 164, BuffID.Electrified, BuffID.Suffocation, BuffID.WitheredArmor, BuffID.WitheredWeapon, BuffID.Weak, 148 }; //164 is distorted and 148 is feralbite
        public static Vector2 GetFrontHandPositionImproved(this Player player, Player.CompositeArmData arm)
        {
            Vector2 result = player.GetFrontHandPosition(arm.stretch, arm.rotation * player.gravDir).Floor();
            if (player.gravDir == -1f)
            {
                result.Y = player.position.Y + (float)player.height + (player.position.Y - result.Y);
            }

            return result;
        }
        public static bool HasEquippedWing(this Player player, int wingType)
        {
            if(player.equippedWings == null)
                return false;
            if (player.equippedWings.ModItem == null)
                return false;
            if (player.equippedWings.ModItem.Type != wingType)
                return false;
            return true;
                   
        }
        public static void SetScreenshake(this Player player, float value)
        {
            if (player.asuw().GeneralScreenShakePower < value)
                player.asuw().GeneralScreenShakePower = value;
        }
        public static void SetScreenAngle(this Player player, float angle, float fadeOut = 0.1f)
        {
            player.asuw().ScreenAngle = MathHelper.ToRadians(angle / 2);
            player.asuw().ScreenAngleFade = fadeOut;
        }

        public static void SetScreenZoomInto(this Player player, float zoom, Vector2 pos, float fadeOut = 0.05f)
        {
            player.asuw().ScreenZoomTo = zoom * 0.2f;
            player.asuw().ScreenZoomToPos = pos;
            player.asuw().ScreenZoomToFade = fadeOut;
        }
        public static void SetDarkLayer(this Player player, float opacity, float fadeOut = 0.1f)
        {
            player.asuw().DarkBackLayerOP = opacity;
            player.asuw().DarkBackLayerFade = fadeOut;
        }
        public static void DisableWingFlapSound(this Player player)
        {
            // vanilla plays a flap sound for all wings barring a few hardcoded exceptions
            // the flapSound flag is used to see if the sound *has been* played, so we set it to true here to prevent it from playing 
            player.flapSound = true;
        }
        public static Vector2 GetBackHandPositionImproved(this Player player, Player.CompositeArmData arm)
        {
            Vector2 result = player.GetBackHandPosition(arm.stretch, arm.rotation * player.gravDir).Floor();
            if (player.gravDir == -1f)
            {
                result.Y = player.position.Y + (float)player.height + (player.position.Y - result.Y);
            }

            return result;
        }
        public static float GetMeleeScale(this Player player, bool addHeldItemScale = true)
        {
            float scale = 1f;
            player.ApplyMeleeScale(ref scale);
            if (addHeldItemScale)
            {
                scale += player.HeldItem.scale - 1f;
            }

            return scale;
        }
        public static Player ToPlayer(this int ins)
        {
            if (ins < 0 || ins >= Main.player.Length || !Main.player[ins].active)
            {
                return Main.LocalPlayer;
            }
            return Main.player[ins];
        }
        public static Vector2 GetDrawCenter(this Player player)
        {
            return player.MountedCenter + player.gfxOffY * Vector2.UnitY;
        }

        public static void DoLifestealDirect(this Player player, NPC target, int amount, float cooldownMultiplier = 1f)
        {
            if (target == null || (target.IsAnEnemy(allowStatues: false) && target.canGhostHeal))
            {
                amount = Math.Min(amount, player.statLifeMax2 - player.statLife);
                amount = Math.Min(amount, 100);
                if (amount > 0 && !(player.lifeSteal <= 0f) && !player.moonLeech)
                {
                    player.lifeSteal -= (float)amount * cooldownMultiplier;
                    player.HealPlayer(amount);
                }
            }
        }

        public static void HealPlayer(this Player player, int amount, HealTextType healTextType = HealTextType.Broadcast)
        {
            player.statLife += amount;
            if (player.statLife > player.statLifeMax2)
            {
                player.statLife = player.statLifeMax2;
            }

            if (healTextType != 0)
            {
                player.HealEffect(amount, healTextType == HealTextType.Broadcast);
            }
        }



    }

    public enum HealTextType
    {
        None,
        Local,
        Broadcast
    }

}

