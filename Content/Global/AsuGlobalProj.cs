using asuw.Content.Buffs;
using asuw.Content.Dusts;
using asuw.Content.Global;
using asuw.Content.Items.Weapons;
using asuw.Content.Projectiles;
using asuw.Content.Projectiles.EOH;
using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;


namespace asuw.Content
{

    public partial class AsuGlobalProj : GlobalProjectile
    {
        public bool horusProj;
        public bool horusCharged;
        public bool stopDetonateHooks = false;
        public override bool InstancePerEntity => true;

        public int defExtraUpdates = -1;
        public int HomingTarget = -1;

        List<float> ods = new List<float>();
        List<Vector2> odp = new List<Vector2>();
        List<Vector2> odv = new List<Vector2>();

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = projectile.GetOwner();

            if (projectile.asuw().horusCharged)
            {
                projectile.velocity *= 2f;
            }

        }

        public override bool PreAI(Projectile projectile)
        {
            return true;
        }
       
        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];

            base.AI(projectile);
        }
        public override void PostAI(Projectile projectile)
        {
            if (projectile.asuw().horusCharged)
            {

                ods.Add(projectile.scale);
                odp.Add(projectile.Center + projectile.velocity);
                odv.Add(projectile.velocity);

                if (odp.Count > 20)
                {
                   
                    ods.RemoveAt(0);
                    odp.RemoveAt(0);
                    odv.RemoveAt(0);
                }
                
      
               
                
            }
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];

            if (projectile.asuw().horusCharged) modifiers.SourceDamage *= player.asuw().butterflyHairpin ? 1.8f : 1.5f;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];

            if (projectile.asuw().horusCharged)
            {
                if (projectile.numHits == 0)
                {
                    //boom
                }
                if (target.asuw().horusHooks.Count > 0 && !stopDetonateHooks)
                {
                    stopDetonateHooks = true;
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center + Utils.NextVector2Circular(Main.rand, 15, 15), Vector2.Zero, ModContent.ProjectileType<HorusBoomVis>(), projectile.damage * 10, projectile.knockBack, player.whoAmI);
                    target.asuw().horusHooks.RemoveAt(0);
                }
            }
        }

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (projectile.asuw().horusCharged)
            {
                Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/Trail", AssetRequestMode.ImmediateLoad).Value;

                List<ColoredVertex> ve = new List<ColoredVertex>();

                for (int i = 0; i < odp.Count; i++)
                {
                    Color b = Color.HotPink;
                    ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + Vector2.UnitX.RotatedBy(-MathHelper.PiOver2 + odv[i].ToRotation()) * projectile.Size * ods[i],
                          new Vector3((i) / ((float)odp.Count - 1), 0, 1),
                          b));
                    ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + Vector2.UnitX.RotatedBy(MathHelper.PiOver2 + odv[i].ToRotation()) * projectile.Size * ods[i],
                          new Vector3((i) / ((float)odp.Count - 1), 1, 1),
                          b));
                }

                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    ShaderFunctions.vertexColored(Main.spriteBatch, Color.DeepPink, Color.Crimson, projectile.Opacity);

                    gd.Textures[0] = trail;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);


                    Main.spriteBatch.ExitShaderRegion();
                }
            }

        }
    }
}
