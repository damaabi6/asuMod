using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using asuw.Content.Global;

namespace asuw.Content.Cooldown.WeaponCooldowns
{
    public class ProjectionDashCD : ModBuff
    {

        public override string Texture => "asuw/Content/Cooldown/WeaponCooldowns/WeaponCooldown";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {

            player.GetModPlayer<AsuPlayer>().CoolTick = 360 - player.GetModPlayer<AsuPlayer>().CoolTickDown;
            player.GetModPlayer<AsuPlayer>().CoolTickDown++;
            player.GetModPlayer<AsuPlayer>().WeaponCooldown = true;

        }

        
        
    }
}
