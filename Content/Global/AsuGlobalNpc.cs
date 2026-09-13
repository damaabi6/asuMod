using asuw.Content.Buffs;
using asuw.Content.Global;
using asuw.Content.Items.Accesories;
using asuw.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace asuw.Content
{
    public partial class AsuGlobalNPC : GlobalNPC
    {
        public int npctick120 = 0;
        public int npctick1200 = 0;
        public int npctick12000 = 0;
        public int pyroApplicator = -1;
        public float bloodBlossomRotVar = 1;
        public int bloodBlossomRotSpeed = 3;
        public int SerratedApplicator = -1;
        public int DestinedSpellApplicator = -1;
        public int SinkingStack = 0;
        public int SinkingCounter = 0;

        public bool IronFlame;
        public bool hasBeenChained = false;
        public bool projected = false;
        public int projectedtick = 0;
        public bool theEndTarget = false;
        public int theEndTargetTick = 0;
        public float concentratedVolleyed = 0;
        public bool plantedHorus = false;


        public List<int> horusHooks = new List<int>();

        public override bool PreAI(NPC npc)
        {
            npctick120++;
            npctick1200++;
            npctick12000++;
            if (npctick120 > 120)
            { npctick120 = 0; }
            if (npctick1200 > 1200)
            { npctick1200 = 0; }
            if (npctick12000 > 12000)
            { npctick12000 = 0; }



            if (theEndTargetTick > 0)
            { theEndTargetTick--; theEndTarget = true; }
            else theEndTarget = false;
            

            if (projected)
            {
                projectedtick--;
            }

            if (projectedtick <= 0)
            {
                projectedtick = 0;
            }

            if (projectedtick > 0)
            {
                projected = true;
            }
            else
            {  projected = false; }



            if (SinkingStack > 0)
            {
                SinkingCounter--;
                if (SinkingCounter <= 0)
                {
                    SinkingCounter = 300;
                    SinkingStack--;
                }
            }
            else
            {
                SinkingCounter = 300;
            }

            return true;
        }

        public override void PostAI(NPC npc)
        {
            
        }


    }
}
