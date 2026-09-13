using asuw.Content.Dusts;
using asuw.Content.Items.Ammos;
using asuw.Content.Projectiles;
using asuw.Content.Rarities;
using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
        //TODO skill
        public int timer = 0;
        public int AmmoSavedPercent = 60;

        public int[] Ammo = new int[2] { 10, 10 };
        public int AltAmmo = 10;
        public bool[] shoot = new bool[2] { false, false };
        public bool useAmmo = false;
        int[] proj = [ModContent.ProjectileType<Solemn>(), ModContent.ProjectileType<Lament>()];
        int index = 0;
        int indexAlt = 0;

        public static readonly SoundStyle ShootW = new("asuw/Content/Sounds/Weapons/Ranged/solemn2") { Volume = 0.4f, MaxInstances = 4 };
        public static readonly SoundStyle ShootB = new("asuw/Content/Sounds/Weapons/Ranged/lament2") { Volume = 0.4f, MaxInstances = 4 };
        public static readonly SoundStyle DingW = new("asuw/Content/Sounds/Weapons/Ranged/DingW") { Volume = 0.5f, MaxInstances = 4 };
        public static readonly SoundStyle DingB = new("asuw/Content/Sounds/Weapons/Ranged/DingB") { Volume = 0.5f, MaxInstances = 4 };
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
			Item.rare = ModContent.RarityType<BW>();
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
        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) > 40;
        public override bool AltFunctionUse(Player player) => AltAmmo > 0 && Ammo[indexAlt] > 0;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<HitscanProjinf>();
            
            int dir = velocity.X > 0 ? 1 : -1;
            position += velocity.RotatedBy(MathHelper.ToRadians(-6 * dir)) * 2;
            position += velocity.normalize() * -50;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
            {
                shoot[indexAlt] = true;
                SoundEngine.PlaySound((indexAlt == 0 ? DingW : DingB) with { MaxInstances = 2 }, player.Center);
                player.SetScreenAngle(Main.rand.NextFloat(2, 3) * (Main.rand.NextBool() ? 1 : -1));
                player.SetScreenZoomInto(0.1f, player.mouseWorld(), 0.1f);
                player.SetDarkLayer(0.1f);
                Projectile.NewProjectile(source, player.mouseWorld(), velocity, ModContent.ProjectileType<HitscanOnceProj>(), damage * 5, knockback, player.whoAmI, 200, 200);
                player.itemTime *= 4;
                player.itemAnimation *= 4;
                Dust explosion = Dust.NewDustPerfect(player.mouseWorld(), ModContent.DustType<ShatteredExplosionNP>(), Vector2.Zero, 0, indexAlt == 0 ? Color.White : Color.Black, Main.rand.NextFloat(0.14f, 0.2f));
                explosion.noGravity = true;
                for (int i = 1; i <= Main.rand.Next(9,13); i++)
                {
                    int butterflyType = indexAlt == 0 ? ModContent.DustType<ButterflyWhite>() : ModContent.DustType<ButterflyBlack>();
                    Vector2 butterflyPos = player.mouseWorld() + Utils.NextVector2Circular(Main.rand, 40, 40);
                    Vector2 butterflyVel = AsuUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(8, 12);
                    Dust butterflies = Dust.NewDustPerfect(butterflyPos, butterflyType, butterflyVel, 0, Color.White, Main.rand.NextFloat(1.5f, 2));
                    butterflies.noGravity = true;
                }
                AltAmmo--;
                indexAlt = indexAlt == 0 ? 1 : 0;
            }
            else
            {
                shoot[index] = true;
                useAmmo = true;
                SoundEngine.PlaySound((index == 0 ? ShootW : ShootB) with { MaxInstances = 2 }, player.Center);
                int Damage = damage;
                float ai0 = 0;
                float ai1 = 0;
                float ai2 = 0;
                Vector2 finalVelocity = velocity.RotatedByRandom(0.02f); //for consistent velocity for the 2 hitscan projectiles
                if (type == ModContent.ProjectileType<HitscanProjinf>())
                {
                    ai0 = 100; // length
                    ai1 = 70; // width
                    ai2 = 50; // lifetime (20 maxupdates)
                    Damage = (int)(Damage * 0.8f);
                    Projectile.NewProjectile(source, position, finalVelocity, ModContent.ProjectileType<HitscanProj>(), damage, knockback, player.whoAmI, ai0, ai1, ai2);
                }
                Projectile.NewProjectile(source, position, finalVelocity, type, Damage, knockback, player.whoAmI, ai0, ai1, ai2);
                AltAmmo++;
                index = index == 0 ? 1 : 0;
                indexAlt = index;
            }
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
        float smearOP = 0;
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
                        shotEffects();
                        if (heldWeapon().useAmmo)
                        {
                            heldWeapon().useAmmo = false;
                            heldWeapon().Ammo[index]--;
                        }
                    }
                    Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot;
                    player.SetHandRotFront(Projectile.rotation);
                    time = 0;
                    smearOP = 0;
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
                        float v = AsuUtils.PingPong(time, reloadDur / 2);
                        smearOP = AsuUtils.QuartIn(v);
                    }
                    else
                    {
                        float rotAmnt = MathHelper.ToRadians(70 * -reloadRanDir);
                        float lerp = AsuUtils.SineOut((time - (reloadDur / 2)) / (reloadDur / 2));
                        Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot + MathHelper.ToRadians(430 * reloadRanDir) + rotAmnt * lerp;
                        player.SetHandRotFront(Projectile.velocity.ToRotation() + recoilRot + MathHelper.ToRadians(70 * reloadRanDir) + rotAmnt * lerp);
                        if (smearOP > 0) smearOP = 0;
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
            string texFlip = reloadRanDir * Projectile.spriteDirection == -1 ? "F" : string.Empty;
            Texture2D smearTex = ModContent.Request<Texture2D>($"asuw/Assets/Particles/Twirl1NoBg{texFlip}", AssetRequestMode.ImmediateLoad).Value;
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new Vector2(texture.Width * 0.13f, texture.Height * 0.5f);
            float rotOffset = MathHelper.ToRadians(Projectile.direction == 1 ? 45 : -45);
            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(smearTex, Projectile.Center - Main.screenPosition, default, Color.Black * Projectile.Opacity * smearOP, Projectile.rotation, smearTex.Size() / 2f, Projectile.scale * 0.24f, effects, 0);
            Main.spriteBatch.ExitShaderRegion();
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        void shotEffects()
        {
            Vector2 dustPos = Projectile.Center + player.velocity + Projectile.velocity.normalize().RotatedBy(-0.15f * Projectile.direction) * 65;
            Color dustColor = Color.Black;
            Dust dust = Dust.NewDustPerfect(dustPos, ModContent.DustType<LightRingNP>(), (Projectile.velocity.normalize() * 0.2f), 0, dustColor, 4f);
            dust.noGravity = true;
            dust.fadeIn = 0.2f;
            Dust dust2 = Dust.NewDustPerfect(dustPos + Projectile.velocity.normalize() * -25, ModContent.DustType<MuzzleFlashNP>(), (Projectile.velocity.normalize() * 4), 0, dustColor, 1f);
            dust2.noGravity = true;
            dust2.fadeIn = 0.5f;
            for (int i = 1; i <= 8; i++)
            {
                Vector2 butterflyVel = i < 4 ? (Projectile.velocity.normalize() * Main.rand.NextFloat(7, 21)).RotatedByRandom(0.4f) : (Projectile.velocity.normalize() * Main.rand.NextFloat(5, 16)).RotatedBy(MathHelper.PiOver4 * (i % 2 == 0 ? 1 : -1)).RotatedByRandom(0.4f);
                Dust dust3 = Dust.NewDustPerfect(dustPos, ModContent.DustType<ButterflyBlack>(), butterflyVel, 0, Color.White, Main.rand.NextFloat(0.5f, 1));
                dust3.noGravity = true;
            }
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
        float smearOP = 0;
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
                        shotEffects();
                        if (heldWeapon().useAmmo)
                        {
                            heldWeapon().useAmmo = false;
                            heldWeapon().Ammo[index]--;
                        }
                    }

                    Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot;
                    player.SetHandRotBack(Projectile.rotation);
                    time = 0;
                    smearOP = 0;
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
                        float v = AsuUtils.PingPong(time, reloadDur / 2);
                        smearOP = AsuUtils.QuartIn(v);
                    }
                    else
                    {
                        float rotAmnt = MathHelper.ToRadians(70 * -reloadRanDir);
                        float lerp = AsuUtils.SineOut((time - (reloadDur / 2)) / (reloadDur / 2));
                        Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot + MathHelper.ToRadians(430 * reloadRanDir) + rotAmnt * lerp;
                        player.SetHandRotBack(Projectile.velocity.ToRotation() + recoilRot + MathHelper.ToRadians(70 * reloadRanDir) + rotAmnt * lerp);
                        smearOP = 0;
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

            if (projectile.ModProjectile is Solemn solemn)
            {
                string texFlip = solemn.reloadRanDir * projectile.spriteDirection == -1 ? "F" : string.Empty;
                Texture2D smearTex = ModContent.Request<Texture2D>($"asuw/Assets/Particles/Twirl1{texFlip}", AssetRequestMode.ImmediateLoad).Value;
                SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
                Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
                Vector2 origin = new Vector2(texture.Width * 0.13f, texture.Height * 0.5f);
                DrawData drawSmear = new DrawData(smearTex, projectile.Center - Main.screenPosition, default, Color.White with { A = 0 } * projectile.Opacity * solemn.smearOP, projectile.rotation, smearTex.Size() / 2f, projectile.scale * 0.24f, effects, 0);
                DrawData draw = new DrawData(texture, projectile.Center - Main.screenPosition, default, lightColor * projectile.Opacity, projectile.rotation, origin, projectile.scale, effects, 0);

                drawInfo.DrawDataCache.Add(drawSmear);
                drawInfo.DrawDataCache.Add(draw);
            }
        }

        void shotEffects()
        {
            Vector2 dustPos = Projectile.Center + player.velocity + Projectile.velocity.normalize().RotatedBy(-0.15f * Projectile.direction) * 65;
            Color dustColor = Color.White;
            Dust dust = Dust.NewDustPerfect(dustPos, ModContent.DustType<LightRingNP>(), (Projectile.velocity.normalize() * 0.2f), 0, dustColor, 4f);
            dust.noGravity = true;
            dust.fadeIn = 0.2f;
            Dust dust2 = Dust.NewDustPerfect(dustPos + Projectile.velocity.normalize() * -25, ModContent.DustType<MuzzleFlashNP>(), (Projectile.velocity.normalize() * 4), 0, dustColor, 1f);
            dust2.noGravity = true;
            dust2.fadeIn = 0.5f;
            for (int i = 1; i <= 8; i++)
            {
                Vector2 butterflyVel = i < 4 ? (Projectile.velocity.normalize() * Main.rand.NextFloat(7, 21)).RotatedByRandom(0.4f) : (Projectile.velocity.normalize() * Main.rand.NextFloat(5, 16)).RotatedBy(MathHelper.PiOver4 * (i % 2 == 0 ? 1 : -1)).RotatedByRandom(0.4f);
                Dust dust3 = Dust.NewDustPerfect(dustPos, ModContent.DustType<ButterflyWhite>(), butterflyVel, 0, Color.White, Main.rand.NextFloat(0.5f, 1));
                dust3.noGravity = true;
            }
        }
    }

}
