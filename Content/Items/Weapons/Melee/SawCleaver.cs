//using CalamityMod;
//using CalamityMod.Particles;
//using CalamityMod.Projectiles.Typeless;
using asuw.Content.Projectiles;
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

namespace asuw.Content.Items.Weapons.Melee
{
    public class SawCleaver : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        int attCount = 0;
        public int shouldTransform = 0;
        public int currentTransform = 0;
        public float bloody = 0;
        public float bloodyTimer = 0;
        public int rallytimer = 0;
        public int rallyHeal = 0;

        public static SoundStyle swing = new SoundStyle("asuw/Content/Sounds/SwingDeep");
        public static SoundStyle swingSlow = new SoundStyle("asuw/Content/Sounds/SwingDeepSlowed");
        public static SoundStyle bloodySlash = new SoundStyle("asuw/Content/Sounds/BloodMuch");
        public static SoundStyle transformLong = new SoundStyle("asuw/Content/Sounds/SawTransform1");
        public static SoundStyle transformShort = new SoundStyle("asuw/Content/Sounds/SawTransform2");
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
		{
			Item.damage = 92;
			Item.DamageType = DamageClass.Melee;
			Item.width = Item.height = 109;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = null;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SawCleaverProj>();
            Item.shootSpeed = 1;
			Item.noMelee = true;
			Item.noUseGraphic = true;
            Item.channel = true;
            Item.ArmorPenetration = 15;
           
        }

        public override void UpdateInventory(Player player)
        {
            if (bloody > 1f) bloody = 1f; 
            if (bloodyTimer > 0) bloodyTimer--;
            if (bloodyTimer == 0 && bloody > 0 && player.miscCounter % 4 == 0) bloody -= 0.01f;

            if (rallytimer > 0) rallytimer--;
            if (rallyHeal > 0 && rallytimer == 0) rallyHeal--;
            if (rallyHeal < 0) rallyHeal = 0;
            if (rallyHeal > player.statLifeMax2) rallyHeal = player.statLifeMax2;
        }

	

        public override bool CanUseItem(Player player)
        {
            if(player.asuw().UseCooldown > 0)
                return false;
            else return true;
        }
        

        public override bool AltFunctionUse(Player player)
        {

            return true;

        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
            {
                if (shouldTransform == 0) shouldTransform = 1;
                else shouldTransform = 0;
            }
                return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attCount, currentTransform);
            if (attCount < 3) attCount++;
            else attCount = 0;

            return false;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += (bloody / 2f);
        }
    }

    public class SawCleaverProj : ModProjectile
    {
        public override string Texture => "asuw/Content/Items/Weapons/Melee/SawCleaverReg";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 30;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 10;
        }
        Player player => Projectile.GetOwner();
        ref float attCount => ref Projectile.ai[0];
        ref float transform => ref Projectile.ai[1];
        ref float time => ref Projectile.ai[2];
        int shouldtransform = 0;
        float duration = 0;
        int pDirection = 1;
        int direction = 1;
        int delay = 0;
        int transformFrame = 0;
        bool attacking = false;
        float bloodAmnt = 0;
        float rotOffset = 0;

        //trail parameter
        List<float> odr = new List<float>();
        List<float> ods = new List<float>();

        SawCleaver heldItem()
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SawCleaver saw)
                return saw;
            else return null;
        }
        public override void OnSpawn(IEntitySource source)
        {
            shouldtransform = heldItem() != null ? heldItem().shouldTransform : 0;
            duration = (int)(player.itemTimeMax * Projectile.MaxUpdates * (1 + (0.5f * (float)shouldtransform)));
            pDirection = player.direction;
            direction = player.direction * ((attCount == 0 || attCount == 2) ? 1 : -1);
            if (attCount == 3) delay = 20;
            else delay = 10;
            transformFrame = 2 * (int)transform;
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SawCleaver saw)
                bloodAmnt = saw.bloody;
            rotOffset = transformFrame == 0 ? MathHelper.ToRadians(-65 * player.direction) : MathHelper.ToRadians(-15 * player.direction);
        }

        public override void AI()
        {
            if (time < duration && !player.dead)
            { 
                Projectile.timeLeft = 2;
                player.itemAnimation = 2;
                player.itemTime = 2;
                player.asuw().UseCooldown = delay;
            }

            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SawCleaver saw)
                bloodAmnt = saw.bloody;
            rotOffset = transformFrame == 0 ? MathHelper.ToRadians(-65 * player.direction) : MathHelper.ToRadians(-15 * player.direction);

            if (transform != shouldtransform)
            {
                if(time == (int)(duration * 0.35f) || time == (int)(duration * 0.45f))
                {
                    if (shouldtransform == 1) transformFrame++;
                    else transformFrame--;
                    if (time == (int)(duration * 0.45f)) transform = shouldtransform;
                }

                Vector2 sparkPos = Projectile.Center + (Projectile.rotation + rotOffset).ToRotationVector2() * 50;

                if(shouldtransform == 1)
                {
                    if(time == (int)(duration * 0.2f))
                    SoundEngine.PlaySound(SawCleaver.transformLong with { PitchVariance = 0.1f }, Projectile.Center);
                    //if (time == (int)(duration * 0.44f))
                    //    for (int i = 0; i < 10; i++)
                    //        GeneralParticleHandler.SpawnParticle(new LineParticle(sparkPos, Projectile.velocity.RotatedByRandom(1) * Main.rand.NextFloat(5,9), true, Main.rand.Next(20, 35), Main.rand.NextFloat(0.5f, 1f), Color.Tomato));
                }
                else
                {
                    if(time == (int)(duration * 0.3f))
                    SoundEngine.PlaySound(SawCleaver.transformShort with { PitchVariance = 0.1f }, Projectile.Center);
                    //if (time == (int)(duration * 0.42f))
                    //    for (int i = 0; i < 10; i++)
                    //        GeneralParticleHandler.SpawnParticle(new LineParticle(sparkPos, Projectile.velocity.RotatedByRandom(1) * Main.rand.NextFloat(5, 9), true, Main.rand.Next(20, 35), Main.rand.NextFloat(0.5f, 1f), Color.Tomato));
                }
            }

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(pDirection);
            float val = time / (int)(duration);
            float lerp = AsuUtils.BackInOut(val);
            float t = time / (int)(duration / 2f);
            float v = t <= 1f ? t : 2f - t;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(-170 * direction) + MathHelper.ToRadians((300 * direction) * lerp);
            player.SetHandRotFront(Projectile.rotation);
            Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
            Projectile.scale = MathHelper.Lerp(0.8f, attCount != 3 ? 1.2f : 1.5f, AsuUtils.ExpoIn(v));
            Projectile.Opacity = MathHelper.Lerp(0, 1f, AsuUtils.ExpoOut(v));
            attacking = lerp > 0.2f && val < 0.77f ;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 trailpos = Projectile.Center + (Projectile.rotation + rotOffset).ToRotationVector2() * texture.Size() * Projectile.scale * (transformFrame == 0 ? 0.45f : 0.85f) * Main.rand.NextFloat(0.5f, 1.1f);

            if (attacking)
            {
                odr.Add(Projectile.rotation);
                ods.Add(MathHelper.Lerp(1f, attCount != 3 ? 1.2f : 1.4f, AsuUtils.CubicIn(v)));

                if (bloodAmnt > Main.rand.NextFloat(0.2f,lerp) && Main.rand.NextFloat(1f) < lerp && val < 0.65f && Main.rand.NextBool())
                {
                    float offVal = Main.rand.NextFloat(-2f, 2f);
                    Vector2 offset = Utils.NextVector2Circular(Main.rand, offVal, offVal);
                    Vector2 velo = (Projectile.rotation + rotOffset).ToRotationVector2().RotatedBy(-70 * Projectile.direction) * Main.rand.NextFloat(2f, 4.5f);
                    //BloodParticle2 blood = new BloodParticle2(trailpos + offset, velo, 20, transformFrame == 0 ? Main.rand.NextFloat(0.1f, 0.3f) : Main.rand.NextFloat(0.2f, 0.45f), Color.Red * 0.8f);
                    //GeneralParticleHandler.SpawnParticle(blood);
                    //if(transformFrame != 0 && val < 0.54f) GeneralParticleHandler.SpawnParticle(blood);
                }
            }
            if (odr.Count > 40 || (!attacking && odr.Count > 0))
            {
                odr.RemoveAt(0);
                ods.RemoveAt(0);
            }

            if (time == (int)(duration * 0.3f))
            {
                SoundEngine.PlaySound((shouldtransform == 0 ? SawCleaver.swing : SawCleaver.swingSlow) with { PitchVariance = 0.2f }, Projectile.Center);
                if(bloodAmnt > 0.2f)
                {
                    SoundEngine.PlaySound(SawCleaver.bloodySlash with { Pitch = 0.3f , PitchVariance = 0.1f , Volume = (bloodAmnt * 0.55f) }, Projectile.Center);
                }
            }

            time++;
        }

        public override void OnKill(int timeLeft)
        {
            if (heldItem() != null) heldItem().currentTransform = (int)transform;
        }
        public override bool? CanDamage() => attacking;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center;
            Vector2 end = start + (Projectile.rotation + rotOffset).ToRotationVector2() * texture.Size() * Projectile.scale * (transformFrame == 0 ? 0.45f : 0.9f);
            float collisionPoint = 0f;
            float collisionWidth = transformFrame == 0 ? 90 : 70;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is SawCleaver saw)
            {
                saw.bloodyTimer = 180;
                if (saw.bloody < 1f) saw.bloody += 0.1f;
                if (saw.rallyHeal > 0)
                {
                    int healAmnt = saw.rallyHeal / 2;
                    player.HealPlayer(healAmnt);
                    saw.rallyHeal -= healAmnt;
                }
            }

            if(shouldtransform == 0)
            {
                target.asuw().SerratedApplicator = player.whoAmI;
                target.asuw().SerratedDMG = (int)(40f * (1f + bloodAmnt));
                target.asuw().SerratedAmnt += 4;
                target.AddBuff(ModContent.BuffType<SerratedDebuff>(), 10);
            }
            else
            {
                target.asuw().SerratedApplicator = player.whoAmI;
                target.asuw().SerratedDMG = (int)(10f * (1f + bloodAmnt));
                target.asuw().SerratedAmnt += 6;
                target.AddBuff(ModContent.BuffType<SerratedDebuff>(), 10);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            string reg = "asuw/Content/Items/Weapons/Melee/SawCleaverReg";
            string transforming = "asuw/Content/Items/Weapons/Melee/SawCleaverTransforming";
            string transformed = "asuw/Content/Items/Weapons/Melee/SawCleaverTransformed";
            string regB = "asuw/Content/Items/Weapons/Melee/SawCleaverRegBloody";
            string transformingB = "asuw/Content/Items/Weapons/Melee/SawCleaverTransformingBloody";
            string transformedB = "asuw/Content/Items/Weapons/Melee/SawCleaverTransformedBloody";
            Texture2D texture = transformFrame != 0 ? transformFrame == 1 ? ModContent.Request<Texture2D>(transforming, AssetRequestMode.AsyncLoad).Value : ModContent.Request<Texture2D>(transformed, AssetRequestMode.AsyncLoad).Value : ModContent.Request<Texture2D>(reg, AssetRequestMode.AsyncLoad).Value;
            Texture2D textureB = transformFrame != 0 ? transformFrame == 1 ? ModContent.Request<Texture2D>(transformingB, AssetRequestMode.AsyncLoad).Value : ModContent.Request<Texture2D>(transformedB, AssetRequestMode.AsyncLoad).Value : ModContent.Request<Texture2D>(regB, AssetRequestMode.AsyncLoad).Value;
            Vector2 origin = transformFrame == 0 ? player.direction == 1 ? new Vector2(30, 22) : new Vector2(30, texture.Height - 22) : direction == 1 ? new Vector2(30, 22) : new Vector2(30, texture.Height - 22);
            SpriteEffects effects = transformFrame == 0 ? player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically : direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/GraySmear", AssetRequestMode.ImmediateLoad).Value;

            List<ColoredVertex> ve = new List<ColoredVertex>();

            for (int i = 0; i < odr.Count; i++)
            {
                Color b = new Color(252, 255, 199);
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + ((odr[i] + rotOffset).ToRotationVector2() * (texture.Width * (transformFrame == 0 ? 0.5f : 0.9f) * ods[i])),
                      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                      b));
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + ((odr[i] + rotOffset).ToRotationVector2() * (texture.Width * (transformFrame == 0 ? 0.1f : 0.5f) * ods[i])),
                      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                      b));
            }

            Color trailCol = Color.Lerp(Color.WhiteSmoke, Color.Lerp(Color.WhiteSmoke, Color.Red, 0.85f), bloodAmnt);

            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                Effect shader = ModContent.Request<Effect>("asuw/Effects/Colorize", AssetRequestMode.ImmediateLoad).Value;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue((trailCol).ToVector4());
                shader.Parameters["color1"].SetValue((trailCol * 0.8f).ToVector4());
                shader.Parameters["alpha"].SetValue(Projectile.Opacity * 0.7f);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();

                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                Main.spriteBatch.ExitShaderRegion();
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotOffset, origin, Projectile.scale, effects, 0);
            Main.spriteBatch.Draw(textureB, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity * bloodAmnt, Projectile.rotation + rotOffset, origin, Projectile.scale, effects, 0);
            return false;
        }
    }

    public class SerratedDebuff : ModBuff
    {
        public override string Texture => "asuw/Content/Items/Weapons/Melee/Pyro";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.asuw().Serrated = true;
        }
        //method from shred debuff
        internal static void TickDebuff(NPC target, AsuGlobalNPC agn)
        {
            if (agn.SerratedApplicator >= 0 && agn.SerratedApplicator < Main.maxPlayers && Main.myPlayer == agn.SerratedApplicator)
            {
                Player applicator = Main.player[agn.SerratedApplicator];

                if (applicator.miscCounter % 5 == 0 && agn.SerratedAmnt > 0)
                {
                    target.AddBuff(ModContent.BuffType<SerratedDebuff>(), 6);
                    int finalDamage = (int)(agn.SerratedDMG);
                    Projectile tick = Projectile.NewProjectileDirect(target.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrikeConst>(), finalDamage, 0f, applicator.whoAmI, target.whoAmI);
                    tick.ArmorPenetration = 1000;
                    for (int i = 0; i < 3; i++)
                    {
                        float width = target.width * 0.7f;
                        float height = target.height * 0.8f;
                        Vector2 offset = new Vector2(Main.rand.NextFloat(-width, width), Main.rand.NextFloat(-height, height * 0.4f));
                        Vector2 velo = (((target.Center + offset) - target.Center).normalize() * Main.rand.NextFloat(1.5f, 3.5f)).RotatedByRandom(0.25f);
                        //GeneralParticleHandler.SpawnParticle(new BloodParticle2(target.Center + offset, velo, 20, Main.rand.NextFloat(0.1f, 0.3f), Color.Red * 0.8f));
                    }
                    agn.SerratedAmnt--; ;
                }
            }
        }
    }

}
