using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Items.Accessories.Wings
{
    public abstract class BaseWings : ModItem
    {
        /// <summary>
        /// Bonus vertical acceleration per frame while player velocity is below 0.<br/>
        /// Defaults to 0.5f.
        /// </summary>
        public virtual float BonusAscentWhileFalling => 0.5f;

        /// <summary>
        /// Bonus vertical acceleration per frame while player velocity is below a velocity threshold determined by RisingSpeedThreshold.<br/>
        /// Defaults to 0.1f.
        /// </summary>
        public virtual float BonusAscentWhileRising => 0.1f;

        /// <summary>
        /// Vertical velocity threshold for activating bonus acceleration from BonusAscentWhileRising when multiplied by the player's jump speed.<br/>
        /// Defaults to 0.5f.
        /// </summary>
        public virtual float RisingSpeedThreshold => 0.5f;

        /// <summary>
        /// Max vertical velocity threshold when multiplied by the player's jump speed.<br/>
        /// Defaults to 1.5f.
        /// </summary>
        public virtual float MaxAscentSpeed => 1.5f;

        /// <summary>
        /// Base vertical acceleration per frame.<br/>
        /// Defaults to 0.1f.
        /// </summary>
        public virtual float BaseAscent => 0.1f;

        public override void SetDefaults()
        {
            Item.accessory = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            if (Item.wingSlot == -1) return;
            ascentWhenFalling = BonusAscentWhileFalling;
            ascentWhenRising = BonusAscentWhileRising;
            maxCanAscendMultiplier = RisingSpeedThreshold;
            maxAscentMultiplier = MaxAscentSpeed;
            constantAscend = BaseAscent;

            AdditionalFlightMovement(player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
        }

        /// <summary>
        /// Addition for any deviations in regular wing movement.<br/>
        /// This is typically for UP boost or hovers.
        /// </summary>
        public virtual void AdditionalFlightMovement(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) { }

       
    }
}
