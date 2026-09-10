using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles
{
    public class IronFireBoomPlayer: ModProjectile
    {
        
        Player player => Main.player[Projectile.owner];
        
        public override string Texture => "asuw/Assets/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 2; // The width of projectile hitbox
            Projectile.height = 2; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 10; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
           
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.ArmorPenetration = 999;
            
           
            
        }
      
       
        public override void AI()
        {
            Projectile.velocity = player.velocity;
           
        }

       
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            float size = (MathHelper.Lerp(target.width, target.height, 0.5f) / 100);
          

           
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers = modifiers with { Dodgeable = false };
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is IronLotus il)
            {
                if (il.IronFlameLevel > 0)
                {
                    modifiers.SetMaxDamage(player.statLife / il.IronFlameLevel);
                }
                else
                { modifiers.SetMaxDamage(player.statLife); }

            }
            modifiers.Knockback *= 0;
            modifiers.ScalingArmorPenetration += 1f;
        }


    }
           
           
}

