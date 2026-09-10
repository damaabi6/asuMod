using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace asuw.Content.Rarities
{
    public class ButterflyHairpinRare : ModRarity
    {
        //learn custom drawing
        public override Color RarityColor => AsuPlayer.SpecialMoveColorBH;
        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }
}
