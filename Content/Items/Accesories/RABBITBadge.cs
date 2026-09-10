using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace asuw.Content.Items.Accesories
{
    public class RABBITBadge : ModItem
    {
        //TODO proper balancing
        public static readonly int AdditiveDamageBonus = 15;
        public static readonly int RangedArmorPenetration = 5;
        public static readonly int AdditiveCritDamageBonus = 50;
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            if (ModLoader.TryGetMod("CalamityMod", out Mod CalamityMod) && CalamityMod.TryFind("BurnishedAuric", out ModRarity BurnishedAuric))
            {
                Item.rare = Item.rare = BurnishedAuric.Type;
            }
            else
            {
                Item.rare = ItemRarityID.Red;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // GetDamage returns a reference to the specified damage class' damage StatModifier.
            // Since it doesn't return a value, but a reference to it, you can freely modify it with mathematics operators (+, -, *, /, etc.).
            // StatModifier is a structure that separately holds float additive and multiplicative modifiers, as well as base damage and flat damage.
            // When StatModifier is applied to a value, its additive modifiers are applied before multiplicative ones.
            // Base damage is added directly to the weapon's base damage and is affected by damage bonuses, while flat damage is applied after all other calculations.
            // In this case, we're doing a number of things:
            // - Adding 25% damage, additively. This is the typical "X% damage increase" that accessories use, use this one.
            // - Adding 12% damage, multiplicatively. This effect is almost never used in Terraria, typically you want to use the additive multiplier above. It is extremely hard to correctly balance the game with multiplicative bonuses.
            // - Adding 4 base damage.
            // - Adding 5 flat damage.
            // Since we're using DamageClass.Generic, these bonuses apply to ALL damage the player deals.
           player.GetDamage(DamageClass.Ranged) += AdditiveDamageBonus / 100f;

            // GetKnockback is functionally identical to GetDamage, but for the knockback stat instead.
            // In this case, we're adding 100% knockback additively, but only for our custom example DamageClass (as such, only our example class weapons will receive this bonus).
            player.GetModPlayer<AsuPlayer>().AdditiveCritDamageBonus += AdditiveCritDamageBonus / 100f;
        }
    }

   
    
}