using asuw.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons.Melee
{
    //TODO polishing
    public class Claw_W : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 14));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public static readonly SoundStyle Hit1 = new("asuw/Content/Sounds/Claw/Hit1") { Volume = 0.3f };
        public static readonly SoundStyle Hit2 = new("asuw/Content/Sounds/Claw/Hit2") { Volume = 0.3f };
        public static readonly SoundStyle Hit3 = new("asuw/Content/Sounds/Claw/Hit1") { Volume = 0.3f };
        public static readonly SoundStyle Hit4 = new("asuw/Content/Sounds/Claw/Hit2") { Volume = 0.3f };
        public static readonly SoundStyle Swing1 = new("asuw/Content/Sounds/Claw/Swing") { Volume = 0.5f };
        public static readonly SoundStyle Swing2 = new("asuw/Content/Sounds/Claw/Swing2") { Volume = 0.5f };
        public static readonly SoundStyle DrillHit = new("asuw/Content/Sounds/Claw/Drill") { Volume = 0.3f };
        public static readonly SoundStyle Drill = new("asuw/Content/Sounds/Claw/Drill2") { Volume = 0.3f };
        public override void SetDefaults()
        {
            Item.damage = 1000;
            Item.ArmorPenetration = 5;
            Item.crit = 20;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 1;
            Item.height = 1;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shootSpeed = 140f;
            Item.shoot = ModContent.ProjectileType<ClawEx>();
            // Item.useAmmo = AmmoID.Bullet;
            Item.value = Item.buyPrice(silver: 1);
            Item.UseSound = new SoundStyle("asuw/Content/Sounds/ClawSlash") { Volume = 0.2f, PitchVariance = 0f, MaxInstances = 3 };
            //Item.autoReuse = true;
            Item.noUseGraphic = true; // Stops the item from drawing in your hands, for the aforementioned reason
            Item.channel = true; // Important as the projectile checks if the player channels
            Item.rare = ItemRarityID.Purple;
            Item.knockBack = 5f;


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

        
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            SoundEngine.PlaySound(new SoundStyle("asuw/Content/Sounds/ClawWoosh") { Volume = 0.6f, PitchVariance = 0f, MaxInstances = 3 });

        }
        

        
            
        
    }
}
