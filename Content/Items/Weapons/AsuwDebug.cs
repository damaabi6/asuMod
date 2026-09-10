using asuw.Content.Projectiles;
using asuw.Content.Projectiles.GBFstuff;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons
{
	public class AsuwDebug : ModItem
	{
        public override void SetDefaults()
		{
			Item.damage = 11;
			Item.ArmorPenetration = 5;
			Item.DamageType = DamageClass.Melee;
			Item.width = 106;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.shootSpeed = 20f;
			//Item.shoot = ModContent.ProjectileType<testRot>();
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item40;
			Item.autoReuse = true;

		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();

		}
     
    }


	
}
