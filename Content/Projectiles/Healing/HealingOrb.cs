using asuw.Content.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles.Healing
{
    public class HealingOrb : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Projectile.HealingProjectile(40, Projectile.owner, 6f, 15f);
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0f, 0f, 100);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].position.X -= Projectile.velocity.X * 0.2f;
            Main.dust[dust].position.Y += Projectile.velocity.Y * 0.2f;
        }
    }
}