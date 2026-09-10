using asuw.Content.Dusts;
using asuw.Content.Global;
using asuw.Content.Projectiles;
using asuw.Content.Projectiles.InfoAndChargeBar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons
{
	public class TheEnd : ModItem
	{
        //TODO total rework
        public NPC targetToHome = null;
        public bool flag = false;
        public bool boom = false;
        public bool skill = false;
        public bool homeIn = true;
        public int healeddAmount = 0;
        public static readonly SoundStyle ShootN = new("asuw/Content/Sounds/hinashot") { Volume = 0.6f, MaxInstances = 25, PitchVariance = 0.2f };
        public static readonly SoundStyle ShootB = new("asuw/Content/Sounds/hinashotboom") { Volume = 0.9f };
        public static readonly SoundStyle Exhaust = new("asuw/Content/Sounds/hinaexhaust");
        public static readonly SoundStyle Theme = new("asuw/Content/Sounds/theendtheme") { Volume = 0.5f };

        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.asuw.hjson' file.
        public override void SetDefaults()
		{
			Item.damage = 666;
			Item.ArmorPenetration = 20;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 198;
			Item.height = 50;
			Item.useTime = Item.useAnimation = 5; 
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.shootSpeed = 40f;
			Item.shoot = ModContent.ProjectileType<TheEndBullet>();
            Item.useAmmo = AmmoID.Bullet;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.knockBack = 7f;
            Item.channel = true;

		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
		public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 60;

       
        public override bool CanUseItem(Player player) => player.asuw().TheEndDestroyerOverheated ? false : true;

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
  
 			Vector2 muzzleOffset = (Vector2.Normalize(velocity) * 10f).RotatedByRandom(0.3f);

			position += muzzleOffset;
		}
        public override bool AltFunctionUse(Player player) => player.asuw().WeaponCooldown || player.asuw().TheEndDestroyerSkill || healeddAmount < 1800 ? false : true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
            {
                skill = true;
                healeddAmount -= 1800;
            }
            else if (!player.asuw().TheEndDestroyerSkill)
            { 
                flag = true;
              
            }
            return false;
        }
        public override void HoldItem(Player player)
        {
            
            int type = ModContent.ProjectileType<TheEndHeld>();
            int type2 = ModContent.ProjectileType<TheEndInfos>();
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.ownedProjectileCounts[type] < 1)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, (Main.MouseWorld - player.MountedCenter).normalize() * 12, type, Item.damage, 0, player.whoAmI);
                }
                if (player.ownedProjectileCounts[type2] < 1)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, (Main.MouseWorld - player.MountedCenter).normalize() * 12, type2, Item.damage, 0, player.whoAmI);
                }

              

            }

            Vector2 spawnpos = new Vector2(Main.rand.NextFloat(-100, 100), Main.rand.NextFloat(-100, 100));
            if (player.asuw().TheEndDestroyerSkill && player.asuw().Timer120 % 30 == 0)
            {
                Projectile.NewProjectile(Item.InheritSource(Item), player.MountedCenter + spawnpos, new Vector2(Main.rand.NextFloat(-1,1),Main.rand.NextFloat(-0.3f, 0.3f)), ModContent.ProjectileType<TheEndSkillNotes>(), 0, 0, player.whoAmI, Main.rand.Next(0, 4), spawnpos.X);
            }
        }

        

    }


    public class TheEndHeld : ModProjectile
    {
        Player player => Projectile.GetOwner();
        public float leverRot = 0;
        public Vector2 leverOfs;
        public Vector2 leverposofs;
        public float heat = 0;
        public bool overheated = false;
        public bool melshot = false;
        public bool overheatshot = false;
        public int shoothome = 0;
        public ref float cooldowntimer => ref Projectile.ai[0];
        public ref float timer => ref Projectile.ai[1];
        public int glowframe = 0;
        public float glowbrightness = 0;
        public int skillshotcount = 0;
        public float purplescale = 1f;
        public float purpleopacity = 0;
        public bool shootskill = false;


        public override string Texture => "asuw/Content/Textures/Extra/TheEndEmpty";
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.friendly = false;
            Projectile.timeLeft = 2;
        }
        public float ofs = 0;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ofs);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ofs = reader.ReadSingle();
        }
        public override void AI()
        {

            player.asuw().mouseRotationListener = true;
            Projectile.Center = player.GetDrawCenter();
            player.heldProj = Projectile.whoAmI;


            Texture2D tex = Projectile.GetTexture();

            leverOfs = Projectile.rotation.ToRotationVector2() * ((leverRot / 5) * Projectile.direction == 1 ? 1 : -1);
            leverposofs = Projectile.rotation.ToRotationVector2() * -37;

            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is TheEnd te)
            {
                 te.targetToHome = AsuUtils.FindTarget_HomingProj(Projectile, player.mouseWorld(), 3000);
                player.asuw().mouseWorldListener = true;
                int infDmgBonus = (int)((float)te.healeddAmount / 100f);

                if (te.targetToHome != null && !te.targetToHome.asuw().theEndTarget)
                {
                    te.targetToHome = null;
                }
                foreach (var n in Main.ActiveNPCs)
                {
                    if (te.targetToHome == null && n.asuw().theEndTarget && n.CanBeChasedBy())
                    {
                        if (AsuUtils.getDistance(n.Center, player.mouseWorld()) <= 3000)
                        {
                            te.targetToHome = n;
                        }
                    }
                }
                if (te.targetToHome != null && te.targetToHome.asuw().theEndTarget && te.homeIn)
                {
                   
                    float targetAngle = Projectile.AngleTo(te.targetToHome.Center);
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(12)).ToRotationVector2() * player.HeldItem.shootSpeed;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
                else
                {

                    float targetAngle = Projectile.AngleTo(player.asuw().mouseWorld);
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(20)).ToRotationVector2() * player.HeldItem.shootSpeed;
                    Projectile.rotation = Projectile.velocity.ToRotation();

                }

                if (te.flag)
                {
                    te.flag = false;
                    int dmgamount = (int)(7f * heat);
                    if (heat < 0.695) heat += 0.005f;
                    if (heat >= 0.69) overheatshot = true;
                    if (heat > 0.1f && dmgamount > 0 && player.statLife > dmgamount * 5)
                    {
                        player.statLife -= dmgamount;
                        CombatText.NewText(player.Hitbox, Color.Purple, $"-{dmgamount}");
                    }
                    glowbrightness += 0.007f;
                    glowframe++;
                    cooldowntimer = 0;
                    leverRot = -40;
                    ofs = -10;
                    shoothome++;
                    muzzleflash();
                    SoundEngine.PlaySound(TheEnd.ShootN, player.Center);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<TheEndBullet>(), (int)((float)Projectile.damage * (1 + heat)) + infDmgBonus, 3f, Projectile.owner, 0);
                
                    if (shoothome == 5 && !player.asuw().TheEndDestroyerSkill && heat < 0.7f)
                    {
                        for (int i = -2; i < 3; i++)
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(0.85f * i)), ModContent.ProjectileType<TheEndBullet>(), ((int)((float)Projectile.damage * (1 + heat)) + infDmgBonus) / 2, 3f, Main.myPlayer, 4);
                        }
                        shoothome = 0;
                    }
                }
                if (te.skill)
                {
                    te.skill = false;
                    overheatshot = false;
                    shoothome = 0;
                    cooldowntimer = 0;
                    skillshotcount = 0;
                    player.asuw().TheEndDestroyerSkill = true;
                    SoundEngine.PlaySound(TheEnd.Theme);
                }
                if (player.asuw().TheEndDestroyerSkill)
                {
                    if (player.controlUseItem && cooldowntimer >= 120) shootskill = true;
                    if (shootskill || cooldowntimer >= 240)
                    {
                        shootskill = false;
                        overheatshot = false;
                        skillshotcount++;
                        leverRot = -60;
                        ofs = -15;
                        cooldowntimer = 0;
                        glowbrightness = 1f;
                        heat = 0.69f;
                        muzzleflash();
                        player.Heal(player.statLifeMax2 / 3);
                        SoundEngine.PlaySound(TheEnd.ShootN, player.Center);
                        SoundEngine.PlaySound(TheEnd.ShootB, player.Center);
                        if (skillshotcount >= 3)
                        {
                            heat = 0.7f;
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<TheEndBullet>(), (int)((float)(Projectile.damage + infDmgBonus) * (player.asuw().butterflyHairpin ? 7.2f : 6)), 3f, Projectile.owner, 1, 0, 1);
                            overheatshot = false;
                            player.asuw().TheEndDestroyerSkill = false;
                            skillshotcount = 0;

                        }
                        for (int i = -10; i <= 11; i++)
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.ToRadians(0.6f * i)), Projectile.velocity.RotatedBy(MathHelper.ToRadians(0.08f * i)), ModContent.ProjectileType<TheEndBullet>(), (int)((float)(Projectile.damage + infDmgBonus) * (player.asuw().butterflyHairpin ? 3.6f : 3f)), 3f, Projectile.owner, 2, 0, 1);
                        }

                    }
                }
                Projectile.timeLeft = 2;
                if (player.asuw().TheEndDestroyerSkill)
                {
                    player.itemTime = 2;
                    player.itemAnimation = 2;
                }
                //overheating dusts
                float texwidth = tex.Width * 0.5f;
                if (Main.rand.NextFloat(heat) >= 0.2f)
                {
                    float dustscale = Main.rand.NextFloat(0.3f, 0.9f);
                    Dust.NewDust(Projectile.Center + Projectile.rotation.ToRotationVector2() * (Main.rand.NextFloat(-texwidth * 0.7f, texwidth * 1.3f)), 8, 8, DustID.Shadowflame, 0, Main.rand.NextFloat(-7, -2), 0, default, dustscale);
                    if (Main.rand.NextFloat(heat) >= 0.4f)
                    {
                        Dust.NewDust(Projectile.Center + Projectile.rotation.ToRotationVector2() * (Main.rand.NextFloat(-texwidth * 0.7f, texwidth * 1.3f)), 8, 8, DustID.Shadowflame, 0, Main.rand.NextFloat(-7, -2), 0, default, dustscale);
                    }
                }
                //smokes & cooldown heal
                if ((heat >= 0.1f && !player.controlUseItem && !player.asuw().TheEndDestroyerSkill) || player.asuw().TheEndDestroyerOverheated)
                {
                    if (timer % 2 == 0)
                    {
                        int healamount = (int)(10f * heat);
                        if (healamount > 0)
                        {
                            player.Heal(healamount);
                            te.healeddAmount += healamount;
                        }
                    }
                    bool fastSmoke = Main.rand.NextBool();
                    var smokeColor = Main.rand.NextBool() ? Color.Indigo : Color.DarkSlateBlue;
                    Vector2 smokeVel = Vector2.UnitY * (fastSmoke ? -15f : -6f);
                    float rotationAngle = Main.rand.NextFloat(-0.08f, 0.08f);
                    smokeVel = smokeVel.RotatedBy(rotationAngle) * Main.rand.NextFloat(0.1f, 1.0f);
                    float smokeScale = Main.rand.NextFloat(0.4f, 1.2f);
                    //SmallSmokeParticle smoke = new SmallSmokeParticle(Projectile.Center + Projectile.rotation.ToRotationVector2() * (Main.rand.NextFloat(-texwidth * 0.5f, texwidth * 1.5f)), smokeVel, Color.DimGray, smokeColor, smokeScale, 100);
                    //if (Main.rand.NextBool()) GeneralParticleHandler.SpawnParticle(smoke);
                    //if (heat > 0.4f) GeneralParticleHandler.SpawnParticle(smoke);

                    if (cooldowntimer == 6 && !player.asuw().TheEndDestroyerOverheated && !player.asuw().TheEndDestroyerSkill) SoundEngine.PlaySound(TheEnd.Exhaust with { Volume = 0.2f }, player.Center);
                }
                if (overheatshot && !player.controlUseItem)
                {
                    player.itemTime = 2;
                    player.itemAnimation = 2;
                }
                //overheatShot(nonskill)
                if (!player.controlUseItem && !player.asuw().TheEndDestroyerSkill && overheatshot && !player.asuw().TheEndDestroyerOverheated)
                {
                    overheatshot = false;
                    player.Heal(player.statLifeMax2 / 3);
                    te.healeddAmount += player.statLifeMax2 / 3;
                    heat = 0f;
                    glowbrightness += 0.007f;
                    glowframe++;
                    cooldowntimer = 0;
                    leverRot = -60;
                    ofs = -15;
                    muzzleflash();
                    SoundEngine.PlaySound(TheEnd.ShootN, player.Center);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<TheEndBullet>(), Projectile.damage * 2, 3f, Projectile.owner, 1);

                    for (int i = 0; i < 25; i++)
                    {
                        float texwidthhalf = tex.Width * 0.5f;
                        bool fastSmoke = Main.rand.NextBool();
                        var smokeColor = Main.rand.NextBool() ? Color.Indigo : Color.DarkSlateBlue;
                        Vector2 smokeVel = Vector2.UnitY * (fastSmoke ? -15f : -6f);
                        float rotationAngle = Main.rand.NextFloat(-0.08f, 0.08f);
                        smokeVel = smokeVel.RotatedBy(rotationAngle) * Main.rand.NextFloat(0.1f, 1.0f);
                        float smokeScale = Main.rand.NextFloat(0.4f, 1.2f);
                        //SmallSmokeParticle smoke = new SmallSmokeParticle(Projectile.Center + Projectile.rotation.ToRotationVector2() * (Main.rand.NextFloat(-texwidthhalf * 0.5f, texwidthhalf * 1.5f)), smokeVel, Color.DimGray, smokeColor, smokeScale, 100);
                        //GeneralParticleHandler.SpawnParticle(smoke);
                    }
                }
                if (AsuKeybinds.WeaponSkill.JustPressed)
                {
                    if (te.homeIn)
                    {
                        CombatText.NewText(player.Hitbox, Color.Indigo, "Auto Aim Deactivated");
                        te.homeIn = false;
                    }
                    else
                    {
                       
                        CombatText.NewText(player.Hitbox, Color.Indigo, "Auto Aim Activated");
                        te.homeIn = true;
                    }
                }
            }
            else
            {
                if (heat > 0)
                {
                    Projectile.timeLeft = 2;
                }
                if (overheatshot)
                {
                    overheatshot = false;
                }

            }
                cooldowntimer++;
                purplescale += 0.01f;
                purpleopacity += 0.02f;
                timer++;
                //lever movement
                if (leverRot < 0) leverRot += 2;
                //overheated
                if (heat >= 0.7f)
                {
                    heat = 0.7f;
                    player.asuw().TheEndDestroyerOverheated = true;
                    overheatshot = false;
                }

                if (heat <= 0) { heat = 0; player.asuw().TheEndDestroyerOverheated = false; }
                //cylinderbrightness
                if (glowbrightness < 0) glowbrightness = 0;
                if (glowframe > 2) glowframe = 0;
                if (glowbrightness > 1f) glowbrightness = 1f;
                //coolingdown
                if (cooldowntimer >= 10 && cooldowntimer % 15 == 0 && (heat > 0 || glowbrightness > 0))
                { heat -= 0.05f; glowbrightness -= 0.08f; }
                //cylinder thing keep rotating
                if ((!player.channel || player.asuw().TheEndDestroyerOverheated || player.asuw().TheEndDestroyerSkill) && (glowbrightness > 0 || heat > 0) && timer % 5 == 0) glowframe++;
                //purple glow
                if (timer % 30 == 0)
                { purplescale = 1f; purpleopacity = 0; }
                //recoil
                ofs *= 0.93f;
                Projectile.Center += Projectile.rotation.ToRotationVector2() * ofs;
                //change player direction
                player.ChangeDir(Projectile.direction);
                //player arm rotation
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(player.direction == 1 ? -45f : -135f));
                //timer reset
                if (timer >= 600) timer = 0;
                //death
                if (player.dead) Projectile.Kill();
                // skill effects
                if (player.asuw().TheEndDestroyerSkill)
                {
                    //Particle portal = new CustomSpark(player.Bottom, Vector2.UnitY * (Main.rand.NextBool() ? 1 : -2), "CalamityMod/Particles/BloomRing", false, 12, Main.rand.NextFloat(0.4f, 0.9f), Color.Indigo, new Vector2(0.1f, 1.5f), true, false, extraRotation: -MathHelper.PiOver2, noShrink: true);
                    //if (timer % 4 == 0) GeneralParticleHandler.SpawnParticle(portal);

                    float ranX = Main.rand.NextFloat(-150, 150);
                    float ranY = Main.rand.NextFloat(-80, -500);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(ranX, ranY), ModContent.DustType<SharpSparkDust>(), (Vector2.UnitY * 10).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 0.5f), 2);
                    dust.noGravity = true;
                    dust.velocity *= (Main.rand.NextBool() ? 0.5f : 0.9f);
                    dust.scale = Main.rand.NextFloat(0.95f, 1.25f) * (Main.rand.NextBool() ? 0.9f : 1);
                    dust.color = Main.rand.NextBool(5) ? Color.MediumPurple : Color.Indigo;
                }


                //debugging

            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is TheEnd te)
            {
                SpriteEffects effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
                Texture2D tex = Projectile.GetTexture();
                Texture2D lever = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/TheEndLever", AssetRequestMode.ImmediateLoad).Value;
                Texture2D purpleglow = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/TheEndBacklight", AssetRequestMode.ImmediateLoad).Value;
                Vector2 leverorigin = Projectile.direction == 1 ? new Vector2(lever.Width / 2, lever.Height) : new Vector2(lever.Width / 2, 0);
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;

                Color color = Color.Lerp(lightColor, Color.DarkViolet, heat);

                if (player.asuw().TheEndDestroyerSkill)
                {
                    for (int i = 1; i <= 24; i++)
                    {
                        float attackLerp = (float)Math.Pow((double)(Utils.GetLerpValue(60, 200, cooldowntimer * 2, true)), (double)(8));
                        float mult = MathHelper.Max(Utils.GetLerpValue(7, 0, i), Utils.GetLerpValue(17, 24, i));
                        float outspace = 6 * attackLerp;
                        Vector2 drawOffset = (((MathHelper.TwoPi * i / 24f).ToRotationVector2().RotatedBy(Projectile.rotation) * outspace) + Main.rand.NextVector2Circular(2, 2));
                        Color auraColor = Main.rand.NextBool() ? Color.MediumPurple with { A = 100 } * mult : Color.Indigo with { A = 100 } * mult;
                        Main.EntitySpriteDraw(purpleglow, drawPosition + drawOffset, null, auraColor, Projectile.rotation, purpleglow.Size() / 2f, Projectile.scale * player.gravDir, effects);
                    }
                }
                else
                {
                    Main.EntitySpriteDraw(purpleglow, drawPosition, null, Color.Fuchsia * purpleopacity * heat * 0.8f, Projectile.rotation, purpleglow.Size() / 2f, purplescale - 0.1f, effects);

                }
                Main.EntitySpriteDraw(tex, drawPosition, null, color, Projectile.rotation, tex.Size() / 2f, Projectile.scale, effects);

                Main.EntitySpriteDraw(lever, drawPosition + leverOfs + leverposofs, null, color, Projectile.rotation + MathHelper.ToRadians((leverRot + 40) * Projectile.direction), leverorigin, Projectile.scale, effects);
            }
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is TheEnd te)
            {
                SpriteEffects effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
                Texture2D Glow = ModContent.Request<Texture2D>($"asuw/Content/Textures/Extra/TheEndGlow{glowframe}", AssetRequestMode.ImmediateLoad).Value;
                Texture2D GlowBright = ModContent.Request<Texture2D>($"asuw/Content/Textures/Extra/TheEndGlow{glowframe}Bright", AssetRequestMode.ImmediateLoad).Value;

                Main.EntitySpriteDraw(Glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Glow.Size() / 2f, Projectile.scale, effects);
                Main.EntitySpriteDraw(GlowBright, Projectile.Center - Main.screenPosition, null, Color.White * glowbrightness, Projectile.rotation, Glow.Size() / 2f, Projectile.scale, effects);
            }
        }

        void muzzleflash()
        {
            Texture2D tex = Projectile.GetTexture();
            float texwidth = tex.Width * 0.67f;

            Vector2 smokeVel = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 7;
            for (int i = 0; i <= 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.rotation.ToRotationVector2() * texwidth, DustID.SteampunkSteam, smokeVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.2f, 0.9f), 80, default, Main.rand.NextFloat(0.3f, 0.7f));
                dust.noGravity = false;
                dust.color = Color.Indigo;
            }
            for (int i = 0; i < 2; i++)
            {
                //Particle smoke = new HeavySmokeParticle(Projectile.Center + Projectile.rotation.ToRotationVector2() * texwidth, smokeVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.7f, 1.6f), Color.MediumPurple, Main.rand.Next(40, 60 + 1), Main.rand.NextFloat(0.1f, 0.3f), 0.5f, Main.rand.NextFloat(-0.1f, 0.2f), Main.rand.NextBool(), required: true);
                //GeneralParticleHandler.SpawnParticle(smoke);
            }
        }
    }
}
