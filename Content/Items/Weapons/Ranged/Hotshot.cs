using asuw.Content.Dusts;
using asuw.Content.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons.Ranged
{
    public class Hotshot : ModItem, ILocalizedModType
    {
        //TODO better sprite
        public bool shoot = false;
        public bool reloading = false;
        public bool shootingBluething = false;
        public int reloadDir = 1;
        public int magCap = 8;
        public int currentMag = 8;
        public int perfectBodyguard = 0;
        public bool reloadParry = false; //more like invincibility

        public static SoundStyle shot = new SoundStyle("asuw/Content/Sounds/Hotshot");
        public static SoundStyle release = new SoundStyle("asuw/Content/Sounds/HotshotRelease");
        public static SoundStyle insert = new SoundStyle("asuw/Content/Sounds/HotshotIn");
        public static SoundStyle dryShot = new SoundStyle("asuw/Content/Sounds/DryShot") with { Pitch = -0.5f };
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.ArmorPenetration = 5;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 12;
            Item.height = 12;
            Item.useTime = 10;
            Item.useAnimation = 20;
            Item.reuseDelay = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shootSpeed = 140f;
            Item.shoot = ModContent.ProjectileType<HotshotHeld>();
            Item.useAmmo = AmmoID.Bullet;
            Item.value = Item.buyPrice(silver: 1);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.rare = ItemRarityID.LightRed;
            Item.knockBack = 3f;

        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(shoot);
            writer.Write(shootingBluething);
        }
        public override void NetReceive(BinaryReader reader)
        {
            shoot = reader.ReadBoolean();
            shootingBluething = reader.ReadBoolean();
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit += perfectBodyguard;
        }
        public override bool CanUseItem(Player player) => reloading ? false : true;
        public override bool CanConsumeAmmo(Item ammo, Player player) => currentMag > 0;
        public override bool AltFunctionUse(Player player) => currentMag < magCap && !reloading;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += (velocity.normalize() * 50).RotatedBy(MathHelper.ToRadians(-16 * player.direction));
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
            {
                reloading = true;
                if (perfectBodyguard < 15 && currentMag == 0) perfectBodyguard++;
                if (player.mouseWorld().X > player.Center.X) reloadDir = 1;
                else reloadDir = -1;
            }
            else
            {
                shoot = true;
                if (shootingBluething)
                {
                    Projectile.NewProjectile(source, position, velocity / 3, ModContent.ProjectileType<HotshotConcentratedBullet>(), damage + perfectBodyguard.SquaredI() * 2 * 2, knockback, player.whoAmI);
                    SoundEngine.PlaySound(new SoundStyle("asuw/Content/Sounds/HotshotBig") with { Volume = 1.25f }, player.Center);
                    player.itemAnimation = player.itemTime;
                }
                else
                {
                    if (currentMag > 0)
                    {
                        Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.03f), type, damage, knockback, player.whoAmI);
                    }
                    else
                    {
                        //poof
                    }
                }
               
                
            }
            return false;
        }

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 vel = player.asuw().mouseNormalFromPlayer;
                if (player.ownedProjectileCounts[Item.shoot] < 1)
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, vel, Item.shoot, Item.damage, Item.knockBack, player.whoAmI);

                Vector2 pos = player.Center + (vel * 60).RotatedBy(MathHelper.ToRadians(-15 * player.direction));
                if (AsuKeybinds.WeaponSkill.JustPressed && perfectBodyguard > 0  && !shootingBluething)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        Dust dust = Dust.NewDustPerfect(player.Center, ModContent.DustType<SharpSparkDust>(), AsuUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(7f, 11f), 0, Color.DodgerBlue, Main.rand.NextFloat(1f, 1.4f));
                        dust.noGravity = true;
                    }
                    shootingBluething = true;
                    //TODO add activation sfx
                }
            }
        }
    }

    public class HotshotHeld : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.localNPCHitCooldown = 6;
        }
        Player player => Projectile.GetOwner();
        ref float reloadTime => ref Projectile.ai[2];
        int reloadDir = 1;
        ref float aim => ref Projectile.ai[0];
        float aimAmnt = 0;
        ref float recoil => ref Projectile.ai[1];
        float swirlOP = 0;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.spriteDirection);
            writer.Write(aim);
            writer.Write(aimAmnt);
            writer.Write(reloadDir);
            writer.Write(swirlOP);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadInt32();
            aim = reader.ReadSingle();
            aimAmnt = reader.ReadSingle();
            reloadDir = reader.ReadInt32();
            swirlOP = reader.ReadSingle();
        }
        public override void OnSpawn(IEntitySource source)
        {
            aim = player.asuw().mouseRotationFromPlayer;
        }
        public override void AI()
        {
            Projectile.scale = 0.85f;
            player.heldProj = Projectile.whoAmI;
            aim = player.asuw().mouseRotationFromPlayer;
            if (aimAmnt > 15) aimAmnt = 15;
            if (aimAmnt < 15) aimAmnt += 0.5f;
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Hotshot hot)
            {
                reloadDir = hot.reloadDir;
                if (!player.dead) Projectile.timeLeft = 2;
                if (!hot.reloading)//not reloading
                {
                    if (hot.shoot)//normal att
                    {
                        hot.shoot = false;
                        recoil = hot.currentMag > 0 || hot.shootingBluething ? - 30 : -10;
                        muzzle(hot.currentMag, hot.shootingBluething);
                        if (hot.currentMag > 0)
                        {
                            SoundEngine.PlaySound(Hotshot.shot with { MaxInstances = 9, PitchVariance = 0.3f, Volume = 0.4f });
                            if(!hot.shootingBluething && hot.currentMag > 0)
                            hot.currentMag--;
                        }
                        else
                        SoundEngine.PlaySound(Hotshot.dryShot with { MaxInstances = 9, PitchVariance = 0.1f, Volume = 0.33f });
                        if (hot.shootingBluething) hot.shootingBluething = false;
                    }
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(aim, MathHelper.ToRadians(aimAmnt)).ToRotationVector2();
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(recoil * Projectile.direction);
                    Projectile.spriteDirection = Projectile.direction;
                    player.ChangeDir(Projectile.direction);
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation * player.gravDir - MathHelper.ToRadians(90f));
                    reloadTime = 0;
                    swirlOP = 0;
                    if (hot.shootingBluething)//bluespew dusts
                    {
                        Vector2 pos = Projectile.Center + player.velocity + (Projectile.velocity.normalize() * 52).RotatedBy(MathHelper.ToRadians(-13f * Projectile.direction));
                        Dust dust = Dust.NewDustPerfect(pos, ModContent.DustType<SharpSparkDust>(), (Vector2.UnitY * Main.rand.NextFloat(-2, -4)).RotatedByRandom(0.5f), 0, Color.DodgerBlue, Main.rand.NextFloat(0.7f, 1f));
                        dust.noGravity = true;
                    }
                }
                else//reloading
                {
                    hot.shoot = false;
                    player.itemTime = 9;
                    player.itemAnimation = 9;
                    aimAmnt = 7f;
                    float dirRot = reloadDir == 1 ? 0 : MathHelper.Pi;
                    float reloadAngle;
                    float armAngle;
                    float phase1n2 = 14;
                    float phase3 = 21;
                    if (reloadTime < phase1n2)//up
                    {
                        float rotAmnt = -MathHelper.PiOver2;
                        float lerp = Utils.GetLerpValue(0f, 1f, reloadTime / phase1n2);
                        reloadAngle = MathHelper.PiOver4 + rotAmnt * AsuUtils.SineOut(lerp);
                        armAngle = reloadAngle;
                        swirlOP = 0;
                        //TODO smoother transition to reloading pos
                    }
                    else if (reloadTime < phase1n2 * 2 && reloadTime >= phase1n2)//slam down
                    {
                        float rotAmnt = MathHelper.PiOver2;
                        float lerp = Utils.GetLerpValue(0f, 1f, (reloadTime - phase1n2) / phase1n2);
                        reloadAngle = -MathHelper.PiOver4 + rotAmnt * AsuUtils.BounceOut(lerp);
                        armAngle = reloadAngle;
                        if(reloadTime == 20)//magdump
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Vector2.UnitX * -reloadDir).RotatedBy(MathHelper.ToRadians(-30 * reloadDir)) * Main.rand.NextFloat(15,20) ,ModContent.ProjectileType<HotshotMag>(), Projectile.damage, Projectile.knockBack, player.whoAmI);

                            for (int i = 0; i < 4; i++)
                                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpSparkDust>(), (Vector2.UnitX * -reloadDir).RotatedBy(MathHelper.ToRadians(-30 * reloadDir)).RotatedByRandom(0.3f) * Main.rand.NextFloat(2, 4), 0, Color.Khaki, Main.rand.NextFloat(0.4f, 0.7f));

                            SoundEngine.PlaySound(Hotshot.release);
                            hot.currentMag = 0;
                        }
                        swirlOP = 0;
                    }
                    else if (reloadTime < phase3 + phase1n2 * 2 && reloadTime >= phase1n2 * 2)//swirl
                    {
                        float rotAmnt = MathHelper.TwoPi * -2;
                        float lerp = Utils.GetLerpValue(0f, 1f, (reloadTime - phase1n2 * 2) / phase3);
                        reloadAngle = rotAmnt * lerp;
                        armAngle = MathHelper.PiOver4 - MathHelper.PiOver2 * lerp;
                        hot.reloadParry = true;
                        if(swirlOP < 0.4f)swirlOP += 0.05f;
                        if (hot.currentMag < hot.magCap && reloadTime % 3 == 0)
                        {
                            hot.currentMag++;
                            SoundEngine.PlaySound(new SoundStyle("asuw/Content/Sounds/ElephantKillerWoosh") with { MaxInstances = 10 , Volume = 0.7f }, Projectile.Center);
                        }
                    }
                    else//done
                    {
                        reloadAngle = -MathHelper.PiOver4;
                        armAngle = reloadAngle;
                        hot.reloadParry = false;
                        if (reloadTime >= 0)
                        {
                            hot.reloading = false;
                            hot.currentMag = hot.magCap;
                            for (int i = 0; i < 4; i++)
                                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpSparkDust>(), (Vector2.UnitX * reloadDir).RotatedBy(MathHelper.ToRadians(-40 * reloadDir)).RotatedByRandom(0.3f) * Main.rand.NextFloat(3, 6), 0, Color.WhiteSmoke * 0.5f, Main.rand.NextFloat(0.4f, 0.7f));
                            SoundEngine.PlaySound(Hotshot.insert);
                        }
                        swirlOP = 0;
                    }
                    Projectile.rotation = dirRot + reloadAngle * reloadDir;
                    Projectile.velocity = Projectile.rotation.ToRotationVector2();
                    Projectile.spriteDirection = reloadDir;
                    player.ChangeDir(reloadDir);
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, dirRot + armAngle * reloadDir * player.gravDir - MathHelper.ToRadians(90f));
                    reloadTime++;
                }//reload
            }//holdingitem
            Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
            recoil *= 0.8f;
            Projectile.ForceNetUpdate();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center + Vector2.UnitX * (-texture.Width * 0.8f);
            Vector2 end = Projectile.Center + Vector2.UnitX * (texture.Width * 0.8f);
            float collisionPoint = 0f;
            float collisionWidth = texture.Width * 1.6f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override bool? CanDamage() => reloadTime >= 30 ? null : false; //only when swirling the gun
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 origin = new Vector2(texture.Width * 0.13f, texture.Height * 0.5f);
 


            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);

            return false;
        }
        void muzzle(int ammo, bool blue)
        {
            Vector2 pos = Projectile.Center + (Projectile.velocity.normalize() * 55).RotatedBy(MathHelper.ToRadians(-16 * Projectile.direction));
            if (blue)
            {
                float rot = MathHelper.ToRadians(Main.rand.NextFloat(10, 22));
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        //float rotate = Projectile.velocity.RotatedByRandom(0.3f).ToRotation() + rot * (i == 0 ? 1 : -1);
                        //GeneralParticleHandler.SpawnParticle(new CustomPulse(pos, (rotate.ToRotationVector2() * Main.rand.NextFloat(10f, 15f)), j < 3 ? Color.SkyBlue : Color.DodgerBlue, "CalamityMod/Particles/ShatteredExplosion", new Vector2(1, 0.1f), rotate, 0.004f, 0.05f + (j < 3 ? 0 : 0.02f), 7, true));
                    }
                }
            }
            else
            {
                if (ammo > 0)
                {
                    float rot = Main.rand.NextFloat(-15f, 15f);
                    for (int i = 0; i < 2; i++)
                    {
                        //GeneralParticleHandler.SpawnParticle(new CustomPulse(pos, Vector2.Zero, i == 0 ? Color.Gold : Color.Orange, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, rot, 0.004f, 0.02f + (i == 0 ? 0 : 0.01f), 5, true));
                        //GeneralParticleHandler.SpawnParticle(new CustomPulse(pos, Vector2.Zero, i == 0 ? Color.Gold : Color.Khaki, "CalamityMod/Particles/BloomCircle", Vector2.One, 0, 0.1f, i == 0 ? 0.5f : 0.35f, 5, true));
                    }
                    for (int j = 0; j < 9; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(pos, ModContent.DustType<SharpSparkDust>(), (Projectile.velocity.normalize() * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(0.3f), 0, Color.Goldenrod, Main.rand.NextFloat(0.6f, 1f));
                        dust.noGravity = true;
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        //GeneralParticleHandler.SpawnParticle(new SmallSmokeParticle(pos, (Projectile.velocity.normalize() * Main.rand.NextFloat(3f, 5f)).RotatedByRandom(0.3f), Color.WhiteSmoke, Color.Lerp(Color.LightSlateGray, Color.DarkSlateBlue, 0.5f), Main.rand.NextFloat(0.5f, 0.8f), 100));
                    }
                }
            }
        }
    }

    public class HotshotConcentratedBullet : ModProjectile
    {
        Player player => Projectile.GetOwner();
        public override string Texture => "asuw/Assets/Blank";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.width = Projectile.height = 18;
            Projectile.timeLeft = 30;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 4;
        }
        public override void AI()
        {
            player.itemTime = player.itemAnimation = 8;
            if (Projectile.timeLeft <= 28)
            {
                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 pos = Projectile.Center + Projectile.velocity * 0.5f * i;
                    //SparkParticle spark = new SparkParticle(pos - Projectile.velocity.normalize() * 3.5f, Projectile.velocity * 0.02f, false, 5, 2f, Color.SkyBlue * 0.5f);
                    //GeneralParticleHandler.SpawnParticle(spark);

                    for (int j = 0; j < 2; j++)
                    {
                        Dust trailingdust = Dust.NewDustPerfect(pos + Vector2.UnitX * Main.rand.NextFloat(-Projectile.velocity.Length()) * i, ModContent.DustType<SharpSparkDust>(), (Projectile.velocity * -0.15f).RotatedBy(MathHelper.ToRadians(10 * i)).RotatedByRandom(0.3f), 0, Color.SkyBlue, Main.rand.NextFloat(0.4f, 0.7f));
                        trailingdust.noGravity = true;
                    }
                }
            }
        }
        public override bool? CanDamage() => Projectile.numHits >= 3 ? false : null;  
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center + Projectile.velocity.normalize() * -30;
            Vector2 end = Projectile.Center + Projectile.velocity.normalize() * 30;
            float collisionPoint = 0f;
            float collisionWidth = 20;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 2; i++)
            {
                //GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center + Projectile.velocity.normalize() * 12, Vector2.Zero, i == 0 ? Color.SkyBlue : Color.Khaki, "CalamityMod/Particles/BloomCircle", Vector2.One, 0, 0.1f, i == 0 ? 0.5f : 0.35f, 6, true));
            }
            for (int k = 0; k < 9; k++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpSparkDust>(), (Projectile.velocity.normalize() * Main.rand.NextFloat(5f, 9f)).RotatedByRandom(0.5f), 0, Main.rand.NextBool() ? Color.SkyBlue : Color.DodgerBlue, Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Hotshot hot)
            {
                hot.perfectBodyguard = 0;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage -= Projectile.numHits / 3f;
        }
        public override void OnKill(int timeLeft)
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Hotshot hot)
            {
                if (Projectile.numHits == 0)
                    hot.perfectBodyguard = (int)(hot.perfectBodyguard / 2f);
                else hot.perfectBodyguard = 0;
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }

    public class HotshotMag : ModProjectile
    {
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
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = false;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = true;
        }
        ref float time => ref Projectile.ai[2];
        float grav = 0.1f;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }
        public override void AI()
        {
            float rotTo = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.rotation = Projectile.rotation.RotTowards(rotTo,MathHelper.ToRadians(9));
            Projectile.Opacity = Utils.GetLerpValue(1f, 0f, time / 90f, true);
            Projectile.velocity.X *= 0.99f;
            if(Projectile.velocity.Y < 20) Projectile.velocity.Y += grav;
            grav += 0.1f;

            time++;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if ( Projectile.velocity.Length() > 1f)
            {
                Projectile.velocity = -Projectile.oldVelocity.RotatedBy(MathHelper.PiOver2 * Projectile.direction) / 2;
            }
            grav *= 0.5f;
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
           overPlayers.Add(index);
        }
    }
}
