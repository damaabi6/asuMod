
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles.Filler
{
    public class ProjectionDashFront : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        public ref float timer => ref Projectile.ai[0];

        public ref float size => ref Projectile.ai[2];

        public ref float tipe => ref Projectile.ai[1];
        public override string Texture => "asuw/Assets/Blank";
        
       

        public override void SetDefaults()
        {
            Projectile.width = 30; // The width of projectile hitbox
            Projectile.height = 30; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Default;                             // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 30; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 170; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
          
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            
            Projectile.spriteDirection = player.direction;

            if (timer < 19)
            {
                size = 15;
            }
            else
            {
                size--;
           
            }
         
            Projectile.Center = (player.MountedCenter + player.velocity.SafeNormalize(Vector2.One) * 2 * size * (tipe * 1.5f)) + new Vector2(0,-5);

            Projectile.netUpdate = true;
            timer++;
        }

 
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("asuw/Content/Textures/PlayerProjections", AssetRequestMode.ImmediateLoad).Value;


            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
                effects = SpriteEffects.None;
            }
            else
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, 0);

            Vector2 drawPos = (Projectile.position - Main.screenPosition) + drawOrigin; 
                Color color = Projectile.GetAlpha(lightColor);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            

            return false;
        }
      
    }

    
}

