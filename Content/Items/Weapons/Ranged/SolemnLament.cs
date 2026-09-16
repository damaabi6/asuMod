using asuw.Content.Dusts;
using asuw.Content.Global;
using asuw.Content.Items.Ammos;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Projectiles;
using asuw.Content.Rarities;
using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
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
    public class SolemnLament : ModItem, ILocalizedModType
    {
        //TODO skill
        public int AmmoSavedPercent = 30;
        bool screenEffects = true;
        public int[] Ammo = [ 10, 10 ];
        public int AltAmmo = 0;
        public int giveAltAmmoCount = 0;
        public bool[] shoot = [ false, false ];
        public bool useAmmo = false;
        int[] proj = [ModContent.ProjectileType<Solemn>(), ModContent.ProjectileType<Lament>()];
        int index = 0;
        int indexAlt = 0;

        public static readonly string soundPath = "asuw/Content/Sounds/Weapons/SolemnLament/";
        public static readonly SoundStyle ShootW = new($"{soundPath}solemn2") { Volume = 0.4f, MaxInstances = 4 };
        public static readonly SoundStyle ShootB = new($"{soundPath}lament2") { Volume = 0.4f, MaxInstances = 4 };
        public static readonly SoundStyle DingW = new($"{soundPath}DingW") { Volume = 0.5f, MaxInstances = 4 };
        public static readonly SoundStyle DingB = new($"{soundPath}DingB") { Volume = 0.5f, MaxInstances = 4 };
        public static readonly SoundStyle Reload = new($"{soundPath}SolemnLamentReload") { Volume = 0.4f, MaxInstances = 2, PitchVariance = 0.2f };

        public new string LocalizationCategory => "Items.Weapons.Ranged";
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
            Item.useTime = 9;
            Item.useAnimation = 9;
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
        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) > AmmoSavedPercent;
        public override bool AltFunctionUse(Player player) => AltAmmo > 0 && Ammo[indexAlt] > 0;

        public override bool CanRightClick() => Main.keyState.PressingShift();
        public override void RightClick(Player player)
        {
            screenEffects = !screenEffects;
            Item.NetStateChanged();
        }
        public override bool ConsumeItem(Player player) => false;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplace("ENABLED", screenEffects ? "ENABLED" : "DISABLED");
        }
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
                if (screenEffects)
                {
                    player.SetScreenAngle(Main.rand.NextFloat(2, 3) * (Main.rand.NextBool() ? 1 : -1));
                    player.SetScreenZoomInto(0.1f, player.mouseWorld(), 0.1f);
                    player.SetDarkLayer(0.1f);
                }
                Projectile altProj = Projectile.NewProjectileDirect(source, player.mouseWorld(), velocity, ModContent.ProjectileType<HitscanOnceProj>(), damage * 5, knockback, player.whoAmI, 200, 200);
                altProj.asuw().applySinking = true;
                altProj.asuw().benefitsFromSinking = true;
                player.itemTime *= 4;
                player.itemAnimation *= 4;
                Dust explosion = Dust.NewDustPerfect(player.mouseWorld(), ModContent.DustType<ShatteredExplosionNP>(), Vector2.Zero, 0, indexAlt == 0 ? Color.White : Color.Black, Main.rand.NextFloat(0.14f, 0.2f));
                explosion.noGravity = true;
                for (int i = 1; i <= Main.rand.Next(9, 13); i++)
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
                Vector2 finalVelocity = velocity.RotatedByRandom(0.02f); //for consistent velocity for the 2 "hitscan" projectiles
                if (type == ModContent.ProjectileType<HitscanProjinf>())
                {
                    ai0 = 100; // length
                    ai1 = 70; // width
                    ai2 = 50; // lifetime (20 maxupdates)
                    Damage = (int)(Damage * 0.8f);
                    Projectile.NewProjectile(source, position, finalVelocity, ModContent.ProjectileType<HitscanProj>(), Damage, knockback, player.whoAmI, ai0, ai1, ai2);
                }
                Projectile defaultProj = Projectile.NewProjectileDirect(source, position, finalVelocity, type, Damage, knockback, player.whoAmI, ai0, ai1, ai2);
                defaultProj.asuw().benefitsFromSinking = true;
                index = index == 0 ? 1 : 0;
                indexAlt = index;
                if (AltAmmo < 6)
                {
                    if (giveAltAmmoCount < 8)
                        giveAltAmmoCount++;
                    else
                    {
                        AltAmmo++;
                        giveAltAmmoCount = 0;
                    }
                }
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

                if (AsuKeybinds.WeaponSkill.JustPressed && player.ownedProjectileCounts[ModContent.ProjectileType<SLLMCoffin>()] < 1)
                {
                    int dir = player.asuw().mouseNormalFromPlayer.X > 0 ? 1 : -1;
                    Vector2 pos = player.MountedCenter + player.asuw().mouseNormalFromPlayer * 60 + player.asuw().mouseNormalFromPlayer.RotatedBy(-MathHelper.PiOver2 * dir) * 40;
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), pos, player.asuw().mouseNormalFromPlayer, ModContent.ProjectileType<SLLMCoffin>(), Item.damage, Item.knockBack, player.whoAmI);
                }
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
                dust3.fadeIn = 0.5f;
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
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            ShaderFunctions.DrawFancyNumbers(player.MountedCenter + Vector2.UnitY * -75, heldWeapon().AltAmmo, Color.White, 0.6f);
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
                dust3.fadeIn = 0.5f;
            }
        }
    }

    public class SLLMCoffin : ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.localNPCHitCooldown = -1;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        Player player => Projectile.GetOwner();
        ref float phase => ref Projectile.ai[1];
        ref float time => ref Projectile.ai[2];

        float xSquish = 1;
        float ySquish = 1;
        Vector2 offset = Vector2.Zero;
        float smashOP = 0;
        float smashScale = 1;
        bool refundAmmo = false;
        SolemnLament heldWeapon()
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SolemnLament sola)
                return sola;
            else
                return null;
        }
        public override void AI()
        {
            int phase0n2Dur = 40;
            int phase1Dur = 120;
            int dir = Projectile.velocity.X > 0 ? 1 : -1;

            if (phase == 0)
            {
                Projectile.timeLeft = 2;
                
                float spawnRot = time < 10 ? MathHelper.ToRadians(-70 * dir) : 0;
                offset = Projectile.velocity.normalize() * (time < 10 ? -20 : 40);
                Projectile.rotation = Projectile.velocity.ToRotation() + spawnRot;

                float squishT = 1 - AsuUtils.QuartOut((time - 10) / (phase0n2Dur - 10));
                ySquish = time < 10 ? 1 : 1 - (0.3f * squishT);
                if (time < 10)
                    Projectile.position += player.velocity;

                if (time == 10)
                {
                    smashOP = 1;
                    smashScale = 1;
                    playSound("Slams");
                }

                if (time < phase0n2Dur)
                    time++;
                else
                {
                    time = 0;
                    phase = 1;
                    Projectile.frame = 1;
                    playSound("Gongg");
                    playSound("Flapping");
                }
            }
            else if (phase == 1)
            {
                Projectile.timeLeft = 2;
                Projectile.rotation = Projectile.velocity.ToRotation();
                float squishT = 1 - AsuUtils.QuadOut(AsuUtils.ExpoOut(time / 20));
                xSquish = time < 20 ? 1 + (0.3f * squishT) : 1;
                Vector2 butterflyVel = (Projectile.velocity.normalize() * Main.rand.NextFloat(19, 31)).RotatedByRandom(0.45f);
                Vector2 butterflyPos = Projectile.Center + offset + Projectile.velocity * 43.5f + Projectile.velocity.RotatedBy(MathHelper.PiOver2 * dir) * Main.rand.Next(-40, 40);
                Dust dust3 = Dust.NewDustPerfect(butterflyPos, ModContent.DustType<ButterflyWhite>(), butterflyVel, 0, Color.White, Main.rand.NextFloat(0.7f, 1.3f));
                dust3.fadeIn = -0.5f;
                if(time % 10 == 0)
                {
                    Projectile.numHits = 0;
                    Projectile.ResetLocalNPCHitImmunity();
                }
                if (time < phase1Dur)
                    time++;
                else
                {
                    time = 0;
                    phase = 2;
                    Projectile.frame = 0;
                    playSound("Closes");
                }
            }
            else
            {
                if (time < phase0n2Dur)
                    Projectile.timeLeft = 2;

                Projectile.rotation = Projectile.velocity.ToRotation();
                float squishT = 1 - AsuUtils.ExpoOut((time / 10));
                xSquish = time < 10 ? 1 - (0.3f * squishT) : 1;
                Projectile.Opacity = 1f - AsuUtils.QuartIn(time / phase0n2Dur);
                time++;
            }

            Dust dusts = Dust.NewDustPerfect(Projectile.Center + offset + Utils.NextVector2Circular(Main.rand, 70, 70), Main.rand.NextBool() ? DustID.Granite : DustID.WhiteTorch, Vector2.UnitY * Main.rand.NextFloat(-4, -8), 0, default, Main.rand.NextFloat(0.8f, 1.2f));
            dusts.noGravity = true;
            dusts.noLight = true;

            if (heldWeapon() != null && refundAmmo)
            {
                refundAmmo = false;
                if(heldWeapon().AltAmmo < 8)
                heldWeapon().AltAmmo++;
            }
            smashOP = Math.Clamp(smashOP - 0.05f, 0, 1);
            smashScale *= 1 + (smashOP / 10f);
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => phase == 1 && time % 10 == 0;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity.normalize() * 800;
            float collisionPoint = 0f;
            float collisionWidth = 350;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.asuw().SinkingStack > 0)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), (int)((float)damageDone * (1 + 0.1f * target.asuw().SinkingStack)), 1f, player.whoAmI, target.whoAmI, 1);
                target.asuw().SinkingStack--;
                if (heldWeapon() != null)
                    refundAmmo = true;
                float offsetAmnt = 7 * AsuGlobalNPC.generalScale(target.Size);
                Vector2 dustOffset = Utils.NextVector2Circular(Main.rand, offsetAmnt, offsetAmnt);
                for(int i = 0; i < 2; i++)
                Dust.NewDustPerfect(target.Center + dustOffset, ModContent.DustType<ShatteredExplosionNP>(), Vector2.Zero, 0, i == 0 ? Color.White : Color.Black, 0.03f * (i == 0 ? 0.7f : 1) * AsuGlobalNPC.generalScale(target.Size));

            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage += 0.5f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D coffin = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Ranged/SolemnLamentCoffin", AssetRequestMode.AsyncLoad).Value;
            Texture2D smash = ModContent.Request<Texture2D>("asuw/Assets/Particles/ShatteredExplosionSquish", AssetRequestMode.AsyncLoad).Value;
            Rectangle rect = coffin.Frame(verticalFrames: 2, frameY: Projectile.frame);
            int dir = Projectile.velocity.X > 0 ? 1 : -1;
            SpriteEffects effects = dir == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Vector2 jitter = Projectile.frame == 0 ? Vector2.Zero : Utils.NextVector2Circular(Main.rand, 2, 2);
            Color color = Color.Lerp(Color.White, lightColor, 0.4f);
            for (int i = 0; i < 2; i++)
            {
                Color smashColor = i == 0 ? Color.Black : Color.White;
                ShaderFunctions.vertexColored(Main.spriteBatch, smashColor, smashColor, smashOP);
                Main.spriteBatch.Draw(smash, Projectile.Center - Main.screenPosition + offset + Projectile.velocity.RotatedBy(MathHelper.PiOver2 * dir) * 64, default, Color.White * smashOP, Projectile.velocity.RotatedBy(MathHelper.PiOver2).ToRotation(), smash.Size() / 2f, Projectile.scale * smashScale * (i == 0 ? 0.05f : 0.03f), i == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
                Main.spriteBatch.ExitShaderRegion();
            }
            Main.spriteBatch.Draw(coffin, Projectile.Center - Main.screenPosition + offset + jitter, rect, color * Projectile.Opacity, Projectile.rotation, rect.Size() / 2f, Projectile.scale * new Vector2(xSquish, ySquish), effects, 0);
            return false;
        }

        void playSound(string soundName)
        {
            SoundEngine.PlaySound(new SoundStyle($"{SolemnLament.soundPath}{soundName}"), Projectile.Center);
        }

    }
    

}
