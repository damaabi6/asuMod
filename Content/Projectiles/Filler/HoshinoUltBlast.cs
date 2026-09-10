using asuw.Content.Buffs;
using asuw.Content.Dusts;
using asuw.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace asuw.Content.Projectiles.Filler
{
    public class HoshinoUltBlast : ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        SoundStyle boom = new("asuw/Content/Sounds/HoshinoBlast");
        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 380; // The width of projectile hitbox
            Projectile.height = 380; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged;                             // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 23; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 0; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {



             
        }

        public override void OnSpawn(IEntitySource source)
        {
            
            SoundEngine.PlaySound(boom with { Volume = 0.75f }, Projectile.Center);

       

            for (int i = 0; i <= 25; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpSparkDust>(), new Vector2(0, -18).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(0.8f, 2.3f);
                dust.color = Color.DeepPink;

                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpSparkDust>(), new Vector2(0, -7).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust2.noGravity = false;
                dust2.scale = Main.rand.NextFloat(0.8f, 2.3f);
                dust2.color = Color.Pink;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= Owner.GetModPlayer<AsuPlayer>().butterflyHairpin ? 1.20f : 1f;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
           
          

        }

        public override void OnKill(int timeLeft)
        {

           
        }
    }
}

