
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;

namespace asuw.Content
{
    public static partial class AsuUtils
    {
        public static void FindAndReplace(this List<TooltipLine> tooltips, string replacedKey, string newKey)
        {
            TooltipLine tooltipLine = tooltips.FirstOrDefault((TooltipLine x) => x.Mod == "Terraria" && x.Text.Contains(replacedKey));
            if (tooltipLine != null)
            {
                tooltipLine.Text = tooltipLine.Text.Replace(replacedKey, newKey);
            }
        }

        public static void FindAndReplaceAll(this List<TooltipLine> tooltips, string replacedKey, string newKey)
        {
            foreach (TooltipLine tooltip in tooltips)
            {
                tooltip.Text = tooltip.Text.Replace(replacedKey, newKey);
            }
        }

        public static void IntegrateHotkey(this List<TooltipLine> tooltips, ModKeybind mhk)
        {
            if (!Main.dedServ && mhk != null)
            {
                string newKey = mhk.TooltipHotkeyString();
                tooltips.FindAndReplace("[KEY]", newKey);
            }
        }

        public static List<string> GetAssignedKeysOrEmpty(this ModKeybind keybind, InputMode mode = InputMode.Keyboard)
        {
            if (keybind == null)
            {
                return new List<string>();
            }

            if (Main.dedServ)
            {
                return new List<string>();
            }

            try
            {
                return keybind.GetAssignedKeys(mode);
            }
            catch
            {
                return new List<string>();
            }
        }

        public static string TooltipHotkeyString(this ModKeybind mhk)
        {
            if (Main.dedServ || mhk == null)
            {
                return "";
            }

            List<string> assignedKeysOrEmpty = mhk.GetAssignedKeysOrEmpty();
            if (assignedKeysOrEmpty.Count == 0)
            {
                return ("HotkeyNotBound");
            }

            StringBuilder stringBuilder = new StringBuilder(16);
            stringBuilder.Append(assignedKeysOrEmpty[0]);
            for (int i = 1; i < assignedKeysOrEmpty.Count; i++)
            {
                stringBuilder.Append(" / ").Append(assignedKeysOrEmpty[i]);
            }

            return stringBuilder.ToString();
        }

    }
}
