using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Dusts;
using asuw.Content.Global;
using asuw.Content.Projectiles.EOH;
using asuw.Content.Projectiles.Filler;
using asuw.Content.Projectiles.InfoAndChargeBar;
//using CalamityMod;
//using CalamityMod.Dusts;
//using CalamityMod.Particles;
//using CalamityMod.Projectiles.Melee;
//using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace asuw.Content.Items.Weapons
{
    //TODO better sprite, revamp projectiles
    public class Horus : ModItem
	{
		public int timer = 0;
		public int AmmoSavedPercent = 60;
		public bool shotGun = false;
		public bool pistolGun = false;
		public int sideCharge = 0;
		public int mainCharge = 0;
		bool bigShot = false;
		public float barrierUp = 0;
		public float shotgunSpread = 5;
        public int ShotgunGlowFrame = 0;
		public int yellowShot = 0;

        public override void SetStaticDefaults()
		{
			ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.FindAndReplaceAll("ff00ff", Utils.Hex3(AsuPlayer.SpecialMoveColorWS1));
			tooltips.FindAndReplaceAll("ff22ff", Utils.Hex3(AsuPlayer.ColorWSA));
			tooltips.FindAndReplaceAll("ff33ff", Utils.Hex3(AsuPlayer.SpecialMoveColorWSC));
		}
		public override void SetDefaults()
		{
			Item.damage = 270;
			Item.ArmorPenetration = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.knockBack = 8f;
			Item.width = 34;
			Item.height = 10;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 80f;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Bullet;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = null;
			Item.autoReuse = true;
            Item.noUseGraphic = true;
        }
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;
			position += muzzleOffset;
		}
        public override void UpdateInventory(Player player)
        {
            if (barrierUp > 0) barrierUp--;

            if (player.asuw().Timer120 % 6 == 0)
            {
                ShotgunGlowFrame = (ShotgunGlowFrame + 1) % 5;
            }
        }

		public override bool AltFunctionUse(Player player) => (mainCharge > 0 || yellowShot > 0) ? false : true;
		
		public override void HoldItem(Player player)
		{
			int type = ModContent.ProjectileType<HorusBarrier>();
            int type2 = ModContent.ProjectileType<HorusHeld>();
			Vector2 heldVel = player.mouseWorld().X > player.MountedCenter.X ? (Vector2.UnitX * 3).RotatedBy(MathHelper.ToRadians((60f))) : (Vector2.UnitX * 3).RotatedBy(MathHelper.ToRadians((120f)));
            if (Main.myPlayer == player.whoAmI)
			{
				if (player.ownedProjectileCounts[type] < 1)
					Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, Vector2.Zero, type, Item.damage, Item.knockBack, player.whoAmI);
                if (player.ownedProjectileCounts[type2] < 1)
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, heldVel, type2, Item.damage, Item.knockBack, player.whoAmI, 120, 120);
            }

            if (AsuKeybinds.WeaponSkill.JustPressed && !player.asuw().WeaponCooldown)
            {
				mainCharge = 3;
				bigShot = false;
            }
        }
		public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= AmmoSavedPercent;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
			{
				SoundStyle bow = new("asuw/Content/Sounds/HorusSidearm");
                SoundStyle boww = new("asuw/Content/Sounds/HorusSidearm2");
                SoundEngine.PlaySound(bow with { Volume = 0.78f, PitchVariance = 0.4f }, player.Center);

                Projectile shot = Projectile.NewProjectileDirect(source, position, velocity * 2, type, damage + (damage / 2), knockback, player.whoAmI);
				if (sideCharge > 0)
				{
                    SoundEngine.PlaySound(boww with { Volume = 0.18f, PitchVariance = 0.4f }, player.Center);
                    shot.asuw().horusCharged = true;
					if (sideCharge == 1)
					{ mainCharge++; bigShot = true; }
					sideCharge--;
				}
				//muzzle
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position + (velocity.normalize() * -5), ModContent.DustType<SharpSparkDust>(), (velocity.normalize() * Main.rand.NextFloat(6f, 18f)).RotatedByRandom(0.4f), 0, Color.LightPink, Main.rand.NextFloat(0.5f, 0.8f));
                    dust.noGravity = true;
                }
                pistolGun = true;
				
			}
			else
			{
				SoundStyle Shoot = new("asuw/Content/Sounds/Horus");
				SoundEngine.PlaySound(Shoot with { Volume = 0.6f, MaxInstances = 30, PitchVariance = 0.2f }, player.Center);
				float numberProjectiles = 5;
				float rotation = MathHelper.ToRadians(shotgunSpread);
				shotGun = true;
                //muzzle
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position + (velocity.normalize() * 39).RotatedBy(MathHelper.ToRadians(-10 * player.direction)), ModContent.DustType<SharpSparkDust>(), (velocity.normalize() * Main.rand.NextFloat(6f, 12f)).RotatedByRandom(0.4f), 0, Color.LightPink, Main.rand.NextFloat(0.5f, 0.8f));
                    dust.noGravity = true;
                    Dust dust2 = Dust.NewDustPerfect(position + (velocity.normalize() * 36).RotatedBy(MathHelper.ToRadians(2 * player.direction)), ModContent.DustType<SharpSparkDust>(), (velocity.normalize() * Main.rand.NextFloat(6f, 12f)).RotatedByRandom(0.4f), 0, Color.LightPink, Main.rand.NextFloat(0.5f, 0.8f));
                    dust2.noGravity = true;
                }
				//shot
				if (yellowShot == 0)
				{
					if (mainCharge < 1)
					{
						//if (sideCharge < (player.asuw().butterflyHairpin ? 24 : 20)) sideCharge++;
						for (int i = 0; i < numberProjectiles; i++)
						{
							Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * Main.rand.NextFloat(0.8f, 1.2f); // Watch out for dividing by 0 if there is only 1 projectile.
							Projectile shot = Projectile.NewProjectileDirect(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
							shot.asuw().horusProj = true;
						}

						if (shotgunSpread > 2) shotgunSpread--;
						else shotgunSpread = 5;
					}
					else
					{
						if (bigShot)
						{
							bigShot = false;
							yellowShot = 3;
							SoundStyle boom = new("asuw/Content/Sounds/HoshinoUlt");
							SoundEngine.PlaySound(boom with { Volume = 0.6f }, player.Center);
							Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HoshinoUlt>(), damage * 8, knockback, player.whoAmI);
							//mainCharge -= (player.asuw().butterflyHairpin ? 24 : 20);
							barrierUp = 660;

							foreach (var npcs in Main.ActiveNPCs)
							{
								npcs.asuw().horusHooks.Clear();
							}

                            foreach (var hooks in Main.ActiveProjectiles)
                            {
								if(hooks.ModProjectile is Planty p)
								{
									if (p.mode == 1)
									{
										hooks.Kill();
                                        Projectile.NewProjectile(source, hooks.Center + Utils.NextVector2Circular(Main.rand, 15, 15), Vector2.Zero, ModContent.ProjectileType<HorusBoomVis>(), damage * 5, hooks.knockBack, player.whoAmI);
                                    }
								}
                            }
                        }
						else
						{
							Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Planty>(), damage * 4, knockback, player.whoAmI);
							sideCharge++;

						}
                        mainCharge--;
                    }
				}
				else
				{
					if(yellowShot > 0)yellowShot--;
                    Vector2 pos = position + (velocity.normalize() * 40);

                    for (int i = 0; i < 5; i++)
                    {
                        //rotaty
                        Projectile.NewProjectileDirect(source, position, (velocity * 1.1f).RotatedByRandom(0.15f), ModContent.ProjectileType<Rotaty>(), damage * 2, knockback, player.whoAmI);

                      
                    }

                    float rot = Main.rand.NextFloat(-15f, 15f);
               

                    for (int i = 1; i <= 20; i++)//straight
                    {
						for (int j = 0; j < 2; j++)
						{
							Projectile.NewProjectileDirect(source, position, (velocity * 1.2f).RotatedByRandom(0.4f), ModContent.ProjectileType<Straighty>(), damage / 8, knockback, player.whoAmI);
						}



                    }
                }

            }
            return false;
        }
    }

	public class HorusHeld : ModProjectile
	{
		Player player => Projectile.GetOwner();
        public override string Texture => "asuw/Content/Textures/HorusBase";
        public ref float Timer => ref Projectile.ai[0];
        public ref float TimerSide => ref Projectile.ai[1];
        public Vector2 sideArmPos;
        public Vector2 sideArmVel;
		public float sideArmOP = 0;
		public float sideArmRot = 0;
        public bool usingSidearm = false;
        public float ofs1 = 0;
        public float Rotofs1 = 0;
        public float ofs2 = 0;
        public float rot2 = 0;
        public float Rotofs2 = 0;
        public float Pumpofs = 0;
		public float grav = 0;
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
		public override void AI()
		{
			//defaults
			player.asuw().mouseRotationListener = true;
			Projectile.Center = player.GetDrawCenter();
			sideArmPos = player.GetDrawCenter();
			player.heldProj = Projectile.whoAmI;
			//gravityPotion
			grav = player.gravDir;
			//restingPosition
			Vector2 restingPos = player.mouseWorld().X > player.MountedCenter.X ? Projectile.Center + (Vector2.UnitX * 10).RotatedBy(MathHelper.ToRadians(60f * grav)) : Projectile.Center + (Vector2.UnitX * 10).RotatedBy(MathHelper.ToRadians(120f * grav));
            float targetAngle = Projectile.AngleTo(restingPos);
			//triggers
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Horus ho)
			{
				if (!player.dead) Projectile.timeLeft = 2;

				if(ho.shotGun)//Shotgun
				{
					ho.shotGun = false;
					usingSidearm = false;
					Timer = 0;
                    ofs1 = -10;
                    Rotofs1 = Main.rand.NextFloat(-7f, -10f) * Projectile.direction * grav;
					if(ho.mainCharge > 0)
					{
						player.itemAnimation = player.itemAnimationMax + (player.itemAnimationMax / 3);
                        player.itemTime = player.itemTimeMax + (player.itemTimeMax / 3);
                    }
                }
				if(ho.pistolGun)//sideArm
				{
					ho.pistolGun = false;
                    usingSidearm = true;
					sideArmOP = 1;
                    TimerSide = 0;
					ofs2 = -6;
					Rotofs2 = Main.rand.NextFloat(-10f, -16f) * Projectile.direction * grav;
					player.itemAnimation = ho.sideCharge > 0 ? 14 : 6;
					player.itemTime = ho.sideCharge > 0 ? 14 : 6;
                }

			}
            //positions n direction
            Projectile.velocity = Timer <= 20 ? player.asuw().mouseNormalFromPlayer * 3 : Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5)).ToRotationVector2() * 3;
            Projectile.rotation = Projectile.velocity.ToRotation();
            sideArmVel = TimerSide <= 20 ? player.asuw().mouseNormalFromPlayer * 17 : sideArmVel.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(4)).ToRotationVector2() * 17;
            sideArmRot = sideArmVel.ToRotation();
            sideArmPos = player.GetDrawCenter() + sideArmVel;
			//timers
            if (Timer < 120) Timer++;
            if (TimerSide < 120) TimerSide++;
            //recoil n rot
            ofs1 *= 0.93f;
            Rotofs1 *= 0.8f;
			ofs2 *= 0.9f;
			Rotofs2 *= 0.58f;
			if (Timer == 4) Pumpofs = -12f;
            //if (Timer == 6) shellCasing 
            Pumpofs *= 0.8f;
            Projectile.Center += Projectile.rotation.ToRotationVector2() * ofs1;
            Projectile.rotation += MathHelper.ToRadians(Rotofs1);
			sideArmPos += sideArmRot.ToRotationVector2() * ofs2;
			sideArmRot += MathHelper.ToRadians(Rotofs2);
			if (TimerSide > 20 && sideArmOP > 0) sideArmOP -= 0.02f; ;
            //change player direction
            player.ChangeDir(Projectile.direction);
            //player arm rotation
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation * grav) - MathHelper.ToRadians (Projectile.direction == 1 ? 75f : 105f));
            if(sideArmOP > 0)player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (sideArmRot * grav) - MathHelper.ToRadians(Projectile.direction == 1? 55f : 145f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D pump = ModContent.Request<Texture2D>("asuw/Content/Textures/HorusPump", AssetRequestMode.AsyncLoad).Value;
            Texture2D side = ModContent.Request<Texture2D>("asuw/Content/Textures/HorusSidearm", AssetRequestMode.AsyncLoad).Value;

            Vector2 origin = new Vector2(tex.Width * 0.15f, tex.Height / 2f);
            Vector2 origin2 = new Vector2(side.Width * 0.1f, side.Height / 2f);

			SpriteEffects effects;
				if(grav == 1) effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
				else effects = Projectile.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Vector2 drawPosition1 = Projectile.Center - Main.screenPosition + (Vector2.UnitY * 6 * grav);
            Vector2 drawPosition2 = drawPosition1 + (Projectile.rotation.ToRotationVector2() * Pumpofs);
            Vector2 drawPosition3 = sideArmPos - Main.screenPosition + (Vector2.UnitY * 8 * grav);

            Main.spriteBatch.Draw(side, drawPosition3, null, lightColor * sideArmOP, sideArmRot, origin2, 0.8f, effects, 0);
            Main.spriteBatch.Draw(tex, drawPosition1, null, lightColor, Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.spriteBatch.Draw(pump, drawPosition2, null, lightColor, Projectile.rotation, origin, Projectile.scale, effects, 0);

            return false;
        }
    }
}

