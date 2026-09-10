using asuw.Content.Buffs;
using asuw.Content.Global;
using asuw.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace asuw.Content
{
    public partial class AsuGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Hemorrhage;
        public int Hemmorhage = 10;
        public bool BlackBlood;
        public int BBlood = 50;
        public bool Smoke;
        public int Smokee = 100;
        public bool Shi;
        public int ShiDMG = 50;
        public bool pyroAffected;
        public bool Serrated;
        public int SerratedAmnt = 0;
        public int SerratedDMG = 10;
        public bool DestinedDeath;
        public int DestinedSpellCount = 0;
        

        public override void ResetEffects(NPC npc)
        {
            Hemorrhage = false;
            Smoke = false;
            BlackBlood = false;
            Shi = false;
            IronFlame = false;
            pyroAffected = false;
            Serrated = false;
            DestinedDeath = false;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {

            if (Hemorrhage)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }


                if (Hemmorhage > 100)
                {
                    Hemmorhage = 100;
                }
                if (Hemmorhage < 10)
                {
                    Hemmorhage = 10;
                }

                npc.lifeRegen -= Hemmorhage * 6;
                if (damage < Hemmorhage / 2)
                {
                    damage = Hemmorhage / 2;
                }
            }

            if (Smoke)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= Smokee * 2;
                if (damage < Smokee / 2)
                {
                    damage = Smokee / 2;
                }
            }
            if (Shi)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= ShiDMG * 3;
                if (damage < ShiDMG)
                {
                    damage = ShiDMG;
                }
            }
            if (BlackBlood)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }


                if (BBlood > 1000)
                {
                    BBlood = 1000;
                }
                if (BBlood < 50)
                {
                    BBlood = 50;
                }

                npc.lifeRegen -= BBlood * 5;
                if (damage < BBlood / 2)
                {
                    damage = BBlood / 2;
                }
            }

            if(pyroAffected)
             Pyro.TickDebuff(npc,this);

            if(Serrated)
                SerratedDebuff.TickDebuff(npc,this);
                
            if(DestinedSpellCount > 0)
                BlackbladeSpellProj.DestinedSpellTickDMG(npc,this);
            
        }
       
        
    }
}
