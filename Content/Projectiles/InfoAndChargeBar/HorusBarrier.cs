using asuw.Content.Global;
using asuw.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace asuw.Content.Projectiles.InfoAndChargeBar
{
    public class HorusBarrier : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        public ref float timer => ref Projectile.ai[0];
        public ref float BarrierScale => ref Projectile.ai[1];
        public ref float BarrierOP => ref Projectile.ai[2];
        public Color BarrierCol;
        public override string Texture => "asuw/Content/Textures/Extra/HorusBarrier";
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Type] = 6;
            // Prevents jitter when stepping up and down blocks and half blocks
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
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
            Projectile.Center = player.GetDrawCenter();

            if (BarrierOP < 0) BarrierOP = 0;
            if (BarrierOP > 1) BarrierOP = 1;

            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is Horus h)
            {
                if (!player.dead) Projectile.timeLeft = 2;

                if (h.barrierUp > 0)
                { if (BarrierOP < 1) BarrierOP += 0.05f; }
                else
                { if (BarrierOP > 0) BarrierOP -= 0.05f; }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter % 6 == 0)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];

            BarrierScale = MathHelper.Lerp(1f, 0.85f, MathHelper.SmoothStep(0, 1, MathHelper.SmoothStep(0, 1, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f)));

            BarrierCol = Color.Lerp(Color.PaleVioletRed, Color.Crimson, MathHelper.SmoothStep(0, 1, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f));

            if(BarrierOP > 0) player.asuw().HorusBarrierUp = true;
            else player.asuw().HorusBarrierUp = false;

        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override void OnKill(int timeLeft)
        {
            player.asuw().HorusBarrierUp = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D Shine = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/HorusBarrierShine", AssetRequestMode.AsyncLoad).Value;
          
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
        
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(texture, player.MountedCenter - Main.screenPosition, frame, BarrierCol * 0.5f * BarrierOP, 0f, origin, 0.12f * BarrierScale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Shine, player.MountedCenter - Main.screenPosition, frame, BarrierCol * 0.7f * BarrierOP, 0f, origin, 0.12f * BarrierScale, SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();

            return false;
        }
    }
}
