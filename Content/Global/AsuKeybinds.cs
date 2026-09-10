using Terraria.ModLoader;

namespace asuw.Content.Global
{
    public class AsuKeybinds : ModSystem
    {
        public static ModKeybind WeaponSkill { get; private set; }

        public static ModKeybind CorruptionEyeActivation { get; private set; }


        public override void Load()
        {
            // Register keybinds            
            WeaponSkill = KeybindLoader.RegisterKeybind(Mod, "Additional Skill Function", "Q");
            CorruptionEyeActivation = KeybindLoader.RegisterKeybind(Mod, "Corruption Eye Activation", "V");
        }

        public override void Unload()
        {
            WeaponSkill = null;
            CorruptionEyeActivation = null;
        }
    }
}
