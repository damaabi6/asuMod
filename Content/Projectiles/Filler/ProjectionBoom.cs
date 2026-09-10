
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

namespace asuw.Content.Projectiles.Filler
{
    public class ProjectionBoom: ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        public ref float tipe => ref Projectile.ai[1];
        private NPC targethom
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        Player player => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 200; // The width of projectile hitbox
            Projectile.height = 200; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 28; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
           
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = 0;
            Projectile.usesLocalNPCImmunity = true;

            
        }
        
        public override void AI()
        {
            if (targethom == null)
            {
                targethom = AsuUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 400);
            }

            if (targethom != null && !targethom.CanBeChasedBy())
            {
                targethom = null;
            }

            if (targethom == null)
                return;

            Projectile.Center = targethom.Center;

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if(target != targethom)
            {
                modifiers.SetMaxDamage(1);
            }
            base.ModifyHitNPC(target, ref modifiers);
        }
     


    }
           
           
}

