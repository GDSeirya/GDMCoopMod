using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace GDMCoopMod
{
    /// <summary>
    /// Shows the game's own overworld emotion bubble (the same one used for the enemy "!") above a FieldPlayer.
    /// The game keys bubbles per character, so each coop player's bubble is independent and a new emote
    /// replaces that character's previous one.
    /// </summary>
    public static class GDMFieldEmote
    {
        private const float CooldownSeconds = 0.4f;
        private static readonly Dictionary<int, float> lastShown = new Dictionary<int, float>();

        public static bool Show(FieldPlayer player, UIDefine.EmotionType type, float time)
        {
            try
            {
                if (player == null) return false;

                GameUIManager uiManager = GameUIManager.Instance;
                if (uiManager == null) return false;

                var fieldUi = uiManager.UIFieldController;
                if (fieldUi == null) return false;

                int id = player.GetInstanceID();
                float now = Time.unscaledTime;
                if (lastShown.TryGetValue(id, out float last) && now - last < CooldownSeconds)
                    return false;
                lastShown[id] = now;

                // Correct bubble height for this character model.
                Vector3 offset = Vector3.zero;
                EventUtility.GetEmotionOffset(player, ref offset);

                // isScaling = true: scale with camera distance. isCheckDistance = false: don't hide the bubble
                // when the camera is far away (the coop camera can zoom out).
                fieldUi.ShowEmotion(player, type, time, ref offset, true, false);
                return true;
            }
            catch (Exception e)
            {
                GDMCoopPlugin.StaticLog.LogWarning("GDMFieldEmote.Show failed: " + e.Message);
                return false;
            }
        }
    }
}
