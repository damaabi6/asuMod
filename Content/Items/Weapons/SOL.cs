using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Dusts;
using asuw.Content.Global;
using asuw.Content.Items.Accesories;
using asuw.Content.Items.Accesories.Wings;
using asuw.Content.Items.Weapons.Ranged;
using asuw.Content.Projectiles;
using asuw.Content.Projectiles.InfoAndChargeBar;
using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons
{
    public class SOL : ModItem
    {
        public bool shoot = false;
        public int blasterTime = 0;
        public float blasterRotTo = 0;
        public int blasterPhase = 0;
        public int hikariyoCharge = 0;
        public float chargeBarOP = 0;
        public float[] individualBarsOP = new float[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 64;
            Item.crit = 10;
            Item.DamageType = DamageClass.Magic;
            Item.width = 102;
            Item.height = 34;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shootSpeed = 40f;
            Item.shoot = ModContent.ProjectileType<SOLSmallHikariyo>();
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.mana = 6;
            Item.ArmorPenetration = 5;
            Item.noUseGraphic = true;

        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 newVelocity = player.asuw().mouseNormalFromPlayer * velocity.Length();
            velocity = newVelocity;
            Vector2 muzzleOffset = (Vector2.Normalize(newVelocity) * 90f);
            position += muzzleOffset;

        }

        public override bool AltFunctionUse(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                int type = ModContent.ProjectileType<SOLHikariyo>();
                if (player.ownedProjectileCounts[type] < 1 && player.equippedWings != null && player.equippedWings.ModItem.Type == ModContent.ItemType<CoreOfSupernova>())
                    return true;
            }
            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
            {
                if(hikariyoCharge < 12)
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SOLChargeUp>(), (int)((float)damage * 1.7f), knockback, player.whoAmI);
                else
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SOLHikariyo>(), (int)((float)damage * 1.7f), knockback, player.whoAmI);
            }
            else
            {
                shoot = true;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                player.asuw().mouseRotationListener = true;
                player.asuw().mouseWorldListener = true;

                int type = ModContent.ProjectileType<SOLHeld>();
                if (player.ownedProjectileCounts[type] < 1)
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, player.asuw().mouseNormalFromPlayer, type, Item.damage, Item.knockBack, player.whoAmI);
            }

        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.equippedWings != null && player.equippedWings.ModItem.Type == ModContent.ItemType<CoreOfSupernova>()) damage += 1;
        }
        public override void UpdateInventory(Player player)
        {
            if (blasterTime > 0) blasterTime--;
            else blasterRotTo = 0;

            for(int i = 0; i < hikariyoCharge; i++)
            {
                individualBarsOP[i] = individualBarsOP[i].Towards(1, 0.05f); 
            }
            for (int i = hikariyoCharge; i < 12; i++)
            {
                individualBarsOP[i] = individualBarsOP[i].Towards(0, 0.1f);
            }

            if (Main.myPlayer == player.whoAmI)
            {
                int type = ModContent.ProjectileType<SOLHikariyo>();
                if (player.ownedProjectileCounts[type] < 1)
                    blasterPhase = 0;
            }
        }

        internal static void DrawBar()
        {
            Player player = Main.LocalPlayer;

            if (!player.active || player.dead)
                return;

            if(player.HeldItem.ModItem != null && player.HeldItem.ModItem is SOL sol)
            {
                Vector2 pos = player.MountedCenter + Vector2.UnitX * 100;
                for(int i = 0; i < 12; i++)
                {
                    Vector2 barPos = pos + Vector2.UnitY * 40 * i;
                    Color color = Color.DodgerBlue * sol.individualBarsOP[i];
                }
            }


        }

    }

    public class SOLHeld : ModProjectile
    {
        Player player => Projectile.GetOwner();
        public override string Texture => "asuw/Content/Items/Weapons/SOL";
        public ref float time => ref Projectile.ai[0];
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
            Projectile.FriendlySetDefaults(DamageClass.Magic, false, -1);
            Projectile.friendly = false;
            Projectile.timeLeft = 2;
        }
        SOL heldWeapon()
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SOL sol)
                return sol;
            else
                return null;
        }
        float recoil = 0;
        float offset = 0;
        public override void AI()
        {
            if (heldWeapon() != null && !player.dead)
            {
                SOL weapon = heldWeapon();
                Projectile.timeLeft = 2;
                player.heldProj = Projectile.whoAmI;
                if (weapon.shoot)
                {
                    weapon.shoot = false;
                    recoil = Main.rand.Next(3, 7);
                    offset = Main.rand.Next(4, 9);

                }
                float recoilRot = MathHelper.ToRadians(-recoil * Projectile.direction * player.gravDir);
                float handRotOffest = MathHelper.ToRadians(40 * Projectile.direction) + MathHelper.ToRadians(offset * 5 * Projectile.direction);
                Projectile.velocity = player.asuw().mouseNormalFromPlayer;
                Projectile.rotation = Projectile.velocity.ToRotation() + recoilRot;
                player.SetHandRotFront(Projectile.velocity.ToRotation() * player.gravDir + handRotOffest);
                player.direction = Projectile.direction;
                Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm) + Projectile.rotation.ToRotationVector2() * -offset;

                if (recoil > 0) recoil *= 0.9f;
                if (offset > 0) offset *= 0.9f;
            }
            else
                Projectile.Kill();

        }
        public override bool PreDraw(ref Color lightColor)
        {
            int dir = Projectile.direction * (int)player.gravDir;
            SpriteEffects effects = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new Vector2(texture.Width * 0.25f, texture.Height * (dir > 0 ? 0.7f : 0.3f));
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);

            return false;
        }

    }

    public class SOLSmallHikariyo : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        public ref float time => ref Projectile.ai[2];

        List<Vector2> points = new List<Vector2>();

        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Magic, penetrate: 3);
            Projectile.timeLeft = 200;
            Projectile.MaxUpdates = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 1; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + player.velocity + Projectile.velocity.normalize() * 15 * i, ModContent.DustType<LightRing>(), (Projectile.velocity.normalize() * 4 * i), 0, Color.RoyalBlue, 2f * i);
                dust.noGravity = true;

                for (int j = 1; j < 3; j++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + player.velocity + Projectile.velocity.normalize() * 12, ModContent.DustType<SquishDust>(), (Projectile.velocity.normalize() * Main.rand.NextFloat(4f, 6f) * i).RotatedByRandom(0.4f), 0, Color.RoyalBlue, Main.rand.NextFloat(0.7f, 1.2f));
                    dust2.noGravity = true;
                }
            }
         
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Opacity = 1;
            Projectile.scale = 1.5f;
            points.Add(Projectile.Center);
            if (points.Count > 20)
                points.RemoveAt(0);
            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float width = 20 * Projectile.scale;
            float halfLength = 50 * Projectile.scale;
            Vector2 start = Projectile.Center - Projectile.velocity.normalize() * halfLength;
            Vector2 end = start + Projectile.velocity.normalize() * halfLength;
            float collisionPoint = 0f;
            float collisionWidth = width;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D light = TextureAssets.Projectile[Type].Value;
            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/Particles/trace_01").Value;

            List<Vector2> points2 = new List<Vector2>();
            for (int i = -30; i <= 0; i++)
            {
                points2.Add(Projectile.Center + Projectile.velocity.normalize() * 3 * i);
            }

            Main.spriteBatch.EnterShaderRegion();

            GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Particles/trace_01");
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(TrailWidth, TrailColor, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 120);

            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(TrailWidth2, TrailColor2, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 120);

            PrimitiveRenderer.RenderTrail(points2, new PrimitiveSettings(TrailWidth3, TrailColor2, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 120);


            //Main.spriteBatch.UseBlendState(BlendState.Additive);
            //for(int i = 0; i < 3; i++)
            //Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity, Projectile.rotation, new Vector2(light.Width, light.Height / 2f), Projectile.scale * 0.75f * ((float)points.Count / 25f) * new Vector2(1.5f,1f), SpriteEffects.None, 0);
            //Main.spriteBatch.ExitShaderRegion();

            return false;
        }

        public Color TrailColor(float completionRatio, Vector2 vertex)
        {
            return Color.DodgerBlue * Projectile.Opacity * AsuUtils.QuartOut(completionRatio);
        }
        public float TrailWidth(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, 18 * Projectile.scale, completionRatio);
        }

        public Color TrailColor2(float completionRatio, Vector2 vertex)
        {
            return Color.White * Projectile.Opacity * AsuUtils.QuartOut(completionRatio);
        }
        public float TrailWidth2(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, 9 * Projectile.scale, completionRatio);
        }

        public float TrailWidth3(float completionRatio, Vector2 vertex)
        {
            float t = completionRatio * 2f;
            float v = t <= 1f ? t : 2f - t;
            float v2 = AsuUtils.QuartOut(v);
            return MathHelper.Lerp(0, 16 * Projectile.scale, v2);
        }

    }
    public class SOLChargeUp : ModProjectile
    {
        Player player => Projectile.GetOwner();
        public override string Texture => "asuw/Assets/Blank";
        public ref float time => ref Projectile.ai[2];
        List<Vector2> windowPos = new List<Vector2>();
        List<Vector2> windowSize = new List<Vector2>();
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Magic, false, -1);
            Projectile.friendly = false;
            Projectile.timeLeft = 10;
        }
        SOL heldWeapon()
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SOL sol)
                return sol;
            return null;
        }
        CoreOfSupernova playerWing()
        {
            if (player.equippedWings != null && player.equippedWings.ModItem is CoreOfSupernova cor)
                return cor;
            return null;
        }
        float duration = 60;

        public override void OnSpawn(IEntitySource source)
        {
            player.asuw().SOLChargeUpActivated = true;
            for (int i = 1; i <= 4; i++)
            {
                float offsetX = 60 * (i / 2f);
                float offsetY = 40 * (i / 2f);
                Vector2 offset = Utils.NextVector2CircularEdge(Main.rand, offsetX, offsetY);
                Vector2 size = new Vector2(Main.rand.Next(40, 50), Main.rand.Next(40, 50)) * MathHelper.Lerp(1, 0.65f, i / 4f);
                windowPos.Add(offset);
                windowSize.Add(size);
            }
        }
        public override void AI()
        {
            if (heldWeapon() == null || playerWing() == null)
                return;

            if (time < duration) Projectile.timeLeft = 2;
            player.itemTime = 15;
            player.itemAnimation = 15;
            Projectile.Center = player.MountedCenter;
            float t = time / duration;
            float v = AsuUtils.PingPong(time, duration);
            player.velocity *= (0.75f + AsuUtils.QuartIn(t) * 0.25f);
            if (time == (int)(duration / 2f))
                if(heldWeapon().hikariyoCharge < 12) heldWeapon().hikariyoCharge++;

            playerWing().purple = ((float)heldWeapon().hikariyoCharge / 24f) * (1 - AsuUtils.QuartIn(t));
            playerWing().bloomRotStrength = Main.rand.NextFloat(0.8f, 0.9f) * (1 - AsuUtils.QuartIn(t));
            playerWing().thrusterStrengthOverride = 1;

            Projectile.frame = (Projectile.frame + 1) % 15;

            time++;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (player.asuw().SOLChargeUpActivated)
            {
                if (heldWeapon().hikariyoCharge < 12) heldWeapon().hikariyoCharge++;
                player.asuw().SOLChargeUpActivated = false;
            }
            CombatText.NewText(player.Hitbox, Color.Red, heldWeapon().hikariyoCharge);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float t = time / duration;
            int currentFrame = (int)Math.Floor(16f * t);
            Texture2D OvalRing = ModContent.Request<Texture2D>("asuw/Assets/Particles/RingOfOvalsAnimated", AssetRequestMode.AsyncLoad).Value;
            Rectangle Frames = OvalRing.Frame(verticalFrames: 15, frameY: Projectile.frame);
            Rectangle Frames2 = OvalRing.Frame(verticalFrames: 15, frameY: 14 - Projectile.frame);
            float v = AsuUtils.PingPong(time, duration);

            Vector2 posOffset1 = Vector2.UnitY * 30 * (1 - AsuUtils.QuadOut(t));
            Vector2 posOffset2 = Vector2.UnitY * 20 * (1 - AsuUtils.QuartOut(t));
            for (int i = 0; i < 2; i++)
            {
                Color ringColor1 =  ( i == 0 ? Color.DodgerBlue : Color.LightSkyBlue) with { A = 0 }
                                    * Projectile.Opacity * AsuUtils.QuartOut(v)
                                    * 0.6f;
                Vector2 squish = i == 0 ? new Vector2(1f, 0.17f) : new Vector2(1.18f, 0.2f);
                Main.spriteBatch.Draw(OvalRing,
                    player.Center - Main.screenPosition + Vector2.UnitY * 60 + posOffset1,
                    Frames,
                    ringColor1,
                    0,
                    Frames.Size() / 2f,
                    Projectile.scale * (i == 0 ? 0.5f : 0.43f) * 0.7f * squish,
                    SpriteEffects.None,
                    0);
            }
            for (int i = 0; i < 2; i++)
            {
                Color ringColor2 = (i == 0 ? Color.DodgerBlue : Color.White) with { A = 0 }
                   * Projectile.Opacity * AsuUtils.QuartOut(v)
                   * (i == 0 ? 1 : 0.6f);
                Vector2 squish = i == 0 ? new Vector2(1f, 0.17f) : new Vector2(1.18f, 0.2f);
                Main.spriteBatch.Draw(OvalRing,
                    player.Center - Main.screenPosition + Vector2.UnitY * 50 + posOffset2,
                    (Frames2),
                    ringColor2,
                    0,
                    Frames2.Size() / 2f,
                    Projectile.scale * (i == 0 ? 0.5f : 0.43f) * 0.45f * squish,
                    SpriteEffects.None,
                    0);
            }
            
            Texture2D TechnoScan = ModContent.Request<Texture2D>("asuw/Assets/Noise/Techno2", AssetRequestMode.AsyncLoad).Value;
            Texture2D TechnoScan2 = ModContent.Request<Texture2D>("asuw/Assets/Noise/Techno", AssetRequestMode.AsyncLoad).Value;
            Texture2D Window = ModContent.Request<Texture2D>("asuw/Assets/UIElements/TechyFrame", AssetRequestMode.AsyncLoad).Value;
            Texture2D WindowText = ModContent.Request<Texture2D>("asuw/Assets/UIElements/TextScroll", AssetRequestMode.AsyncLoad).Value;
            List<ColoredVertex> vertices = new List<ColoredVertex>();
            for (int i = 0; i < 20; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    Vector2 pos = player.MountedCenter - Main.screenPosition - Vector2.UnitX * 120 + Vector2.UnitX * 12 * i;
                    Vector2 posY = pos - Vector2.UnitY * 150 * (j == 0 ? 1 : -1);
                    Vector3 coords = new Vector3(i / (20f - 1f), j, 1);
                    vertices.Add(new ColoredVertex(posY, coords, Color.AliceBlue));
                }
            }
            float scanOP = AsuUtils.QuadOut(v);
            var gd = Main.graphics.GraphicsDevice;
            ShaderFunctions.vertexScanUp(Main.spriteBatch, Color.SkyBlue, Color.LightSkyBlue, scanOP, AsuUtils.QuadInOut(t), 0.4f);
            gd.Textures[0] = TechnoScan2;
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
            Main.spriteBatch.ExitShaderRegion();

            ShaderFunctions.vertexScanUp(Main.spriteBatch, Color.DodgerBlue, Color.SkyBlue, scanOP, AsuUtils.QuadInOut(t), 0.3f);
            gd.Textures[0] = TechnoScan;
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
            Main.spriteBatch.ExitShaderRegion();

            for (int i = 0; i < windowPos.Count; i++)
            {
                List<ColoredVertex> windows = new List<ColoredVertex>();
                List<ColoredVertex> texts = new List<ColoredVertex>();
                for (int j = 0; j < 20; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector2 pos = player.MountedCenter + windowPos[i] - Main.screenPosition - Vector2.UnitX * windowSize[i].X + Vector2.UnitX * (windowSize[i].X / 10f) * j;
                        Vector2 posY = pos - Vector2.UnitY * windowSize[i].Y * (k == 0 ? 1 : -1);
                        Vector3 coords = new Vector3(j / (20f - 1f), k, 1);
                        windows.Add(new ColoredVertex(posY, coords, Color.AliceBlue));
                        Vector2 posT = player.MountedCenter + windowPos[i] - Main.screenPosition - Vector2.UnitX * (windowSize[i].X * 0.65f) + Vector2.UnitX * ((windowSize[i].X * 0.65f) / 10f) * j;
                        Vector2 posYT = posT - Vector2.UnitY * (windowSize[i].Y * 0.65f) * (k == 0 ? 1 : -1);
                        texts.Add(new ColoredVertex(posYT, coords, Color.AliceBlue));
                    }
                }
                //ShaderFunctions.vertexScanUp(Main.spriteBatch, Color.DodgerBlue, Color.SkyBlue, scanOP, AsuUtils.QuadInOut(t), 0.4f);
                ShaderFunctions.vertexColorBloom(Main.spriteBatch, Color.RoyalBlue, Color.DodgerBlue, scanOP * 0.6f);
                gd.Textures[0] = Window;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, windows.ToArray(), 0, windows.Count - 2);
                Main.spriteBatch.ExitShaderRegion();

                ShaderFunctions.vertexScrollUp(Main.spriteBatch, Color.RoyalBlue, Color.DodgerBlue, scanOP * 0.6f);
                gd.Textures[0] = WindowText;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, texts.ToArray(), 0, texts.Count - 2);
                Main.spriteBatch.ExitShaderRegion();
            }

            return false;
        }

    }

    public class SOLHikariyo : ModProjectile
    {
        Player player => Projectile.GetOwner();
        public override string Texture => "asuw/Assets/Blank";
        public ref float time => ref Projectile.ai[0];
        public ref float phase => ref Projectile.ai[1];
        int duration = 120;
        float lightScale = 0;
        float lightOP = 1;
        float lightRingsRot = 0;
        float lightRingsScale = 0;
        float lightRingsOP = 0;
        float blingScale = 1;
        float blingOP = 1;
        float blingRot = 0;
        float bling2OP = 0;
        float bling2Scale = 0;

        List<Vector2> zap1points = new List<Vector2>();
        List<Vector2> zap2points = new List<Vector2>();
        float zap1OP = 0;
        float zap2OP = 0;

        int initialCharge = 0;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Magic, false, -1);
            Projectile.friendly = true;
            Projectile.timeLeft = 10;
            Projectile.localNPCHitCooldown = 5;
            Projectile.Opacity = 0;
        }
        SOL heldWeapon()
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SOL sol)
                return sol;
            else
                return null;
        }

        CoreOfSupernova playerWing()
        {
            if (player.equippedWings != null && player.equippedWings.ModItem is CoreOfSupernova cor)
                return cor;
            
            return null;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (heldWeapon() == null || playerWing() == null)
                return;

            Projectile.Opacity = 0;
            initialCharge = heldWeapon().hikariyoCharge;
            lightRingsRot = AsuUtils.randomRot();
        }
        public override void AI()
        {
            if(heldWeapon() == null || playerWing() == null)
                return;

            //-------commons-------------------------------||
            int dir = player.mouseWorld().X > player.Center.X ? 1 : -1;
            Projectile.Center = player.Center - (Vector2.UnitY * player.height * 0.6f * player.gravDir) + Projectile.velocity.normalize() * 95;
            float rotToAngle = Projectile.AngleTo(Projectile.Center + player.asuw().mouseNormalFromPlayer * 300); 
            float maxChange = MathF.Abs(Projectile.velocity.ToRotation().AngleBetween(rotToAngle) / 20);
            Projectile.velocity = phase == 0 ? (Vector2.UnitX * dir) : Projectile.velocity.ToRotation().RotTowards(rotToAngle,maxChange).ToRotationVector2();
            float lerp = time / duration;
            float t = time / (int)(duration / 2f);
            float v = t <= 1f ? t : 2f - t;
            Projectile.scale = MathHelper.Lerp(0, 1, AsuUtils.QuadOut(AsuUtils.ExpoOut(v))) * (phase == 2 ? 0.76f : 1);
            heldWeapon().blasterRotTo = phase == 0 ? 0 : Projectile.velocity.ToRotation();
            heldWeapon().blasterPhase = (int)phase;
            //---------------------------------------------||


            //-------phases--------------------------------||
            if (phase == 1)//windup
            {
                if (Projectile.Opacity < 1) Projectile.Opacity += 0.05f;
            }
            else if(phase == 2)//shoot
            {
                player.itemTime = 15;
                player.itemAnimation = 15;
                player.SetScreenshake(3f);
                float blingDur = (int)(duration * 0.2f);
                float bt = time / blingDur;
                bling2OP = time <= (int)(blingDur ) ? 1 - AsuUtils.QuadIn(bt) : 0;
                bling2Scale = AsuUtils.CubicOut(bt);
                playerWing().purple = 1 * AsuUtils.ExpoOut(lerp);
                playerWing().bloomRotStrength = Main.rand.NextFloat(0.8f,1f) * AsuUtils.ExpoOut(lerp);
                playerWing().thrusterStrengthOverride = 1;
                heldWeapon().hikariyoCharge = (int)MathHelper.Lerp(initialCharge, 0, time / duration);
                
            }
            //---------------------------------------------||


            //-------zaps----------------------------------||
            if (time % 20 == 0)
            {
                if(zap1points.Count > 0)
                    zap1points.Clear();
                for (int i = 0; i < Main.rand.Next(45, 55); i++)
                {
                    zap1points.Add(Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2) * Main.rand.Next(-20, 20));
                }
                zap1OP = 1;
            }
            if (time % 20 == 10)
            {
                if (zap2points.Count > 0)
                    zap2points.Clear();
                for (int i = 0; i < Main.rand.Next(45,55); i++)
                {
                    zap2points.Add(Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2) * Main.rand.Next(-30, 30));
                }
                zap2OP = 1;
            }

            if (zap1OP > 0) zap1OP *= 0.9f;
            if (zap2OP > 0) zap2OP *= 0.9f;
            //---------------------------------------------||


            //-------scoping-------------------------------||
            if (player.asuw().mouseRight && phase > 0)
            player.scope = true;
            //---------------------------------------------||



            lightScale = MathHelper.Lerp(0.25f, 1, AsuUtils.QuadOut(lerp)) * (0.2f + 0.01f * MathF.Sin(time));
            float lerpLR = time / (duration * 0.79f);
            lightRingsScale = MathHelper.Lerp(0.7f, 0.1f, AsuUtils.QuintIn(lerpLR));
            lightRingsOP = (AsuUtils.QuartIn(lerpLR));
            blingRot += 0.1f * AsuUtils.QuintOut(lerp);


            if (time > duration * 0.74f)
            {
                lightOP *= 0.9f;
                blingScale *= 1.03f;
            }
            if (time > duration * 0.85f)
            {
                blingOP *= 0.84f;
            }
            if (time < duration)
            {
                if(phase == 0 && time > (int)(duration * 0.15f))
                {
                    time = 0;
                    phase = 1;
                }
                Projectile.timeLeft = 2;
                heldWeapon().blasterTime = 2;
            }
            else if (phase == 1)
            {
                Projectile.timeLeft = 2;
                time = 0;
                phase = 2;
                
            }
            time++;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 4; i < 70; i++)
            {

                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity.normalize() * Main.rand.Next(25, 28) * i + Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.Next(-3,3), ModContent.DustType<SquishDust>(), Projectile.velocity * Main.rand.Next(3, 5), 0, Color.Fuchsia, Main.rand.NextFloat(2, 3));
                dust2.noGravity = true;
                
            }
        }
        public override bool? CanDamage() => phase == 2;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float width = (450 - (150 * AsuUtils.CubicIn(time / duration))) * Projectile.scale;
            float length = 2000;
            Vector2 start = Projectile.Center ;
            Vector2 end = start + Projectile.velocity.normalize() * length;
            float collisionPoint = 0f;
            float collisionWidth = width;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float distance = MathF.Min(1500, MathF.Max(100, Projectile.Distance(target.Center))); //from 100 - 1500 distance
            float timeBonus = AsuUtils.CubicIn(time / duration) * 0.5f;
            float dmgBonus = (1 - AsuUtils.QuadOut(distance / 1500f)) * 0.5f + timeBonus;
            modifiers.FinalDamage += dmgBonus;
            modifiers.DamageVariationScale *= 0.1f;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (heldWeapon() == null)
                return false;

            Texture2D lightEdge = ModContent.Request<Texture2D>("asuw/Assets/Particles/Light1").Value;
            Texture2D light = ModContent.Request<Texture2D>("asuw/Assets/Particles/Light2").Value;
            Texture2D innerLight = ModContent.Request<Texture2D>("asuw/Assets/Particles/Bling2").Value;
            Texture2D LightTrails = ModContent.Request<Texture2D>("asuw/Assets/Boom1").Value;
            Texture2D LightRing1 = ModContent.Request<Texture2D>("asuw/Assets/Particles/LightRing1").Value;
            Texture2D LightRing2 = ModContent.Request<Texture2D>("asuw/Assets/Particles/LightRing").Value;
            Texture2D Bling = ModContent.Request<Texture2D>("asuw/Assets/Particles/Bling1").Value;
            Texture2D Bling2 = ModContent.Request<Texture2D>("asuw/Assets/Particles/Bling3").Value;
            Texture2D Glow = ModContent.Request<Texture2D>("asuw/Assets/Particles/circle_05").Value;

            if (phase == 1)
            {

                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.spriteBatch.Draw(lightEdge, Projectile.Center - Main.screenPosition, default, Color.Lerp(Color.RoyalBlue, Color.White, 0.3f) * (0.67f + 0.3f * (1 - lightOP)) * Projectile.Opacity, Projectile.rotation, lightEdge.Size() / 2f, lightScale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, default, Color.Lerp(Color.RoyalBlue, Color.White, 0.75f) * 0.67f * lightOP * Projectile.Opacity, Projectile.rotation, light.Size() / 2f, lightScale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, default, Color.White * lightOP * Projectile.Opacity, Projectile.rotation, Glow.Size() / 2f, lightScale * 1.2f, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(LightRing1, Projectile.Center - Main.screenPosition, default, Color.Lerp(Color.RoyalBlue, Color.White, 0.45f) * 0.89f * lightOP * lightRingsOP * Projectile.Opacity, lightRingsRot + (blingRot * 0.2f), LightRing1.Size() / 2f, lightRingsScale * 1.8f, SpriteEffects.None, 0);

                for (int i = 0; i < 2; i++)
                {
                    Main.spriteBatch.Draw(LightTrails, Projectile.Center - Main.screenPosition, default, Color.Lerp(Color.RoyalBlue, Color.White, 0.3f) * 0.89f * lightOP * lightRingsOP * Projectile.Opacity, lightRingsRot, LightTrails.Size() / 2f, lightRingsScale * 7.5f, SpriteEffects.None, 0);

                    Main.spriteBatch.Draw(LightRing2, Projectile.Center - Main.screenPosition, default, Color.Lerp(Color.RoyalBlue, Color.White, 0.45f) * 0.89f * (1f - lightOP) * blingOP * Projectile.Opacity, blingRot, LightRing2.Size() / 2f, blingScale * 0.5f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(Bling, Projectile.Center - Main.screenPosition, default, Color.Lerp(Color.RoyalBlue, Color.White, 0.45f) * 0.89f * (1f - lightOP) * blingOP * Projectile.Opacity, blingRot, Bling.Size() / 2f, blingScale * 0.5f, SpriteEffects.None, 0);
                }
                Main.spriteBatch.ExitShaderRegion();
    
                Effect shader = ModContent.Request<Effect>("asuw/Effects/ColorizeBloom", AssetRequestMode.ImmediateLoad).Value;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue(Color.Lerp(Color.Maroon, Color.Fuchsia, 0.45f).ToVector4());
                shader.Parameters["color1"].SetValue((Color.Lerp(Color.OrangeRed, Color.Magenta, 0.65f)).ToVector4());
                shader.Parameters["alpha"].SetValue(0.5f * (1 - lightOP) * Projectile.Opacity);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();
                Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, default, Color.White, Projectile.rotation, Glow.Size() / 2f, lightScale * Projectile.scale * 1.2f, SpriteEffects.None, 0);
                for (int i = 0; i < 2; i++)
                Main.spriteBatch.Draw(innerLight, Projectile.Center - Main.screenPosition, default, Color.White, blingRot + (MathHelper.PiOver2 * i), innerLight.Size() / 2f, lightScale * Projectile.scale * 2f, SpriteEffects.None, 0);
                Main.spriteBatch.ExitShaderRegion();

            }
            else if(phase == 2) 
            {
                List<Vector2> points = new List<Vector2>();
                List<Vector2> points2 = new List<Vector2>();
                List<Vector2> zapPoints = new List<Vector2>();
                List<Vector2> zapPoints2 = new List<Vector2>();
                List<Vector2> smokePoints = new List<Vector2>();
                List<Vector2> smokePoints2 = new List<Vector2>();
                Vector2 pointsOffset = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.Next(-5, 6); //slight jitter
                for (int i = 0; i < 100; i++)
                {
                    points.Add(Projectile.Center + pointsOffset + Projectile.velocity.normalize() * 30 * i);
                    points2.Add(points[i] - Projectile.velocity.normalize() * 10);
                }

                for(int i = 0; i < zap1points.Count; i++)
                {
                    zapPoints.Add(Projectile.Center + Projectile.velocity.normalize() * (50 + (int)(15 * (1 - zap1OP))) * i + zap1points[i] + pointsOffset);
                }
                for (int i = 0; i < zap2points.Count; i++)
                {
                    zapPoints2.Add(Projectile.Center + Projectile.velocity.normalize() * (50 + (int)(15 * (1 - zap2OP))) * i + zap2points[i] + pointsOffset);
                }

                for (int i = 0; i < 50; i++)
                {
                    smokePoints.Add(Projectile.Center - Projectile.velocity.normalize() * 30 * (1 + i));
                    smokePoints2.Add(Projectile.Center - Projectile.velocity.normalize() * 50 * (0.4f + i));
                }


                for (int i = 0; i < 2; i++)
                {
                    GameShaders.Misc["asuw:ArisuBlast"].SetShaderTexture("asuw/Assets/Noise/SuperPerlin");
                    PrimitiveRenderer.RenderTrail(smokePoints, new PrimitiveSettings(SmokeWidth2, Smoke, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);

                    PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(SmokeWidth, Smoke2, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);

                    //GameShaders.Misc["asuw:ArisuBlast"].SetShaderTexture("asuw/Assets/Noise/Turbulence");
                    //PrimitiveRenderer.RenderTrail(smokePoints2, new PrimitiveSettings(SmokeWidth3, Smoke, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);

                    GameShaders.Misc["asuw:ArisuBlast"].SetShaderTexture("asuw/Assets/White");
                    PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(TrailWidth, BackInner, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);

                    GameShaders.Misc["asuw:ArisuBlast"].SetShaderTexture("asuw/Assets/Noise/StreakBright");
                    PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(TrailWidth, Inner, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);

                    GameShaders.Misc["asuw:ArisuBlast"].SetShaderTexture("asuw/Assets/Noise/Dots2");
                    PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(TrailWidth, White, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);

                    GameShaders.Misc["asuw:ArisuBlast"].SetShaderTexture("asuw/Assets/Trails/FlameTrail2");
                    PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(InnerPinkWidth, InnerPink, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);

                    for (int j = 0; j < 2; j++)
                    {
                        GameShaders.Misc["asuw:ArisuBlast"].SetShaderTexture("asuw/Assets/Trails/SquigglyLine");
                        PrimitiveRenderer.RenderTrail(zapPoints, new PrimitiveSettings(ZapWidth1, Zap1, (_, _) => Vector2.Zero, smoothen: false, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);
                        PrimitiveRenderer.RenderTrail(zapPoints2, new PrimitiveSettings(ZapWidth2, Zap2, (_, _) => Vector2.Zero, smoothen: false, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);
                    }


                    GameShaders.Misc["asuw:ArisuBlastEdge"].SetShaderTexture("asuw/Assets/Trails/TrailEdges");
                    PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(EdgesWidth, Edges, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlastEdge"]), 120);



                    GameShaders.Misc["asuw:ArisuBlast"].SetShaderTexture("asuw/Assets/Noise/NoiseSharps");
                    PrimitiveRenderer.RenderTrail(points2, new PrimitiveSettings(OuterWidth, Outer, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);

                    GameShaders.Misc["asuw:ArisuBlast"].SetShaderTexture("asuw/Assets/Noise/Turbulence");
                    PrimitiveRenderer.RenderTrail(points2, new PrimitiveSettings(OuterWidth2, Outer2, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);

                    PrimitiveRenderer.RenderTrail(points2, new PrimitiveSettings(OuterWidth22, Outer22, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:ArisuBlast"]), 120);


                }

                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.spriteBatch.Draw(Bling2, Projectile.Center - Main.screenPosition, default, Color.Lerp(Color.Fuchsia, Color.White, 0.55f) * 0.89f * bling2OP * Projectile.Opacity, blingRot * 0.3f, Bling2.Size() / 2f, 1 + bling2Scale, SpriteEffects.None, 0);
                Main.spriteBatch.ExitShaderRegion();
            }
            return false;
        }

        public Color BackInner(float completionRatio, Vector2 vertex)
        {
            return Color.Lerp(Color.MidnightBlue, Color.RoyalBlue, 0.56f);
        }
        public Color Inner(float completionRatio, Vector2 vertex)
        {
            Color MainColor = Color.Lerp(Color.Fuchsia, Color.DodgerBlue, 0.88f);
            return Color.Lerp(Color.Fuchsia, MainColor, AsuUtils.ExpoOut(completionRatio));
        }
        public Color InnerPink(float completionRatio, Vector2 vertex)
        {
            return Color.Lerp(Color.Fuchsia, Color.White, 0.5f) * 0.8f;
        }
        public Color White(float completionRatio, Vector2 vertex)
        {
            return Color.White;
        }
        public Color Smoke(float completionRatio, Vector2 vertex)
        {
            return Color.White * AsuUtils.ExpoOut(completionRatio) * (1f - AsuUtils.QuadOut(AsuUtils.ExpoOut(completionRatio))) * Projectile.scale;
        }
        public Color Smoke2(float completionRatio, Vector2 vertex)
        {
            return Color.White * (1f - AsuUtils.ExpoOut(completionRatio)) * (Projectile.scale * 1.4f);
        }
        public Color Zap1(float completionRatio, Vector2 vertex)
        {
            Color mainColor = Color.Lerp(Color.Fuchsia, Color.DodgerBlue, AsuUtils.ExpoOut(completionRatio));
            return Color.Lerp(mainColor, Color.White, 0.8f) * zap1OP * Projectile.scale;
        }
        public Color Zap2(float completionRatio, Vector2 vertex)
        {
            Color mainColor = Color.Lerp(Color.Fuchsia, Color.DodgerBlue, AsuUtils.ExpoOut(completionRatio));
            return Color.Lerp(mainColor, Color.White, 0.8f) * zap2OP * Projectile.scale;
        }
        public Color Edges(float completionRatio, Vector2 vertex)
        {
            return Color.Lerp(Color.Lerp(Color.Violet, Color.LightSkyBlue, AsuUtils.QuadOut(completionRatio)), Color.White , 0.3f) * 0.85f;
        }
        public Color Outer(float completionRatio, Vector2 vertex)
        {
            Color mainColor = Color.Lerp(Color.Fuchsia, Color.White, 0.3f);
            return Color.Lerp(Color.White, mainColor, AsuUtils.ExpoOut(completionRatio)) * MathHelper.Lerp(1, 0, MathF.Min(completionRatio * 6, 1));
        }
        public Color Outer2(float completionRatio, Vector2 vertex)
        {
            return Color.White * MathHelper.Lerp(1, 0, MathF.Min(completionRatio * 7, 1));
        }
        public Color Outer22(float completionRatio, Vector2 vertex)
        {
            return Color.White * MathHelper.Lerp(1, 0, MathF.Min(completionRatio * 4.5f, 1));
        }
        public float TrailWidth(float completionRatio, Vector2 vertex)
        {
            float mainWidth = 200 - (30 * AsuUtils.CubicIn(time / duration) * 0.7f);
            return MathHelper.Lerp(0, mainWidth * Projectile.scale, AsuUtils.ExpoOut(AsuUtils.ExpoOut(completionRatio)));
        }
        public float SmokeWidth(float completionRatio, Vector2 vertex)
        {
            float mainWidth = 300 - (50 * AsuUtils.CubicIn(time / duration) * 0.7f);
            return MathHelper.Lerp(0, mainWidth * Projectile.scale, AsuUtils.ExpoOut(AsuUtils.ExpoOut(completionRatio)));
        }
        public float SmokeWidth2(float completionRatio, Vector2 vertex)
        {
            float mainWidth = 400 - (100 * AsuUtils.CubicIn(time / duration) * 0.7f);
            return MathHelper.Lerp(0, mainWidth, AsuUtils.QuadOut(AsuUtils.ExpoOut(completionRatio)));
        }
        public float SmokeWidth3(float completionRatio, Vector2 vertex)
        {
            float mainWidth = 320 - (70 * AsuUtils.CubicIn(time / duration) * 0.7f);
            return MathHelper.Lerp(0, mainWidth, AsuUtils.QuadOut(AsuUtils.ExpoOut(completionRatio)));
        }
        public float InnerPinkWidth(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, 100 * Projectile.scale, AsuUtils.ExpoOut(AsuUtils.ExpoOut(completionRatio)));
        }
        public float ZapWidth1(float completionRatio, Vector2 vertex)
        {
            return 20 * zap1OP;
        }
        public float ZapWidth2(float completionRatio, Vector2 vertex)
        {
            return 20 * zap2OP;
        }
        public float EdgesWidth(float completionRatio, Vector2 vertex)
        {
            float mainWidth = 220 - (40 * AsuUtils.CubicIn(time / duration) * 0.7f);
            return MathHelper.Lerp(0, mainWidth * Projectile.scale, AsuUtils.ExpoOut(AsuUtils.ExpoOut(completionRatio)));
        }

        public float OuterWidth(float completionRatio, Vector2 vertex)
        {
            float mainWidth = MathHelper.Lerp(220, 440 - (170 * (time / duration)), AsuUtils.ExpoOut(completionRatio));
            return MathHelper.Lerp(0, mainWidth * Projectile.scale, AsuUtils.ExpoOut(AsuUtils.ExpoOut(completionRatio)));
        }

        public float OuterWidth2(float completionRatio, Vector2 vertex)
        {
            float mainWidth = MathHelper.Lerp(220, 400 - (150 * (time / duration)), AsuUtils.ExpoOut(completionRatio));
            return MathHelper.Lerp(0, mainWidth * Projectile.scale, AsuUtils.ExpoOut(AsuUtils.ExpoOut(completionRatio)));
        }

        public float OuterWidth22(float completionRatio, Vector2 vertex)
        {
            float mainWidth = MathHelper.Lerp(150, 300 - (150 * (time / duration)), AsuUtils.ExpoOut(completionRatio));
            return MathHelper.Lerp(0, mainWidth * Projectile.scale, AsuUtils.ExpoOut(AsuUtils.ExpoOut(completionRatio)));
        }

    }
}
