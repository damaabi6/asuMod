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
    public class Planty : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        public override string Texture => "asuw/Assets/Blank";
        Player player => Projectile.GetOwner();
        bool stop = false;
        float dis = 0;
        float width = 20;
        const float maxLifeTime = 600;
        Vector2 angle;

        List<float> ods = new List<float>();
        List<Vector2> odp = new List<Vector2>();
        List<Vector2> odv = new List<Vector2>();
        public ref float mode => ref Projectile.ai[0];
        ref float time => ref Projectile.ai[1];
        private NPC hookedNPC
        {
            get => Projectile.ai[2] == 0 ? null : Main.npc[(int)Projectile.ai[2] - 1];
            set
            {
                Projectile.ai[2] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.timeLeft = 180;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 2;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (mode == 1)
            {
                Vector2 hookPos = hookedNPC.Center + (angle * dis);
                Projectile.velocity = angle;
                Projectile.Center = hookPos;
                Projectile.rotation = 0;
                if (time < maxLifeTime && hookedNPC.asuw().horusHooks.Contains(Projectile.whoAmI)) Projectile.timeLeft = 2;

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 10)
                {
                    Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
                    Projectile.frameCounter = 0;
                }

                if (odp.Count > 0)
                {
                    ods.RemoveAt(0);
                    odp.RemoveAt(0);
                    odv.RemoveAt(0);
                }

                time++;
            }
            else
            {
                odp.Add(Projectile.Center + Projectile.velocity);
                ods.Add(Projectile.scale);
                odv.Add(Projectile.velocity);

                if (odp.Count > 20)
                {
                    ods.RemoveAt(0);
                    odp.RemoveAt(0);
                    odv.RemoveAt(0);
                }
                Projectile.rotation += (1f * Projectile.direction);
            }
            
           
        }
        public override bool? CanDamage() => mode == 1 ? false : null;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center + (Projectile.velocity.normalize() * width * 2);
            Vector2 end = Projectile.Center + (Projectile.velocity.normalize() * width * -3);
            float collisionPoint = 0f;
            float collisionWidth = width * 2;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, collisionWidth, ref collisionPoint);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits == 0 && !stop)
            {
                hookedNPC = target;
                target.asuw().horusHooks.Add(Projectile.whoAmI);
                mode = 1; 
                dis = Vector2.Distance(target.Center,Projectile.Center);
                angle = Utils.DirectionTo(target.Center,Projectile.Center);
                stop = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (hookedNPC != null) hookedNPC.asuw().horusHooks.Remove(Projectile.whoAmI);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = ModContent.Request<Texture2D>("asuw/Content/Projectiles/EOH/Star", AssetRequestMode.ImmediateLoad).Value;
            Texture2D TextureGlow = ModContent.Request<Texture2D>("asuw/Content/Projectiles/EOH/StarGlow", AssetRequestMode.ImmediateLoad).Value;
            Texture2D TextureAnim = ModContent.Request<Texture2D>("asuw/Content/Projectiles/EOH/StarFlicker", AssetRequestMode.ImmediateLoad).Value;

        
            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/Trail", AssetRequestMode.ImmediateLoad).Value;

            List<ColoredVertex> ve = new List<ColoredVertex>();

            for (int i = 0; i < odp.Count; i++)
            {
                Color b = Color.HotPink;
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + Vector2.UnitX.RotatedBy(-MathHelper.PiOver2 + odv[i].ToRotation()) * width,
                        new Vector3((i) / ((float)odp.Count - 1), 0, 1),
                        b));
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + Vector2.UnitX.RotatedBy(MathHelper.PiOver2 + odv[i].ToRotation()) * width,
                        new Vector3((i) / ((float)odp.Count - 1), 1, 1),
                        b));
            }

            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                Effect shader = ModContent.Request<Effect>("asuw/Effects/ColorizeBloom", AssetRequestMode.ImmediateLoad).Value;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue((Color.Crimson).ToVector4());
                shader.Parameters["color1"].SetValue((Color.DeepPink).ToVector4());
                shader.Parameters["alpha"].SetValue(Projectile.Opacity);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();

                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);


                Main.spriteBatch.ExitShaderRegion();
            }
            
            if(mode == 1)
            {
                Rectangle frame = TextureAnim.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
                Main.spriteBatch.Draw(TextureAnim, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }


            return false;
        }
    }
}
