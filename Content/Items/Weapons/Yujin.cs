
using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Projectiles;
using asuw.Content.Projectiles.Filler;
using asuw.Content.Projectiles.InfoAndChargeBar;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons
{ 
	public class Yujin : ModItem
    {
        //TODO rework, move projectile here
        public float UpOrDown = 0;
        public float npcTimer = 0;
        public float ShiDMGInc = 0;
        public NPC firstHit;
		public override void SetStaticDefaults()
		{
			ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
		}
		public int heal = 0;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplaceAll("ff00ff", Utils.Hex3(AsuPlayer.SpecialMoveColorWS1));
            tooltips.FindAndReplaceAll("ff11ff", Utils.Hex3(Color.Yellow));
            tooltips.FindAndReplaceAll("ff44ff", Utils.Hex3(Color.DarkRed));
            tooltips.FindAndReplaceAll("ff22ff", Utils.Hex3(AsuPlayer.ColorWSA));
            tooltips.FindAndReplaceAll("ff33ff", Utils.Hex3(AsuPlayer.SpecialMoveColorWSC));
        }

        public override void SetDefaults()
		{
			Item.damage = 74;
			Item.DamageType = DamageClass.Melee;
			Item.width = 120;
			Item.height = 120;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.channel = true;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Shi>();
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BreakerBlade);
            recipe.AddIngredient(ItemID.PalladiumBar, 12);
            recipe.AddIngredient(ItemID.SoulofNight, 8);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.BreakerBlade);
            recipe2.AddIngredient(ItemID.CobaltBar, 12);
            recipe.AddIngredient(ItemID.SoulofNight, 8);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }

        public override bool AltFunctionUse(Player player)
        {
            if (player.GetModPlayer<AsuPlayer>().WeaponCooldown)
            { return false; }
            else
            {
                return true;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<BoundaryOfDeath>(), damage * 2, knockback, player.whoAmI);
                return false;
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 0,  UpOrDown == 1? 1 : 0);
            }
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            //reset when BoD triggers
            int bod = ModContent.ProjectileType<BoundaryOfDeath4>();
            if (Main.myPlayer == player.whoAmI)
                if (player.ownedProjectileCounts[bod] > 0) ShiDMGInc = 0f;

            if (heal > 0) //ur pretty much unkillable
            {
                AsuUtils.HealPlayer(player, 1);
                heal--;
            }
            
            //dmgInc (or reduction)
            if (ShiDMGInc < -44f)
            { ShiDMGInc = -44f; }
            if (ShiDMGInc > 44f)
            { ShiDMGInc = 44f; }

            if (player.HeldItem != Item) heal = 0;
        }

        public override void HoldItem(Player player)
        {
            player.AddBuff(BuffID.PotionSickness, 1800);
        }
	}
}
