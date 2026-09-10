using asuw.Content.Projectiles;
using asuw.Content.Projectiles.Filler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons
{
	public class Amiya : ModItem
	{
        //TODO total rework
        public int frameCounter = 0;
        public int frame = 0;
        public static Color mainColor = new Color(185, 24, 45);
        public override void SetStaticDefaults()
        {
            
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 16));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public static readonly SoundStyle Cast = new("asuw/Content/Sounds/AmiyaCast") { Volume = 0.6f , MaxInstances = 10};
        public static readonly SoundStyle pew = new("asuw/Content/Sounds/AmiyaPew") { Volume = 0.85f, MaxInstances = 3 };

        public override void SetDefaults()
        {
            Item.damage = 270;
            Item.crit = 2;
            Item.ArmorPenetration = 10;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.width = 34;
            Item.height = 34;
            Item.useTime = 83;
            Item.useAnimation = 83;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shootSpeed = 5f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.value = Item.buyPrice(silver: 1);
            Item.UseSound = new SoundStyle("asuw/Content/Sounds/AmiyaAtt") { Volume = 1f, PitchVariance = 0.2f, MaxInstances = 3 };
            Item.autoReuse = true;
            Item.knockBack = 10f;
            Item.noUseGraphic = true;

            if (ModLoader.TryGetMod("CalamityMod", out Mod CalamityMod) && CalamityMod.TryFind("CalamityRed", out ModRarity CalamityRed))
            {


                Item.rare = Item.rare = CalamityRed.Type;
            }
            else
            {
                Item.rare = ItemRarityID.Red;
            }

        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            if (ModLoader.TryGetMod("CalamityMod", out Mod CalamityMod) && CalamityMod.TryFind("ShadowspecBar", out ModItem ShadowspecBar) && CalamityMod.TryFind("AscendantSpiritEssence", out ModItem AscendantSpiritEssence) && CalamityMod.TryFind("DraedonsForge", out ModTile DraedonsForge))
            {
                recipe.AddIngredient(ShadowspecBar, 5);
                recipe.AddIngredient(AscendantSpiritEssence, 10);
                recipe.AddTile(DraedonsForge);
            }

            recipe.Register();
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

            type = ModContent.ProjectileType<AmiyaBall>();
            position += muzzleOffset;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
           
            for (int i = 0; i < 3; i++)
            {
                float ran = Main.rand.NextFloat(2f,4f);
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(40)) * ran, type, damage, knockback, player.whoAmI);

                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(30)) * ran, ModContent.ProjectileType<AmiyaBallBullet>(), damage, knockback, player.whoAmI);
            }

  
            if (player.direction > 0)
            {
                Vector2 cen = player.Center + (player.velocity * -1) + new Vector2(-50, -30);
                Projectile.NewProjectile(source, cen, new Vector2(0, 0), ModContent.ProjectileType<AmiyaBack>(), damage, knockback, player.whoAmI);
                Projectile.NewProjectileDirect(source, (player.Center + new Vector2(20, -1)), Vector2.Zero ,ModContent.ProjectileType<AmiyaBallHold>(), damage, knockback, player.whoAmI);
            }
            else if (player.direction < 0)
            {
                Vector2 cen = player.Center + (player.velocity * -1) + new Vector2(50, -30);
                Projectile.NewProjectile(source, cen, new Vector2(0, 0), ModContent.ProjectileType<AmiyaBack>(), damage, knockback, player.whoAmI);
                Projectile.NewProjectileDirect(source, (player.Center + new Vector2(-20, -1)), Vector2.Zero, ModContent.ProjectileType<AmiyaBallHold>(), damage, knockback, player.whoAmI);
            }
            


            return false;
        }


        //public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        //{

            //    Vector2 vel = velocity * -2f;
            //    Vector2 pos = player.Center + vel;
            //    Projectile.NewProjectile(source, pos, new Vector2(0,0), ModContent.ProjectileType<AmiyaBack>(), damage, knockback, player.whoAmI);

            //    for (int i = 0; i < 3; i++) {


            //          Projectile.NewProjectile(source, pos, velocity + new Vector2(Main.rand.NextFloat(-10f,10f), Main.rand.NextFloat(-10f, 10f)), ModContent.ProjectileType<CasterBall>(), damage , knockback, player.whoAmI);



            //    }

            //    return true;
            //}

   



    }
}
