using asuw.Content.Dusts;
using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;


namespace asuw.Content.Items.Weapons.Melee
{
	public class Moonveil : ModItem
	{
        //TODO total mc swords rework
        public float attCount = 0;
        public float FP = 100;
        public static SoundStyle swing = new SoundStyle("asuw/Content/Sounds/MediumSwing");
        public static SoundStyle shoot = new SoundStyle("asuw/Content/Sounds/MoonveilProj");
        public static SoundStyle sheathe = new SoundStyle("asuw/Content/Sounds/Sheath");
        public static SoundStyle pUnsheate = new SoundStyle("asuw/Content/Sounds/PerfectUnsheathe");
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
		{
			Item.damage = 136;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<MoonveilBlade>();
			Item.noMelee = true;
			Item.noUseGraphic = true;
            Item.shootSpeed = 1;
           
        }

        public override bool MeleePrefix() => true;
        public override bool AltFunctionUse(Player player) => FP > 30 ? true : false;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (attCount < 4)
                attCount++;
            else attCount = 0;
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SheatedMoonveil>(), damage, knockback, player.whoAmI, attCount);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attCount);
            }

            return false;
        }


        public override void UpdateInventory(Player player)
        {
            if (FP < 100 && player.miscCounter % 5 == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<SheatedMoonveil>()] < 1) FP++;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
           
            base.ModifyWeaponDamage(player, ref damage);
        }

    }
    public class SheatedMoonveil : ModProjectile
    {
        public override string Texture => "asuw/Content/Items/Weapons/Melee/Moonveil";
        ref float time => ref Projectile.ai[0];
        ref float time2 => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 10;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 2;
        }
        Player player => Projectile.GetOwner();
        Vector2 origin = Vector2.Zero;
        int direction = 1;
        float duration = 40;
        float bonusDMG = 0;
        float perfect = 0;

        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            direction = player.direction;
            SoundEngine.PlaySound(Moonveil.sheathe, Projectile.Center);
        }
        public override void AI()
        {
            float lerp = ((float)time / duration);
            float eased = AsuUtils.QuadInOut(lerp);
            Projectile.rotation = (MathHelper.PiOver2 + (MathHelper.Pi * MathHelper.Lerp(0.4f, 0.65f, eased))) * direction;
            player.SetHandRotFront((direction == 1 ? 0f : MathF.PI) + (MathHelper.PiOver2 * direction) * MathHelper.Lerp(0f, 0.7f, eased));
            Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(direction);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            float x = MathHelper.Lerp(texture.Width * 0.5f, texture.Width * 0.2f, eased);
            float y = MathHelper.Lerp(texture.Height * 0.5f, direction != 1 ? texture.Height * 0.8f : texture.Height * 0.2f, eased);

            origin = new Vector2(x, y);

            if (player.asuw().mouseRight)
            {
                Projectile.timeLeft = 2;
                player.itemAnimation = 2;
                player.itemTime = 2;
            }
            else
                Projectile.Kill();

            perfect = (int)(duration * 0.7f) - 2;
            Vector2 OpeningPos = Projectile.Center + (-Projectile.rotation.ToRotationVector2() * 25) * direction;
            if (time < perfect)
            {
                Dust dust = Dust.NewDustPerfect(OpeningPos + player.velocity + Utils.NextVector2Circular(Main.rand,3,3), ModContent.DustType<SharpSparkDust>(), (OpeningPos - Projectile.Center).normalize().RotatedByRandom(0.6f) * 6, 0, Color.MidnightBlue, 0.8f);
                dust.noGravity = true;
                dust.rotation = dust.velocity.ToRotation();
                bonusDMG = time / perfect;
            }
            else if (time >= perfect && time < duration)
                bonusDMG = 1.5f;
            else
                bonusDMG = 0.9f;

            if (time == perfect)
            {
                SoundEngine.PlaySound(Moonveil.pUnsheate, Projectile.Center);
                Vector2 pos = Projectile.Center + player.velocity;
                for (int i = 0; i < 9; i++)
                {
                    Dust dust = Dust.NewDustPerfect(pos, ModContent.DustType<SharpSparkDust>(), Projectile.velocity.RotatedBy(40 * i) * 5, 0, Color.MidnightBlue, 1.3f);
                    dust.noGravity = true;
                    dust.rotation = dust.velocity.ToRotation();
                }
                Dust dust2 = Dust.NewDustPerfect(pos, ModContent.DustType<StarBurst>(), Vector2.Zero, 0, Color.MidnightBlue, 0.6f);
                float rot = AsuUtils.randomRot();
              
            }

            if (time < duration)
                time++; 
           
        }
        public override void OnKill(int timeLeft)
        {
            if (time > duration / 2.5f)
            {
                if (player.HeldItem != null && player.HeldItem.ModItem is Moonveil moon) moon.FP -= 30;
                player.ChangeDir(player.mouseWorld().X > player.Center.X ? 1 : -1);
                float dirToMouse = Projectile.AngleTo(player.mouseWorld());
                Vector2 vel = dirToMouse.ToRotationVector2();
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), player.Center, vel, ModContent.ProjectileType<MoonveilBlade>(), (int)((float)Projectile.damage * (1f + bonusDMG)), 3f, player.whoAmI, 4, 1);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D textureScabbard = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/MoonveilSheatheRe", AssetRequestMode.ImmediateLoad).Value;
            SpriteEffects effects = direction != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Vector2 originS = direction != 1 ? new Vector2(textureScabbard.Width * 0.2f, textureScabbard.Height * 0.8f) : new Vector2(textureScabbard.Width * 0.2f, textureScabbard.Height * 0.2f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + (MathHelper.PiOver2 * (direction != 1 ? -1.3f : -0.7f)), origin, Projectile.scale, effects, 0);
            Main.spriteBatch.Draw(textureScabbard, Projectile.Center - Main.screenPosition , default, lightColor * Projectile.Opacity, Projectile.rotation + (MathHelper.PiOver2 * (direction != 1 ? -1.3f : -0.7f)), originS, Projectile.scale, effects, 0);
            return false;
        }
    }

    public class MoonveilBlade : ModProjectile
    {
        public override string Texture => "asuw/Content/Items/Weapons/Melee/Moonveil";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 30;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 15;
        }
        Player player => Projectile.GetOwner();
        ref float attCount => ref Projectile.ai[0];
        ref float transientMoonlight => ref Projectile.ai[1];
        ref float time => ref Projectile.ai[2];

        float duration = 0;
        int pDirection = 1;
        int direction = 1;
        float rotOffset = 0;

        //trail parameter
        List<float> odr = new List<float>();
        float trailOP = 0;
        public override void OnSpawn(IEntitySource source)
        {
            duration = player.itemTimeMax * Projectile.MaxUpdates;
            pDirection = player.direction;
            direction = player.direction * ((attCount == 1 || attCount == 2) ? -1 : 1);
            rotOffset = MathHelper.ToRadians(direction * 45);
        }
        public override void AI()
        {
            if(time < duration)
            {
                Projectile.timeLeft = 2;
                player.itemAnimation = 2;
                player.itemTime = 2;
                
            }

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(pDirection);
            float val = time / (int)(duration);
            float lerp = AsuUtils.CircInOut(val);
            float v = AsuUtils.PingPong(time, duration);

            if (time == (int)(duration * 0.3f))
            {
                if(transientMoonlight != 1)SoundEngine.PlaySound(Moonveil.swing with { Volume = 0.35f, Pitch = 0.7f, PitchVariance = 0.4f }, Projectile.Center);
                else SoundEngine.PlaySound(Moonveil.shoot with { MaxInstances = 2, PitchVariance = 0.2f, Volume = 0.8f }, Projectile.Center);
            }
            if (transientMoonlight == 1 && time == (int)(duration * 0.4f))
            {
                Vector2 pos = player.Center + Projectile.velocity * 140 + player.velocity;
                Vector2 squish = new Vector2(0.3f, 1.3f);
                float ranRot = Main.rand.NextFloat(-0.07f, 0.07f);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), player.Center + Projectile.velocity.normalize() * 40, Projectile.velocity * 40, ModContent.ProjectileType<MoonveilProj>(), (int)((float)Projectile.damage * 1.5f), 3f, player.whoAmI);
                for (int i = 0; i < 2; i++)
                {
                   //particles
                }
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(pos, ModContent.DustType<ShineStarDust>(), (Projectile.velocity * Main.rand.NextFloat(8f, 14f)).RotatedByRandom(1.2f), 0, Color.RoyalBlue, Main.rand.NextFloat(0.4f, 0.85f));
                    dust.noGravity = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(-160 * direction) + MathHelper.ToRadians((280 * direction) * lerp);
            player.SetHandRotFront(Projectile.rotation);
            Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
            Projectile.scale = (attCount != 4 ? 1f : MathHelper.Lerp(1f, 1.4f, AsuUtils.SineIn(v))) * player.GetMeleeScale();
            Projectile.Opacity = MathHelper.Lerp(0f, 1f, AsuUtils.QuadOut(v));
            trailOP = MathHelper.Lerp(0, 1, AsuUtils.QuadOut(v));

            if (time > (int)(duration * 0.2f))
            {
                odr.Add(Projectile.rotation);
            }

            if(odr.Count > 40)
            {
                odr.RemoveAt(0);
            }

            time++;
        }
        public override bool? CanDamage() => Projectile.Opacity >= 0.9f;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center;
            Vector2 end = start + (Projectile.rotation + rotOffset).ToRotationVector2() * texture.Width * 1.2f * Projectile.scale;
            float collisionPoint = 0f;
            float collisionWidth = 90;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = direction == 1 ? new Vector2(texture.Width * 0.1f, texture.Height * 0.9f) : new Vector2(texture.Width * 0.1f, texture.Height * 0.1f);
            SpriteEffects effects = direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/MotionTrail2", AssetRequestMode.ImmediateLoad).Value;
            float trailOffset = MathHelper.ToRadians(-45 * direction);

            List<ColoredVertex> ve = new List<ColoredVertex>();
            for (int i = 0; i < odr.Count; i++)
            {
                Color b = new Color(252, 255, 199);
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + ((odr[i] + rotOffset + trailOffset).ToRotationVector2() * (texture.Width * 1.27f * Projectile.scale)),
                      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                      b));
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + ((odr[i] + rotOffset + trailOffset).ToRotationVector2() * (texture.Width * 0.2f * Projectile.scale)),
                      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                      b));
            }

            Color trailCol = transientMoonlight == 1 ? Color.RoyalBlue : Color.White;
            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                ShaderFunctions.vertexColorBloom(Main.spriteBatch, trailCol, trailCol, trailOP * (attCount != 4 ? 0.5f : 1f));
                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                Main.spriteBatch.ExitShaderRegion();
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotOffset, origin, Projectile.scale, effects, 0);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }

    public class MoonveilProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 70;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 4;
            Projectile.Opacity = 0;
        }
        Player player => Projectile.GetOwner();
        ref float attCount => ref Projectile.ai[0];
        ref float transform => ref Projectile.ai[1];
        ref float time => ref Projectile.ai[2];

        float duration = 70; //lifetime
        float halfWidth = 0;
        Vector2 squish = Vector2.One;

        //fade lines parameter
        List<Vector2> flp = new List<Vector2>(); //pos
        List<float> fls = new List<float>(); //scale
        List<int> flt = new List<int>(); //type
        int totalLines = 8;

        List<Vector2> odp = new List<Vector2>(); //pos

        public override void OnSpawn(IEntitySource source)
        {
            halfWidth = (float)TextureAssets.Projectile[Type].Value.Width / 2f;
            for (int i = 0; i < totalLines; i++)
            {
                Vector2 pos = (Projectile.velocity.normalize() * -halfWidth) + (Projectile.velocity.normalize() * (halfWidth * Main.rand.NextFloat(0.8f, 1f))).RotatedByRandom(i < totalLines / 2 ? float.Pi : float.Pi * 0.6f);
                flp.Add(pos);
                fls.Add(Main.rand.NextFloat(0.6f, 1f));
                flt.Add(Main.rand.Next(1, 3));
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.94f;
            if (Projectile.timeLeft == duration - 1) Projectile.Opacity = 1;
            if(Projectile.timeLeft < (duration / 2)) Projectile.Opacity *= 0.95f;

            float lerp = (float)Projectile.timeLeft / duration;

            squish = new Vector2(1f + (1f * AsuUtils.CubicIn(lerp)), 1f);

            float velValue = MathHelper.Lerp(1f, 0.2f, lerp);
            if (Projectile.timeLeft > (duration * 0.58f))
                for (int i = 0; i < 2; i++)
                {
                    Vector2 posEdge = Projectile.Center + (Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2) * ((i == 0 ? halfWidth * 0.8f : -halfWidth * 0.8f) * Main.rand.NextFloat(0.7f, 1f)));
                    Dust dustS = Dust.NewDustPerfect(posEdge, ModContent.DustType<SharpSparkDust>(), (Projectile.velocity * (velValue * Main.rand.NextFloat(0.8f, 1f)) * -1).RotatedByRandom(0.3f), 0, Color.MidnightBlue, Main.rand.NextFloat(1f, 1.6f));
                    dustS.noGravity = true;
                }
            if (Main.rand.NextBool(Projectile.timeLeft < (duration / 2) ? 4 : 2))
            {
                Vector2 pos = Projectile.Center + (Projectile.velocity.normalize() * -halfWidth) + (Projectile.velocity.normalize() * (halfWidth * Main.rand.NextFloat(0.8f ,1.05f))).RotatedByRandom(float.Pi);
                Dust dust = Dust.NewDustPerfect(pos, ModContent.DustType<ShineStarDust>(), Projectile.velocity * (velValue * Main.rand.NextFloat(0.8f, 1f)) * -1, 0, Color.RoyalBlue, Main.rand.NextFloat(0.3f, 0.8f));
                dust.noGravity = true;
                Dust dustS2 = Dust.NewDustPerfect(pos, ModContent.DustType<SharpSparkDust>(), (Projectile.velocity * (velValue * Main.rand.NextFloat(0.8f, 1f)) * -1).RotatedByRandom(0.05f), 0, Color.MidnightBlue, Main.rand.NextFloat(1f, 1.6f));
                dustS2.noGravity = true;
                if (Main.rand.NextBool(3) && Projectile.timeLeft < duration * 0.9f)
                {
                    Vector2 pos2 = Projectile.Center + (Projectile.velocity.normalize() * -halfWidth) + (Projectile.velocity.normalize() * (halfWidth * Main.rand.NextFloat(0.7f, 1f))).RotatedByRandom(float.Pi);
                    //particles
                }
            }
            odp.Add(Projectile.Center + Projectile.velocity.normalize() * (halfWidth * 0.5f));

            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center + Projectile.velocity.normalize() * -halfWidth * Projectile.scale * squish;
            Vector2 end = start + Projectile.velocity.normalize() * texture.Width * Projectile.scale * squish;
            float collisionPoint = 0f;
            float collisionWidth = texture.Width;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D textureinner = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/MoonveilProjInner", AssetRequestMode.ImmediateLoad).Value;
            Texture2D fadeLine1 = ModContent.Request<Texture2D>("asuw/Assets/FadeLine1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D fadeLine2 = ModContent.Request<Texture2D>("asuw/Assets/FadeLine2", AssetRequestMode.ImmediateLoad).Value;

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Trails/ScarletDevilStreak");
            GameShaders.Misc["asuw:TrailBackward"].Apply();

            PrimitiveRenderer.RenderTrail(odp, new PrimitiveSettings(TrailWidth, TrailColor, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 180);
            GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Trails/SylvestaffStreak");
            PrimitiveRenderer.RenderTrail(odp, new PrimitiveSettings(TrailWidth, TrailColor2, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 180);

            Main.spriteBatch.ExitShaderRegion();

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 2; i++) 
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, Color.MidnightBlue * Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, Projectile.scale * squish, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(textureinner, Projectile.Center - Main.screenPosition + Projectile.velocity.normalize() * 20, default, Color.White * Projectile.Opacity, Projectile.rotation, textureinner.Size() / 2f, Projectile.scale * new Vector2(1f,1.06f) * squish, SpriteEffects.None, 0);

            for (int i = 0; i < totalLines; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (flt[i] == 1)
                        Main.spriteBatch.Draw(fadeLine1, Projectile.Center - Main.screenPosition + flp[i], default, (j == 0 ? Color.MidnightBlue : Color.DodgerBlue * 0.8f) * Projectile.Opacity, Projectile.rotation, new Vector2(fadeLine1.Width * 0.7f, fadeLine1.Height / 2f), fls[i] * (j == 0 ? 1 : 0.6f), SpriteEffects.None, 0);
                    else
                        Main.spriteBatch.Draw(fadeLine2, Projectile.Center - Main.screenPosition + flp[i], default, (j == 0 ? Color.MidnightBlue : Color.DodgerBlue * 0.8f) * Projectile.Opacity, Projectile.rotation, new Vector2(fadeLine1.Width * 0.6f, fadeLine1.Height / 2f), fls[i] * (j == 0 ? 1 : 0.6f), SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.ExitShaderRegion();

            return false;
        }
        public Color TrailColor(float completionRatio, Vector2 vertex)
        {
            Color result = Color.Lerp(Color.MidnightBlue, Color.DodgerBlue, completionRatio);
            return Color.Lerp(result, Color.White, 0.2f) * Projectile.Opacity * completionRatio;
        }
        public Color TrailColor2(float completionRatio, Vector2 vertex)
        {
            Color result = Color.Lerp(Color.MidnightBlue, Color.RoyalBlue, completionRatio);
            return result * Projectile.Opacity * completionRatio;
        }
        public float TrailWidth(float completionRatio, Vector2 vertex)
        {
            float t = completionRatio * 2f;
            float v = t <= 1f ? t : 2f - t;
            float v2 = AsuUtils.QuadOut(v);
            return MathHelper.Lerp(0, TextureAssets.Projectile[Type].Value.Height * Projectile.scale, v2);
        }
    }
}
