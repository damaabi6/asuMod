using asuw.Content.Buffs;
using asuw.Content.Global;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace asuw.Content.Global
{
    internal static class ProjectileCommon
    {
        public static void HealingProjectile(this Projectile projectile, int healing, int playerToHeal, float homingVelocity, float inertia, bool autoHomes = true, int timeCheck = 120)
        {
            int target = playerToHeal;
            Player player = Main.player[target];
            float homingSpeed = homingVelocity;
            if (player.lifeMagnet)
                homingSpeed *= 1.5f;

            Vector2 playerVector = player.Center - projectile.Center;
            float playerDist = playerVector.Length();
            if (playerDist < 50f && projectile.position.X < player.position.X + player.width && projectile.position.X + projectile.width > player.position.X && projectile.position.Y < player.position.Y + player.height && projectile.position.Y + projectile.height > player.position.Y)
            {
                if (projectile.owner == Main.myPlayer && !Main.LocalPlayer.moonLeech)
                {
                    int healAmt = healing;
                    player.HealPlayer(healAmt);
                    NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, target, healAmt, 0f, 0f, 0, 0, 0);
                }
                projectile.Kill();
            }
            if (autoHomes)
            {
                playerDist = homingSpeed / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
                projectile.velocity.X = (projectile.velocity.X * inertia + playerVector.X) / (inertia + 1f);
                projectile.velocity.Y = (projectile.velocity.Y * inertia + playerVector.Y) / (inertia + 1f);
            }
            else if (player.lifeMagnet && projectile.timeLeft < timeCheck)
            {
                playerDist = homingVelocity / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
                projectile.velocity.X = (projectile.velocity.X * inertia + playerVector.X) / (inertia + 1f);
                projectile.velocity.Y = (projectile.velocity.Y * inertia + playerVector.Y) / (inertia + 1f);
            }
        }
    }
}
