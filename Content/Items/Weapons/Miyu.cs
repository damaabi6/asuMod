
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons
{
	public class Miyu : ModItem
	{
        //TODO total rework, better sprite
        public override void SetDefaults()
		{
			Item.damage = 670;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Bullet;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Purple;

            Item.UseSound = new SoundStyle("asuw/Content/Sounds/Miyu") { Volume = 0.7f, PitchVariance = 0.2f, MaxInstances = 30 };
			Item.autoReuse = true;
			Item.noMelee = true;
            Item.knockBack = 5f;



			
		

        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 45;

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-19f, 3f);
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 89f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}

            type = ModContent.ProjectileType<Projectiles.Miyuu>(); ;
			damage *= 2;

            
        }



    }
}

