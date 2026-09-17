using asuw.Content.Buffs;
using asuw.Content.Cooldown;
using asuw.Content.Dusts;
using asuw.Content.Global;
using asuw.Content.Particles;
using asuw.Content.Projectiles;
using asuw.Effects;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons.Melee
{
    public class BlackBlade : ModItem
    {
        int attDir = 1;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 144;
            Item.ArmorPenetration = 5;
            Item.crit = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = 1;
            Item.height = 1;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<BlackBladeHold>();
            Item.value = Item.buyPrice(silver: 1);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Purple;
            Item.knockBack = 7f;
            Item.channel = true;


        }
        public override bool CanUseItem(Player player)
        {
            if (player.asuw().UseCooldown > 0)
                return false;
            else return true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            if (ModLoader.TryGetMod("CalamityMod", out Mod CalamityMod) && CalamityMod.TryFind("LifeAlloy", out ModItem LifeAlloy) && CalamityMod.TryFind("DivineGeode", out ModItem DivineGeode))
            {
                recipe.AddIngredient(LifeAlloy, 5);
                recipe.AddIngredient(ItemID.FragmentNebula, 10);
                recipe.AddIngredient(ItemID.LunarBar, 5);

            }
            else
            {

                recipe.AddIngredient(ItemID.FragmentNebula, 10);
                recipe.AddIngredient(ItemID.LunarBar, 5);

            }
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BlackBladeSkill>(), damage, knockback, player.whoAmI);
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attDir);
                attDir *= -1;
            }
            return false;
        }
    }

    public class MalikethBlackBlade : ModItem
    {
        int attDir = 1;
        int spellCount = 0;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 334;
            Item.ArmorPenetration = 5;
            Item.crit = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = 1;
            Item.height = 1;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<BlackBladeHold>();
            Item.value = Item.buyPrice(silver: 1);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Purple;
            Item.knockBack = 7f;
            Item.channel = true;


        }
        public override bool CanUseItem(Player player)
        {
            if (player.asuw().UseCooldown > 0)
                return false;
            else return true;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
        
        }
       
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (spellCount > 0)
            {
                if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BlackBladeSkill>(), damage, knockback, player.whoAmI, 1);
                else
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BlackBladeSpell>(), damage, knockback, player.whoAmI);
                }
                spellCount--;
            }
            else
            {
                if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
                    spellCount = 2;
                else
                {
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attDir, 1);
                    attDir *= -1;
                }
            }
            return false;
        }
    }

    public class BlackBladeHold : ModProjectile
    {
        public override string Texture => "asuw/Content/Items/Weapons/Melee/BlackBlade";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 30;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 10;
        }
        Player player => Projectile.GetOwner();
        ref float attDir => ref Projectile.ai[0];
        bool isMalikeths => Projectile.ai[1] == 1;
        ref float time => ref Projectile.ai[2];

        int duration = 0;
        int dir = 1;
        float rotTo = 0;
        float speedRatio = 1;
        //trail parameter
        List<float> odr = new List<float>();
        float auraLength = 0;

        List<Vector2> tip = new List<Vector2>();
        List<Vector2> blade = new List<Vector2>();
        public override void OnSpawn(IEntitySource source)
        {
            dir = player.direction * (int)attDir;
            speedRatio = (float)player.itemTimeMax / (isMalikeths ? 40f : 50f); //40 / 50 is base speed
            duration = player.itemTimeMax * Projectile.MaxUpdates;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(130 * dir);
        }
        public override void AI()
        {
            int prep = (int)(duration * 0.4f);
            float t = time / (int)(duration / 2f);
            float v = t <= 1f ? t : 2f - t;
            float lerp = time < prep ? AsuUtils.QuadOut(time / prep) : AsuUtils.ExpoOut((time - prep) / (duration - prep));
            player.ChangeDir(Projectile.direction);
            dir = player.direction * (int)attDir;
            if (time < prep)
            {
                Projectile.velocity = player.asuw().mouseNormalFromPlayer;
                rotTo = Projectile.velocity.ToRotation() + MathHelper.ToRadians((130 + 30 * lerp) * dir);
                float dis = Projectile.rotation.AngleBetween(rotTo);
                float maxChange = (MathF.Abs(dis) / (MathHelper.PiOver2 * 1.5f)) / Projectile.MaxUpdates;
                Projectile.rotation = Projectile.rotation.RotTowards(rotTo, maxChange);
            }
            else
            {
                Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.ToRadians(25) * (time / duration) * dir).ToRotation() + MathHelper.ToRadians((160 - 290 * lerp) * dir);
            }
            player.heldProj = Projectile.whoAmI;
            player.SetHandRotFront(Projectile.rotation);
            Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
            Projectile.scale = isMalikeths ? 1.8f : 1.5f;
            Projectile.Opacity = MathHelper.Lerp(0, 1, AsuUtils.QuadOut(v));
            auraLength = MathHelper.Lerp(0, 1, AsuUtils.QuintOut(v));
            if (time > prep)
            {
                odr.Add(Projectile.rotation - (0.028f * dir));
                tip.Add(Projectile.rotation.ToRotationVector2() * TextureAssets.Projectile[Type].Value.Width * Projectile.scale * 1.2f);
                blade.Add(Projectile.rotation.ToRotationVector2() * TextureAssets.Projectile[Type].Value.Width * Projectile.scale * 0.75f);
            }
            if (odr.Count > 80)
            {
                odr.RemoveAt(0);
                tip.RemoveAt(0);
                blade.RemoveAt(0);
            }
            if (time < duration)
            {
                Projectile.timeLeft = 2;
                player.itemTime = 2;
                player.itemAnimation = 2;
                player.asuw().UseCooldown = (int)(20f * speedRatio);
            }
            time++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center;
            Vector2 end = start + (Projectile.rotation).ToRotationVector2() * texture.Width * 1.3f * Projectile.scale;
            float collisionPoint = 0f;
            float collisionWidth = 100;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = isMalikeths ? ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/MalikethBlackBlade").Value : TextureAssets.Projectile[Type].Value;
            Vector2 origin = dir == -1 ? new Vector2(texture.Width - 8, 6) : new Vector2(texture.Width - 8, texture.Height - 6);
            Texture2D glow = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/BlackBladeGlow").Value;
            Vector2 originGlow = dir == -1 ? new Vector2(glow.Width - 15, 13) : new Vector2(glow.Width - 15, glow.Height - 13);
            float rotOffset = MathHelper.ToRadians(dir == -1 ? -135 : 135);
            SpriteEffects effects = dir == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/Trails/trail69", AssetRequestMode.ImmediateLoad).Value;
            Texture2D trail2 = ModContent.Request<Texture2D>("asuw/Assets/Noise/StreakBright", AssetRequestMode.ImmediateLoad).Value;
            List<ColoredVertex> ve = new List<ColoredVertex>();
            for (int i = 0; i < odr.Count; i++)
            {
                Color b = new Color(252, 255, 199);
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + ((odr[i]).ToRotationVector2() * (texture.Width * 1.3f * Projectile.scale)),
                      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                      b));
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition,
                      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                      b));
            }

            Main.spriteBatch.EnterShaderRegion();

            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                if (isMalikeths)
                {
                    ShaderFunctions.vertexTrail(Main.spriteBatch, Color.Black, Color.Black, Projectile.Opacity, 2);
                    gd.Textures[0] = trail2;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
                else
                {
                    ShaderFunctions.vertexColored(Main.spriteBatch, Color.Black, Color.Black, Projectile.Opacity);
                    gd.Textures[0] = trail;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }

            List<Vector2> posT = new List<Vector2>();
            List<Vector2> posB = new List<Vector2>();
            for (int i = 0; i < tip.Count; i++)
            {
                posT.Add(Projectile.Center + tip[i]);
                posB.Add(Projectile.Center + blade[i]);
            }
            if (isMalikeths)
            {
                if (Projectile.Opacity > 0.6f)
                {
                    GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Trails/ScarletDevilStreak");
                    PrimitiveRenderer.RenderTrail(posB, new PrimitiveSettings(TrailWidthB, TrailColorB2, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 120);

                    GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Streak1");
                    PrimitiveRenderer.RenderTrail(posT, new PrimitiveSettings(TrailWidthB3, TrailColorB, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 120);

                    GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Trails/SylvestaffStreak");
                    PrimitiveRenderer.RenderTrail(posT, new PrimitiveSettings(TrailWidthB2, TrailColorB3, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 120);

                }
                Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
                Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, default, Color.Black * 0.7f * Projectile.Opacity, Projectile.rotation + rotOffset, originGlow, Projectile.scale, effects, 0);
                Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 12, default, Color.Red * Projectile.Opacity, Projectile.rotation + rotOffset, originGlow, Projectile.scale * 0.8f, effects, 0);
                Main.spriteBatch.ExitShaderRegion();
            }

      
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotOffset, origin, Projectile.scale, effects, 0);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        public Color TrailColorB(float completionRatio, Vector2 vertex)
        {
            return Color.Black * Projectile.Opacity * completionRatio;
        }
        public float TrailWidthB(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, TextureAssets.Projectile[Type].Value.Height * 0.85f * Projectile.scale, AsuUtils.QuadOut(completionRatio));
        }
        public Color TrailColorB2(float completionRatio, Vector2 vertex)
        {
            Color result = Color.Lerp(Color.Maroon, Color.Red, completionRatio);
            return Color.Lerp(result, Color.White, 0.15f) * Projectile.Opacity * completionRatio;
        }

        public Color TrailColorB3(float completionRatio, Vector2 vertex)
        {
            Color result = Color.Lerp(Color.Maroon, Color.Firebrick, completionRatio);
            return result * Projectile.Opacity * completionRatio;
        }
        public float TrailWidthB2(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, TextureAssets.Projectile[Type].Value.Height / 2f * Projectile.scale, AsuUtils.QuadOut(completionRatio));
        }
        public float TrailWidthB3(float completionRatio, Vector2 vertex)
        {
            float t = completionRatio * 2f;
            float v = t <= 1f ? t : 2f - t;
            float v2 = AsuUtils.QuadOut(v);
            return MathHelper.Lerp(0, TextureAssets.Projectile[Type].Value.Height / 2f * Projectile.scale, v2);
        }

    }
    public class BlackBladeSkill : ModProjectile
    {
        public override string Texture => "asuw/Content/Items/Weapons/Melee/BlackBlade";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 30;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 10;
        }
        Player player => Projectile.GetOwner();
        bool isMalikeths => Projectile.ai[0] == 1;
        ref float mode => ref Projectile.ai[1];
        ref float time => ref Projectile.ai[2];

        int duration = 0;
        float spearScale = 1;
        //trail parameter
        List<Vector2> tip = new List<Vector2>();
        List<Vector2> blade = new List<Vector2>();

        List<NPC> npcsHit = new List<NPC>();
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = isMalikeths ? 1.8f : 1.5f;
            duration = (player.itemTimeMax / 2) * Projectile.MaxUpdates;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians((-140) * Projectile.direction);
            player.AddCD(CoolDownID.BlackBladeCD, 400);
        }
        public override void AI()
        {
            if (!isMalikeths)
            {
                float t = time / (int)(duration / 2f);
                float v = t <= 1f ? t : 2f - t;
                float lerp = AsuUtils.QuintOut(time / duration);
                if (time == 1)
                    player.velocity = Projectile.velocity * 30;
                if (time > (int)(duration * 0.5f))
                    player.velocity *= 0.985f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians((-170 + 320 * lerp) * Projectile.direction);
                Projectile.Opacity = Utils.GetLerpValue(1f, 0, AsuUtils.QuadIn(time / duration), true);
                if (time > 1)
                {
                    tip.Add(Projectile.rotation.ToRotationVector2() * TextureAssets.Projectile[Type].Value.Width * Projectile.scale * 0.96f);
                    blade.Add(Projectile.rotation.ToRotationVector2() * TextureAssets.Projectile[Type].Value.Width * Projectile.scale * 0.75f);
                }
                if (tip.Count > 60)
                {
                    tip.RemoveAt(0);
                    blade.RemoveAt(0);
                }

                
            }
            else
            {
                float t = time / (int)(duration / 2f);
                float v = t <= 1f ? t : 2f - t;
                float lerp = AsuUtils.QuintOut(time / duration);
                if (time == 1)
                    player.velocity = Projectile.velocity * 50;
                if (time > (int)(duration * 0.5f))
                    player.velocity *= 0.978f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.scale = MathHelper.Lerp(1.8f, 1.5f, AsuUtils.QuadIn(time / duration));
                spearScale *= 0.978f;
                Projectile.Opacity = Utils.GetLerpValue(1f, 0, AsuUtils.QuadIn(time / duration), true);

                Dust dust = Dust.NewDustPerfect(player.Center + Utils.NextVector2Circular(Main.rand, player.width, player.width), DustID.CrimsonTorch, player.velocity * Main.rand.NextFloat(-0.5f, -0.1f), Scale : 2);
                dust.noGravity = true;

                if (time < duration * 0.6f && time % Projectile.MaxUpdates * 2 == 0)
                    for (int i = -1; i < 2; i += 2)
                    {
                        Vector2 pos = Projectile.Center + player.velocity + Projectile.rotation.ToRotationVector2() * TextureAssets.Projectile[Type].Value.Size() * 0.8f * Projectile.scale + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i) * 8;
                        Vector2 vel3 = Projectile.velocity.RotatedBy(-0.12f * i) * -12f;
                        // particle
                    }
            }
            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.direction);
            player.SetHandRotFront(Projectile.rotation);
            Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
            if (time < duration)
            {
                Projectile.timeLeft = 2;
                player.itemTime = 2;
                player.itemAnimation = 2;
                player.asuw().UseCooldown = (int)(isMalikeths ? 35f : 25f);
            }
            time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center;
            Vector2 end = start + (Projectile.rotation).ToRotationVector2() * texture.Width * (isMalikeths ? 1.5f : 1.3f) * Projectile.scale;
            float collisionPoint = 0f;
            float collisionWidth = isMalikeths ? 170 : 100;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!npcsHit.Contains(target)) npcsHit.Add(target);
        }
        public override void OnKill(int timeLeft)
        {
            if (npcsHit.Count > 0)
            {
                NPC idealNPC = npcsHit.Where(npc => npc != null && npc.active)
                    .OrderByDescending(npc => npc.life).FirstOrDefault();
    

                if(idealNPC != null)
                 Projectile.NewProjectile(Projectile.InheritSource(Projectile), idealNPC.Center, Vector2.Zero, ModContent.ProjectileType<BlackBladeSkillRed>(), (int)((float)Projectile.damage * 1.5f), 3f, player.whoAmI, idealNPC.whoAmI, isMalikeths ? 1 : 0);

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = isMalikeths ? ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/MalikethBlackBlade").Value : TextureAssets.Projectile[Type].Value;
            Vector2 origin = Projectile.direction == 1 ? new Vector2(texture.Width - 8, 6) : new Vector2(texture.Width - 8, texture.Height - 6);
            float rotOffset = MathHelper.ToRadians(Projectile.direction == 1 ? -135 : 135);
            SpriteEffects effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D glow = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/BlackBladeGlow").Value;
            Texture2D Outline = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/BlackBladeOutline").Value;
            Vector2 originGlow = Projectile.direction == 1 ? new Vector2(glow.Width - 15, 13) : new Vector2(glow.Width - 15, glow.Height - 13);
            Vector2 originOutline = Projectile.direction == 1 ? new Vector2(Outline.Width - 10, 8) : new Vector2(Outline.Width - 10, Outline.Height - 8);
            Texture2D spear = ModContent.Request<Texture2D>("asuw/Assets/SpearGlow", AssetRequestMode.AsyncLoad).Value;
            Vector2 spearOrigin = new Vector2(0, spear.Height / 2f);

            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/GraySmear", AssetRequestMode.ImmediateLoad).Value;
            Texture2D trail2 = ModContent.Request<Texture2D>("asuw/Assets/TopLine", AssetRequestMode.ImmediateLoad).Value;

            List<Vector2> posT = new List<Vector2>();
            List<Vector2> posB = new List<Vector2>();
            for (int i = 0; i < tip.Count; i++)
            {
                posT.Add(Projectile.Center + tip[i]);
                posB.Add(Projectile.Center + blade[i]);
            }

            Main.spriteBatch.EnterShaderRegion();

            if (time < (float)duration * 0.8f && !isMalikeths)
            {
                GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/StreakGoop");
                //PrimitiveRenderer.RenderTrail(posB, new PrimitiveSettings(TrailWidthB, TrailColorB, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 240);
                GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/StreakSolid");
                PrimitiveRenderer.RenderTrail(posT, new PrimitiveSettings(TrailWidth2, TrailColorB, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 120);

                GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Trails/SylvestaffStreak");
                PrimitiveRenderer.RenderTrail(posT, new PrimitiveSettings(TrailWidth, TrailColor, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 120);
            }


            
            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, default, Color.Black * 0.7f * Projectile.Opacity, Projectile.rotation + rotOffset, originGlow, Projectile.scale, effects, 0);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 12, default, Color.Red * Projectile.Opacity, Projectile.rotation + rotOffset, originGlow, Projectile.scale * 0.8f, effects, 0);
            Main.spriteBatch.ExitShaderRegion();
            
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotOffset, origin, Projectile.scale, effects, 0);

            Main.spriteBatch.ExitShaderRegion();

            if(isMalikeths)
            {
                //Vector2 spearPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * texture.Size() * 0.7f * Projectile.scale;
                //Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
                //Main.spriteBatch.Draw(spear, spearPos - Main.screenPosition, default, Color.Black * Projectile.Opacity * 0.7f, Projectile.rotation, spearOrigin, Projectile.scale * 1.2f * spearScale * new Vector2(1.5f, 1), SpriteEffects.None, 0);
                //Main.spriteBatch.Draw(spear, spearPos - Main.screenPosition, default, Color.Red * Projectile.Opacity, Projectile.rotation, spearOrigin, Projectile.scale * spearScale * new Vector2(1.5f,1), SpriteEffects.None, 0);
                //Main.spriteBatch.ExitShaderRegion();
            }
            return false;

        }

        public Color TrailColorB(float completionRatio, Vector2 vertex)
        {
            return Color.Black * Projectile.Opacity * completionRatio;
        }
        public float TrailWidthB(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, TextureAssets.Projectile[Type].Value.Height * 1.2f * Projectile.scale, AsuUtils.ExpoOut(completionRatio));
        }
        public Color TrailColor(float completionRatio, Vector2 vertex)
        {
            Color result = Color.Lerp(Color.Maroon, Color.Red, completionRatio);
            return result * Projectile.Opacity * completionRatio;
        }
        public float TrailWidth(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, TextureAssets.Projectile[Type].Value.Height / 1.2f * Projectile.scale, AsuUtils.QuadOut(completionRatio));
        }
        public float TrailWidth2(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, TextureAssets.Projectile[Type].Value.Height / 2f * Projectile.scale, AsuUtils.QuadOut(completionRatio));
        }


    }
    public class BlackBladeSkillRed : ModProjectile
    {
        public override string Texture => "asuw/Content/Items/Weapons/Melee/BlackBladeRed";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 30;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 2;
        }
        Player player => Projectile.GetOwner();
        public bool invalidTarget => (Projectile.ai[0] < 0f || Projectile.ai[0] > 199f);
        private NPC Target => Main.npc[(int)Projectile.ai[0]];
        List<Vector2> targetPrevCenter = new List<Vector2>();
        Vector2 spawnPos;
        bool isMalikeths => Projectile.ai[1] == 1;
        ref float time => ref Projectile.ai[2];
        ref float mode => ref Projectile.localAI[0];

        int duration = 0;
        List<Vector2> points = new List<Vector2>();
        public override void OnSpawn(IEntitySource source)
        {
            duration = player.itemTimeMax * Projectile.MaxUpdates;
            Projectile.Opacity = 0;
            spawnPos = Projectile.Center;
            for (int i = 0; i < 4; i++)
            {
                points.Add(Vector2.Zero);
            }
        }
        public override bool? CanDamage() => mode == 1;

        public override void AI()
        {
            if (mode == 0)
            {
                float t = time / (int)(duration / 2f);
                float v = t <= 1f ? t : 2f - t;
                Projectile.rotation = MathHelper.PiOver2;
                float floatAmnt = (float)TextureAssets.Projectile[Type].Value.Height;
                Vector2 pos;
                if (!invalidTarget)
                {
                    pos = Target.Center;
                    targetPrevCenter.Add(Target.Center);
                }
                else if (targetPrevCenter.Count > 0)
                    pos = targetPrevCenter[targetPrevCenter.Count - 1];
                else
                    pos = spawnPos;

                Projectile.Center = pos - Vector2.UnitY * (floatAmnt * AsuUtils.QuadOut(v));
                Projectile.Opacity = MathHelper.Lerp(0, 1, AsuUtils.QuintOut(time / duration));
                if (time < duration)
                {
                    Projectile.timeLeft = 2;
                    time++;
                }
                else
                {
                    Projectile.timeLeft = isMalikeths ? 60 : 20;
                    time = 0;
                    points.Clear();
                    mode = 1;
                }
                float randHeight = Main.rand.NextFloat(-150,-1);
                int type = DustID.CrimsonTorch;
                float scale = type == DustID.CrimsonTorch ? 1 : 0.7f;
                if (time > 0)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Vector2.UnitY * randHeight + Utils.NextVector2Circular(Main.rand, 30, 30), type, Vector2.UnitY * -3 * (MathF.Abs(randHeight) / 75f), newColor: Color.Red, Scale: scale);
                    dust.noGravity = true;
                }
            }
            else
            {
                Projectile.Resize(500, 500);
                Projectile.Opacity = 0;
                if (time == 1)
                { }//particle
                Vector2 center = !invalidTarget ? Target.Center + (Target.velocity * 0.6f) : Projectile.Center;
                if (isMalikeths)
                {
                    if (time % 3 == 0)
                    {
                        Vector2 offset = (Vector2.UnitX * 250).RotatedBy(AsuUtils.randomRot());
                        Vector2 pos = center + offset;
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), pos, (center - pos).normalize().RotatedByRandom(0.7f) * 24, ModContent.ProjectileType<BlackBladeSlashes>(), (int)((float)Projectile.damage * 0.3f), 3f, player.whoAmI);
                    }
                    float strength = 1 - AsuUtils.QuadIn(time / 60f);
                    ShaderFunctions.ZoomBlur(center, strength, 1f, 0.15f);
                }
                time++;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (isMalikeths) target.AddBuff(ModContent.BuffType<MalikethsDestinedDeath>(), 300);
            else target.AddBuff(ModContent.BuffType<DestinedDeath>(), 300);
        }
        public override void OnKill(int timeLeft)
        {
            ShaderFunctions.DisableScreenShader("asuw:ZoomBlur");
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = isMalikeths ? ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/MalikethBlackBladeRed").Value : TextureAssets.Projectile[Type].Value;
            Texture2D glow = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/BlackBladeGlow", AssetRequestMode.ImmediateLoad).Value;
            Vector2 originGlow = new Vector2(8, glow.Height - 13);
            Vector2 origin = new Vector2(0, texture.Height - 7);
            float rotOffset = MathHelper.ToRadians(-135);
            SpriteEffects effects = SpriteEffects.None;


            for (int i = 0; i < points.Count; i++)
            {
                points[i] = Projectile.Center + (Vector2.UnitY * -(((float)TextureAssets.Projectile[Type].Value.Height) * (points.Count - 1 - i)));
            }

            GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Streak1");
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(TrailWidth, TrailColor, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 40);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity, Projectile.rotation + rotOffset, origin, Projectile.scale, effects, 0);

            if (isMalikeths)
            {
                GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Trails/ScarletDevilStreak");
                PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(TrailWidth, TrailColor2, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 40);
            }
            return false;
        }

        public Color TrailColor(float completionRatio, Vector2 vertex)
        {
            Color result = Color.Lerp(Color.Black, Color.Red, completionRatio);
            return result * Projectile.Opacity * AsuUtils.QuadIn(completionRatio);
        }
        public Color TrailColor2(float completionRatio, Vector2 vertex)
        {
            return Color.Black * Projectile.Opacity * AsuUtils.QuadIn(completionRatio);
        }
        public float TrailWidth(float completionRatio, Vector2 vertex)
        {
            float t = completionRatio * 2f;
            float v = t <= 1f ? t : 2f - t;
            float v2 = AsuUtils.QuartOut(v);
            return MathHelper.Lerp(0, (TextureAssets.Projectile[Type].Value.Height) * Projectile.scale, v2);
        }
    }

    public class BlackBladeSlashes : ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 30;
            Projectile.localNPCHitCooldown = -1;
            Projectile.height = Projectile.width = 150;
            Projectile.MaxUpdates = 5;
        }
        Player player => Projectile.GetOwner();
        ref float time => ref Projectile.ai[2];

        float rotamnt = 0;
        float length = 100;
        int dir = 1;

        List<Vector2> points = new List<Vector2>();

        public override void OnSpawn(IEntitySource source)
        {
            length = Main.rand.NextFloat(90,170);
            rotamnt = MathHelper.ToRadians(Main.rand.NextFloat(2, 4));
            dir = Main.rand.NextBool() ? 1 : -1;
        }
        public override void AI()
        {
            float t = 1 - (time / length);
            Projectile.velocity = Projectile.velocity.RotatedBy(rotamnt * t * dir);
            Projectile.velocity *= (0.97f);
            Projectile.Opacity = AsuUtils.CubicOut(t) * 1.2f;
            points.Add(Projectile.Center);
            if (points.Count > 60)
                points.RemoveAt(0);
            if(time % 6 == 0 && t > 0.14f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SharpSparkDust>(), -Projectile.velocity.RotatedByRandom(0.8f) * 0.4f, 0, Color.Red, Main.rand.NextFloat(0.4f, 0.9f));
                dust.noGravity = true;
                if (Main.rand.NextBool())
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.Firework_Yellow, -Projectile.velocity * 0.5f, Scale: Main.rand.NextFloat(0.3f, 0.6f));
                    dust2.noGravity = true;
                }
            }
            if (time < length)
                Projectile.timeLeft = 2;
            time++;
        }
        public override bool? CanDamage() => Projectile.velocity.Length() > 2f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MalikethsDestinedDeath>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["asuw:SlashForward"].SetShaderTexture("asuw/Assets/White");
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(TrailWidthB, TrailColorB, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:SlashForward"]), 40);
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(TrailWidthR, TrailColorR, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:SlashForward"]), 40);
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(TrailWidthG, TrailColorG, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:SlashForward"]), 40);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        //Red
        public Color TrailColorR(float completionRatio, Vector2 vertex)
        {
            Color result = Color.Lerp(Color.Maroon, Color.Firebrick, completionRatio);
            return result * Projectile.Opacity * completionRatio;
        }
        public float TrailWidthR(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, 20 * Projectile.scale * (1 - AsuUtils.ExpoIn(completionRatio)), AsuUtils.QuadOut(completionRatio));
        }
        //Black
        public Color TrailColorB(float completionRatio, Vector2 vertex)
        {
            return Color.Black * Projectile.Opacity * completionRatio;
        }
        public float TrailWidthB(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, 45 * Projectile.scale * (1 - AsuUtils.QuartIn(completionRatio)), AsuUtils.QuadOut(completionRatio));
        }
        //Gold
        public Color TrailColorG(float completionRatio, Vector2 vertex)
        {
            return Color.LightGoldenrodYellow * Projectile.Opacity * AsuUtils.ExpoIn((completionRatio));
        }
        public float TrailWidthG(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, 3f * Projectile.scale * (1 - AsuUtils.ExpoIn(completionRatio)), AsuUtils.QuadOut(completionRatio));
        }

    }

    public class BlackBladeSpell : ModProjectile
    {
        public override string Texture => "asuw/Content/Items/Weapons/Melee/MalikethBlackBladeRed";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 30;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 10;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(130 * Projectile.direction);
        }
        Player player => Projectile.GetOwner();
        ref float attDir => ref Projectile.ai[0];
        bool isMalikeths => Projectile.ai[1] == 1;
        ref float time => ref Projectile.ai[2];

        int duration = 0;

        //trail parameter
        List<float> odr1 = new List<float>();
        List<float> odr2 = new List<float>();
        public override void OnSpawn(IEntitySource source)
        {
            duration = (int)((float)player.itemTimeMax * 1.5f * (float)Projectile.MaxUpdates);
        }
        public override void AI()
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            float lerp = time / duration;
            float t = time / (int)(duration * 0.5f);
            float v = t <= 1f ? t : 2f - t;

            float rotTo = Projectile.velocity.ToRotation() + MathHelper.ToRadians(130 * Projectile.direction) + MathHelper.ToRadians(400 * Projectile.direction * AsuUtils.ExpoInOut(lerp));
            float dis = Projectile.rotation.AngleBetween(rotTo);
            float maxChange = (MathF.Abs(dis) / (MathHelper.PiOver2 * 1.5f)) / Projectile.MaxUpdates;

            if (lerp <= 0.4f)
            {
                Projectile.velocity = player.asuw().mouseNormalFromPlayer;
                if (time < 2)
                    Projectile.rotation = rotTo;
                else
                    Projectile.rotation = Projectile.rotation.RotTowards(rotTo, maxChange);
            }
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(130 * Projectile.direction) + MathHelper.ToRadians(400 * Projectile.direction * AsuUtils.ExpoInOut(lerp));

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.direction);
            player.SetHandRotFront(Projectile.rotation);
            Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
            Projectile.scale = 1.6f;
            Projectile.Opacity = MathHelper.Lerp(0, 1, AsuUtils.QuadOut(v));
            if (lerp > 0.4f)
            {
                odr1.Add(Projectile.rotation + (0.028f * Projectile.direction));
                odr2.Add(Projectile.rotation + (0.028f * Projectile.direction));

                float t2 = ((lerp - 0.4f) / 0.6f) * 2.5f;
                Vector2 end = Projectile.Center + player.velocity + (Projectile.rotation).ToRotationVector2() * texture.Width * 1.12f * Projectile.scale;
                Vector2 vel = Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2 * Projectile.direction);
                if (lerp < 0.6f)
                {
                    //GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(end, vel.RotatedByRandom(0.3f) * Main.rand.NextFloat(5, 8), Color.Lerp(Color.Maroon, Color.Black, Main.rand.NextBool() ? 0.75f : 0.45f), 20, Main.rand.NextFloat(3f, 4f) * t2, 0.7f, Main.rand.NextFloat(-0.05f, 0.05f)), manualDrawLayerOverride : GeneralDrawLayer.BeforeProjectiles);
                    int type = Main.rand.NextBool() ? ModContent.DustType<SharpSparkDust>() : DustID.Firework_Yellow;
                    Dust dust = Dust.NewDustPerfect(end, type, vel.RotatedByRandom(0.5f) * Main.rand.NextFloat(7, 9), Scale: Main.rand.NextFloat(0.6f, 0.9f) * (type == DustID.Firework_Yellow ? 1 : 2), newColor : type == DustID.Firework_Yellow ? default : Color.Khaki);
                    dust.noGravity = true;
                }
            }
            if (odr1.Count > 30)
                odr1.RemoveAt(0);
            if (odr2.Count > 60)
                odr2.RemoveAt(0);

            if (time == (int)((float)duration * 0.46f))
            {
                float rot = AsuUtils.randomRot();
                float rot2 = AsuUtils.randomRot();
                Vector2 end = Projectile.Center + player.velocity + Projectile.velocity.normalize() * texture.Width * Projectile.scale;
                for (int i = 0; i < 4; i++)
                {
                    //boom
                }
                for (int j = 0; j < 12; j++)
                {
                   //boomsmoke

                    Dust dust = Dust.NewDustPerfect(end, ModContent.DustType<SharpSparkDust>(), Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(8, 10), Scale: Main.rand.NextFloat(0.6f, 0.9f) * 2, newColor: Color.Khaki);
                    dust.noGravity = true;
                }
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), player.Center + Projectile.velocity.normalize() * 30, Projectile.velocity * 30, ModContent.ProjectileType<BlackbladeSpellProj>(), (int)((float)Projectile.damage * 1.5f), 3f, player.whoAmI);
            }

            if (time < duration)
            {
                Projectile.timeLeft = 2;
                player.itemTime = 2;
                player.itemAnimation = 2;
                player.asuw().UseCooldown = (int)(20f);
            }
           
            time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = Projectile.direction == 1 ? new Vector2(texture.Width - 8, 6) : new Vector2(texture.Width - 8, texture.Height - 6);
            float rotOffset = MathHelper.ToRadians(Projectile.direction == 1 ? -135 : 135);
            SpriteEffects effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D glow = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/BlackBladeGlow").Value;
            Vector2 originGlow = Projectile.direction == 1 ? new Vector2(glow.Width - 15, 13) : new Vector2(glow.Width - 15, glow.Height - 13);

            List<ColoredVertex> ve = new List<ColoredVertex>();
            List<ColoredVertex> ve2 = new List<ColoredVertex>();
            List<ColoredVertex> veT = new List<ColoredVertex>();
            for (int i = 0; i < odr1.Count; i++)
            {
                Color b = new Color(252, 255, 199);
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + ((odr1[i]).ToRotationVector2() * (texture.Width * 1.3f  * Projectile.scale)),
                      new Vector3((i) / ((float)odr1.Count - 1), 0, 1),
                      b));
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition,
                      new Vector3((i) / ((float)odr1.Count - 1), 1, 1),
                      b));
            }
            for (int i = 0; i < odr2.Count; i++)
            {
                Color b = new Color(252, 255, 199);
                ve2.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + ((odr2[i]).ToRotationVector2() * (texture.Width * 1.3f * Projectile.scale)),
                      new Vector3((i) / ((float)odr2.Count - 1), 0, 1),
                      b));
                ve2.Add(new ColoredVertex(Projectile.Center - Main.screenPosition,
                      new Vector3((i) / ((float)odr2.Count - 1), 1, 1),
                      b));

                veT.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + ((odr2[i]).ToRotationVector2() * (texture.Width * 1.65f * Projectile.scale)),
                      new Vector3((i) / ((float)odr2.Count - 1), 0, 1),
                      b));
                veT.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + ((odr2[i]).ToRotationVector2() * (texture.Width * 0.85f * Projectile.scale)),
                      new Vector3((i) / ((float)odr2.Count - 1), 1, 1),
                      b));
            }
            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/Noise/Streak", AssetRequestMode.ImmediateLoad).Value;
            Texture2D trail2 = ModContent.Request<Texture2D>("asuw/Assets/SwordSlashTexture", AssetRequestMode.ImmediateLoad).Value;
            Texture2D trail3 = ModContent.Request<Texture2D>("asuw/Assets/GlowTrail", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.EnterShaderRegion();
            if (ve2.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                ShaderFunctions.vertexTrail(Main.spriteBatch, Color.Black, Color.Black, Projectile.Opacity, 2);
                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve2.ToArray(), 0, ve2.Count - 2);
                Main.spriteBatch.ExitShaderRegion();
            }
            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                ShaderFunctions.vertexColorBloom(Main.spriteBatch, Color.Maroon, Color.Lerp(Color.Red, Color.Maroon, 0.4f), Projectile.Opacity);
                gd.Textures[0] = trail2;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                Main.spriteBatch.ExitShaderRegion();
            }
            if(veT.Count >=3)
            {
                var gd = Main.graphics.GraphicsDevice;
                ShaderFunctions.vertexColorBloom(Main.spriteBatch, Color.Goldenrod, Color.Khaki, Projectile.Opacity);
                gd.Textures[0] = trail3;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, veT.ToArray(), 0, veT.Count - 2);
                Main.spriteBatch.ExitShaderRegion();
            }

            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, default, Color.Red * 0.75f * Projectile.Opacity, Projectile.rotation + rotOffset, originGlow, Projectile.scale, effects, 0);
            Main.spriteBatch.ExitShaderRegion();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity, Projectile.rotation + rotOffset, origin, Projectile.scale, effects, 0);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
    public class BlackbladeSpellProj : ModProjectile
    {
        public override string Texture => "asuw/Assets/Blank";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 300;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 5;
            Projectile.height = Projectile.width = 250;
        }
        Player player => Projectile.GetOwner();
        ref float attCount => ref Projectile.ai[0];
        ref float transform => ref Projectile.ai[1];
        ref float time => ref Projectile.ai[2];


        public override void OnSpawn(IEntitySource source)
        {
           
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.97f;
            if (Projectile.velocity.Length() < 1) Projectile.Opacity *= 0.97f;
        }

        public static void DestinedSpellTickDMG(NPC target, AsuGlobalNPC agn)
        {
            if (agn.DestinedSpellApplicator >= 0 && agn.DestinedSpellApplicator < Main.maxPlayers && Main.myPlayer == agn.DestinedSpellApplicator)
            {
                Player applicator = Main.player[agn.DestinedSpellApplicator];
                // once every 1 sec
                if (applicator.miscCounter % 60 == 0)
                {
                    //boom effect
                    Projectile.NewProjectileDirect(target.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrikeConst>(), int.MaxValue, 0f, applicator.whoAmI, target.whoAmI, 444, ModContent.BuffType<MalikethsDestinedDeath>());
                    agn.DestinedSpellCount--;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MalikethsDestinedDeath>(), 600);
            target.asuw().DestinedSpellApplicator = player.whoAmI;
            target.asuw().DestinedSpellCount += 4;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("asuw/Assets/Particles/slash_01", AssetRequestMode.ImmediateLoad).Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, default, Color.Maroon with { A = 0 } * Projectile.Opacity, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
     
    }
}
