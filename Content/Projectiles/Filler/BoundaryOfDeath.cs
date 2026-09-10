
using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Items.Weapons;
using asuw.Content.Projectiles.Healing;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection.Metadata;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace asuw.Content.Projectiles
{
    public class BoundaryOfDeath : ModProjectile
    {
        
        Player player => Main.player[Projectile.owner];

        public override string Texture => "asuw/Content/Textures/shislash";
        int ran = 1;
        public bool stop;

        private ref float Timer => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 121; // The width of projectile hitbox
            Projectile.height = 121; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 10; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
           
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Vector2 vectorToCursor = player.asuw().mouseWorld - Projectile.Center;
            player.velocity = vectorToCursor.SafeNormalize(Vector2.One) * 100;
            player.AddBuff(ModContent.BuffType<ShiCooldown>(), player.GetModPlayer<AsuPlayer>().butterflyHairpin ? 420 : 360);
            player.statLife -= player.statLife / 2;
            ran = Main.rand.Next(4);
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Yujin y) 
                y.heal = (int)((float)(player.statLifeMax2 - player.statLife) / (player.statLife > (player.statLifeMax2 / 3) ? 2f : 1.6f));


        }
        public override void AI()
        {

            Projectile.rotation = player.velocity.ToRotation();
            Projectile.Center = player.Center;

            Projectile.direction = player.direction;
            Projectile.spriteDirection = Projectile.direction;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Yujin yu)
            {
                if(yu.ShiDMGInc < 44)yu.ShiDMGInc -= 1f;
                if (yu.ShiDMGInc >= 44 || yu.ShiDMGInc <= -44) ran = 0;
            }


               
            player.immune = true;
            player.immuneNoBlink = true;
            player.immuneTime = 30;
            for (int k = 0; k < player.hurtCooldowns.Length; k++)
                player.hurtCooldowns[k] = player.immuneTime;


            player.asuw().LungingDown = true;
            player.mount?.Dismount(player);
            player.RemoveAllGrapplingHooks();
            
           
            if (Timer > 5 && Timer <= 8)
               player.velocity *= 0.45f; 


         

            //for (int i = 0; i < 4; i++)
            //{
            //    float ran = Main.rand.NextFloat(0.1f, 0.3f);
            //    float vecran = Main.rand.NextFloat(-20f, 20f);
            //    Particle spark2 = new LineParticle(player.Center + new Vector2(vecran, vecran), player.velocity * -1 * ran, false, 40, Main.rand.NextFloat(1f, 2.5f), Color.DarkRed);
            //    GeneralParticleHandler.SpawnParticle(spark2);
            //    Particle spark3 = new AltLineParticle(player.Center + new Vector2(vecran * -1, vecran * -1), player.velocity * -1 * ran, false, 40, Main.rand.NextFloat(0.5f, 2f), Color.Red);
            //    GeneralParticleHandler.SpawnParticle(spark3);
            //}

           

            Projectile.netUpdate = true;
            Timer++;

           

        }

        public override void OnKill(int timeLeft)
        {
            
            player.asuw().LungingDown = false;
            
        }

        public override bool PreDraw(ref Color lightColor)
        {

            SpriteEffects effects;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D slashglow = ModContent.Request<Texture2D>("asuw/Content/Textures/shislashglow", AssetRequestMode.ImmediateLoad).Value;

            Vector2 origin = new Vector2(0, texture.Height / 2f);

            if (Projectile.spriteDirection > 0)
            {
                effects = SpriteEffects.None;
            }
            else
            {

                effects = SpriteEffects.FlipVertically;
            }

            

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.spriteBatch.Draw(slashglow, Projectile.Center - Main.screenPosition, default, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0);



            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ( texture.Width * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 45f * Projectile.scale, ref collisionPoint);


        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

          

            if (ran == 0 && stop == false && Projectile.numHits == 0)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<BoundaryOfDeath4>(), Projectile.damage * 10, Projectile.knockBack, Projectile.owner);
                stop = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (player.GetModPlayer<AsuPlayer>().butterflyHairpin)
            { modifiers.FinalDamage += 0.2f; }
        }
       
        
        
    }
           
           
}

