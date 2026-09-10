using asuw.Content.Global;
using asuw.Content.Projectiles;
using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons.Melee
{
	public class StaffOfHoma : ModItem, ILocalizedModType
    {
        //this is so fun to make
        //TODO add sfx
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public float stamina = 100;
        int currentFrame = 0;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
		{
			Item.damage = 180;
			Item.ArmorPenetration = 5;
            Item.knockBack = 4;
			Item.DamageType = DamageClass.Melee;
            Item.width = 106;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.shootSpeed = 20f;
			Item.shoot = ModContent.ProjectileType<HomaStaff>();
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
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

        public override void UpdateInventory(Player player)
        {
            if (stamina < 100 && player.miscCounter % 10 == 0)
            {
                stamina++;
                if (player.velocity.Y == 0 && stamina < 100) stamina++; 
            }

            if (player.miscCounter % 5 == 0)
                currentFrame = (currentFrame + 1) % 9;
        }

        public override void PostUpdate()
        {
            if (Main.LocalPlayer.miscCounter % 5 == 0)
                currentFrame = (currentFrame + 1) % 9;
        }
        public override bool AltFunctionUse(Player player) => stamina >= (player.asuw().butterflyHairpin ? 25 : 20) ? true : false;
       
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
            {
               Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI,1);
                stamina -= (player.asuw().butterflyHairpin ? 25 : 20);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
                return false;
        }
        public override void HoldItem(Player player)
        {
            if(Main.myPlayer == player.whoAmI)
            {
                if(AsuKeybinds.WeaponSkill.JustPressed && !player.HasBuff(ModContent.BuffType<Pyro>()))
                {
                    player.AddBuff(ModContent.BuffType<Pyro>(), 600);
                    player.statLife -= (int)((float)player.statLife / 2f);
                    if (player.statLife <= 2) player.Hurt(PlayerDeathReason.ByPlayerItem(Item.whoAmI, Item), player.statLifeMax2, 1);
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<ParamitaPapilio>(), Item.damage, Item.knockBack, player.whoAmI);
                }
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplace("(fraction)", Main.LocalPlayer.asuw().butterflyHairpin ? "[c/" + Utils.Hex3(AsuPlayer.SpecialMoveColorBH) + ":1/4]" : "1/5");
            tooltips.FindAndReplace("-Crimson Bouquet", Main.LocalPlayer.asuw().butterflyHairpin ? "[c/" + Utils.Hex3(AsuPlayer.SpecialMoveColorBH) + ":-Crimson Bouquet]" : "-Crimson Bouquet");

            var hotkey = AsuKeybinds.WeaponSkill.TooltipHotkeyString();
            tooltips.FindAndReplace("{0}", hotkey);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Texture2D pyroEnhanced = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/HomaPyro", AssetRequestMode.AsyncLoad).Value;
            Texture2D pyroEnhancedGlow = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/StaffOfHomaGlow", AssetRequestMode.AsyncLoad).Value;
            Texture2D flames = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/flamesHoma", AssetRequestMode.AsyncLoad).Value;

            Rectangle Frames = pyroEnhanced.Frame(verticalFrames: 9, frameY: currentFrame);

            Vector2 drawOrigin1 = texture.Size() / 2f;
            Vector2 drawOrigin2 = Frames.Size() / 2f;

            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<Pyro>()))
            {
                spriteBatch.Draw(pyroEnhanced, position, Frames, Color.White, 0f, drawOrigin2, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(flames, position, Frames, Color.Crimson with { A = 0 }, 0f, drawOrigin2, scale, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(texture, position, null, Color.White, 0f, drawOrigin1, scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Texture2D pyroEnhanced = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/HomaPyro", AssetRequestMode.AsyncLoad).Value;
            Texture2D pyroEnhancedGlow = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/StaffOfHomaGlow", AssetRequestMode.AsyncLoad).Value;
            Texture2D flames = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/flamesHoma", AssetRequestMode.AsyncLoad).Value;

            Rectangle Frames = pyroEnhanced.Frame(verticalFrames: 9, frameY: currentFrame);

            Vector2 drawOrigin1 = texture.Size() / 2f;
            Vector2 drawOrigin2 = Frames.Size() / 2f;
            Vector2 drawposition1 = Item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin1.Y);
            Vector2 drawposition2 = Item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin2.Y);

            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<Pyro>()))
            {
                spriteBatch.UseBlendState(BlendState.Additive);
                spriteBatch.Draw(pyroEnhancedGlow, drawposition1, null, Color.OrangeRed * 0.8f, 0f, drawOrigin1, 1f, SpriteEffects.None, 0);
                spriteBatch.ExitShaderRegion();
                spriteBatch.Draw(pyroEnhanced, drawposition2, Frames, Color.White, 0f, drawOrigin2, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(flames, drawposition2, Frames, Color.Crimson with { A = 0 }, 0f, drawOrigin2, 1f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(texture, drawposition1, null, lightColor, 0f, drawOrigin1, 1f, SpriteEffects.None, 0);

            }

            return false;
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(stamina);
        }

        public override void NetReceive(BinaryReader reader)
        {
           stamina = reader.ReadSingle();
        }
     
    }
    public class Pyro : ModBuff
    {
        internal static float Damage = 20;
        internal static int FramesPerDamageTick = 10;
        public override string Texture => "asuw/Content/Items/Weapons/Melee/Pyro";
      
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.asuw().PyroEnchanted = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.asuw().pyroAffected = true;
        }
        //method from shred debuff
        internal static void TickDebuff(NPC target, AsuGlobalNPC agn)
        {
            if (agn.pyroApplicator >= 0 && agn.pyroApplicator < Main.maxPlayers && Main.myPlayer == agn.pyroApplicator)
            {
                Player applicator = Main.player[agn.pyroApplicator];

                // Only deal damage once every several frames.
                if (applicator.miscCounter % FramesPerDamageTick == 0)
                {
                    float bonusHPDMG = 2f - (float)((double)applicator.statLife / (double)applicator.statLifeMax2);
                    int finalDamage = (int)(Damage * bonusHPDMG);//compensate DR
                    Projectile tick = Projectile.NewProjectileDirect(target.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrikeConst>(), finalDamage, 0f, applicator.whoAmI, target.whoAmI);
                    tick.ArmorPenetration = 1000;
                }
            }
        }
        //called in modifyinterfacelayers
        internal static void drawBloom(NPC target, AsuGlobalNPC agn)
        {
            Texture2D bloom = ModContent.Request<Texture2D>("asuw/Assets/BloodBlossom", AssetRequestMode.AsyncLoad).Value;
            Texture2D bloomInner = ModContent.Request<Texture2D>("asuw/Assets/BloodBlossomInner", AssetRequestMode.AsyncLoad).Value;
            Texture2D bloomGlow = ModContent.Request<Texture2D>("asuw/Assets/BloodBlossomGlow", AssetRequestMode.AsyncLoad).Value;
            if (agn.pyroApplicator >= 0 && agn.pyroApplicator < Main.maxPlayers && Main.myPlayer == agn.pyroApplicator)
            {
                Player applicator = Main.player[agn.pyroApplicator];
                float rotSpeed = agn.bloodBlossomRotSpeed;
                float opacity = 1f - ((float)((double)applicator.statLife / (double)applicator.statLifeMax2) * 0.6f);
                float rot = (MathHelper.TwoPi * agn.bloodBlossomRotVar) * (Main.GlobalTimeWrappedHourly % rotSpeed / rotSpeed);
                float scale = ((float)target.width / 300f) < 0.2f ? 0.2f : ((float)target.width / 300f) > 1.1f ? 1.1f : ((float)target.width / 300f);

                SpriteBatch sb = Main.spriteBatch;
                Effect shader = ModContent.Request<Effect>("asuw/Effects/ColorizeBloom", AssetRequestMode.ImmediateLoad).Value;
                Effect shader2 = ModContent.Request<Effect>("asuw/Effects/Colorize", AssetRequestMode.ImmediateLoad).Value;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader2.Parameters["color2"].SetValue(Color.Lerp(Color.Firebrick, Color.OrangeRed, 0.25f).ToVector4());
                shader2.Parameters["color1"].SetValue((Color.Maroon).ToVector4());
                shader2.Parameters["alpha"].SetValue(opacity);
                shader2.CurrentTechnique.Passes["EffectPass"].Apply();
                sb.Draw(bloomGlow, target.Center - Main.screenPosition, default, Color.White * opacity, rot, bloomGlow.Size() / 2f, scale, SpriteEffects.None, 0);
                sb.Draw(bloomInner, target.Center - Main.screenPosition, default, Color.White * opacity * 0.7f, rot, bloom.Size() / 2f, scale, SpriteEffects.None, 0);
                sb.ExitShaderRegion();
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue(Color.Lerp(Color.Firebrick, Color.OrangeRed, 0.5f).ToVector4());
                shader.Parameters["color1"].SetValue((Color.Maroon).ToVector4());
                shader.Parameters["alpha"].SetValue(opacity);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();
                sb.Draw(bloom, target.Center - Main.screenPosition, default, Color.White * opacity, rot, bloom.Size() / 2f, scale, SpriteEffects.None, 0);
                sb.ExitShaderRegion();
            }
        }
    }

    public class ParamitaPapilio : ModProjectile
    {
        public override string Texture => "asuw/Content/Items/Weapons/Melee/ParamitaPapilio";
        Player player => Projectile.GetOwner();
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
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.friendly = false;
            Projectile.timeLeft = 30;
        }
        ref float time => ref Projectile.ai[1];
        public override void AI()
        {
            Projectile.Center = player.GetDrawCenter();
            float lerp = time / 30f;
            float eased = AsuUtils.ExpoOut(time / 30f);
            Projectile.scale = MathHelper.Lerp(0.1f, 0.5f, eased);
            Projectile.Opacity = 1f - lerp;
            time++;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D Glow = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/ParamitaPapilioGlow", AssetRequestMode.AsyncLoad).Value;
            SpriteBatch sb = Main.spriteBatch;
            Effect shader = ModContent.Request<Effect>("asuw/Effects/ColorizeBloom", AssetRequestMode.ImmediateLoad).Value;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            shader.Parameters["color2"].SetValue((Color.OrangeRed).ToVector4());
            shader.Parameters["color1"].SetValue((Color.Maroon).ToVector4());
            shader.Parameters["alpha"].SetValue(Projectile.Opacity);
            shader.CurrentTechnique.Passes["EffectPass"].Apply();
            sb.Draw(Glow, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            sb.Draw(texture, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            sb.ExitShaderRegion();
            return false;
        }
    }

    public class HomaStaff : ModProjectile
	{
        public override string Texture => "asuw/Content/Items/Weapons/Melee/StaffOfHoma";

        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 9;
            // Prevents jitter when stepping up and down blocks and half blocks
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.timeLeft = 30;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 40;
            Projectile.scale = 1.3f;
        }
        Player player => Projectile.GetOwner();
        ref float chargeAtt => ref Projectile.ai[0]; 
        ref float time => ref Projectile.ai[1];
        ref float attType => ref Projectile.ai[2];

        //trail parameters
        List<float> odr = new List<float>();
        List<float> ods = new List<float>();
        List<Vector2> odp = new List<Vector2>();
        Color color1;
        Color color2;
        List<Vector2> tip = new List<Vector2>();

        float playerWingTime = 0;
        bool pyro = false;
        bool stopPyroBoom = false;
        bool yesDmg = false;
        Vector2 spearScale = new Vector2(0,0);
        Vector2 aim;

        //chargeatt
        float thrustDuration = 0;
        public override void OnSpawn(IEntitySource source)
        {
            aim = player.asuw().mouseNormalFromPlayer;
            thrustDuration = 15 * Projectile.MaxUpdates;
            if(chargeAtt != 0) playerWingTime = player.wingTime;
        }
		public override void AI()
		{
            if(player.dead) Projectile.Kill();
            if(player.HasBuff(ModContent.BuffType<Pyro>())) pyro = true;
            else pyro = false;
            if (pyro)
            {
                color1 = Color.Lerp(Color.Maroon, Color.Orange, 0.5f);
                color2 = Color.OrangeRed;

                Projectile.frameCounter++;
                if (Projectile.frameCounter % 150 == 0)
                    Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
            else
            {
                color1 = new Color(255, 224, 110);
                color2 = Color.White;
                Projectile.frameCounter = 0;
            }
            player.heldProj = Projectile.whoAmI;
            player.itemAnimation = 10;
            player.itemTime = 10;
            Projectile.timeLeft = 2;
            float att0n2dur = 20f * Projectile.MaxUpdates;
            float att1dur = 12f * Projectile.MaxUpdates;
            float att3dur = 18f * Projectile.MaxUpdates;
            float att4dur = 40f * Projectile.MaxUpdates;
            float att5dur = 30f * Projectile.MaxUpdates;
            if (chargeAtt == 0)
            {
                if(player.HeldItem.ModItem != null && player.HeldItem.ModItem is StaffOfHoma homa && homa.stamina >= (player.asuw().butterflyHairpin ? 25 : 20) && player.asuw().mouseRight && !player.controlUseItem) //instantly charge att when possible
                {
                    player.itemAnimation = 0;
                    player.itemTime = 0;
                    Projectile.Kill();
                }
                if(attType != 5)//increase size when flamin
                {
                    Projectile.scale = pyro ? 1.35f : 1f;
                }
                if (attType == 0)
                {
                    float lerp = MathHelper.Lerp(1, -1f, AsuUtils.QuintOut(time / att0n2dur));
                    float rot = MathHelper.ToRadians(-160f * Projectile.direction) * lerp;
                    Projectile.velocity = aim;
                    Projectile.rotation = Projectile.velocity.ToRotation() + rot;
                    player.SetHandRotFront(Projectile.rotation);
                    Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
                    player.ChangeDir(Projectile.direction);
                    addTrail(att0n2dur);
                    flames(-1, att0n2dur);
                    getYesDmg(att0n2dur);
                    //if (time == 10) SoundEngine.PlaySound(CommonSoundstyle.lightWoosh with { Volume = 1.1f, PitchVariance = 0.15f }, Projectile.Center);
                    if (time < att0n2dur * 0.8f) time++;
                    else
                    {
                        cleartrail();
                        if (!attacking()) Projectile.Kill();
                        Projectile.ResetLocalNPCHitImmunity();
                        Projectile.numHits = 0;
                        attType = 1;
                        aim = player.asuw().mouseNormalFromPlayer;
                        time = 0;
                    }
                }
                else if (attType == 1)
                {
                    float lerp = MathHelper.Lerp(0, 1f, AsuUtils.QuintOut(time / att1dur));
                    float lerp2 = MathHelper.Lerp(1f, 0f, AsuUtils.QuintOut(time / att1dur));
                    float offset = 40f * lerp;
                    Projectile.velocity = aim;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    player.SetHandRotFront(Projectile.rotation);
                    Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm) + (aim * offset);
                    player.ChangeDir(Projectile.direction);
                    float SpearX = 1.1f * lerp2;
                    float SpearY = 0.55f * lerp2;
                    spearScale = new Vector2(SpearX, SpearY);
                    flames(1, att1dur);
                    getYesDmg(att1dur);
                    //if (time == 7) SoundEngine.PlaySound(CommonSoundstyle.quickWoosh with { Pitch = -0.2f, PitchVariance = 0.1f }, Projectile.Center);
                    if (time < att1dur) time++;
                    else
                    {
                        cleartrail();
                        spearScale = new Vector2(0, 0);
                        if (!attacking()) Projectile.Kill();
                        Projectile.ResetLocalNPCHitImmunity();
                        Projectile.numHits = 0;
                        attType = 2;
                        aim = player.asuw().mouseNormalFromPlayer;
                        time = 0;
                    }
                }
                else if (attType == 2)
                {
                    float lerp = MathHelper.Lerp(1, -1.5f, AsuUtils.QuintOut(time / att0n2dur));
                    float rot = MathHelper.ToRadians(140f * Projectile.direction) * lerp;
                    Projectile.velocity = aim;
                    Projectile.rotation = Projectile.velocity.ToRotation() + rot;
                    player.SetHandRotFront(Projectile.rotation);
                    Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
                    player.ChangeDir(Projectile.direction);
                    addTrail(att0n2dur);
                    flames(1, att0n2dur);
                    getYesDmg(att0n2dur);
                    //if (time == 13) SoundEngine.PlaySound(CommonSoundstyle.lightWoosh with { Volume = 1.1f, PitchVariance = 0.15f }, Projectile.Center);
                    if (time < (int)(att0n2dur * 0.8f)) time++;
                    else
                    {
                        cleartrail();
                        if (!attacking()) Projectile.Kill();
                        Projectile.ResetLocalNPCHitImmunity();
                        Projectile.numHits = 0;
                        attType = 3;
                        aim = player.asuw().mouseNormalFromPlayer;
                        time = 0;
                    }
                }
                else if (attType == 3)
                {
                    float lerp = MathHelper.Lerp(0, 1f, AsuUtils.SineOut(time / att3dur));
                    float lerp2 = MathHelper.Lerp(0, 1f, AsuUtils.CubicIn(time / att3dur));
                    float rot = (((MathHelper.TwoPi) + MathHelper.ToRadians(40)) * Projectile.direction) * lerp;
                    float rothand = ((MathHelper.ToRadians(110)) * Projectile.direction) * lerp2;
                    float handOffset = Projectile.direction > 0 ? 0 : float.Pi;
                    Projectile.velocity = aim;
                    Projectile.rotation = Projectile.velocity.ToRotation() + rot;
                    player.SetHandRotFront(MathHelper.ToRadians(-80 * Projectile.direction) + handOffset + rothand);
                    Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
                    player.ChangeDir(Projectile.direction);
                    addTrail(att3dur);
                    flames(-1, att3dur);
                    getYesDmg(att3dur);
                    if (time < att3dur) time++;
                    else
                    {
                        cleartrail();
                        if (!attacking()) Projectile.Kill();
                        Projectile.ResetLocalNPCHitImmunity();
                        Projectile.numHits = 0;
                        attType = 4;
                        aim = player.asuw().mouseNormalFromPlayer;
                        time = 0;
                    }
                }
                else if (attType == 4)
                {
                    int Half = (int)(att4dur * 0.5f);
                    if (time == Half) Projectile.ResetLocalNPCHitImmunity();
                    float lerp = MathHelper.Lerp(0f, 1f, AsuUtils.CubicInOut(time / att4dur));
                    float rot = (((MathHelper.TwoPi * 2) + MathHelper.ToRadians(120)) * Projectile.direction) * lerp;
                    Projectile.velocity = aim;
                    Projectile.rotation = Projectile.velocity.ToRotation() + (MathHelper.ToRadians(40 * Projectile.direction)) + rot;
                    player.SetHandRotFront(Projectile.rotation);
                    Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
                    player.ChangeDir(Projectile.direction);
                    addTrail(att4dur);
                    flames(-1, att4dur);
                    getYesDmg(att4dur);
                    if (time < att4dur) time++;
                    else
                    {
                        cleartrail();
                        if (!attacking()) Projectile.Kill();
                        Projectile.ResetLocalNPCHitImmunity();
                        Projectile.numHits = 0;
                        attType = 5;
                        aim = player.asuw().mouseNormalFromPlayer;
                        time = 0;
                    }
                }
                else if (attType == 5)
                {
                    float lerp = MathHelper.Lerp(1f, -1f, AsuUtils.ExpoInOut(time / att5dur));
                    float lerpScale = AsuUtils.CubicIn(lerpLoopBack(att5dur));
                    float rot = ((MathHelper.ToRadians(160)) * Projectile.direction) * lerp;
                    Projectile.scale = pyro ? 1.35f + (0.7f * lerpScale) : 1f + (0.5f * lerpScale);
                    Projectile.velocity = aim;
                    Projectile.rotation = Projectile.velocity.ToRotation() + rot;
                    player.SetHandRotFront(Projectile.rotation);
                    Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);
                    player.ChangeDir(Projectile.direction);
                    addTrail(att5dur);
                    flames(1, att5dur);
                    getYesDmg(att5dur);
                    if (time < att5dur) time++;
                    else
                    {
                        cleartrail();
                        Projectile.Kill();
                        Projectile.ResetLocalNPCHitImmunity();
                        Projectile.numHits = 0;
                        attType = 6;
                        aim = player.asuw().mouseNormalFromPlayer;
                        time = 0;
                    }
                }
            }
            else
            {
                Projectile.velocity = aim;
                Projectile.rotation = Projectile.velocity.ToRotation();
                player.SetHandRotFront(Projectile.rotation);
                player.ChangeDir(Projectile.direction);
                Projectile.Center = player.GetFrontHandPositionImproved(player.compositeFrontArm);

                if (time == 1)
                    player.velocity = Projectile.velocity * 50;
                
                if(time > (int)(thrustDuration * 0.5f))
                    player.velocity *= 0.995f;

                player.immune = true;
                player.immuneNoBlink = true;
                player.immuneTime = 30;
                for (int k = 0; k < player.hurtCooldowns.Length; k++)
                    player.hurtCooldowns[k] = player.immuneTime;
                yesDmg = true;
                player.itemTime = 10;
                player.itemAnimation = 10;
                Projectile.timeLeft = 2;
                player.asuw().LungingDown = true;
                player.mount?.Dismount(player);
                player.wingTime = 0; //disable flight
                player.blockExtraJumps = true; //disable extra jumps
                player.StopExtraJumpInProgress(); //disable extra jumps
                player.RemoveAllGrapplingHooks(); //disable hooks
                float lerp = MathHelper.Lerp(1f,0,AsuUtils.SineOut(time / thrustDuration));
                float SpearX = 1.1f * lerp;
                float SpearY = 0.55f * lerp;
                spearScale = new Vector2(SpearX, SpearY);

                Texture2D texture = TextureAssets.Projectile[Type].Value;
                float rot = Main.rand.NextFloat(-0.05f, 0.05f);
                float scale = Main.rand.NextFloat(0.2f, 0.5f);
                Vector2 Pos = Projectile.Center + player.velocity + Projectile.rotation.ToRotationVector2() * texture.Size() * AsuUtils.QuintOut(time / thrustDuration);
                if (time < thrustDuration * 0.7f)
                {
                    if (pyro)
                    {
                        //if (time % 30 == 0)
                        //    for (int i = -1; i < 2; i += 2)
                        //    {
                        //        Vector2 pos2 = Pos + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i) * 10;
                        //        Vector2 vel3 = Projectile.velocity.RotatedBy(-0.1f * i) * -12f;
                                //GeneralParticleHandler.SpawnParticle(new SmallSmokeParticle(Pos, vel3.RotatedByRandom(1f), Main.rand.NextBool() ? Color.OrangeRed : Color.Black, Color.Lerp(Color.Black, Color.OrangeRed, 0.85f), Main.rand.NextFloat(0.9f, 1.4f), 40));
                                //GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Pos, (vel3).RotatedByRandom(0.2f), (Main.rand.NextBool() ? Color.OrangeRed : Color.Lerp(Color.Firebrick, Color.Orange, 0.5f)), 15, scale, 2f, rot));
                                //GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Pos, (vel3).RotatedByRandom(0.2f), (Main.rand.NextBool() ? Color.Lerp(Color.Maroon, Color.Black, 0.5f) : Color.Lerp(Color.Firebrick, Color.Black, 0.5f)), 15, scale / 2f, 2f, rot));
                                //for (int j = 0; j < 3; j++)
                                //    GeneralParticleHandler.SpawnParticle(new LineParticle(Pos, vel3.RotatedByRandom(0.2f), false, 15, Main.rand.NextFloat(0.3f, 0.4f), Color.OrangeRed));

                                //GeneralParticleHandler.SpawnParticle(new CustomSpark(pos2, vel3, "CalamityMod/Particles/BloomCircle", false, 10, 0.3f, Color.OrangeRed, new Vector2(0.3f, 3f), shrinkSpeed: 0.2f));
                            //}
                    }
                    else
                    {
                        //if (time % 30 == 0)
                        //    for (int i = -1; i < 2; i += 2)
                        //    {
                        //        Vector2 pos2 = Pos + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i) * 10;
                        //        Vector2 vel3 = Projectile.velocity.RotatedBy(-0.1f * i) * -12f;
                        //        for (int j = 0; j < 2; j++)
                        //            GeneralParticleHandler.SpawnParticle(new LineParticle(Pos, vel3.RotatedByRandom(0.2f), false, 15, Main.rand.NextFloat(0.3f, 0.4f), Color.Gold));

                        //        GeneralParticleHandler.SpawnParticle(new CustomSpark(pos2, vel3, "CalamityMod/Particles/BloomCircle", false, 10, 0.3f, Color.Gold, new Vector2(0.3f, 3f), shrinkSpeed: 0.2f));
                        //    }
                    }
                }

                if (time < thrustDuration) time++;
                else Projectile.Kill();
            }
            if(time == Projectile.MaxUpdates || time == 0)Projectile.ForceNetUpdate();
        }
        public override void OnKill(int timeLeft)
        {
            if (chargeAtt != 0)
            {
                player.asuw().LungingDown = false;
                if (!stopPyroBoom) player.wingTime = playerWingTime;
                else player.wingTime = player.wingTimeMax;
                player.blockExtraJumps = false;
            }
        }
        bool attacking() => player.controlUseItem;
        float lerpLoopBack(float dur)
        {
            float t = time / (int)(dur / 2f);           
            float v = t <= 1f ? t : 2f - t;
            return v;
        }

        void cleartrail()
        {
            odp.Clear();
            ods.Clear();
            odr.Clear();
            tip.Clear();
        }
        void addTrail(float dur)
        {
           
            if (time > (int)(dur * 0.1f))
            {
                odp.Add(Projectile.Center);
                odr.Add(Projectile.rotation);
                ods.Add(Projectile.scale);
                tip.Add((Projectile.rotation.ToRotationVector2() * (TextureAssets.Projectile[Type].Value.Height / 1.32f) * Projectile.scale));
                if (odp.Count > 200)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                    tip.RemoveAt(0);
                }
            }
            else
            {
                if (odp.Count > 0)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                    tip.RemoveAt(0);
                }
            }
        }
        void flames(float dir, float dur)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 muzzlePos = Projectile.Center + player.velocity + Projectile.rotation.ToRotationVector2() * texture.Size() * Main.rand.NextFloat(0.67f, 0.82f) * Projectile.scale;
            Vector2 vel = Projectile.rotation.ToRotationVector2().RotatedBy(80 * Projectile.direction * dir) * Main.rand.NextFloat(-8f, 10f);
            Vector2 vel2 = Projectile.rotation.ToRotationVector2().RotatedBy(-90 * Projectile.direction * dir) * Main.rand.NextFloat(4f, 7f);
            float rot = Main.rand.NextFloat(-0.05f, 0.05f);
            float scale = Main.rand.NextFloat(0.2f, 0.5f);

            bool spawnRange;
            if (attType == 0 || attType == 1 || attType == 2)
            {
                if (time < (int)(dur * 0.4f)) spawnRange = true;
                else spawnRange = false;
            }
            else if (attType == 3)
            {
                if (time < (int)(dur * 0.9f)) spawnRange = true;
                else spawnRange = false;
            }
            else if (attType == 4)
            {
                if (time > (int)(dur * 0.2f) && time < (int)(dur * 0.8f)) spawnRange = true;
                else spawnRange = false;
            }
            else
            {
                if (time > (int)(dur * 0.15f) && time < (int)(dur * 0.75f)) spawnRange = true;
                else spawnRange = false;
            }

            if (spawnRange)
            {
                if (attType != 1)
                {
                    //if (pyro)
                    //{
                    //    if (time % 8 == 0)
                    //    {
                    //        GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(muzzlePos, (vel).RotatedByRandom(0.4f), (Main.rand.NextBool() ? Color.OrangeRed : Color.Lerp(Color.Firebrick, Color.Orange, 0.5f)), 15, scale, 2f, rot));
                    //        GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(muzzlePos, (vel).RotatedByRandom(0.4f), (Main.rand.NextBool() ? Color.Lerp(Color.Maroon, Color.Black, 0.5f) : Color.Lerp(Color.Firebrick, Color.Black, 0.5f)), 15, scale / 2f, 2f, rot));
                    //    }
                    //    if (time % 5 == 0)
                    //        GeneralParticleHandler.SpawnParticle(new LineParticle(muzzlePos, vel2.RotatedByRandom(0.2f), false, 15, Main.rand.NextFloat(0.3f, 0.4f), Color.OrangeRed));
                    //}
                    //else
                    //{
                    //    if (time % 10 == 0)
                    //        GeneralParticleHandler.SpawnParticle(new LineParticle(muzzlePos, vel2.RotatedByRandom(0.2f), false, 15, Main.rand.NextFloat(0.3f, 0.4f), Color.Gold));
                    //}
                }
                else
                {
                    Vector2 Pos = Projectile.Center + player.velocity + Projectile.rotation.ToRotationVector2() * texture.Size() * (1.8f * AsuUtils.QuintOut(time / dur));
                    if (pyro)
                    {
                        //if (time % 20 == 0)
                        //    for (int i = -1; i < 2; i += 2)
                        //    {
                        //        Vector2 vel3 = Projectile.velocity.RotatedBy(-0.1f * i) * -12f;
                        //        GeneralParticleHandler.SpawnParticle(new SmallSmokeParticle(Pos, vel3.RotatedByRandom(1f), Main.rand.NextBool() ? Color.OrangeRed : Color.Black, Color.Lerp(Color.Black, Color.OrangeRed, 0.85f), Main.rand.NextFloat(0.9f, 1.4f), 30));
                        //        GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Pos, (vel3).RotatedByRandom(0.2f), (Main.rand.NextBool() ? Color.OrangeRed : Color.Lerp(Color.Firebrick, Color.Orange, 0.5f)), 20, scale, 2f, rot));
                        //        GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Pos, (vel3).RotatedByRandom(0.2f), (Main.rand.NextBool() ? Color.Lerp(Color.Maroon, Color.Black, 0.5f) : Color.Lerp(Color.Firebrick, Color.Black, 0.5f)), 20, scale / 2f, 2f, rot));
                        //        for (int j = 0; j < 3; j++)
                        //            GeneralParticleHandler.SpawnParticle(new LineParticle(Pos, vel3.RotatedByRandom(0.2f), false, 15, Main.rand.NextFloat(0.3f, 0.4f), Color.OrangeRed));
                        //    }
                    }
                    else
                    {
                        //if (time % 30 == 0)
                        //    for (int i = -1; i < 2; i += 2)
                        //    {
                        //        Vector2 vel3 = Projectile.velocity.RotatedBy(-0.1f * i) * -12f;
                        //        for (int j = 0; j < 2; j++)
                        //            GeneralParticleHandler.SpawnParticle(new LineParticle(Pos, vel3.RotatedByRandom(0.2f), false, 15, Main.rand.NextFloat(0.3f, 0.4f), Color.Gold));
                        //    }
                    }
                }
            }
        }
        void getYesDmg(float dur)
        {
            if (attType == 0 || attType == 2)
            {
                if (time < (int)(dur * 0.3f)) yesDmg = true;
                else yesDmg = false;
            }
            else if (attType == 1)
            {
                if (time < (int)(dur * 0.1f)) yesDmg = true;
                else yesDmg = false;
            }
            else if (attType == 3)
            {
                if (time > (int)(dur * 0.5f) && time < (int)(dur * 0.9f)) yesDmg = true;
                else yesDmg = false;
            }
            else if (attType == 4)
            {
                if (time > (int)(dur * 0.3f) && time < (int)(dur * 0.7f)) yesDmg = true;
                else yesDmg = false;
            }
            else
            {
                if (time > (int)(dur * 0.2f) && time < (int)(dur * 0.7f)) yesDmg = true;
                else yesDmg = false;
            }
        }
        public override bool? CanDamage() => yesDmg;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * texture.Size() * Projectile.scale * (attType == 1 || chargeAtt != 0 ? 1.7f : 1f);
            float collisionPoint = 0f;
            float collisionWidth = attType == 1 || chargeAtt != 0 ? 90 : 50;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (chargeAtt != 0 && player.asuw().butterflyHairpin)
                modifiers.SourceDamage += 0.25f;

            float dmgBonus = 0;
            if (attType == 5) dmgBonus += 0.2f;//+20% (final slash)
            if (pyro) dmgBonus += 0.4f;//+40% (pyro enhanched)
            if (chargeAtt != 0) dmgBonus += 0.3f; //+30% (charge att)
            dmgBonus += (0.5f - (((float)player.statLife / (float)player.statLifeMax2) * 0.5f));// 0% - 50% (higher dmg the lower the player's hp)
            modifiers.FinalDamage += dmgBonus;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int pyroType = ModContent.BuffType<Pyro>();
            if (pyro)
            {
                if (!target.HasBuff(pyroType)) //set random rot
                {
                    target.asuw().bloodBlossomRotVar = Main.rand.NextBool() ? -1 : 1;
                    target.asuw().bloodBlossomRotSpeed = Main.rand.Next(2, 5);
                }
                if (chargeAtt != 0)
                {
                    if (!stopPyroBoom && target.HasBuff(pyroType))// trigger boom
                    {
                        Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrikeConst>(), (int)((float)damageDone * 1.5f), 8f, player.whoAmI, target.whoAmI);
                        float scale = ((float)target.width / 300f) < 0.2f ? 0.2f : ((float)target.width / 300f) > 1.1f ? 1.1f : ((float)target.width / 300f);
                        //for (int i = 0; i < 360; i += 60)
                        //{
                        //    GeneralParticleHandler.SpawnParticle(new SquishyLightParticle(target.Center, (Vector2.UnitY * (scale * 15)).RotatedBy(MathHelper.ToRadians(i)), scale * 1.5f, Color.OrangeRed, 20));
                        //    if (i <= 120)
                        //        GeneralParticleHandler.SpawnParticle(new CustomPulse(target.Center, Vector2.Zero, i == 0 ? Color.BurlyWood : Color.OrangeRed, "CalamityMod/Particles/SmokeExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), i == 0 ? 0.07f : 0.1f, i == 0 ? scale * 0.7f : scale, 30));
                        //}
                        if (AsuUtils.sync())
                            target.DelBuff(target.FindBuffIndex(pyroType));
                        stopPyroBoom = true; //stop triggering da boom
                       
                    }
                    else
                        target.AddBuff(pyroType, 300); //only add buff if boom hasnt triggered
                }
                else
                    target.AddBuff(pyroType, 300);// normal att always add buff

                int healAmnt;
                if (chargeAtt == 0)
                    healAmnt = Main.rand.Next(3,7) - (Projectile.numHits * 2); //random heal
                else
                    healAmnt = (int)(((float)(player.statLifeMax2 - player.statLife) / 5f) * (Projectile.numHits == 0 ? 1 : 0) * (player.asuw().butterflyHairpin ? 1.25f : 1f)); //healing get more effective the lower the user HP is
                if (healAmnt > 0) player.HealPlayer(healAmnt);
                
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D pyroEnhanced = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/HomaPyro", AssetRequestMode.AsyncLoad).Value;

            Texture2D pyroEnhancedGlow = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/StaffOfHomaGlow", AssetRequestMode.AsyncLoad).Value;
            Texture2D flames = ModContent.Request<Texture2D>("asuw/Content/Items/Weapons/Melee/flamesHoma", AssetRequestMode.AsyncLoad).Value;

            Rectangle pyroFrame = pyroEnhanced.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 originPyro;
            Texture2D spear = ModContent.Request<Texture2D>("asuw/Assets/SpearGlow", AssetRequestMode.AsyncLoad).Value;
            Vector2 spearOrigin = new Vector2(0,spear.Height / 2f);

            Vector2 spearPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * texture.Size() * 0.5f * Projectile.scale;

            if (Projectile.spriteDirection > 0)// why is this here, the sprite is symmetrical
            {
                origin = new Vector2(texture.Width * 0.35f, texture.Height * 0.65f);
                originPyro = new Vector2(pyroFrame.Width * 0.35f, pyroFrame.Height * 0.65f);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(texture.Width * 0.65f, texture.Height * 0.35f);
                originPyro = new Vector2(pyroFrame.Width * 0.65f, pyroFrame.Height * 0.35f);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/BoxySmear2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D trail2 = ModContent.Request<Texture2D>("asuw/Assets/GradientTop", AssetRequestMode.ImmediateLoad).Value;
           
            List<ColoredVertex> ve = new List<ColoredVertex>();

            for (int i = 0; i < odr.Count; i++)
            {
                Color b = new Color(252, 255, 199);
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (odr[i].ToRotationVector2() * (origin.Length() * 1.3f * ods[i])),
                      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                      b));
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (odr[i].ToRotationVector2() * ((origin.Length() / 2f)  * ods[i])),
                      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                      b));
            }

            List<Vector2> poss = new List<Vector2>();
            for (int i = 0; i < tip.Count; i++)
            {
                poss.Add(Projectile.Center + tip[i]);
            }

            if(pyro)
            {

                GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Trails/ScarletDevilStreak");
                GameShaders.Misc["asuw:TrailBackward"].Apply();
                PrimitiveRenderer.RenderTrail(poss, new PrimitiveSettings(TrailWidth, TrailColor, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 180);
                GameShaders.Misc["asuw:TrailBackward"].SetShaderTexture("asuw/Assets/Trails/SylvestaffStreak");
                PrimitiveRenderer.RenderTrail(poss, new PrimitiveSettings(TrailWidth2, TrailColor2, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["asuw:TrailBackward"]), 180);
                Main.spriteBatch.ExitShaderRegion();

                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.spriteBatch.Draw(pyroEnhancedGlow, Projectile.Center - Main.screenPosition, default, Color.OrangeRed * 0.7f * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);
                Main.spriteBatch.ExitShaderRegion();
                Main.spriteBatch.Draw(pyroEnhanced, Projectile.Center - Main.screenPosition, pyroFrame, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, originPyro, Projectile.scale, effects, 0);
                Main.spriteBatch.Draw(flames, Projectile.Center - Main.screenPosition, pyroFrame, Color.Firebrick with { A = 0 } * Projectile.Opacity, Projectile.rotation + rotationOffset, originPyro, Projectile.scale, effects, 0);

            }
            else
            {
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;
                    ShaderFunctions.vertexColorBloom(Main.spriteBatch, color1, color2, Projectile.Opacity);

                    gd.Textures[0] = trail;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                    Main.spriteBatch.ExitShaderRegion();
                }

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);
            }

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(spear, spearPos - Main.screenPosition, default, color1 * Projectile.Opacity, Projectile.rotation, spearOrigin, Projectile.scale * spearScale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spear, spearPos - Main.screenPosition, default, Color.Khaki * Projectile.Opacity, Projectile.rotation, spearOrigin, Projectile.scale * spearScale * 0.65f, SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();

            return false;
        }

        public Color TrailColor(float completionRatio, Vector2 vertex)
        {
            Color result = Color.Lerp(Color.DarkRed, Color.Orange, completionRatio);
            return Color.Lerp(result, Color.White, 0.2f) * Projectile.Opacity * completionRatio;
        }
        public Color TrailColor2(float completionRatio, Vector2 vertex)
        {
            Color result = Color.Lerp(Color.Maroon, Color.OrangeRed, completionRatio);
            return result * Projectile.Opacity * completionRatio;
        }
        public float TrailWidth(float completionRatio, Vector2 vertex)
        {
            float t = completionRatio * 2f;
            float v = t <= 1f ? t : 2f - t;
            float v2 = AsuUtils.QuadOut(v);
            return MathHelper.Lerp(0, TextureAssets.Projectile[Type].Value.Height / 2f * Projectile.scale, v2);
        }
        public float TrailWidth2(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, TextureAssets.Projectile[Type].Value.Height / 2f * Projectile.scale, AsuUtils.QuadOut(completionRatio));
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.spriteDirection);
            writer.WriteVector2(aim);
            writer.Write(pyro);
            writer.Write(thrustDuration);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadInt32();
            aim = reader.ReadVector2();
            pyro = reader.ReadBoolean();
            thrustDuration = reader.ReadSingle();
        }


    }

}
