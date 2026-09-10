using asuw.Content.Dusts;
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
namespace asuw.Content.Items.Ammos
{
    public class TracerBullet : ModItem
    {
        //this only exists because i want a homing pre HM bullet lol
        //TODO better item sprite
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 20;
            Item.damage = 4;
            Item.DamageType = DamageClass.Ranged;
            Item.consumable = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(copper: 8);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<TracerBulletProj>();
            Item.shootSpeed = 0.3f;
            Item.ammo = AmmoID.Bullet;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe(333).
                AddIngredient(ItemID.MusketBall, 333).
                AddIngredient(ItemID.Ectoplasm).
                AddTile(TileID.Hellforge).
                Register();
        }
    }

    public class TracerBulletProj : ModProjectile
    {
        Player player => Projectile.GetOwner();
        public override string Texture => "asuw/Assets/Blank";

        const float maxSpeed = 8;
        const float minSpeed = 3;
        bool slowDown = false;
        bool exploded = false;

        readonly Color[] colorSelect = [Color.Red, Color.DodgerBlue, Color.Lime, Color.DarkOrange];
        Color traceColor;

        float attRateModifier = 0;
        List<int> modeTwoProjs = new List<int>();
        List<int> modeThreeProjs = new List<int>();
        int modeTwoProjsCount => modeTwoProjs.Count;
        int modeThreeProjsCount => modeThreeProjs.Count;


        List<float> odr = new List<float>();//unused currently
        List<float> ods = new List<float>();
        List<Vector2> odp = new List<Vector2>();
        List<Vector2> odv = new List<Vector2>();

        ref float hit => ref Projectile.ai[1]; //same as colors
        ref float mode => ref Projectile.ai[1]; //same as colors
        ref float time => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            AsuUtils.FriendlySetDefaults(Projectile, DamageClass.Ranged, true, -1);
            Projectile.width = Projectile.height = 14;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 10;
            Projectile.timeLeft = 480;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(slowDown);
            writer.Write(exploded);
            writer.Write(traceColor.R);
            writer.Write(traceColor.G);
            writer.Write(traceColor.B);
            writer.Write(traceColor.A);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            slowDown = reader.ReadBoolean();
            exploded = reader.ReadBoolean();
            traceColor.R = reader.ReadByte();
            traceColor.G = reader.ReadByte();
            traceColor.B = reader.ReadByte();
            traceColor.A = reader.ReadByte();
        }
        public override void OnSpawn(IEntitySource source)
        {
            mode = player.asuw().tracerBulletColor;
            traceColor = colorSelect[(int)mode];
            if (Projectile.velocity.Length() > maxSpeed)
                Projectile.velocity = Projectile.velocity.normalize() * maxSpeed;
            attRateModifier = (float)player.itemTimeMax / 5f;
            if (player.asuw().tracerBulletColor < 3) player.asuw().tracerBulletColor++;
            else player.asuw().tracerBulletColor = 0;

            countDuplicates();
        }
        public override bool? CanDamage()
        {
            if (Projectile.numHits == 0) return null;

            return mode switch
            {
                3 => null,
                1 => Projectile.numHits >= 3 ? (bool?)false : null,
                _ => false
            };
        }
        public override bool PreAI()
        {
            if (time == Projectile.MaxUpdates) Projectile.ForceNetUpdate();
            return true;
        }
        public override void PostAI()
        {
            countDuplicates();

            if (mode == 0)
            {
                AsuUtils.HomeInOnNPC(Projectile, false, 150f * (1 + attRateModifier / 10), Projectile.velocity.Length(), 17f);
                if (Projectile.velocity.Length() < minSpeed)
                {
                    float veldelta = Projectile.velocity.Length().AbsDelta(minSpeed);
                    Projectile.velocity *= veldelta;
                }
            }
            if (slowDown)
            {
                if (Projectile.scale > 0.05f) Projectile.timeLeft = 2;
                Projectile.velocity *= 0.985f;
                Projectile.Opacity *= 0.992f;
                Projectile.scale *= 0.992f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, traceColor.ToVector3() * Projectile.Opacity);
            odp.Add(Projectile.Center + Projectile.velocity * 4);
            odr.Add(Projectile.rotation);
            ods.Add(Projectile.scale);
            odv.Add(Projectile.velocity);
            if(odp.Count > 50)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
                ods.RemoveAt(0);
                odv.RemoveAt(0);
            }

            if (Projectile.numHits == 0 && time > Projectile.MaxUpdates && time % 3 == 0 && Projectile.velocity.Length() > minSpeed)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity + Utils.NextVector2Circular(Main.rand, Projectile.width / 2f, Projectile.height / 2f), ModContent.DustType<SharpSparkDust>(), (Projectile.velocity * -0.4f).RotatedByRandom(0.1f), 0, traceColor, Main.rand.NextFloat(0.4f, 0.55f) * Projectile.scale);
                dust.noGravity = true;
            }

            time++;
        }
        private void countDuplicates()
        {
            foreach (var Proj in Main.ActiveProjectiles)
            {
                float timeDelta = Proj.timeLeft.AbsDelta(Projectile.timeLeft);
                if (Proj.type == Projectile.type &&
                    Proj.whoAmI != Projectile.whoAmI &&
                    Proj.owner == player.whoAmI &&
                    timeDelta < Projectile.MaxUpdates)
                {
                    if (Proj.ai[1] == 2 &&
                        !modeTwoProjs.Contains(Proj.whoAmI))
                            modeTwoProjs.Add(Proj.whoAmI);

                    if (Proj.ai[1] == 3 &&
                        !modeThreeProjs.Contains(Proj.whoAmI))
                        modeThreeProjs.Add(Proj.whoAmI);
                }
            }
        }
        private void explode()
        {
            exploded = true;
            float modifier = MathF.Max(2, MathF.Min(5, attRateModifier / 2)) / (1 + (modeThreeProjsCount / 3));
            int resize = (int)(35f * modifier);
            Projectile.Resize(resize, resize);
            for (int i = 0; i < 2; i++)
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Explosion1>(), Vector2.Zero, 0, traceColor, modifier / 10);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return !exploded;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (mode != 1)
            {
                Projectile.timeLeft = 20 * Projectile.MaxUpdates;
                Projectile.velocity *= 0;
                Projectile.Opacity = 0;
                if (mode == 2)
                {
                    int lifestealDefaultAmnt = Math.Min(6, (int)Math.Ceiling(attRateModifier));
                    int lifesteal = lifestealDefaultAmnt - modeTwoProjsCount; //hard limit of 6, -modeTwoProjsCount as a slight nerf to shotguns with lotsa bullets.
                    if (lifesteal > 0) player.DoLifestealDirect(target, lifesteal);
                }
                if (mode == 3)
                {
                    if(Projectile.numHits == 0) explode();
                    target.AddBuff(BuffID.OnFire3, 30 + ((int)Math.Ceiling(attRateModifier * 10)));
                }
            }
            else
            slowDown = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if(mode == 1) modifiers.FinalDamage -= ((float)Projectile.numHits / 5f);
            if (mode == 3 && Projectile.numHits > 0) modifiers.FinalDamage -= 0.7f;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D trail = ModContent.Request<Texture2D>("asuw/Assets/Particles/trace_05", AssetRequestMode.ImmediateLoad).Value;
            List<ColoredVertex> ve = new List<ColoredVertex>();
            for (int i = 0; i < odp.Count; i++)
            {
                Color b = Color.HotPink;
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + Vector2.UnitX.RotatedBy(-MathHelper.PiOver2 + odv[i].ToRotation()) * Projectile.Size * ods[i],
                      new Vector3((i) / ((float)odp.Count - 1), 0, 1),
                      b));
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + Vector2.UnitX.RotatedBy(MathHelper.PiOver2 + odv[i].ToRotation()) * Projectile.Size * ods[i],
                      new Vector3((i) / ((float)odp.Count - 1), 1, 1),
                      b));
            }
            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                ShaderFunctions.vertexColorBloom(Main.spriteBatch, Color.Lerp(traceColor, Color.Magenta, 0.67f), traceColor, Projectile.Opacity);
                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                Main.spriteBatch.ExitShaderRegion();
            }
        }
    }
}
