using asuw.Content.Global;
using asuw.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace asuw.Content.Projectiles.InfoAndChargeBar
{
    public class TheEndInfos: ModProjectile
    {

        Player player => Main.player[Projectile.owner];
        ref float timer => ref Projectile.ai[0];
        public float lerpamount = 0.2f;
        public bool stop = false;
        public override string Texture => "asuw/Content/Textures/Extra/TheEndTargetedCrosshair";
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
            if (timer > 120) { timer = 0; }
            timer++;


            if (lerpamount >= 0.65f)
                stop = true;
            if (lerpamount <= 0.1f)
                stop = false;
                

            if (!stop)
            lerpamount += 0.02f;
            else
            lerpamount -= 0.02f;






            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is TheEnd te && !player.dead)
            {
                Projectile.timeLeft = 2;

               
            }




        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            behindNPCs.Remove(index);
            behindProjectiles.Remove(index);
            behindNPCsAndTiles.Remove(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is TheEnd te)
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                Vector2 origin = texture.Size() * 0.5f;
                float scale;
               
                foreach (var npcs in Main.ActiveNPCs)
                {
                    if (npcs.asuw().theEndTarget && npcs == te.targetToHome)
                    {
                        
                        if (((float)npcs.height / 100) < 1.5f)
                        {
                            scale = (float)npcs.height / 100;
                        }
                        else
                        {
                            scale = 1.5f;
                        }
                        Main.EntitySpriteDraw(texture, npcs.Center - Main.screenPosition, null, Color.White, 0, origin, scale, SpriteEffects.None);
                    }
                }

                Texture2D texture2 = ModContent.Request<Texture2D>("asuw/Content/Textures/Extra/TheEndSkillActivateable").Value;

                Vector2 DrawPosition = (player.Center + new Vector2(0, 50)) - Main.screenPosition;

                string HealedAmountText = (te.healeddAmount).ToString();

                Color colorDraw = te.healeddAmount >= 1800 ? te.healeddAmount >= 3600 ? Color.Violet : Color.MediumPurple : Color.DarkOrchid;
                Color colorLerped = Color.Lerp(colorDraw, te.healeddAmount >= 3600 ? Color.White : Color.Violet, lerpamount);
                Vector2 origin2 = texture2.Size() * 0.5f;
                if (!player.asuw().TheEndDestroyerSkill)
                {
                    
                    Main.EntitySpriteDraw(texture2, DrawPosition + (Vector2.UnitX * -6), null, te.healeddAmount >= 1800 ? Color.Lerp(colorLerped, Color.White, 0.35f) : Color.Lerp(colorDraw, Color.White, 0.35f), 0, origin2, Vector2.One * Main.UIScale * 0.8f, SpriteEffects.None);
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, HealedAmountText, DrawPosition, te.healeddAmount >= 1800 ? colorLerped : colorDraw, 0f, Vector2.Zero, Vector2.One * Main.UIScale * 0.75f);
                }
            }
            return false;
        }
      
    }
}
