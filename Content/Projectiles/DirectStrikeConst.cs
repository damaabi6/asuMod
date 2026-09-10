using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles
{
    public class DirectStrikeConst : ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        public bool invalidTarget => (Projectile.ai[0] < 0f || Projectile.ai[0] > 199f);

        // You can set Projectile AI 1 & 2 to the X/Y velocity that you would like to launch the target, even if the direct strike deals no damage.
        // This lets Direct Strikes be used as "Direct Nudges" which deal no damage but push something around.
        ref float dmgLimit => ref Projectile.ai[1];
        ref float buffType => ref Projectile.ai[2];
        // If you set Knockback to anything below zero, the custom knockback will be able to effect enemies that normally ignore knockback
        public bool hasStongDisplacement = false;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            if (Projectile.knockBack < 0)
            {
                hasStongDisplacement = true;
                Projectile.knockBack = 0;
            }

            // If the target is moving VERY fast, direct strikes spawned on top of them can actually miss
            // Setting a target will guarantee hits on said target by teleporting the projectile onto their center every frame
            // Setting a target will guarantee hits on said target by teleporting the projectile onto them every frame
            if (!invalidTarget)
                Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center;
        }

        // If the AI parameter isn't a valid NPC slot, it can hit anything. Otherwise it can only hit one NPC.
        public override bool? CanHitNPC(NPC target)
        {
            if (invalidTarget || Projectile.ai[0] == target.whoAmI)
                return null;
            return (bool?)false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                NPC target = Main.npc[(int)Projectile.ai[0]];
                return true;
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            modifiers.ScalingArmorPenetration += 1f;
            modifiers.SourceDamage *= 0;
            modifiers.FlatBonusDamage += (int)dmgLimit;
            modifiers.SetMaxDamage((int)dmgLimit);
            modifiers.SetCrit();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (buffType > 0 && ModContent.GetModBuff((int)buffType) != null)
                target.AddBuff((int)buffType, 120);

        }
    }
}

