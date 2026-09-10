using asuw.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons
{
	public class Kayoko : ModItem
	{
		//TODO total rework
		public override void SetDefaults()
		{
			Item.damage = 100;
			Item.ArmorPenetration = 5;
			Item.crit = 50;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 102;
			Item.height = 34;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.shootSpeed = 40f;
			Item.shoot = ProjectileID.BulletHighVelocity;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.scale = 0.8f;

		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-12f, 4f);
		}



		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
				Item.reuseDelay = 60;
			else
				Item.reuseDelay = 2;

			return base.CanUseItem(player);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			if (player.altFunctionUse == 2)
			{
				SoundEngine.PlaySound(new SoundStyle("asuw/Content/Sounds/Roar") { Volume = 0.7f, PitchVariance = 0.1f, MaxInstances = 1 });
                Vector2 launchVelocity = new Vector2(0, 0);
				Projectile.NewProjectile(source, position, launchVelocity, ModContent.ProjectileType<Fear>(), damage, knockback, player.whoAmI);
				return false;
			}
			else
			{
                SoundEngine.PlaySound(new SoundStyle("asuw/Content/Sounds/Kayok") { Volume = 0.5f, PitchVariance = 0.2f, MaxInstances = 30 });
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            }
            return false;


		}


	}
}
