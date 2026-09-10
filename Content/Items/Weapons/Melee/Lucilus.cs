using asuw.Content.Projectiles;
using asuw.Content.Projectiles.GBFstuff;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static log4net.Appender.RollingFileAppender;
using static Mono.CompilerServices.SymbolWriter.CodeBlockEntry;

namespace asuw.Content.Items.Weapons.Melee
{
	public class Lucilus : ModItem
	{
        //TODO add weapon icon, more skills, sprite in progress by noho
        int tipe = 0;
		float FlipSwords = 1;
		float armRot = 0;
		public float armRotAmnt = 20f;
		//wow
		float idleTime0 = 0;
        float idleTime1 = 0;
        float idleTime2 = 0;

        int[] projs = new int[3] { ModContent.ProjectileType<Lusilly2>(), ModContent.ProjectileType<Lusilly3>() , ModContent.ProjectileType<Lusilly1>()};
        
        public override string Texture => "asuw/Assets/Blank";

        public static SoundStyle hit4 = new("asuw/Content/Sounds/HitGut");

        public override void SetDefaults()
		{
			Item.damage = 1300;
			Item.ArmorPenetration = 10;
			Item.DamageType = DamageClass.Melee;
			Item.width = 106;
			Item.height = 40;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.shootSpeed = 40f;
			Item.shoot = ModContent.ProjectileType<Lusilly2>();
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = null;
			Item.autoReuse = true;
			Item.knockBack = 7;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();

		}

		public override bool AltFunctionUse(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<RedSaw>()] < 1 ? true : false;



        public override void UpdateInventory(Player player)
        {
			if (armRotAmnt < 20f) armRotAmnt++;
			
			if(idleTime0 > 0) idleTime0--;
            if (idleTime1 > 0) idleTime1--;
            if (idleTime2 > 0) idleTime2--;

        }

        public override float UseSpeedMultiplier(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<RedSaw>()] > 0 ? 1.5f : 1f;

        public override void HoldItem(Player player)
        {
			player.asuw().mouseWorldListener = true;

			float rotTo = (MathHelper.ToRadians(160f * player.direction * FlipSwords) * player.gravDir) + player.asuw().mouseRotationFromPlayer;
			armRot = armRot.RotTowards(rotTo, MathHelper.ToRadians(armRotAmnt));

			Vector2 posX = Vector2.UnitX * player.direction * -30;
			Vector2 posY = Vector2.UnitY * -90;


			if (Main.myPlayer == player.whoAmI)
			{
                if (player.ownedProjectileCounts[projs[0]] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<RedSaw>()] < 1)
                {
					Vector2 pos = player.MountedCenter + posX * 2.5f + posY * 0.2f;
                    if (idleTime0 == 0) Projectile.NewProjectile(player.GetSource_ItemUse(Item), pos, Vector2.Zero, projs[0], Item.damage, Item.knockBack, player.whoAmI, 1);
                }
                if (player.ownedProjectileCounts[projs[1]] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<RedSaw>()] < 1)
                {
                    Vector2 pos = player.MountedCenter + posX * 3.3f + posY * 0.2f;
                    if (idleTime1 == 0) Projectile.NewProjectile(player.GetSource_ItemUse(Item), pos, Vector2.Zero, projs[1], Item.damage, Item.knockBack, player.whoAmI, 1);
                }
                if (player.ownedProjectileCounts[projs[2]] < 1)
				{
					if (player.controlUseItem)
						player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot - MathHelper.ToRadians(90f));

                    Vector2 pos = player.MountedCenter + posX + posY;
					if (idleTime2 == 0)Projectile.NewProjectile(player.GetSource_ItemUse(Item), pos, Vector2.Zero, projs[2], Item.damage, Item.knockBack, player.whoAmI, 1);

				}
				
			}
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
			{
				if (tipe != 2) tipe = 2;
                Projectile.NewProjectile(source, position, velocity.normalize() * 20f, ModContent.ProjectileType<RedSaw>(), damage / 8, knockback, player.whoAmI);
            }
			else
			{
				if (tipe < 2)
				{
					if (player.ownedProjectileCounts[ModContent.ProjectileType<RedSaw>()] < 1)
					{
                        if (tipe == 0) idleTime0 = 100;
                        if (tipe == 1) idleTime1 = 100;
                        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, FlipSwords, 0, 1);
					}
                    tipe++;
                }
				else
				{
					tipe = 0;
					FlipSwords *= -1;
                    idleTime2 = 100;
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, FlipSwords, 0, 1);
                }
				
			}

            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = projs[tipe];
			if (type == ModContent.ProjectileType<Lusilly1>()) damage *= 2;

         
        }

    }

}
