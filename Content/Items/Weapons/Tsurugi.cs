using asuw.Content.Items.Weapons.Ranged;
using asuw.Content.Projectiles;
using asuw.Content.Projectiles.Filler;
using asuw.Content.Projectiles.InfoAndChargeBar;
using asuw.Effects;
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
using Terraria.ModLoader.IO;

namespace asuw.Content.Items.Weapons
{
    public class Tsurugi : ModItem, ILocalizedModType
    {
        //TODO weapon skill
        public bool[] shoot = new bool[] { false, false };
        public int[] chamber = new int[] { 2, 2 };
        int BoG = 0;

        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 180;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 68;
            Item.height = 15;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 40f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = new SoundStyle("asuw/Content/Sounds/Horus") { Volume = 0.7f, PitchVariance = 0.2f, MaxInstances = 30 };
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.noUseGraphic = true;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(shoot[0]);
            writer.Write(shoot[1]);
        }
        public override void NetReceive(BinaryReader reader)
        {
            shoot[0] = reader.ReadBoolean();
            shoot[1] = reader.ReadBoolean();
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

        public override void HoldItem(Player player)
        {
            int blood = ModContent.ProjectileType<BloodTsu>();
            int gunpowder = ModContent.ProjectileType<GunpowderTsu>();
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 Vel = player.asuw().mouseNormalFromPlayer;
                if (player.ownedProjectileCounts[blood] < 1)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, Vel, blood, Item.damage, Item.knockBack, player.whoAmI);
                }
                if (player.ownedProjectileCounts[gunpowder] < 1)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, Vel, gunpowder, Item.damage, Item.knockBack, player.whoAmI);
                }
            }
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = player.asuw().mouseNormalFromPlayer * velocity.Length();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 7;
            float rotation = MathHelper.ToRadians(7);
            shoot[BoG] = true;
            if (BoG < 1) BoG++;
            else BoG = 0;
            for (int i = 0; i < numberProjectiles; i++)
            {
                float rotationAngle = MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1));
                Vector2 perturbedSpeed = velocity.RotatedBy(rotationAngle);
                Vector2 offset = velocity.RotatedBy(MathHelper.PiOver2) * rotationAngle * -2;
                Projectile.NewProjectile(source, position + offset, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
           
            return false;
        }
    }

    public class BloodTsu : ModProjectile
    {
        public override string Texture => "asuw/Content/Items/Weapons/Tsurugi";
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
            Projectile.timeLeft = 30;
        }
        Player player => Projectile.GetOwner();
        ref float aimTo => ref Projectile.ai[0]; //aim
        ref float time => ref Projectile.ai[1];
        ref float phase => ref Projectile.ai[2]; // aiming(0), shooting(1), dumping(2), dumped(3)
        float originalAimTo = 0;
        float rotTomouse = 0;
        float recoil = 0;
        float throwrot = 0;
        float throwRotVar = 1;
        int direction = 1;

        //muzzle
        float muzzleOP = 0;
        float muzzleRot = 0;
        Vector2 muzzleScale = new Vector2(0, 0);
        Vector2 muzzlePos = new Vector2(0, 0);

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.spriteDirection);
            writer.Write(aimTo);
            writer.Write(rotTomouse);
            writer.Write(recoil);
            writer.Write((int)direction);
            writer.Write(muzzleOP);
            writer.WriteVector2(muzzlePos);
            writer.Write(muzzleRot); 
            writer.WriteVector2(muzzleScale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadInt32();
            aimTo = reader.ReadSingle();
            rotTomouse = reader.ReadSingle();
            recoil = reader.ReadSingle();
            direction = reader.ReadInt32();
            muzzleOP = reader.ReadSingle();
            muzzlePos = reader.ReadVector2();
            muzzleRot = reader.ReadSingle();
            muzzleScale = reader.ReadVector2();
        }

        Tsurugi heldWeapon()
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Tsurugi tsu)
                return tsu;
            else
                return null;
        }
        public override void OnSpawn(IEntitySource source)
        {
            originalAimTo = MathHelper.ToRadians(140);
            Projectile.rotation = Projectile.velocity.ToRotation() + aimTo;
            direction = Projectile.velocity.X > 0 ? 1 : -1;
            rotTomouse = 11;
        }
        public override void AI()
        {
            //if (player.ownedProjectileCounts[Projectile.type] >= 2)
            //{
            //	player.itemTime = 2;
            //	player.itemAnimation = 2;
            //}

            int phase0dur = (int)Math.Floor(20d /** attSpeed*/);
            int phase1dur = (int)Math.Floor(15d /** attSpeed*/);

            player.heldProj = Projectile.whoAmI;
            player.asuw().mouseWorldListener = true;

            if (phase != 3)
            {
                if (heldWeapon() != null && phase == 0)
                    Projectile.timeLeft = 2;
                player.ChangeDir(direction);
                AsuUtils.SetHandRotFront(player, Projectile.rotation);
            }

            Projectile.spriteDirection = direction;
            //player.PickAmmo(player.HeldItem, out int bulletAMMO, out float SpeedNoUse, out int bulletDamage, out float kBackNoUse, out _);

            if (phase == 0)
            {
                float lerp = MathHelper.Lerp(1, 0f, AsuUtils.BackOut(time / phase0dur));
                recoil *= 0.85f;
                updateDir();
                aimTo = originalAimTo;
                if (rotTomouse < 11) rotTomouse += 0.5f;

                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(player.asuw().mouseRotationFromPlayer, MathHelper.ToRadians(rotTomouse)).ToRotationVector2();
                Projectile.rotation = Projectile.velocity.ToRotation() + ((aimTo * lerp) * direction) + MathHelper.ToRadians(recoil);
                position(true);

                if (time < phase0dur) time++;
                if (heldWeapon() != null && heldWeapon().shoot[0])
                {
                    heldWeapon().shoot[0] = false;
                    heldWeapon().chamber[0]--;
                    time = 0;
                    recoil = -50 * direction;
                    Projectile.timeLeft = phase1dur + 10;
                    phase = 1;
                    aimTo = Projectile.rotation;
                    muzzleScale.X = 1;
                    muzzleScale.Y = 0.8f;
                    muzzleRot = Projectile.rotation;
                    muzzlePos = Projectile.Center + (Vector2.UnitX.RotatedBy(Projectile.rotation).RotatedBy(MathHelper.ToRadians(-7 * direction)) * TextureAssets.Projectile[Type].Value.Width * 0.7f);
                    muzzleSmokes();
                }
            }
            else if (phase == 1)
            {
                Projectile.velocity = aimTo.ToRotationVector2();
                Projectile.rotation = aimTo + MathHelper.ToRadians(recoil);
                recoil *= 0.85f;
                rotTomouse = 1;
                position(true);
                if (time < phase1dur) time++;
                else
                {
                    if (heldWeapon() != null)
                    {
                        if (heldWeapon().chamber[0] <= 0)
                        {
                            time = 0;
                            Projectile.timeLeft = 30;
                            phase = 2;
                        }
                        else
                        {
                            time = phase0dur;
                            phase = 0;
                            Projectile.ResetLocalNPCHitImmunity();
                            Projectile.numHits = 0;
                        }
                    }
                }
            }
            else if (phase == 2)
            {
                float phaseUp = (int)Math.Floor(13d); ;
                float phaseThrow = (int)Math.Floor(18d); ;
                Projectile.velocity = aimTo.ToRotationVector2();
                position(true);
                if (time < phaseUp)
                {
                    float rotAmnt = -MathHelper.PiOver2;
                    float lerp = Utils.GetLerpValue(0f, 1f, time / phaseUp);
                    Projectile.rotation = aimTo + (rotAmnt * AsuUtils.QuadIn(lerp) * direction);
                }
                else
                {
                    float rotAmnt = MathHelper.Pi;
                    float lerp = Utils.GetLerpValue(0f, 1f, (time - phaseUp) / phaseThrow);
                    Projectile.rotation = aimTo + (-MathHelper.PiOver2 * direction) + (rotAmnt * AsuUtils.QuintOut(lerp) * direction);
                }

                if (time < phaseUp + (phaseThrow / 3f)) time++;
                else
                {
                    time = 0;
                    phase = 3;
                    throwrot = Projectile.rotation;
                    throwRotVar = Main.rand.NextFloat(1.3f, 1.7f);
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * 10;
                    Projectile.timeLeft = 80;
                }
            }
            else if (phase == 3)
            {
                float rotAmnt = throwrot + (MathHelper.TwoPi * direction * throwRotVar);
                float lerp = Utils.GetLerpValue(0f, 1f, AsuUtils.CircOut(time / 80f));
                Projectile.rotation = rotAmnt * lerp;
                Projectile.velocity *= 0.93f;
                position(false);
                if (Projectile.Opacity > 0.1f) Projectile.Opacity *= 0.85f;
                else { Projectile.Opacity = 0; Projectile.Kill(); }

                if (time < 80) time++;
                else { Projectile.Kill(); }

            }
            else
            {
                updateDir();
                Projectile.Opacity = 0;
                if (time < 20) time++;
                else
                {
                    time = 0;
                    phase = 0;
                }
            }

            if (muzzleScale.X > 0) muzzleScale.X *= 0.7f;
            if (muzzleScale.Y > 0) muzzleScale.Y *= 0.6f;
            if (muzzleScale.Length() > float.Epsilon) muzzleOP = 1;
            else muzzleOP = 0;

            Projectile.ForceNetUpdate();
        }
  
        public override void OnKill(int timeLeft)
        {
            if (heldWeapon() != null && heldWeapon().chamber[0] <= 0)
                heldWeapon().chamber[0] = 2;
        }
        void updateDir()
        {
            if (player.asuw().mouseNormalFromPlayer.X < 0)
                direction = -1;
            else
                direction = 1;
        }
        void position(bool stickToHand)
        {
            if (stickToHand)
            {
                Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
            }
            else
            {
                Projectile.Center += Projectile.velocity;
            }
        }
        void muzzleSmokes()
        {
            Vector2 vel = Vector2.UnitX.RotatedBy(muzzleRot) * 40;
            for (int i = 1; i <= 20; i++)//straight
            {
                //smokes
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture;
            texture = TextureAssets.Projectile[Type].Value;
            Texture2D muzzle = ModContent.Request<Texture2D>("asuw/Assets/Muzzle", AssetRequestMode.AsyncLoad).Value;
            Texture2D muzzleSpark = ModContent.Request<Texture2D>("asuw/Assets/MuzzleSpark", AssetRequestMode.AsyncLoad).Value;
            Vector2 origin = new Vector2(texture.Width * 0.33f, texture.Height * 0.6f);
            Vector2 origin2 = new Vector2(0, muzzle.Height * 0.5f);
            Vector2 origin2s = new Vector2(0, muzzleSpark.Height * 0.5f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);

            Effect shader = ModContent.Request<Effect>("asuw/Effects/ColorizeBloom", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            shader.Parameters["color2"].SetValue((Color.OrangeRed).ToVector4());
            shader.Parameters["color1"].SetValue((Color.Maroon).ToVector4());
            shader.Parameters["alpha"].SetValue(Projectile.Opacity);
            shader.CurrentTechnique.Passes["EffectPass"].Apply();

            Main.spriteBatch.Draw(muzzleSpark, muzzlePos - Main.screenPosition, default, lightColor * muzzleOP, muzzleRot, origin2s, 1.2f * muzzleScale * new Vector2(1, 0.5f), effects, 0);
            Main.spriteBatch.Draw(muzzle, muzzlePos - Main.screenPosition, default, lightColor * muzzleOP, muzzleRot, origin2, 2f * muzzleScale, effects, 0);
            Main.spriteBatch.ExitShaderRegion();

            return false;
        }
    }

    public class GunpowderTsu : ModProjectile
    {
        public override string Texture => "asuw/Content/Items/Weapons/TsurugiBack";
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
            Projectile.timeLeft = 30;
            Projectile.friendly = false;
        }
        Player player => Projectile.GetOwner();
        ref float aimTo => ref Projectile.ai[0]; //aim
        ref float time => ref Projectile.ai[1];
        ref float phase => ref Projectile.ai[2]; // aiming(0), shooting(1), dumping(2), dumped(3)
        float originalAimTo = 0;
        float rotTomouse = 0;
        float recoil = 0;
        float throwrot = 0;
        float throwRotVar = 1;
        int direction = 1;

        //muzzle
        float muzzleOP = 0;
        float muzzleRot = 0;
        Vector2 muzzleScale = new Vector2(0, 0);
        Vector2 muzzlePos = new Vector2(0, 0);

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.spriteDirection);
            writer.Write(aimTo);
            writer.Write(rotTomouse);
            writer.Write(recoil);
            writer.Write((int)direction);
            writer.Write(muzzleOP);
            writer.WriteVector2(muzzlePos);
            writer.Write(muzzleRot);
            writer.WriteVector2(muzzleScale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadInt32();
            aimTo = reader.ReadSingle();
            rotTomouse = reader.ReadSingle();
            recoil = reader.ReadSingle();
            direction = reader.ReadInt32();
            muzzleOP = reader.ReadSingle();
            muzzlePos = reader.ReadVector2();
            muzzleRot = reader.ReadSingle();
            muzzleScale = reader.ReadVector2();
        }
        Tsurugi heldWeapon()
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Tsurugi tsu)
                return tsu;
            else
                return null;
        }
        public override void OnSpawn(IEntitySource source)
        {
            originalAimTo = MathHelper.ToRadians(140);
            Projectile.rotation = Projectile.velocity.ToRotation() + aimTo;
            direction = Projectile.velocity.X > 0 ? 1 : -1;
            rotTomouse = 11;
        }
        public override void AI()
        {
            //if (player.ownedProjectileCounts[Projectile.type] >= 2)
            //{
            //	player.itemTime = 2;
            //	player.itemAnimation = 2;
            //}

            int phase0dur = (int)Math.Floor(20d);
            int phase1dur = (int)Math.Floor(15d);

            if (phase != 3)
            {
                if (heldWeapon() != null && phase == 0)
                    Projectile.timeLeft = 2;

                player.ChangeDir(direction);
                AsuUtils.SetHandRotBack(player, Projectile.rotation);
            }
            Projectile.spriteDirection = direction;
            player.asuw().mouseWorldListener = true;
            player.asuw().mouseRotationListener = true;

            //player.PickAmmo(player.HeldItem, out int bulletAMMO, out float SpeedNoUse, out int bulletDamage, out float kBackNoUse, out _);

            if (phase == 0)
            {
                float lerp = MathHelper.Lerp(1, 0f, AsuUtils.BackOut(time / phase0dur));
                recoil *= 0.85f;
                updateDir();
                aimTo = originalAimTo;
                if (rotTomouse < 11) rotTomouse += 0.5f;
                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(player.asuw().mouseRotationFromPlayer, MathHelper.ToRadians(rotTomouse)).ToRotationVector2();
                Projectile.rotation = Projectile.velocity.ToRotation() + ((aimTo * lerp) * direction) + MathHelper.ToRadians(recoil);
                position(true);

                if (time < phase0dur) time++;
                if (heldWeapon() != null && heldWeapon().shoot[1])
                {
                    heldWeapon().shoot[1] = false;
                    heldWeapon().chamber[1]--;
                    time = 0;
                    recoil = -50 * direction;
                    Projectile.timeLeft = phase1dur + 10;
                    phase = 1;
                    aimTo = Projectile.rotation;
                    muzzleScale.X = 1;
                    muzzleScale.Y = 0.8f;
                    muzzleRot = Projectile.rotation;
                    muzzlePos = Projectile.Center + (Vector2.UnitX.RotatedBy(Projectile.rotation).RotatedBy(MathHelper.ToRadians(-7 * direction)) * TextureAssets.Projectile[Type].Value.Width * 0.7f);
                    muzzleSmokes();
                }
            }
            else if (phase == 1)
            {
                Projectile.velocity = aimTo.ToRotationVector2();
                Projectile.rotation = aimTo + MathHelper.ToRadians(recoil);
                recoil *= 0.85f;
                rotTomouse = 1;
                position(true);
                if (time < phase1dur) time++;
                else
                {
                    if (heldWeapon() != null )
                    {
                        if (heldWeapon().chamber[1] <= 0)
                        {
                            time = 0;
                            phase = 2;
                            Projectile.timeLeft = 30;
                        }
                        else
                        {
                            time = phase0dur;
                            phase = 0;
                            Projectile.ResetLocalNPCHitImmunity();
                            Projectile.numHits = 0;
                        }
                    }
                }
            }
            else if (phase == 2)
            {
                float phaseUp = (int)Math.Floor(13d); ;
                float phaseThrow = (int)Math.Floor(18d); ;
                Projectile.velocity = aimTo.ToRotationVector2();
                position(true);
                if (time < phaseUp)
                {
                    float rotAmnt = -MathHelper.PiOver2;
                    float lerp = Utils.GetLerpValue(0f, 1f, time / phaseUp);
                    Projectile.rotation = aimTo + (rotAmnt * AsuUtils.QuadIn(lerp) * direction);
                }
                else
                {
                    float rotAmnt = MathHelper.Pi;
                    float lerp = Utils.GetLerpValue(0f, 1f, (time - phaseUp) / phaseThrow);
                    Projectile.rotation = aimTo + (-MathHelper.PiOver2 * direction) + (rotAmnt * AsuUtils.QuintOut(lerp) * direction);
                }

                if (time < phaseUp + (phaseThrow / 3f)) time++;
                else
                {
                    time = 0;
                    phase = 3;
                    throwrot = Projectile.rotation;
                    throwRotVar = Main.rand.NextFloat(1.3f, 1.7f);
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * 10;
                    Projectile.timeLeft = 80;
                }
            }
            else if (phase == 3)
            {
                float rotAmnt = throwrot + (MathHelper.TwoPi * direction * throwRotVar);
                float lerp = Utils.GetLerpValue(0f, 1f, AsuUtils.CircOut(time / 80f));
                Projectile.rotation = rotAmnt * lerp;
                Projectile.velocity *= 0.93f;
                position(false);
                if (Projectile.Opacity > 0.1f) Projectile.Opacity *= 0.85f;
                else { Projectile.Opacity = 0; Projectile.Kill(); }

                if (time < 80) time++;
                else { Projectile.Kill(); }

            }
            else
            {
                updateDir();
                Projectile.Opacity = 0;
                if (time < 20) time++;
                else
                {
                    time = 0;
                    phase = 0;
                }
            }

            if (muzzleScale.X > 0) muzzleScale.X *= 0.7f;
            if (muzzleScale.Y > 0) muzzleScale.Y *= 0.6f;
            if (muzzleScale.Length() > float.Epsilon) muzzleOP = 1;
            else muzzleOP = 0;

            Projectile.ForceNetUpdate();
        }

        public override void OnKill(int timeLeft)
        {
            if (heldWeapon() != null && heldWeapon().chamber[1] <= 0)
                heldWeapon().chamber[1] = 2;
        }
        void updateDir()
        {
            if (player.asuw().mouseNormalFromPlayer.X < 0)
                direction = -1;
            else
                direction = 1;
        }
        void position(bool stickToHand)
        {
            if (stickToHand)
            {
                Projectile.Center = player.GetBackHandPositionImproved(player.compositeBackArm);
            }
            else
            {
                Projectile.Center += Projectile.velocity;
            }
        }

        void muzzleSmokes()
        {
            Vector2 vel = Vector2.UnitX.RotatedBy(muzzleRot) * 40;
            for (int i = 1; i <= 20; i++)
            {
              //smokes
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D muzzle = ModContent.Request<Texture2D>("asuw/Assets/Muzzle", AssetRequestMode.AsyncLoad).Value;
            Texture2D muzzleSpark = ModContent.Request<Texture2D>("asuw/Assets/MuzzleSpark", AssetRequestMode.AsyncLoad).Value;
            Vector2 origin2 = new Vector2(0, muzzle.Height * 0.5f);
            Vector2 origin2s = new Vector2(0, muzzleSpark.Height * 0.5f);

            ShaderFunctions.vertexColorBloom(Main.spriteBatch, Color.Maroon, Color.OrangeRed, Projectile.Opacity);

            Main.spriteBatch.Draw(muzzleSpark, muzzlePos - Main.screenPosition, default, lightColor * muzzleOP, muzzleRot, origin2s, 1.2f * muzzleScale * new Vector2(1,0.5f), effects, 0);
            Main.spriteBatch.Draw(muzzle, muzzlePos - Main.screenPosition, default, lightColor * muzzleOP, muzzleRot, origin2, 2f * muzzleScale, effects, 0);
            Main.spriteBatch.ExitShaderRegion();

            return false;
        }

        public static void drawOffhand(ref PlayerDrawSet drawInfo, Player player, Color lightColor)
        {
            Projectile projectile = null;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<GunpowderTsu>() && proj.owner == player.whoAmI)
                {
                    projectile = proj;
                    break;
                }
            }

            if (projectile == null)
                return;

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture;
            texture = TextureAssets.Projectile[projectile.type].Value;
            Vector2 origin = new Vector2(texture.Width * 0.33f, texture.Height * 0.6f);
            DrawData heldProj = new DrawData(texture, projectile.Center - Main.screenPosition, default, lightColor * projectile.Opacity, projectile.rotation, origin, projectile.scale, effects, 0);

            drawInfo.DrawDataCache.Add(heldProj);
        }
       
    }

   
    

}
