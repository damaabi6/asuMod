using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons
{
	public class Honor : ModItem
	{
        //TODO total rework, better rocket sprite
        int timer = 0;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplaceAll("ff00ff", Utils.Hex3(AsuPlayer.SpecialMoveColorWS1));
            tooltips.FindAndReplaceAll("ff11ff", Utils.Hex3(AsuPlayer.ColorWSP));
            tooltips.FindAndReplaceAll("ff22ff", Utils.Hex3(AsuPlayer.ColorWSA));
            tooltips.FindAndReplaceAll("ff33ff", Utils.Hex3(AsuPlayer.SpecialMoveColorWSC));
            tooltips.FindAndReplaceAll("ffssff", Utils.Hex3(Color.DarkRed));
        }
        public override void SetDefaults()
		{
			Item.damage = 138;
			Item.crit = 0;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 60;
			Item.height = 34;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.shootSpeed = 40f;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Bullet;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Lime;
			Item.UseSound = new SoundStyle("asuw/Content/Sounds/Haru") { Volume = 0.3f, PitchVariance = 0.2f, MaxInstances = 30 };
			Item.autoReuse = true;
            Item.knockBack = 2f;


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
			return new Vector2(-13f, 0f);
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

     
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (type == ProjectileID.Bullet)
			{
				type = ProjectileID.BulletHighVelocity;
			}

           
         

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

            
			Vector2 pos = new Vector2(position.X, position.Y + Main.rand.NextFloat(-800f, -900f));

            Vector2 vectorToCursor = Main.MouseWorld - pos;
            Vector2 vectorToPlayer = player.Center - pos;

            
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
            {
        
                SoundEngine.PlaySound(new SoundStyle("asuw/Content/Sounds/Warn") { Volume = 1.2f, PitchVariance = 0.2f, MaxInstances = 5 });
                  
       
                Projectile.NewProjectile(source, pos, vectorToPlayer.SafeNormalize(Vector2.One) * 60, ModContent.ProjectileType<RudalAw>(), damage , knockback, player.whoAmI);
                for (int i = 0; i < 16; i++)
				{
                    Projectile.NewProjectile(source, pos, (vectorToCursor.SafeNormalize(Vector2.One) * Main.rand.Next(25, 40)).RotatedByRandom(0.34f), ModContent.ProjectileType<Rudal>(), damage + 25, knockback, player.whoAmI);
                }

                player.AddBuff(ModContent.BuffType<MakotoCooldown>(), (player.GetModPlayer<AsuPlayer>().butterflyHairpin ? 360 : 300));


                timer = 0;
                return false;
            }
            else
            {
                if (timer % 2 == 0)
                {
                    Projectile.NewProjectile(source, pos, vectorToCursor.SafeNormalize(Vector2.One) * 35, ModContent.ProjectileType<Rudal>(), damage * 2, knockback, player.whoAmI);
                    timer = 0;
                }

            
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                timer++;

                
            }

            return false;

        }
	}
}
