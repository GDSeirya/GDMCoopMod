using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDMCoopMod
{
    public static class GDMSoundRegistry
    {
        public enum ModSfx
        {
            ActionBuzzer,
            TitleScreenStart,
            MenuSelect,
            MenuDecide,
            MenuOpen,
            MenuClose
        }

        private static readonly Dictionary<ModSfx, string> Map = new() {
            { ModSfx.ActionBuzzer, "se_btl_0013_01" },
            { ModSfx.TitleScreenStart, "se_sys_0001_01" },
            { ModSfx.MenuSelect, "se_sys_0003_01" },
            { ModSfx.MenuDecide, "se_sys_0001_02" },
            { ModSfx.MenuOpen, "se_sys_0004_01" },
            { ModSfx.MenuClose, "se_sys_0002_01" },
        };

        public static void PlaySe(ModSfx sfx, float volume = 1f, bool isIgnorePause = false, float delayTime = 0f)
        {
            GameSoundManager.PlaySe(Map[sfx], volume, isIgnorePause, delayTime);
        }
    }
}
