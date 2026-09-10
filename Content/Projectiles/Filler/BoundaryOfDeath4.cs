using asuw.Content.Items.Weapons;
using asuw.Content.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;

namespace asuw.Content.Projectiles
{
    public class BoundaryOfDeath4 : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];

        public float scale = 1.5f;
        public bool stop = false;

        public override string Texture => "asuw/Content/Textures/ShiUlt";
        SoundStyle yuzin = new("asuw/Content/Sounds/Yuzin_Special_Atk");
        public override void SetDefaults()
        {
            Projectile.width = 426; // The width of projectile hitbox
            Projectile.height = 389; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
          
            Projectile.timeLeft = 120; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            
        }

       
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(yuzin with { Volume = 0.75f }, Projectile.Center);

        }
        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Timer++;
            if (scale > 1f)
            {  if (Timer % 2 == 0)
                { 
                    scale -= 0.1f;
                }
            }
            if (Timer % 2 == 0)
            {
                Projectile.alpha -= 30;
            }
           


           


        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.One * -10, ModContent.ProjectileType<BoundaryOfDeathUp>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
           Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.One * 10, ModContent.ProjectileType<BoundaryOfDeathDown>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
           
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Yujin y) y.heal = player.statLifeMax2 / 2;

           

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            var origin = new Vector2(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, origin, scale, SpriteEffects.None, 0);


            // Since we are doing a custom draw, prevent it from normally drawing
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetCrit();

            if (player.GetModPlayer<AsuPlayer>().butterflyHairpin)
            { modifiers.FinalDamage += 0.2f; }

            modifiers.ScalingArmorPenetration += 1f;

           
        }
     

    }
           
           
}

