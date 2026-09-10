using asuw.Content.Buffs;
using asuw.Content.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;


namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {
        
        public bool Smoke;
        public bool Hemorrhage;



        public override void UpdateBadLifeRegen()
        {
            float totalNegativeLifeRegen = 0;

            void ApplyDoTDebuff(bool hasDebuff, int negativeLifeRegenToApply)
            {
                if (!hasDebuff)
                    return;

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                totalNegativeLifeRegen += negativeLifeRegenToApply;
            }

            ApplyDoTDebuff(Smoke, 20); //20 is 50 dmg in 5 seconds
            ApplyDoTDebuff(Hemorrhage, 10);
            ApplyDoTDebuff(ShiDash, 500);

            Player.lifeRegen -= (int)totalNegativeLifeRegen;
        }
            
        
           
        

       
            

        
    }
}
