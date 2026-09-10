using asuw.Content.Projectiles;
using ExampleMod.Content.Rarities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons
{
	public class Mika : ModItem
	{
        //TODO total rework
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplaceAll("ff00ff", Utils.Hex3(AsuPlayer.SpecialMoveColorWS1));
            tooltips.FindAndReplaceAll("ff11ff", Utils.Hex3(AsuPlayer.ColorWSP));
        }
        public override void SetDefaults()
		{

			if (ModLoader.TryGetMod("CalamityMod", out Mod CalamityMod) && CalamityMod.TryFind("CosmicPurple", out ModRarity CosmicPurple)) 
			{
				Item.damage = 81;
                Item.ArmorPenetration = 5;
                Item.rare = CosmicPurple.Type;
            }
			else {
                Item.damage = 18;
                Item.ArmorPenetration = 1;
                Item.rare = ModContent.RarityType<Mikas>();
            }
				
			Item.crit = 96;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.width = 34;
			Item.height = 34;
			Item.useTime = 4;
			Item.useAnimation = 4;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.noMelee = true;
			Item.shootSpeed = 60f;
			Item.shoot = ModContent.ProjectileType<Mikafist>();
			Item.value = Item.buyPrice(silver: 1);
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.scale = 0.9f;
            Item.noUseGraphic = true;
            Item.knockBack = 5f;
        }
		public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            if (ModLoader.TryGetMod("CalamityMod", out Mod CalamityMod) && CalamityMod.TryFind("LifeAlloy", out ModItem LifeAlloy) && CalamityMod.TryFind("AscendantSpiritEssence", out ModItem AscendantSpiritEssence) && CalamityMod.TryFind("CosmicAnvil", out ModTile CosmicAnvil))
            {
                recipe.AddIngredient(LifeAlloy, 5);
                recipe.AddIngredient(AscendantSpiritEssence, 5);
                recipe.AddIngredient(ItemID.FragmentVortex, 10);
                recipe.AddIngredient(ItemID.FragmentNebula, 10);
                recipe.AddTile(CosmicAnvil);
            }
            else
            {
                recipe.AddIngredient(ItemID.FragmentVortex, 18);
                recipe.AddIngredient(ItemID.FragmentNebula, 18);
                recipe.AddTile(TileID.LunarCraftingStation);
            }
            recipe.Register();
        }


	
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			type = 0;
        }

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			Vector2 pos = new Vector2(position.X + Main.rand.NextFloat(-100f, 100f), position.Y + Main.rand.NextFloat(-100f, 100f));
            Vector2 pos2 = new Vector2(position.X + Main.rand.NextFloat(-50f, 50f), position.Y + Main.rand.NextFloat(-50f, 50f));

            Projectile.NewProjectile(source, pos, velocity, ModContent.ProjectileType<Mikafist>(), damage * 2, knockback, player.whoAmI);
            Projectile.NewProjectile(source, pos, velocity, ModContent.ProjectileType<Mikafist2>(), damage * 2, knockback, player.whoAmI);
            Projectile.NewProjectile(source, pos2, velocity, ModContent.ProjectileType<Mikafist>(), damage , knockback, player.whoAmI);
            Projectile.NewProjectile(source, pos2, velocity, ModContent.ProjectileType<Mikafist2>(), damage , knockback, player.whoAmI);
            return true;
		}
	}
}
