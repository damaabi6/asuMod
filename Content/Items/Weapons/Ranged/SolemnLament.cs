using asuw.Content.Projectiles;
using asuw.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Xml.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons.Ranged
{
    public class SolemnLament : ModItem
    {
        //TODO total rework, move projectiles here
        public int timer = 0;
        public int AmmoSavedPercent = 60;

        public int[] Ammo = new int[2] { 10, 10 };
        public bool[] shoot = new bool[2] { false, false };
        int[] proj = new int[2] { ModContent.ProjectileType<Solemn>(), ModContent.ProjectileType<Lament>() };
        int index = 0;
		
        public static readonly SoundStyle ShootW = new("asuw/Content/Sounds/Weapons/Ranged/solemn2") { Volume = 0.4f, MaxInstances = 4 };
        public static readonly SoundStyle ShootB = new("asuw/Content/Sounds/Weapons/Ranged/lament2") { Volume = 0.4f, MaxInstances = 4 };
        public static readonly SoundStyle DingW = new("asuw/Content/Sounds/ButterFlyMan_StongAtk_White") { Volume = 0.5f, MaxInstances = 4 };
        public static readonly SoundStyle DingB = new("asuw/Content/Sounds/ButterFlyMan_StongAtk_Black") { Volume = 0.5f, MaxInstances = 4 };
        public static readonly SoundStyle Reload = new("asuw/Content/Sounds/Weapons/Ranged/SolemnLamentReload") { Volume = 0.4f, MaxInstances = 2, PitchVariance = 0.2f };

        public override void SetStaticDefaults()
        {
			ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
		{
			Item.damage = 47;
			Item.DamageType = DamageClass.Ranged;
			Item.knockBack = 8f;
			Item.width = 220;
			Item.height = 134;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 40f;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Bullet;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.UseSound = null;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.channel = true;

		}

        
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

        public override bool CanUseItem(Player player) => (Ammo[index] > 0);
        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) > 22;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            int dir = velocity.X > 0 ? 1 : -1;
            position += velocity.RotatedBy(MathHelper.ToRadians(-6 * dir)) * 2;
            position += velocity.normalize() * -50;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            shoot[index] = true;
            SoundEngine.PlaySound((index == 0 ? ShootW : ShootB) with { MaxInstances = 2 }, player.Center);
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            index = index == 0 ? 1 : 0;
            return false;
		}

       
        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                player.asuw().mouseRotationListener = true;
                player.asuw().mouseWorldListener = true;

                if (player.ownedProjectileCounts[proj[0]] < 1)
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, player.asuw().mouseNormalFromPlayer, proj[0], Item.damage, Item.knockBack, player.whoAmI);
                if (player.ownedProjectileCounts[proj[1]] < 1)
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, player.asuw().mouseNormalFromPlayer, proj[1], Item.damage, Item.knockBack, player.whoAmI);
            }
        }
		
	}

    public class Lament : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.localNPCHitCooldown = 6;
        }
        Player player => Projectile.GetOwner();
        ref float time => ref Projectile.ai[2];

        SolemnLament heldWeapon()
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SolemnLament sola)
                return sola;
            else
                return null;
        }
        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        const int index = 1;
        float recoil = 0;
        int reloadDur = 60;
        int reloadRanDir = 1;
        public override void AI()
        {
            if (heldWeapon() != null && !player.dead)
            {
                Projectile.timeLeft = 2;

                int Ammo = heldWeapon().Ammo[index];

                player.heldProj = Projectile.whoAmI;

                Projectile.velocity = player.asuw().mouseNormalFromPlayer;
                player.ChangeDir(Projectile.direction);
                float recoilRot = +MathHelper.ToRadians(recoil * -Projectile.direction);
                if (Ammo > 0 || recoil > 10f)
                {
                    if (heldWeapon().shoot[index])
                    {
                        heldWeapon().shoot[index] = false;
                        recoil = 30;

                        heldWeapon().Ammo[index]--;
                    }
                    Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot;
                    player.SetHandRotFront(Projectile.rotation);
                    time = 0;
                }
                else
                {

                    if (time < reloadDur / 2)
                    {
                        if (time == 0)
                            reloadRanDir = Main.rand.NextBool() ? 1 : -1;
                        if (time == (int)(reloadDur / 4))
                            SoundEngine.PlaySound(SolemnLament.Reload, Projectile.Center);
                        float rotTo = MathHelper.ToRadians(430 * reloadRanDir);
                        float handRotTo = MathHelper.ToRadians(70 * reloadRanDir);
                        float lerp = AsuUtils.QuartInOut(time / (reloadDur / 2));
                        Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot + rotTo * lerp;
                        player.SetHandRotFront(Projectile.velocity.ToRotation() + recoilRot + handRotTo * lerp);
                    }
                    else
                    {
                        float rotAmnt = MathHelper.ToRadians(70 * -reloadRanDir);
                        float lerp = AsuUtils.SineOut((time - (reloadDur / 2)) / (reloadDur / 2));
                        Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot + MathHelper.ToRadians(430 * reloadRanDir) + rotAmnt * lerp;
                        player.SetHandRotFront(Projectile.velocity.ToRotation() + recoilRot + MathHelper.ToRadians(70 * reloadRanDir) + rotAmnt * lerp);
                    }

                    if (time < reloadDur)
                        time++;
                    else
                        heldWeapon().Ammo[index] = 10;
                }
                Projectile.spriteDirection = Projectile.direction;
                Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
                if (recoil > 0) recoil *= 0.85f;
            }
            else
                Projectile.Kill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new Vector2(texture.Width * 0.13f, texture.Height * 0.5f);
            float rotOffset = MathHelper.ToRadians(Projectile.direction == 1 ? 45 : -45);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            
            return false;
        }
    }

    public class Solemn : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.localNPCHitCooldown = 6;
        }
        Player player => Projectile.GetOwner();
        ref float time => ref Projectile.ai[2];

        SolemnLament heldWeapon()
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SolemnLament sola)
                return sola;
            else
                return null;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        const int index = 0;
        float recoil = 0;
        int reloadDur = 60;
        int reloadRanDir = 1;
        public override void AI()
        {
            if (heldWeapon() != null && !player.dead)
            {
                Projectile.timeLeft = 2;

                int Ammo = heldWeapon().Ammo[index];

                Projectile.velocity = player.asuw().mouseNormalFromPlayer;
                player.ChangeDir(Projectile.direction);
                float recoilRot = +MathHelper.ToRadians(recoil * -Projectile.direction);
                if (Ammo > 0 || recoil > 10f)
                {
                    if (heldWeapon().shoot[index])
                    {
                        heldWeapon().shoot[index] = false;
                        recoil = 30;

                        heldWeapon().Ammo[index]--;
                    }

                    Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot;
                    player.SetHandRotBack(Projectile.rotation);
                    time = 0;
                }
                else
                {
                    if (time < (reloadDur / 2))
                    {
                        if (time == 0)
                            reloadRanDir = Main.rand.NextBool() ? 1 : -1;
                        if (time == (int)(reloadDur / 4))
                            SoundEngine.PlaySound(SolemnLament.Reload, Projectile.Center);
                        float rotTo = MathHelper.ToRadians(430 * reloadRanDir);
                        float handRotTo = MathHelper.ToRadians(70 * reloadRanDir);
                        float lerp = AsuUtils.QuartInOut(time / (reloadDur / 2));
                        Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot + rotTo * lerp;
                        player.SetHandRotBack(Projectile.velocity.ToRotation() + recoilRot + handRotTo * lerp);
                    }
                    else
                    {
                        float rotAmnt = MathHelper.ToRadians(70 * -reloadRanDir);
                        float lerp = AsuUtils.SineOut((time - (reloadDur / 2)) / (reloadDur / 2));
                        Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot + MathHelper.ToRadians(430 * reloadRanDir) + rotAmnt * lerp;
                        player.SetHandRotBack(Projectile.velocity.ToRotation() + recoilRot + MathHelper.ToRadians(70 * reloadRanDir) + rotAmnt * lerp);
                    }

                    if (time < reloadDur)
                        time++;
                    else
                        heldWeapon().Ammo[index] = 10;
                }
                Projectile.spriteDirection = Projectile.direction;
                Projectile.Center = player.GetBackHandPositionImproved(player.compositeBackArm);
                if (recoil > 0) recoil *= 0.85f;
            }
            else
                Projectile.Kill();

        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        //called in offhandLayer
        public static void drawOffhand(ref PlayerDrawSet drawInfo, Player player, Color lightColor)
        {
            Projectile projectile = null;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<Solemn>() && proj.owner == player.whoAmI)
                {
                    projectile = proj;
                    break;
                }

            }

            if (projectile == null)
                return;

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            Vector2 origin = new Vector2(texture.Width * 0.13f, texture.Height * 0.5f);
            DrawData draw = new DrawData(texture, projectile.Center - Main.screenPosition, default, lightColor * projectile.Opacity, projectile.rotation, origin, projectile.scale, effects, 0);

            drawInfo.DrawDataCache.Add(draw);
        }
    }

}
