
using asuw.Content.Cooldown;
using asuw.Content.Cooldown.WeaponCooldowns;
using asuw.Content.Items.Weapons;
using asuw.Content.Items.Weapons.Melee;
using asuw.Content.Projectiles;
using asuw.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tModPorter;
using ReLogic.Content;

namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {
        private List<CoolDown> Cooldowns = [new SolemnLamentCD(), new BlackBladeCD()];
        public List<CoolDown> ActiveCooldowns = [];

        public void addCooldown(int cooldownID, int duration)
        {
            int index = -1;
            foreach (var cooldown in Cooldowns)
            {
                if (cooldown.id == cooldownID)
                {
                    index = Cooldowns.IndexOf(cooldown);
                    break;
                }
            }

            if (index < 0)
                return;

            Cooldowns[index].tick = duration;
            Cooldowns[index].duration = duration;
            if (!ActiveCooldowns.Contains(Cooldowns[index]))
            ActiveCooldowns.Add(Cooldowns[index]);
        }

        private void UpdateCooldowns()
        {
            if (Player.dead || !Player.active)
            {
                foreach (var cooldown in Cooldowns)
                { cooldown.tick = 0; }
                ActiveCooldowns.Clear();
                return;
            }

            foreach (var cooldown in Cooldowns)
            {
                cooldown.Update();

                if (cooldown.tick < 1)
                    ActiveCooldowns.Remove(cooldown);
            }
        }

        public void DrawCooldownBar()
        {
            if (ActiveCooldowns.Count < 1)
                return;

            Vector2 screenPos = Player.MountedCenter + Vector2.UnitY * 80 - Vector2.UnitX * 15;

            int i = 0;
            foreach (var cooldown in ActiveCooldowns)
            {
                if (!cooldown.DrawCondition())
                    continue;
                
                float barWidth = 51;
                float barHeight = 20;
                float yOffset = barHeight * 1.6f * i;
                Vector2 finalPos = screenPos + Vector2.UnitY * yOffset;
                Vector2 framePos = finalPos - Vector2.UnitX * 6;
                float frameWidth = barWidth * 1.1f;
                float finalBarWidth = ((float)cooldown.tick / (float)cooldown.duration) * barWidth;
                ShaderFunctions.DrawRectangle(framePos, frameWidth, barHeight, Color.Lerp(Color.DimGray, Color.Black, 0.5f), 1, 0);
                ShaderFunctions.DrawRectangle(finalPos, finalBarWidth, barHeight, cooldown.color, 1, 0);
                Texture2D frame = ModContent.Request<Texture2D>("asuw/Assets/UIElements/CooldownFrame", AssetRequestMode.ImmediateLoad).Value;
                Main.spriteBatch.Draw(frame, framePos - Main.screenPosition, null, Color.White, 0, new Vector2(0, frame.Height / 2f), 1, 0, 0);
                Vector2 iconPos = finalPos - Main.screenPosition - Vector2.UnitX * 18;
                Texture2D icon = ModContent.Request<Texture2D>(cooldown.texture, AssetRequestMode.ImmediateLoad).Value;
                Main.spriteBatch.Draw(icon, iconPos, null, Color.White, 0, icon.Size() / 2f, 0.75f, 0, 0);

                i++;
            }
        }


    }
}
