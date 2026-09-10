using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Global;
using asuw.Content.Projectiles;
//using CalamityMod;
//using CalamityMod.Items.Materials;
//using CalamityMod.Particles;
//using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

//namespace asuw.Content.Items.Weapons
//{
//    //TODO organize, proper balancing
//    public class Coronacht : ModItem
//    {
//        public int pullString = 0;
//        public bool failStop = false;
//        public int daedalusMod = 0;
//        public int daedalusModMax = 0;
//        public int daedalusModSpecial = 0;
//        public int daedalusModSpecialMax = -1;
//        public int progressionbonus = 0;
//        public bool chargedShot = false;
//        public bool special = false;
//        public bool used = false;
//        public override void SetDefaults()
//        {
//            Item.damage = 190;
//            Item.DamageType = DamageClass.Ranged;
//            Item.width = 32;
//            Item.height = 32;
//            Item.useTime = 5;
//            Item.useAnimation = 5;
//            Item.useStyle = ItemUseStyleID.Shoot;
//            Item.knockBack = 6;
//            Item.value = Item.buyPrice(silver: 1);
//            Item.rare = ItemRarityID.Blue;
//            Item.UseSound = null;
//            Item.useAmmo = AmmoID.Arrow;
//            Item.autoReuse = true;
//            Item.shoot = ModContent.ProjectileType<CoronachtBow>();
//            Item.noMelee = true;
//            Item.noUseGraphic = true;
//            Item.shootSpeed = 10;
//            Item.channel = true;
//            daedalusModMax = 0;
//            daedalusModSpecialMax = -1;



//        }

        
//        public override void AddRecipes()
//        {
//            Recipe recipe = CreateRecipe();
//            recipe.AddIngredient(ItemID.OrichalcumBar, 13);
//            recipe.AddIngredient(ItemID.HallowedBar, 9);
//            recipe.AddIngredient(ItemID.SoulofLight, 6);
//            recipe.AddTile(TileID.MythrilAnvil);
//            recipe.Register();

         
//        }

//        public override bool AltFunctionUse(Player player)
//        {
//            if (player.asuw().WeaponCooldown || AsuKeybinds.WeaponSkill.Current || daedalusModSpecialMax < 0)
//                return false;
//            else
//                return true;

//        }

//        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
//        {
//            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
//            {
//                special = true;
//            }
//            else
//            {
//                used = true;
//                if (pullString != 7)
//                {
//                    pullString++;
//                    if (daedalusMod == 5 && pullString != 7) pullString++;
//                }
//                if (pullString == 7) chargedShot = true;
//            }

//            return false;
//        }

//        public override void HoldItem(Player player)
//        {
//            if (player.inventory[58] == Item)
//            {
//                //for some reason using this weapon while its in the mouse slot shoots a bajillion arrows and wont stop
//                failStop = true;
//            }
//            else
//            {
//                failStop = false;
//                int type = ModContent.ProjectileType<CoronachtBow>();
//                if (Main.myPlayer == player.whoAmI)
//                    if (player.ownedProjectileCounts[type] < 1)
//                        Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, (Main.MouseWorld - player.MountedCenter).normalize() * 12, type, Item.damage, 0, player.whoAmI);
//            }




//        }
//        public override bool CanUseItem(Player player) => failStop ? false : true;

//        public static int GetLevel1()
//        {
//            int Level = 0;
//            bool flag = true;
//            void Check(bool f)
//            {
//                if (f && flag)
//                {
//                    Level++;
//                }
//                else
//                {
//                    flag = false;
//                }
//            }

//            //All 15 levels
//            Check(NPC.downedPlantBoss);
//            Check(NPC.downedGolemBoss);
//            Check(NPC.downedMoonlord);
//            Check(DownedBossSystem.downedProvidence);
//            Check(DownedBossSystem.downedDoG);

//            return Level;

//        }

//        public static int GetLevel2()
//        {
//            //return Main.LocalPlayer.inventory[9].stack;
//            int Level = -1;
//            bool flag = true;
//            void Check(bool f)
//            {
//                if (f && flag)
//                {
//                    Level++;
//                }
//                else
//                {
//                    flag = false;
//                }
//            }

//            //All 15 levels
//            Check(NPC.downedMoonlord);
//            Check(DownedBossSystem.downedDoG);
//            Check(DownedBossSystem.downedYharon);
//            Check(DownedBossSystem.downedExoMechs || DownedBossSystem.downedCalamitas);


//            return Level;

//        }

      

//        public override void UpdateInventory(Player player)
//        {
//            int level = GetLevel1();
//            int levelspecial = GetLevel2();
//            int dmgBonus = (DownedBossSystem.downedExoMechs && DownedBossSystem.downedCalamitas) ? 1000 : 0;

//            if (daedalusModMax != level)
//            {
//                int dmg = Item.damage;
//                int dmg2 = Item.damage;
//                int dmg3 = Item.damage;
//                switch (level)
//                {
//                    case 0: dmg = 260; break;
//                    case 1: dmg = 380; break;
//                    case 2: dmg = 480; break;
//                    case 3: dmg = 560; break;
//                    case 4: dmg = 840; break;
//                    case 5: dmg = 1020; break;

//                }
//                switch (levelspecial)
//                {
//                    case -1: dmg2 = 0; break;
//                    case 0: dmg2 = 200; break;
//                    case 1: dmg2 = 520; break;
//                    case 2: dmg2 = 1140; break;
//                    case 3: dmg2 = 1500; break;


//                }
//                dmg3 = dmgBonus;

//                Item.damage = dmg + dmg2 + dmg3;
//                daedalusModMax = level;
//                daedalusModSpecialMax = levelspecial;
//                progressionbonus = dmgBonus == 1000 ? 1 : 0;

//                Item.Prefix(Item.prefix);
//            }
            

//        }
//        public override void ModifyTooltips(List<TooltipLine> tooltips)
//        {
//            tooltips.FindAndReplace("[LV]",(daedalusModMax + daedalusModSpecialMax + progressionbonus + 1).ToString());
//            tooltips.FindAndReplace("[SU]", daedalusModSpecialMax < 0 ? "(LOCKED)" : "Volley Fire");

//            tooltips.FindAndReplace("M1", daedalusModMax < 1 ? "(LOCKED)" : "Twin Shot - ");
//            tooltips.FindAndReplace("M2", daedalusModMax < 2 ? "(LOCKED)" : "Chain Shot - "); 
//            tooltips.FindAndReplace("M3", daedalusModMax < 3 ? "(LOCKED)" : "Triple Shot - ");
//            tooltips.FindAndReplace("M4", daedalusModMax < 4 ? "(LOCKED)" : "Explosive Shot - "); 
//            tooltips.FindAndReplace("M5", daedalusModMax < 5 ? "(LOCKED)" : "Flurry Shot - ");
//            tooltips.FindAndReplace("U1", daedalusModMax < 1 ? " Defeat Plantera" : "Fires 2 shots side-by-side");
//            tooltips.FindAndReplace("U2", daedalusModMax < 2 ? " Defeat Golem" : "Fires arrows that bounces to up to 3 foes");
//            tooltips.FindAndReplace("U3", daedalusModMax < 3 ? " Defeat MoonLord" : "Fires 3 shots in a spread pattern");
//            tooltips.FindAndReplace("U4", daedalusModMax < 4 ? " Defeat Providence" : "Fires explosive non-piercing arrows"); 
//            tooltips.FindAndReplace("U5", daedalusModMax < 5 ? " Defeat The Devourer" : "Rapidly fires shots automatically, Cannot Power Shot");

            

//            tooltips.FindAndReplace("S1", daedalusModSpecialMax < 0 ? "(LOCKED)" : "Relentless Volley - ");
//            tooltips.FindAndReplace("S2", daedalusModSpecialMax < 1 ? "(LOCKED)" : "Piercing Volley - ");
//            tooltips.FindAndReplace("S3", daedalusModSpecialMax < 2 ? "(LOCKED)" : "Charged Volley - ");
//            tooltips.FindAndReplace("S4", daedalusModSpecialMax < 3 ? "(LOCKED)" : "Concentrated Volley - ");
//            tooltips.FindAndReplace("UC1", daedalusModSpecialMax < 0 ? " Defeat Moonlord" : "Fires 6 more arrows");
//            tooltips.FindAndReplace("UC2", daedalusModSpecialMax < 1 ? " Defeat The Devourer" : "Fires weaker armor-piercing arrows ");
//            tooltips.FindAndReplace("UC3", daedalusModSpecialMax < 2 ? " Defeat Yharon" : "Arrows can be charged to increase damage");
//            tooltips.FindAndReplace("UC4", daedalusModSpecialMax < 3 ? " Defeat A God-Rivaling Entity" : "Fires a volley of arrows straight to the cursor, Foes hit by these arrows receives extra damage from this bow's attacks,");
//            tooltips.FindAndReplace("UC5", daedalusModSpecialMax < 3 ? "_________________________ " : "this effect is independent for each npcs hit and stack infinitely");

            

//            tooltips.FindAndReplace("ff00ff", daedalusModMax < 0 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.IndianRed));
//            tooltips.FindAndReplace("ff11ff", daedalusModMax < 1 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.IndianRed));
//            tooltips.FindAndReplace("ff22ff", daedalusModMax < 2 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.IndianRed));
//            tooltips.FindAndReplace("ff33ff", daedalusModMax < 3 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.IndianRed));
//            tooltips.FindAndReplace("ff44ff", daedalusModMax < 4 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.IndianRed));
//            tooltips.FindAndReplace("ff55ff", daedalusModMax < 5 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.IndianRed));

//            tooltips.FindAndReplace("ff66ff", daedalusModSpecialMax < 0 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.PaleVioletRed));
//            tooltips.FindAndReplace("ff77ff", daedalusModSpecialMax < 1 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.PaleVioletRed));
//            tooltips.FindAndReplace("ff88ff", daedalusModSpecialMax < 2 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.PaleVioletRed));
//            tooltips.FindAndReplace("ff99ff", daedalusModSpecialMax < 3 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.PaleVioletRed));
//            tooltips.FindAndReplace("ffffff", daedalusModSpecialMax < 3 ? Utils.Hex3(Color.DimGray) : Utils.Hex3(Color.PaleVioletRed));

//        }
//    }


//    public class CoronachtBow : ModProjectile
//    {
//        Player player => Projectile.GetOwner();
//        ref float timer => ref Projectile.ai[0];
//        public bool volley = false;
//        public float shineTimer = 0;
//        public float volleyTimer = 0;
//        public float chargedVolley = 0;
//        public float shineOP = 0;
//        public float textOP = 0;
//        public float volleyDir = 0;
//        public float volleyDirect = 1;
//        public override string Texture => "asuw/Content/Textures/Coronacht";
//        public override bool? CanHitNPC(NPC target)
//        {
//            return false;
//        }
//        public override bool? CanCutTiles()
//        {
//            return false;
//        }
//        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
//        {
//            return false;
//        }
//        public override void SetStaticDefaults()
//        {
//            // Total count animation frames
//            Main.projFrames[Type] = 8;

//            // Prevents jitter when stepping up and down blocks and half blocks
//            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
//        }
//        public override void SetDefaults()
//        {
//            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
//            Projectile.friendly = false;
//            Projectile.timeLeft = 2;
//        }
//        public override void OnSpawn(IEntitySource source)
//        {
//            textOP = 80f;
//        }
//        public override void AI()
//        {
//            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Coronacht c)
//            {
//                Projectile.frame = c.pullString;
//                if(!player.dead && !c.failStop) Projectile.timeLeft = 2;
//                Projectile.Center = player.GetDrawCenter();
//                player.heldProj = Projectile.whoAmI;
//                player.Calamity().mouseWorldListener = true;
//                player.ChangeDir(Projectile.direction);
//                shineOP = shineTimer < 15 ? shineTimer < 5 ? MathHelper.SmoothStep(0, 1, shineTimer / 5) : MathHelper.SmoothStep(1, 0, (shineTimer - 5) / 10) : 0;

//                if (c.special)
//                {
//                    c.special = false;
//                    volley = true;
//                    volleyTimer = -45;
//                    volleyDir = (player.ClampedMouseWorld() - Projectile.Center).ToRotation();
//                    volleyDirect = Projectile.direction;
//                }

//                if (!volley)
//                {
//                    if(c.used)
//                    {
//                        c.used = false;
//                        if(timer<35)timer += 5;
//                        if (c.daedalusMod == 5) if (timer < 35) timer += 5;

//                    }
//                    Projectile.velocity = player.Calamity().mouseNormalFromPlayer * player.HeldItem.shootSpeed;
//                    Projectile.rotation = Projectile.velocity.ToRotation();

//                    if (!player.CantUseHoldout())
//                    {
//                        if (c.daedalusMod == 5)
//                        {
//                            if (c.pullString == 7)
//                            {

//                                mainAttack(c.pullString, c.chargedShot, c.daedalusMod, false);

//                                //float tension = (float)c.pullString / 7f;
//                                //Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, !c.chargedShot ? Projectile.velocity * (2 + tension) : Projectile.velocity * 3, ModContent.ProjectileType<coronachtArrow>(), Projectile.damage * (1 + (int)tension), 3f, Projectile.owner, 0, c.pullString);
//                                c.pullString = 0;
//                                c.chargedShot = false;
//                                timer = 0;
//                                for (int i = 0; i < 4; i++)
//                                {
//                                    Color color = Color.Lerp(Color.BlueViolet, Color.DeepPink, Main.rand.NextFloat(0f, 0.6f));
//                                    Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(10f) * (i + Main.rand.NextFloat(-0.5f, 0.5f))) * (Main.rand.NextFloat(8f, 12f));
//                                    BoltParticle bolt = new BoltParticle(Projectile.Center + (Projectile.velocity.normalize() * 15), velocity, false, 18, Main.rand.NextFloat(0.4f, 0.6f), color, new Vector2(0.6f, 1f), true);
//                                    GeneralParticleHandler.SpawnParticle(bolt);
//                                }
//                            }

//                        }
//                    }
//                    else
//                    {
                       
//                        if (c.pullString > 0)
//                        {
//                            int chargedBonusdmg = c.daedalusMod == 0 ? shineTimer < 22 ? 1 : 0 : shineTimer < 15 ? 1 : 0;
//                            if (c.pullString > 3 && c.daedalusMod != 5)
//                            { 
//                                mainAttack(c.pullString, c.chargedShot, c.daedalusMod, chargedBonusdmg == 1);
//                                for (int i = 0; i < 4; i++)
//                                {
//                                    Color color = Color.Lerp(Color.BlueViolet, Color.DeepPink, Main.rand.NextFloat(0f, 0.6f));
//                                    Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(10f) * (i + Main.rand.NextFloat(-0.5f, 0.5f))) * (Main.rand.NextFloat(8f, 12f));
//                                    BoltParticle bolt = new BoltParticle(Projectile.Center + (Projectile.velocity.normalize() * 15), velocity, false, 18, Main.rand.NextFloat(0.4f, 0.6f), color, new Vector2(0.6f, 1f), true);
//                                    GeneralParticleHandler.SpawnParticle(bolt);
//                                }
//                            }
//                            if (chargedBonusdmg == 1 && c.chargedShot && c.daedalusMod != 5)
//                            {
//                                CombatText.NewText(player.Hitbox, Color.Red, "Perfect!");
//                            }
//                            //float tension = (float)c.pullString / 7f;
//                            //Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, !c.chargedShot ? Projectile.velocity * (2 + tension) : Projectile.velocity * 3, ModContent.ProjectileType<coronachtArrow>(), Projectile.damage * (1 + (int)tension), 3f, Projectile.owner, 0, c.pullString);
                            
//                        }

//                        c.pullString = 0;
//                        c.chargedShot = false;
//                        timer = 0;
//                    }
//                }
//                else
//                {
//                    Projectile.velocity = volleyDir.ToRotationVector2() * player.HeldItem.shootSpeed;
//                    player.itemAnimation = 2;
//                    player.itemTime = 2;
//                    float specialAi = c.daedalusModSpecial;
//                    Color color = Color.Lerp(Color.BlueViolet, Color.DeepPink, Main.rand.NextFloat(0f, 0.6f));
//                    Vector2 velocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(8f) * (2 + Main.rand.NextFloat(-0.5f, 0.5f))) * (Main.rand.NextFloat(8f, 12f));
//                    BoltParticle bolt = new BoltParticle(Projectile.Center + (Projectile.velocity.normalize() * 15), velocity, false, 18, Main.rand.NextFloat(0.4f, 0.6f), color, new Vector2(0.6f, 1f), true);
//                    if (c.daedalusModSpecial != 2)
//                    {
//                        c.chargedShot = false;
//                        if (volleyTimer <= 45)
//                        {
//                            player.asuw().dmgModifier = -0.5f;
//                            player.velocity.X *= 0.3f;
//                            player.velocity.Y *= 0f;
//                            player.AddBuff(ModContent.BuffType<HorusCooldown>(), 360);
//                            float rotVolley = MathHelper.ToRadians(volleyTimer * volleyDirect);
//                            Projectile.rotation = volleyDir + rotVolley;

//                            if (c.daedalusModSpecial == 0)
//                            {
//                                volleyTimer += 3f;
//                                if (volleyTimer % 2 == 0)
//                                {
//                                    c.pullString = 4;
//                                    timer = 8;
//                                    volleyAttack(false, 0, specialAi);
//                                    GeneralParticleHandler.SpawnParticle(bolt);
//                                }
//                                else
//                                {
//                                    timer = 35;
//                                    c.pullString = 7;
//                                }
//                            }
//                            else
//                            {

                               
//                                volleyTimer += 5f;
//                                if (volleyTimer % 10 == 0)
//                                {
//                                    c.pullString = 4;
//                                    timer = 8;
//                                    volleyAttack(false,0,specialAi);
//                                    GeneralParticleHandler.SpawnParticle(bolt);
//                                }
//                                else
//                                {
//                                    timer = 35;
//                                    c.pullString = 7;
//                                }
//                            }

//                        }
//                        else
//                        { volley = false; c.pullString = 0; timer = 0; volleyDir = 0; player.asuw().dmgModifier = 0; player.AddBuff(ModContent.BuffType<HorusCooldown>(), 360); }
//                    }
//                    else
//                    {

//                        if (volleyTimer <= 45)
//                        {

//                            float rotVolley = MathHelper.ToRadians(volleyTimer * volleyDirect);
//                            Projectile.rotation = volleyDir + rotVolley;
//                            player.asuw().dmgModifier = -0.5f;
//                            player.velocity.X *= 0.3f;
//                            player.velocity.Y *= 0f;
//                            player.AddBuff(ModContent.BuffType<HorusCooldown>(), 360);
//                            if (player.Calamity().mouseRight && volleyTimer <= -45)
//                            {
//                                if (c.pullString != 7 && chargedVolley % 5 == 0)
//                                {
//                                    c.pullString++;
//                                }
//                                if (c.pullString == 7) c.chargedShot = true;
//                                if (timer < 35) timer++;
//                                if (chargedVolley < 35) chargedVolley++;
//                            }
//                            else
//                            {
//                                volleyTimer += 5f;
//                                if (volleyTimer % 10 == 0)
//                                {
//                                    c.pullString = 4;
//                                    timer = 8;
//                                    volleyAttack(true,chargedVolley, specialAi);
//                                    GeneralParticleHandler.SpawnParticle(bolt);
//                                }
//                                else
//                                {
//                                    timer = 35;
//                                    c.pullString = 7;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            volley = false; c.pullString = 0; timer = 0; volleyDir = 0; player.asuw().dmgModifier = 0;  chargedVolley = 0; c.chargedShot = false; 
//                        }
//                    }
//                }

//                //chargeIndicator
//                if (c.chargedShot)
//                {
//                    shineTimer++;
//                }
//                else
//                {
//                    shineTimer = 0;
//                }
               

//                if (textOP > 0)
//                {
//                    textOP--;
//                }

//                if (AsuKeybinds.WeaponSkill.JustPressed)
//                {
//                    textOP = 80f;
//                    if (player.Calamity().mouseRight)
//                    {
//                        if (c.daedalusModSpecial < c.daedalusModSpecialMax) c.daedalusModSpecial++;
//                        else c.daedalusModSpecial = 0;
//                    }
//                    else
//                    {
//                        if (c.daedalusMod < c.daedalusModMax) c.daedalusMod++;
//                        else c.daedalusMod = 0;
//                    }
//                }

              

//            }
//            else
//            {
//                timer = 0;
//            }

//        }
        
//        public override bool PreDraw(ref Color lightColor)
//        {
//            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Coronacht c)
//            {
//                SpriteEffects effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
//                Texture2D tex = Projectile.GetTexture();
//                Texture2D arrowTex = ModContent.Request<Texture2D>("asuw/Content/Textures/coronachtArrow").Value;
//                Texture2D arrowGlowTex = ModContent.Request<Texture2D>("asuw/Content/Textures/coronachtArrowGlow").Value;
//                Texture2D GlowTex = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/WhiteBall").Value;
//                Vector2 drawPositionBow = Projectile.Center - Main.screenPosition;

//                Vector2 PositionArrow;
//                Vector2 PositionArrow2;
//                Vector2 lerpedArrowPos;
//                Vector2 lerpedArrowPos2;
//                float arrow2Rot = -15;
//                float arrow3Rot = 15;

//                //Projectile.Center + (Projectile.rotation.ToRotationVector2() * 40)
//                if (c.daedalusMod == 1 && !volley)
//                {
//                    PositionArrow = Projectile.Center + ((Projectile.velocity.normalize().RotatedBy(MathHelper.ToRadians(50))) * 15) + (Projectile.velocity.normalize() * 15);
//                    PositionArrow2 = Projectile.Center + ((Projectile.velocity.normalize().RotatedBy(MathHelper.ToRadians(-50))) * 15) + (Projectile.velocity.normalize() * 15);
//                    lerpedArrowPos = Vector2.Lerp(PositionArrow, PositionArrow + (Projectile.rotation.ToRotationVector2() * -35), timer / 35f) - Main.screenPosition;
//                    lerpedArrowPos2 = Vector2.Lerp(PositionArrow2, PositionArrow2 + (Projectile.rotation.ToRotationVector2() * -35), timer / 35f) - Main.screenPosition;
//                }
//                else if (c.daedalusMod == 3 && !volley)
//                {
//                    PositionArrow = Projectile.Center + (Projectile.rotation.ToRotationVector2() * 40);
//                    PositionArrow2 = Projectile.Center + (Projectile.velocity.normalize().RotatedBy(MathHelper.ToRadians(-15))) * 30;


//                    lerpedArrowPos = Vector2.Lerp(PositionArrow, PositionArrow + (Projectile.rotation.ToRotationVector2() * -42), timer / 35f) - Main.screenPosition;
//                    lerpedArrowPos2 = Vector2.Lerp(PositionArrow2, PositionArrow + (Projectile.rotation.ToRotationVector2() * -42), timer / 35f) - Main.screenPosition;

//                }
//                else
//                {
//                    PositionArrow = Projectile.Center + (Projectile.rotation.ToRotationVector2() * 40);
//                    PositionArrow2 = Projectile.Center + (Projectile.rotation.ToRotationVector2() * 40);

//                    lerpedArrowPos = Vector2.Lerp(PositionArrow, PositionArrow + (Projectile.rotation.ToRotationVector2() * -42), timer / 35f) - Main.screenPosition;
//                    lerpedArrowPos2 = Vector2.Lerp(PositionArrow, PositionArrow + (Projectile.rotation.ToRotationVector2() * -42), timer / 35f) - Main.screenPosition;

//                }

//                Vector2 PositionArrow3 = Projectile.Center + (Projectile.velocity.normalize().RotatedBy(MathHelper.ToRadians(15))) * 30;
//                Vector2 lerpedArrowPos3 = Vector2.Lerp(PositionArrow3, PositionArrow + (Projectile.rotation.ToRotationVector2() * -42), timer / 35f) - Main.screenPosition;

//                Rectangle frame = tex.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
//                Vector2 origin = frame.Size() * 0.5f;

//                Main.EntitySpriteDraw(tex, drawPositionBow, frame, Color.White, Projectile.rotation, origin, Projectile.scale + 0.5f, effects, 0);

//                if (c.pullString > 0)
//                {

//                    for (int i = 1; i <= 12; i++)
//                    {
//                        float attackLerp = (float)Math.Pow((double)(Utils.GetLerpValue(60, 200, c.pullString * 20, true)), (double)(8));
//                        float mult = MathHelper.Max(Utils.GetLerpValue(7, 0, i), Utils.GetLerpValue(17, 24, i));
//                        float outspace = 6 * attackLerp;
//                        Vector2 drawOffset = (((MathHelper.TwoPi * i / 24f).ToRotationVector2().RotatedBy(Projectile.rotation) * outspace) + Main.rand.NextVector2Circular(2, 2));
//                        Color auraColor = Main.rand.NextBool() ? Color.Violet with { A = 100 } * mult : Color.MistyRose with { A = 100 } * mult;
//                        Main.EntitySpriteDraw(arrowGlowTex, lerpedArrowPos + drawOffset, null, auraColor * ((float)Projectile.frame / 7f) * 0.5f, Projectile.rotation, arrowGlowTex.Size() / 2f, 1.8f * player.gravDir, effects);
//                    }
//                    Main.EntitySpriteDraw(arrowTex, lerpedArrowPos, null, Color.White * ((float)Projectile.frame / 7f), Projectile.rotation, arrowTex.Size() / 2f, 1.5f * player.gravDir, effects);

//                    //arrow2
//                    if ((c.daedalusMod == 1 || c.daedalusMod == 3) && !volley)
//                    {
//                        for (int i = 1; i <= 12; i++)
//                        {
//                            float attackLerp = (float)Math.Pow((double)(Utils.GetLerpValue(60, 200, c.pullString * 20, true)), (double)(8));
//                            float mult = MathHelper.Max(Utils.GetLerpValue(7, 0, i), Utils.GetLerpValue(17, 24, i));
//                            float outspace = 6 * attackLerp;
//                            Vector2 drawOffset = (((MathHelper.TwoPi * i / 24f).ToRotationVector2().RotatedBy(Projectile.rotation) * outspace) + Main.rand.NextVector2Circular(2, 2));
//                            Color auraColor = Main.rand.NextBool() ? Color.Violet with { A = 100 } * mult : Color.MistyRose with { A = 100 } * mult;
//                            Main.EntitySpriteDraw(arrowGlowTex, lerpedArrowPos2 + drawOffset, null, auraColor * ((float)Projectile.frame / 7f) * 0.5f, Projectile.rotation + (c.daedalusMod == 3 ? MathHelper.ToRadians(arrow2Rot) : 0), arrowGlowTex.Size() / 2f, 1.8f * player.gravDir, effects);
//                        }
//                        Main.EntitySpriteDraw(arrowTex, lerpedArrowPos2, null, Color.White * ((float)Projectile.frame / 7f), Projectile.rotation + (c.daedalusMod == 3 ? MathHelper.ToRadians(arrow2Rot) : 0), arrowTex.Size() / 2f, 1.5f * player.gravDir, effects);
//                    }
//                    //arrow3
//                    if (c.daedalusMod == 3 && !volley) 
//                    {
//                        for (int i = 1; i <= 12; i++)
//                        {
//                            float attackLerp = (float)Math.Pow((double)(Utils.GetLerpValue(60, 200, c.pullString * 20, true)), (double)(8));
//                            float mult = MathHelper.Max(Utils.GetLerpValue(7, 0, i), Utils.GetLerpValue(17, 24, i));
//                            float outspace = 6 * attackLerp;
//                            Vector2 drawOffset = (((MathHelper.TwoPi * i / 24f).ToRotationVector2().RotatedBy(Projectile.rotation) * outspace) + Main.rand.NextVector2Circular(2, 2));
//                            Color auraColor = Main.rand.NextBool() ? Color.Violet with { A = 100 } * mult : Color.MistyRose with { A = 100 } * mult;
//                            Main.EntitySpriteDraw(arrowGlowTex, lerpedArrowPos3 + drawOffset, null, auraColor * ((float)Projectile.frame / 7f) * 0.5f, Projectile.rotation + MathHelper.ToRadians(arrow3Rot), arrowGlowTex.Size() / 2f, 1.8f * player.gravDir, effects);
//                        }
//                        Main.EntitySpriteDraw(arrowTex, lerpedArrowPos3, null, Color.White * ((float)Projectile.frame / 7f), Projectile.rotation + MathHelper.ToRadians(arrow3Rot), arrowTex.Size() / 2f, 1.5f * player.gravDir, effects);
//                    }

//                    //chargedIndicator
//                    if (c.chargedShot && c.daedalusMod != 5) Main.EntitySpriteDraw(GlowTex, (Projectile.Center + (Projectile.rotation.ToRotationVector2() * 20)) - Main.screenPosition, null, Color.White * shineOP, Projectile.rotation, GlowTex.Size() / 2f, Projectile.scale + 0.5f, effects, 0);
//                }
//            }
//            return false;
//        }

//        public override void PostDraw(Color lightColor)
//        {
//            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Coronacht c)
//            {

//                Texture2D daedalusText = ModContent.Request<Texture2D>($"asuw/Content/Textures/DaedalusMods/daedalus{c.daedalusMod}").Value;
//                Texture2D daedalusText2 = ModContent.Request<Texture2D>($"asuw/Content/Textures/DaedalusMods/daedalusSpecial{c.daedalusModSpecial}").Value;
//                Vector2 drawPosition = (player.Center + (Vector2.UnitY * -90)) - Main.screenPosition;
//                Vector2 drawPosition2 = (player.Center + (Vector2.UnitY * -150)) - Main.screenPosition;
//                if(c.daedalusModMax > 0)
//                Main.EntitySpriteDraw(daedalusText, drawPosition, null, Color.Firebrick * (textOP / 120f), 0f, daedalusText.Size() / 2f, 0.5f, SpriteEffects.None, 0);
//                if(c.daedalusModSpecialMax > 0)
//                Main.EntitySpriteDraw(daedalusText2, drawPosition2, null, Color.HotPink * (textOP / 120f), 0f, daedalusText.Size() / 2f, 0.5f, SpriteEffects.None, 0);

//            }
//        }

//        private void mainAttack(int pullString, bool charged, int daedalusmod, bool chargeBonus)
//        {
//            float tension = (float)(pullString - 3) / 4f;
//            int chargedBonusdmg = chargeBonus ? 1 : 0;
//            Vector2 vel = Projectile.velocity * (2 + tension);
//            if (daedalusmod == 1)
//            {
//                //twinshot
//                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + (Projectile.velocity.normalize().RotatedBy(MathHelper.ToRadians(50))) * 20, vel, ModContent.ProjectileType<coronachtArrow>(), (int)((float)Projectile.damage * (tension + (charged ? chargedBonusdmg : 0))), 3f, Projectile.owner, 0, pullString);
//                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + (Projectile.velocity.normalize().RotatedBy(MathHelper.ToRadians(-50)) * 20), vel, ModContent.ProjectileType<coronachtArrow>(), (int)((float)Projectile.damage * (tension + (charged ? chargedBonusdmg : 0))), 3f, Projectile.owner, 1, pullString);
//            }
//            else if (daedalusmod == 2)
//            {
//                //ChainShot
//                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<coronachtArrow>(), (int)(((float)Projectile.damage * 0.75f) * (tension + (charged ? chargedBonusdmg : 0))), 3f, Projectile.owner, 2, pullString);
//            }
//            else if (daedalusmod == 3)
//            {
//                //tripleshot
//                for (int i = -1; i < 2; i++)
//                {

//                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel.RotatedBy(MathHelper.ToRadians(8 * i)), ModContent.ProjectileType<coronachtArrow>(), (int)(((float)Projectile.damage * 0.8f) * (tension + (charged ? chargedBonusdmg : 0))), 3f, Projectile.owner, 3, pullString);
//                }
//            }
//            else if (daedalusmod == 4)
//            {
//                //explosiveShot
//                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<coronachtArrow>(), (int)(((float)Projectile.damage * 0.6f) * (1 + tension + (charged ? chargedBonusdmg : 0))), 3f, Projectile.owner, 4, pullString);
//            }
          
//            else if (daedalusmod == 5)
//            {
//                //flurryShot
//                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<coronachtArrow>(), (int)((float)Projectile.damage * 1.5f), 3f, Projectile.owner, 5, pullString);
//            }
//            else
//            {
//                //perfectShot
//                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<coronachtArrow>(), (int)((float)Projectile.damage * (1 + tension + (charged ? chargedBonusdmg + 0.75f : 0))), 3f, Projectile.owner, 0, pullString);
//            }
//        }

//        private void volleyAttack(bool charged,float daedalusmod2dmg, float extraAi)
//        {

//            if (charged)
//            {
//                Vector2 vel = Projectile.rotation.ToRotationVector2() * 30;
//                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<coronachtArrow>(), (int)((float)Projectile.damage * (0.1f + (daedalusmod2dmg / 20))), 3f, Projectile.owner, 100, 4);
//            }
//            else if(extraAi != 3)
//            {
//                Vector2 vel = Projectile.rotation.ToRotationVector2() * 30;
//                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<coronachtArrow>(), (int)((float)Projectile.damage * 0.85f), 3f, Projectile.owner, 100 + extraAi, 4);
//            }
//            else
//            {
               
//                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + (Projectile.rotation.ToRotationVector2() * 50), Projectile.velocity, ModContent.ProjectileType<coronachtArrow>(), (int)((float)Projectile.damage * 0.85f), 3f, Projectile.owner, 100 + extraAi, 4);
//            }
//        }

//        public override void OnKill(int timeLeft)
//        {
//            volley = false;
//            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Coronacht c)
//            {
//                c.pullString = 0;
//            }


//        }
//    }
//}
