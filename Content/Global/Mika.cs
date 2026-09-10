using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Rarities
{
    public class Mikas : ModRarity
    {
        public override Color RarityColor => new Color(Main.DiscoR / 0.56f, (byte)(Main.DiscoG / 0.611f), 255);

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset < 0)
            { // If the offset is -1 or -2 (a negative modifier).
                return ModContent.RarityType<Rarity1>(); // Make the rarity of items that have this rarity with a negative modifier the lower tier one.
            }

            return Type; // no 'higher' tier to go to, so return the type of this rarity.
        }

    }
}
