using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.IO;
using System.Collections.Generic;
using asuw.Effects;

namespace asuw.Content.Items.Ammos
{
    public class HitscanProjinf : ModProjectile
    {   
        //not actually hitscan
        //infinite piercing
        Player player => Projectile.GetOwner();
        public override string Texture => "asuw/Assets/Blank";
        const float speed = 20;
        ref float width => ref Projectile.ai[0]; 
        ref float height => ref Projectile.ai[1];
        ref float lifeTime => ref Projectile.ai[2];
        ref float time => ref Projectile.localAI[0];
        public override void SetDefaults()
        {
            AsuUtils.FriendlySetDefaults(Projectile, DamageClass.Ranged, true, -1);
            Projectile.width = Projectile.height = 14;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 20;
            Projectile.timeLeft = 50;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadUInt16();
        }
        public override bool PreAI()
        {
            if(Projectile.velocity.Length() != speed)
                Projectile.velocity = Projectile.velocity.normalize() * speed;
            if (lifeTime > 0)
            {
                if (time < lifeTime)
                    Projectile.timeLeft = 2;
                time++;
            }
            return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center + Projectile.velocity.normalize() * (-width);
            Vector2 end = start + Projectile.velocity.normalize() * (width);
            float collisionPoint = 0f;
            float collisionWidth = height;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
    }

    public class HitscanProj : ModProjectile
    {
        //not actually hitscan
        Player player => Projectile.GetOwner();
        public override string Texture => "asuw/Assets/Blank";
        const float speed = 20;
        ref float width => ref Projectile.ai[0];
        ref float height => ref Projectile.ai[1];
        ref float lifeTime => ref Projectile.ai[2];
        ref float time => ref Projectile.localAI[0];
        public override void SetDefaults()
        {
            AsuUtils.FriendlySetDefaults(Projectile, DamageClass.Ranged, true, 1);
            Projectile.width = Projectile.height = 14;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 20;
            Projectile.timeLeft = 60;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadUInt16();
        }
        public override bool PreAI()
        {
            if (Projectile.velocity.Length() != speed)
                Projectile.velocity = Projectile.velocity.normalize() * speed;
            if (lifeTime > 0)
            {
                if (time < lifeTime)
                    Projectile.timeLeft = 2;
                time++;
            }
            return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center + Projectile.velocity.normalize() * (-width);
            Vector2 end = start + Projectile.velocity.normalize() * (width);
            float collisionPoint = 0f;
            float collisionWidth = height;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
    }

    public class HitscanOnceProj : ModProjectile
    {
        //actually hitscan
        Player player => Projectile.GetOwner();
        public override string Texture => "asuw/Assets/Blank";
        const float speed = 20;
        ref float width => ref Projectile.ai[0];
        ref float height => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            AsuUtils.FriendlySetDefaults(Projectile, DamageClass.Ranged, false, -1);
            Projectile.width = Projectile.height = 14;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 20;
            Projectile.timeLeft = 20;
        }
        public override bool PreAI()
        {
            Projectile.velocity *= 0;
            return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center + Vector2.UnitX * -(width / 2f);
            Vector2 end = Projectile.Center + Vector2.UnitX * (width / 2f);
            float collisionPoint = 0f;
            float collisionWidth = height;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
    }
}
