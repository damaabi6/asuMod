using asuw.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons
{
	public class Sakura : ModItem
	{
        //TODO total rework
        public int frameCounter = 0;
        public int frame = 0;

        public static readonly SoundStyle Hit1 = new("asuw/Content/Sounds/Hanafuda/Hit1") { Volume = 0.3f };
        public static readonly SoundStyle Hit2 = new("asuw/Content/Sounds/Hanafuda/Hit2") { Volume = 0.3f };
        public static readonly SoundStyle Swing1 = new("asuw/Content/Sounds/Hanafuda/Slash1") { Volume = 0.5f };
        public static readonly SoundStyle Swing2 = new("asuw/Content/Sounds/Hanafuda/Slash2") { Volume = 0.5f };
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 24));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
		{
            Item.damage = 310;
			Item.ArmorPenetration = 5;
			Item.crit = 30;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 60;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.shootSpeed = 80f;
			Item.shoot = ModContent.ProjectileType<Hanafuda>();
           // Item.useAmmo = AmmoID.Bullet;
			Item.value = Item.buyPrice(silver: 1);
			//Item.autoReuse = true;
            Item.noUseGraphic = true; // Stops the item from drawing in your hands, for the aforementioned reason
            Item.channel = true; // Important as the projectile checks if the player channels
            Item.rare = ItemRarityID.Purple;
            Item.knockBack = 7f;


        }
		public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            if (ModLoader.TryGetMod("CalamityMod", out Mod CalamityMod) && CalamityMod.TryFind("LifeAlloy", out ModItem LifeAlloy) && CalamityMod.TryFind("DivineGeode", out ModItem DivineGeode))
            {
                recipe.AddIngredient(LifeAlloy, 5);
                recipe.AddIngredient(ItemID.FragmentNebula, 10);
                recipe.AddIngredient(ItemID.LunarBar, 5);

            }
            else
            {
                
                recipe.AddIngredient(ItemID.FragmentNebula, 10);
                recipe.AddIngredient(ItemID.LunarBar, 5);
                
            }
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2f, 0f);
		}

	}
}
