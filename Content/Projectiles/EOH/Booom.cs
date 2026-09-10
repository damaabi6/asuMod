using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles.EOH
{
    public class HorusBoomVis : ModProjectile
    {
        Player player => Projectile.GetOwner();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
        }
        public override string Texture => "asuw/Assets/Blank";

        ref float from => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.timeLeft = 2;
            Projectile.MaxUpdates = 2;
            Projectile.localNPCHitCooldown = -1;
        }

        int frameSpeed = 5;

        public override void OnSpawn(IEntitySource source)
        {
            Texture2D TextureBlack = ModContent.Request<Texture2D>("asuw/Content/Projectiles/EOH/BoomBlack", AssetRequestMode.ImmediateLoad).Value;
            Projectile.timeLeft = (Main.projFrames[Type] * frameSpeed);
            Projectile.scale = from == 1 ? 1.1f : 0.5f;
            Projectile.rotation = from == 1 ? 0 : Main.rand.NextFloat(-0.1f, 0.1f);
            Projectile.Resize((int)((float)TextureBlack.Width * Projectile.scale), (int)((float)TextureBlack.Height * Projectile.scale));
            if(from == 1)
            {
                //for (int i = 1; i < 3; i++) 
                //GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center + (Vector2.UnitY * 100), Vector2.Zero, Color.DarkOrange, "CalamityMod/Particles/DustyCircleHardEdge", new Vector2(1, 0.3f), 0, 0.05f, 0.1f * i, 30));

                
            }
        }
        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Remove(index);
            behindNPCsAndTiles.Remove(index);
            behindProjectiles.Remove(index);
            overPlayers.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = ModContent.Request<Texture2D>("asuw/Content/Projectiles/EOH/BoomSmooth", AssetRequestMode.ImmediateLoad).Value;
            Texture2D TextureGlow = ModContent.Request<Texture2D>("asuw/Content/Projectiles/EOH/BoomGlow", AssetRequestMode.ImmediateLoad).Value;
            Texture2D TextureBlack = ModContent.Request<Texture2D>("asuw/Content/Projectiles/EOH/BoomBlack", AssetRequestMode.ImmediateLoad).Value;
            Texture2D TextureBlackGlow = ModContent.Request<Texture2D>("asuw/Content/Projectiles/EOH/BoomBlackInner", AssetRequestMode.ImmediateLoad).Value;

            Rectangle frame = Texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(TextureGlow, Projectile.Center - Main.screenPosition, frame, Color.Crimson, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();


            if (Projectile.frame == 0)
            {
                Main.spriteBatch.Draw(TextureBlack, Projectile.Center - Main.screenPosition, null, Color.Gold, Projectile.rotation, TextureBlack.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.spriteBatch.Draw(TextureBlackGlow, Projectile.Center - Main.screenPosition, null, Color.Crimson, Projectile.rotation, TextureBlackGlow.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
                Main.spriteBatch.ExitShaderRegion();
            }


            return false;
        }
    }
}
