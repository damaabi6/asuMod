using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace asuw.Content
{
    public static partial class AsuUtils
    {
        public static Player GetOwner(this Projectile proj)
        {
            if (proj.owner < 0)
            {
                return null;
            }
            return proj.owner.ToPlayer();
        }
        public static void HomeInOnNPC(Projectile projectile, bool ignoreTiles, float distanceRequired, float homingVelocity, float inertia, bool respectIFrames = false)
        {
            if (!projectile.friendly)
            {
                return;
            }

            if (projectile.asuw().defExtraUpdates == -1)
            {
                projectile.asuw().defExtraUpdates = projectile.extraUpdates;
            }

            Vector2 center = projectile.Center;
            bool flag = false;
            float num = 25000f;
            int num2 = -1;
            ActiveEntityIterator<NPC>.Enumerator enumerator = Main.ActiveNPCs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NPC current = enumerator.Current;
                float num3 = current.width / 2 + current.height / 2;
                if (current.CanBeChasedBy(projectile) && projectile.WithinRange(current.Center, distanceRequired + num3) && (!respectIFrames || (projectile.localNPCImmunity[current.whoAmI] <= 0 && projectile.localNPCImmunity[current.whoAmI] != -1 && current.immune[projectile.owner] <= 0)))
                {
                    float num4 = Vector2.Distance(current.Center, projectile.Center);
                    if (respectIFrames && Projectile.perIDStaticNPCImmunity[projectile.type][current.whoAmI] > Main.GameUpdateCount)
                    {
                        num4 += 1600f;
                    }

                    if (num4 < num && (ignoreTiles || Collision.CanHit(projectile.Center, 1, 1, current.Center, 1, 1)))
                    {
                        num = num4;
                        num2 = current.whoAmI;
                    }
                }
            }

            if (num2 != -1)
            {
                center = Main.npc[num2].Center;
                flag = true;
            }

            if (flag)
            {
                projectile.extraUpdates = projectile.asuw().defExtraUpdates + 1;
                Vector2 vector = (center - projectile.Center).SafeNormalize(Vector2.UnitY);
                projectile.velocity = (projectile.velocity * inertia + vector * homingVelocity) / (inertia + 1f);
                projectile.asuw().HomingTarget = num2;
            }
            else
            {
                projectile.extraUpdates = projectile.asuw().defExtraUpdates;
            }
        }

        public static void HomeInOnSelectedNPC(Projectile projectile, NPC target, bool ignoreTiles = true, float homingVelocity = 0.5f, float maxSpeed = 10f, float inertia = 0.985f, float overspeedReduction = 0.95f, bool accelerate = false)
        {
            if (target != null && (ignoreTiles || Collision.CanHit(projectile.Center, 1, 1, target.Center, 1, 1)))
            {
                Vector2 vector = (target.Center - projectile.Center).SafeNormalize(Vector2.UnitX);
                if (projectile.velocity.Length() < maxSpeed)
                {
                    projectile.velocity = projectile.velocity * inertia + vector * homingVelocity;
                }
                else
                {
                    projectile.velocity *= overspeedReduction;
                }

                projectile.asuw().HomingTarget = target.whoAmI;
            }

            if (target == null && projectile.velocity.Length() < maxSpeed && accelerate)
            {
                projectile.velocity *= 1.0055f;
            }
        }

        public static void ForceNetUpdate(this Projectile proj, bool ignoreCurrentNetSpam = true)
        {
            proj.netUpdate = true;
            if (proj.netSpam >= 10 || ignoreCurrentNetSpam)
            {
                proj.netSpam = 0;
            }
        }


    }
}
