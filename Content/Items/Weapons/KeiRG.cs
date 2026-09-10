using asuw.Content.Dusts;
using asuw.Content.Projectiles;
using asuw.Content.Projectiles.InfoAndChargeBar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;


namespace asuw.Content.Items.Weapons
{
	public class KeiRG : ModItem
	{
        //TODO rework, add basic att and weapon skill
        public bool shoot = false;
        public int dmgValue = 0;
		public override void SetDefaults()
		{
			Item.damage = 400;
			Item.DamageType = DamageClass.Magic;
			Item.width = 48;
			Item.height = 48;
			Item.useTime = 100;
			Item.useAnimation = 100;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.shoot = ModContent.ProjectileType<KeiRGHeld>();
			Item.shootSpeed = 45f;

		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Wood, 30);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            dmgValue = damage;
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
            {
               
            }
            else
            {
                shoot = true;
            }
           
            return false;
        }

        public override void HoldItem(Player player)
        {
            int type = ModContent.ProjectileType<KeiRGHeld>();
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.ownedProjectileCounts[type] < 1)
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, (Main.MouseWorld - player.MountedCenter).normalize() * 4, type, Item.damage, Item.knockBack, player.whoAmI);
            }
        }
    }

	public class KeiRGHeld : ModProjectile
	{
        Player player => Projectile.GetOwner();
        public ref float Timer => ref Projectile.ai[0];

        public SlotId SoundSlot;
        public override string Texture => "asuw/Content/Textures/Extra/KeiRGBase";
        public float lerpamnt = 0f;
        public bool coolingdown = false;
        public float ofs = 0;
        public bool shooting = false;
        public bool hasShot = false;
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

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadInt32();
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Magic, false, -1);
            Projectile.friendly = false;
            Projectile.timeLeft = 2;
            Projectile.scale = 1.5f;
        }

        public void StopSounds()
        {
            if (SoundEngine.TryGetActiveSound(SoundSlot, out var audio2) && audio2.IsPlaying)
            {
                audio2?.Stop();
            }
        }
        public override void AI()
        {

            player.asuw().mouseRotationListener = true;
            Projectile.Center = player.GetDrawCenter();
            Projectile.spriteDirection = Projectile.direction;
            player.heldProj = Projectile.whoAmI;

            Projectile.velocity = player.asuw().mouseNormalFromPlayer * player.HeldItem.shootSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is KeiRG krg)
            {
                lerpamnt = Timer / 60f;
                int damage = krg.dmgValue;
                if (!player.dead) Projectile.timeLeft = 2;
                if(krg.shoot)
                {
                    if (!player.CantUseHoldout())
                    {
                        float increase = Timer > 30 ? 0.8f : 0f;
                        Projectile.Center += new Vector2(Main.rand.NextFloat(-0.5f - increase, 0.5f + increase), Main.rand.NextFloat(-0.5f - increase, 0.5f + increase));
                        shooting = true;

                        if (Timer == 0)
                        {
                            SoundStyle sound = new("asuw/Content/Sounds/SwordOfLight/KeiRailgun");
                            SoundSlot = SoundEngine.PlaySound(sound with { Volume = 1.4f }, Projectile.Center);
                        }

                        if (Timer >= 60)
                        {
                            krg.shoot = false;
                            coolingdown = true;
                            ofs = -20;
                            shooting = false;
                            hasShot = true;
                            player.itemAnimation = 65;
                            player.itemTime = 65;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<KeiBol>(), damage, Projectile.knockBack, player.whoAmI);
                            Vector2 smokePos = player.Center + (Projectile.rotation.ToRotationVector2() * -10f).RotatedBy(70f * Projectile.direction);
                            for (int k = 0; k < 9; k++)
                            {
                               // SmallSmokeParticle smoke = new SmallSmokeParticle(smokePos, ((smokePos - player.Center).normalize() * Main.rand.NextFloat(6f,18f)).RotatedByRandom(0.4f), Color.LightSteelBlue, Color.LightSlateGray, Main.rand.NextFloat(0.4f,1.2f), 100);
                               //GeneralParticleHandler.SpawnParticle(smoke);

                               // SmallSmokeParticle smoke2 = new SmallSmokeParticle(smokePos, ((smokePos - player.Center).normalize() * Main.rand.NextFloat(6f, 18f)).RotatedByRandom(0.4f), Color.SteelBlue, Color.Lerp(Color.DarkSlateBlue,Color.DimGray,0.5f), Main.rand.NextFloat(0.4f, 1.2f), 100);
                               // GeneralParticleHandler.SpawnParticle(smoke2);
                            }
                            for (int i = 1; i < 5; i++)
                            {
                                //Particle portal = new CustomSpark(Projectile.Center + (Projectile.velocity.normalize() * 70f), Projectile.velocity.normalize() * (float)i * 2f, "CalamityMod/Particles/BloomRing", false, 30, 0.62f - ((float)i / 7), Color.Violet, new Vector2(0.5f, 1.5f), true, false, extraRotation: -MathHelper.PiOver2, noShrink: true);
                                //GeneralParticleHandler.SpawnParticle(portal);

                                //Particle spark2 = new CustomSpark(Projectile.Center + (Projectile.velocity.normalize() * 70f), Projectile.velocity.normalize() * (float)i * 2f, "CalamityMod/Particles/BloomCircle", false, 16, 0.95f, Color.DeepPink, new Vector2(1.8f, 0.8f), true, true, glowOpacity: 0.9f, shrinkSpeed: 0.8f);
                                //GeneralParticleHandler.SpawnParticle(spark2);
                            }
                            
                        }
                        else
                        {
                            player.itemAnimation = (int)(Timer / 2f);
                            player.itemTime = (int)(Timer / 2f);
                            for (int k = 0; k < 5; k++)
                            {
                                //Vector2 velocity = Projectile.velocity.normalize().RotatedByRandom(MathHelper.TwoPi);
                                //Particle spark = new CustomSpark(Projectile.Center + (Projectile.velocity.normalize() * 145f), velocity * 20f, "CalamityMod/Particles/BloomLineFade", false, 3, 0.05f, Color.Tomato * lerpamnt * 0.85f, new Vector2(2.8f, 1), shrinkSpeed: 0.5f);
                                //GeneralParticleHandler.SpawnParticle(spark);

                                //Particle blastRing = new CustomPulse(Projectile.Center + (Projectile.velocity.normalize() * 145f), Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 1.5f, Color.MediumVioletRed * lerpamnt * 0.9f, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 2f, 1f, 4, true);
                                //GeneralParticleHandler.SpawnParticle(blastRing);
                            }
                            for (int i = 0; i < 3; i++)
                            {
                                //Particle Smokey = new CircularSmearSmokeyVFX(Projectile.Center + (Projectile.velocity.normalize() * 145f) + new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f)), Color.Thistle * lerpamnt * 0.5f, Main.rand.NextFloat(MathHelper.TwoPi), Main.rand.NextFloat(0.7f, 1.1f));
                                //GeneralParticleHandler.SpawnParticle(Smokey);

                                //Particle Smokeysmol = new CircularSmearSmokeyVFX(Projectile.Center + (Projectile.velocity.normalize() * 145f) + new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f)), Color.Thistle * lerpamnt, Main.rand.NextFloat(MathHelper.TwoPi), Main.rand.NextFloat(0.35f, 0.56f));
                                //GeneralParticleHandler.SpawnParticle(Smokeysmol);
                            }

                        }

                       
                        Timer++;
                    }
                    else
                    {
                        hasShot = false;
                        krg.shoot = false;
                        shooting = false;

                        for (int i = 0; i < (int)(Timer / 6f); i++)
                        {
                            Dust.NewDustPerfect(Projectile.Center + (Projectile.velocity.normalize() * 145f) + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<SquareDustFilled>(), Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 5f), 0, Color.MediumVioletRed, Main.rand.NextFloat(0.6f, 1.1f) + (Timer / 100));
                        }
                    }
                }
                else
                {
                    shooting = false;
                    if (Timer > 0)
                    {
                        Timer -= 3;
                    }
                    else
                    { 
                        coolingdown = false;
                    }
                }
            }
            //sound set to projectile center
            if (SoundEngine.TryGetActiveSound(SoundSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Projectile.Center;
            //sound kill
            if(!hasShot && !shooting)
            {
                StopSounds();
            }
            //recoil
            ofs *= 0.93f;
            Projectile.Center += Projectile.rotation.ToRotationVector2() * ofs;
            //change player direction
            player.ChangeDir(Projectile.direction);
            //timer n shot sfx
            if (Timer < 0)
            {
                Timer = 0;
            }
            //player arm rotation
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(player.direction == 1 ? -45f : -135f));

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D RailBot = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/KeiRGRailDOWN", AssetRequestMode.ImmediateLoad).Value;
            Texture2D RailUp = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/KeiRGRailUP", AssetRequestMode.ImmediateLoad).Value;
            Texture2D Tubes = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/KeiRGTubeThingy", AssetRequestMode.ImmediateLoad).Value;
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 BottomRail = Projectile.Center + (Projectile.rotation.ToRotationVector2() * -15f).RotatedBy(MathHelper.ToRadians(-25f * Projectile.direction));
            Vector2 UpperRail = Projectile.Center + (Projectile.rotation.ToRotationVector2() * -15f).RotatedBy(MathHelper.ToRadians(25f * Projectile.direction));
            Vector2 tube = Projectile.Center + (Projectile.rotation.ToRotationVector2() * -9f).RotatedBy(MathHelper.ToRadians(45f * Projectile.direction));
            Vector2 tubelerped = Vector2.SmoothStep(tube, Projectile.Center, 1f - lerpamnt) - Main.screenPosition;
            Vector2 BottomRailLerp = Vector2.SmoothStep(Projectile.Center, BottomRail, lerpamnt) - Main.screenPosition;
            Vector2 UpperRailLerp = Vector2.SmoothStep(Projectile.Center, UpperRail, lerpamnt) - Main.screenPosition;

            Main.EntitySpriteDraw(Tubes, coolingdown? tubelerped : drawPosition, null, lightColor, Projectile.rotation, Tubes.Size() / 2f, Projectile.scale, effects);
            Main.EntitySpriteDraw(tex, drawPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, effects);
            Main.EntitySpriteDraw(RailBot, BottomRailLerp, null, lightColor, Projectile.rotation, RailBot.Size() / 2f, Projectile.scale, effects);
            Main.EntitySpriteDraw(RailUp, UpperRailLerp, null, lightColor, Projectile.rotation, RailUp.Size() / 2f, Projectile.scale, effects);

            return false;
        }


    }
}
