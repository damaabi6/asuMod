
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles.Filler
{
    public class ShaterredProjectSparks : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        public ref float timer => ref Projectile.ai[0];

        public int rotir;

        public ref float tipe => ref Projectile.ai[1];

        public ref float scale => ref Projectile.ai[2];

        public override string Texture => "asuw/Assets/Blank";
        
       

        public override void SetDefaults()
        {
            Projectile.width = 36; // The width of projectile hitbox
            Projectile.height = 31; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Default;                             // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 30; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.scale = Main.rand.NextFloat(0.3f, 0.7f) + (scale / 2);
        }
        public override void OnSpawn(IEntitySource source)
        {
            rotir = Main.rand.Next(-5, 5);
        }
        public override void AI()
        {
 
            Projectile.velocity *= 0.93f;

            Projectile.rotation = MathHelper.ToRadians(rotir * timer);
            Projectile.netUpdate = true;
            timer++;
        }
         
 
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>($"asuw/Content/Textures/Extra/SparksShaterProj{tipe}", AssetRequestMode.AsyncLoad).Value;


            SpriteEffects effects = SpriteEffects.None;

          

            Vector2 drawOrigin = new Vector2(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 drawPos = (Projectile.Center - Main.screenPosition); 
                Color color = Projectile.GetAlpha(lightColor);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            

            return false;
        }
      
    }

    
}

