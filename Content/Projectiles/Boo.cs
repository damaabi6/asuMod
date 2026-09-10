using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles
{
    public class Boo : ModProjectile
    {




        public override void SetDefaults()
        {
            Projectile.width = 1050; // The width of projectile hitbox
            Projectile.height = 1050; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 24; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }


       

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (ModLoader.TryGetMod("CalamityMod", out Mod CalamityMod) && CalamityMod.TryFind<ModBuff>("WitherDebuff", out ModBuff WitherDebuff) && CalamityMod.TryFind<ModBuff>("DemonicFlames", out ModBuff DemonicFlames))

                
            {
                target.AddBuff(WitherDebuff.Type, 1200);
                target.AddBuff(BuffID.ShadowFlame, 1200);

                CalamityMod.Call("SetDebuffVulnerability", target, "heat", true);
                CalamityMod.Call("SetDebuffVulnerability", target, "sickness", true);
                CalamityMod.Call("SetDebuffVulnerability", target, "cold", true);
                CalamityMod.Call("SetDebuffVulnerability", target, "electricity", true);
                CalamityMod.Call("SetDebuffVulnerability", target, "water", true);

                // Must specify an NPC, debuff type as a string, and whether to add or remove a vulnerability as a bool
            }
            else
            {
                target.AddBuff(BuffID.ShadowFlame, 1200);
            }
        }
        
    }
           
           
}

