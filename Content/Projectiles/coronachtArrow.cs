
using asuw.Content.Items.Weapons;
using asuw.Content.Projectiles.Filler;
using asuw.Content.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

//namespace asuw.Content.Projectiles
//{
//    public class coronachtArrow : ModProjectile
//    {
//        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
//        public ref float mods => ref Projectile.ai[0];
//        public ref float timer => ref Projectile.ai[2];
//        public float homingStrength = 0.1f;
//        public ref float scaled => ref Projectile.ai[1];
//        public NPC targetToChain = null;
//        public List<NPC> chained = new List<NPC>();
//        public int chainedAMount = 0;


   
//        Player player => Main.player[Projectile.owner];


//        public override void SetDefaults()
//        {
//            Projectile.width = Projectile.height = 20;
//            Projectile.friendly = true; 
//            Projectile.hostile = false; 
//            Projectile.DamageType = DamageClass.Ranged; 
//            Projectile.penetrate = -1; 
//            Projectile.timeLeft = 200; 
//            Projectile.alpha = 0; 
//            Projectile.light = 0.5f; 
//            Projectile.ignoreWater = true; 
//            Projectile.tileCollide = false; 
//            Projectile.extraUpdates = 8; 
//            Projectile.usesLocalNPCImmunity = true;
//            Projectile.localNPCHitCooldown = -1;
         
//        }

//        public override void OnSpawn(IEntitySource source)
//        {
//            int scalebonus = (int)(scaled * 2f);
//            if(mods < 100)Projectile.Resize(Projectile.width + scalebonus, Projectile.height + scalebonus);
//        }
 
       
      
//        public override void AI()
//        {
           
//            Projectile.rotation = Projectile.velocity.ToRotation();

//            timer++;
//            if (timer > 5)
//            {

//                float scaleFinal = (scaled - 3) * 1.5f;



//                //LAZORS
//                //AltSparkParticle spark = new AltSparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 30, 1.6f + (scaleFinal / 10f), Color.White with { A = 80 });
//                //GeneralParticleHandler.SpawnParticle(spark);

//                //if (mods < 100)
//                //{
//                //    AltSparkParticle spark2 = new AltSparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 20, 1.6f + (scaleFinal / 10f), Color.Violet with { A = 0 });
//                //    GeneralParticleHandler.SpawnParticle(spark2);

//                //    Vector2 sparkPosition = Projectile.Center - (Projectile.velocity / 3f);
//                //    Vector2 sparkVelocity = -Projectile.velocity * 0.01f * Main.rand.NextFloat(0.5f, 1.5f);

//                //    Particle spark3 = new CustomSpark(sparkPosition, sparkVelocity, "CalamityMod/Particles/BloomLineFade", false, 6, 0.057f + (scaleFinal / 100f), Color.WhiteSmoke * 0.85f, new Vector2(0.45f, 0.9f), shrinkSpeed: 0.4f);
//                //    GeneralParticleHandler.SpawnParticle(spark3);
//                //}

//                AltSparkParticle spark = new AltSparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 30, 1.4f + (scaleFinal / 9f), Color.White with { A = 80 } * 0.1f);
//                GeneralParticleHandler.SpawnParticle(spark);

//                if (mods < 100)
//                {
//                    AltSparkParticle spark2 = new AltSparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 20, 1.4f + (scaleFinal / 9f), Color.Violet with { A = 0 } * 0.2f);
//                    GeneralParticleHandler.SpawnParticle(spark2);

//                    Vector2 sparkPosition = Projectile.Center - (Projectile.velocity / 3f);
//                    Vector2 sparkVelocity = -Projectile.velocity * 0.01f * Main.rand.NextFloat(0.5f, 1.5f);

//                    Particle spark3 = new CustomSpark(sparkPosition, sparkVelocity, "CalamityMod/Particles/BloomLineFade", false, 6, 0.057f + (scaleFinal / 100f), Color.WhiteSmoke * 0.85f, new Vector2(0.45f, 0.9f), shrinkSpeed: 0.4f);
//                    GeneralParticleHandler.SpawnParticle(spark3);
//                }
//                else
//                {
//                    if (mods != 103)
//                    {
//                        NPC target = AsuUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 2000);
//                        if (mods != 101) homingStrength += 0.005f;
//                        else homingStrength += 0.01f;

//                        if (target != null) Projectile.velocity = AsuUtils.SmoothHomingBehavior(Projectile, target.Center, 1, homingStrength);
//                    }
                   
//                }

//            }

//        }

//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            if (mods < 100)
//            {
//                if (mods == 4)
//                {
//                    Projectile.Kill();
//                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ExplosiveCoronacht>(), (int)((float)Projectile.damage * 0.85f), 3f, Projectile.owner);
//                }
//                if (mods == 2)
//                {
//                    Particle boom = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Azure * 0.7f, "CalamityMod/Particles/DustyCircleHardEdge", Vector2.One, Main.rand.NextFloat(-15f, 15f), 0.02f, 0.04f, 12);
//                    GeneralParticleHandler.SpawnParticle(boom);
//                    List<NPC> order = new List<NPC>();
//                    chained.Add(target);
//                    if (chainedAMount < 3)
//                    {
//                        targetToChain = AsuUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 2000);

//                        float dist = 2000;
//                        foreach (var n in Main.ActiveNPCs)
//                        {
//                            if (targetToChain != null && (targetToChain == target || chained.Contains(targetToChain)))
//                            {
//                                if (AsuUtils.getDistance(n.Center, Projectile.Center) <= dist && n.CanBeChasedBy() && !chained.Contains(n))
//                                {
//                                    dist = AsuUtils.getDistance(n.Center, Projectile.Center);
//                                    targetToChain = n;
//                                }
//                            }
//                        }

//                        chainedAMount++;

//                        // If the projectile hits the left or right side of the tile, reverse the X velocity
//                        if (targetToChain == null)
//                        {
//                            if (Math.Abs(Projectile.velocity.X - Projectile.oldVelocity.X) > float.Epsilon)
//                            {
//                                Projectile.velocity.X = -Projectile.oldVelocity.X;
//                            }

//                            // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
//                            if (Math.Abs(Projectile.velocity.Y - Projectile.oldVelocity.Y) > float.Epsilon)
//                            {
//                                Projectile.velocity.Y = -Projectile.oldVelocity.Y;
//                            }

//                        }
//                        else
//                        {
//                            float length = Projectile.velocity.Length();
//                            Projectile.velocity = (targetToChain.Center - Projectile.Center).normalize() * length;
//                        }
//                    }
//                    else
//                    {
//                        Projectile.Kill();
//                    }
//                }
//            }
//            else
//            {
//                if(mods != 101)Projectile.Kill();
//                else
//                {
//                    Projectile.ResetLocalNPCHitImmunity();
//                    if (Projectile.numHits >= 3) Projectile.Kill();
//                }      
//                if (mods == 103)
//                {
//                    target.asuw().concentratedVolleyed++;
//                }
                
//            }
//        }

//        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
//        {
//            if(mods == 0)
//            {
//                modifiers.CritDamage += 0.1f;
//            }
//            if (mods == 2)
//            {
//                modifiers.SourceDamage *= (1f + ((float)chainedAMount) / 2f);
//            }

//            if(mods == 101)
//            {
//                modifiers.SourceDamage *= 0.3f;
//                modifiers.ScalingArmorPenetration += 1f;
//            }
          
           
//        }

     
//        public override void OnKill(int timeLeft)
//        {
//            chained.Clear();
//        }

//    }
//}
