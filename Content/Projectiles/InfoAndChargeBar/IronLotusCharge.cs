using asuw.Content.Global;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace asuw.Content.Projectiles.InfoAndChargeBar
{
    public class IronLotusCharge : ModProjectile
    {

        Player player => Main.player[Projectile.owner];

        public int burnflameframe = 0;
        public double burnflamenum = 8;
        public int burnflamelevel = 1;
        public ref float timer => ref Projectile.ai[0];
        public override string Texture => "asuw/Assets/Blank";
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
            
            Projectile.spriteDirection = 1;
            Projectile.Center = player.MountedCenter;

            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is IronLotus il )
            {
                Projectile.timeLeft = 2;
                if (il.IronLotusFlame)
                {
                    timer++;
                    if(timer % 15 == 0)burnflameframe++;
                    if (burnflameframe > 3) burnflameframe = 0;

                    burnflamenum = Math.Ceiling((double)((480 - (il.IronFlameTick)) / 60)) + 1;
                    burnflamelevel = 7 - il.IronFlameLevel;
                    if(burnflamelevel > 6) burnflamelevel = 6;

                }
            }

            

            

        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void PostDraw(Color lightColor)
        {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is IronLotus il)
            {

                Texture2D texture;
                if (!il.IronLotusFlame)
                {
                    texture = ModContent.Request<Texture2D>($"asuw/Content/Textures/Chargedowns/IronLotusCharge{il.IronLotusChargeNum}").Value;

                }
                else
                {
                    texture = ModContent.Request<Texture2D>($"asuw/Content/Textures/Chargedowns/IronLotusFlame{il.IronLotusChargeNum}").Value;
                }
                Vector2 drawPosition = (player.Center + new Vector2(0, -120)) - Main.screenPosition;
                Color drawColor = Color.White;
                Vector2 origin = texture.Size() * 0.5f;
                Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, 0, origin, 4, SpriteEffects.None);

                if (il.IronLotusFlame)
                {
                    Texture2D textureFire = ModContent.Request<Texture2D>($"asuw/Content/Textures/Chargedowns/BurnAnim{burnflameframe}").Value;
                    Vector2 fireorigin = textureFire.Size() * 0.5f;
                    Texture2D textureNum = ModContent.Request<Texture2D>($"asuw/Content/Textures/Chargedowns/FireNum{burnflamenum}").Value;
                    Vector2 firenumorigin = textureNum.Size() * 0.5f;
                    Texture2D textureBurn = ModContent.Request<Texture2D>($"asuw/Content/Textures/Chargedowns/Burnlvl{burnflamelevel}").Value;
                    Vector2 BurnOrigin = textureBurn.Size() * 0.5f;
                    Texture2D textureBg = ModContent.Request<Texture2D>("asuw/Content/Textures/Chargedowns/BlackBg").Value;
                    Vector2 BgOrigin = textureBg.Size() * 0.5f;
                    Vector2 drawPositionfire = (player.Center + new Vector2(-20, 60)) - Main.screenPosition;
                    Vector2 drawPositionnum = (player.Center + new Vector2(-20, 70)) - Main.screenPosition;
                    Vector2 drawPositionburn = (player.Center + new Vector2(20, 60)) - Main.screenPosition;
                    Vector2 drawPositionbg = (player.Center + new Vector2(0, 60)) - Main.screenPosition;
                    Main.EntitySpriteDraw(textureBg, drawPositionbg, null, Color.White * 0.2f, 0, BgOrigin, 3f, SpriteEffects.None);
                    Main.EntitySpriteDraw(textureFire, drawPositionfire, null, Color.White, 0, fireorigin, 0.3f, SpriteEffects.None);
                    Main.EntitySpriteDraw(textureNum, drawPositionnum, null, Color.White, 0, firenumorigin, 1.5f, SpriteEffects.None);
                    Main.EntitySpriteDraw(textureBurn, drawPositionburn, null, Color.White, 0, BurnOrigin, 0.8f, SpriteEffects.None);


                }
            }
        }
    }
}
