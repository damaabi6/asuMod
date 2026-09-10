using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Projectiles;
using asuw.Content.Projectiles.Filler;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using asuw.Content.Global;

namespace asuw.Content.Items.Weapons
{
	public class ProjectionSorcery : ModItem
	{
        //TODO rework and polishing, need actual sprite
        public static readonly SoundStyle glass1 = new("asuw/Content/Sounds/glass1") { Volume = 0.7f, MaxInstances = 1 };
        public static readonly SoundStyle glass2= new("asuw/Content/Sounds/glass2") { Volume = 0.47f, MaxInstances = 1 };
        public static readonly SoundStyle glass3 = new("asuw/Content/Sounds/glass3") { Volume = 0.7f, MaxInstances = 1 };
        public static readonly SoundStyle glass4 = new("asuw/Content/Sounds/glass4") { Volume = 0.7f, MaxInstances = 1 };
        public static readonly SoundStyle glass5 = new("asuw/Content/Sounds/glass5") { Volume = 0.7f, MaxInstances = 1 };
        public static readonly SoundStyle glass6 = new("asuw/Content/Sounds/glass6") { Volume = 0.7f, MaxInstances = 1 };
        public override void SetDefaults()
		{
			Item.damage = 70;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<ProjectPunch>();
			Item.shootSpeed = 30;
			Item.noMelee = true;
			Item.noUseGraphic = true;



        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
            if (target.asuw().projected == false && !player.asuw().ProjectionDashing)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<ProjectionScreen>(), Item.damage, 1, player.whoAmI);
                target.asuw().projectedtick = 120;
            }
            
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
                player.AddBuff(ModContent.BuffType<ProjectionDashCD>(), (player.GetModPlayer<AsuPlayer>().butterflyHairpin ? 420 : 360));
                Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<ProjectionDash>(), damage, 1, player.whoAmI);
               

                for (int i = 1; i < 9; i ++)
				{
                    Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<ProjectionDashFront>(), 1, 1, player.whoAmI,0,i,1);
                }
            }
			else
			{
				Vector2 pos = player.MountedCenter + new Vector2(Main.rand.NextFloat(-25, 25), Main.rand.NextFloat(-25, 25));
				float ran = Main.rand.NextFloat(60, 80);

                Vector2 vectortocursor = player.asuw().mouseWorld - pos;
                Projectile.NewProjectile(source, pos, vectortocursor.SafeNormalize(Vector2.One) * ran, type, damage, knockback, player.whoAmI);
            }

            return false;


		}

		public override void HoldItem(Player player)
		{
		}


        }
}
