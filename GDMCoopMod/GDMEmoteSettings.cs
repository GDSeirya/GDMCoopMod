using BepInEx.Configuration;
using Game;

namespace GDMCoopMod
{
    /// <summary>
    /// Config for overworld emotion bubbles. These entries live in the plugin's normal config file
    /// (BepInEx/config/gdm.coopmod.cfg), which BepInEx creates on first launch if it does not exist.
    ///
    /// Two sets of four emotes:
    ///   Set 0: face buttons alone.
    ///   Set 1: right trigger held + face button.
    ///
    /// Values are UIDefine.EmotionType numbers:
    /// 0 Sweat, 1 Exclamation, 2 Question, 3 Notice, 4 Angry, 5 Note, 6 Gloomy, 7 Heart, 8 LightBulb,
    /// 9 ColdSweat, 10 Laugh, 11 TurnPale, 12 Exclamation2, 13 Silence, 14 SweatReverse,
    /// 15 NoticeReverse, 16 Sleep
    /// </summary>
    public static class GDMEmoteSettings
    {
        private const string EmoteList =
            "0 Sweat, 1 Exclamation, 2 Question, 3 Notice, 4 Angry, 5 Note, 6 Gloomy, 7 Heart, " +
            "8 LightBulb, 9 ColdSweat, 10 Laugh, 11 TurnPale, 12 Exclamation2, 13 Silence, " +
            "14 SweatReverse, 15 NoticeReverse, 16 Sleep";

        private const string FaceSection = "Emotes - Face Buttons";
        private const string TriggerSection = "Emotes - Trigger + Face Buttons";

        // Slot order: 0 = South, 1 = East, 2 = West, 3 = North.
        private static readonly string[] ButtonNames =
        {
            "South (A / Cross)",
            "East (B / Circle)",
            "West (X / Square)",
            "North (Y / Triangle)",
        };

        private static readonly int[] FaceDefaults = { 1, 2, 7, 5 };      // Exclamation, Question, Heart, Note
        private static readonly int[] TriggerDefaults = { 10, 4, 6, 8 };  // Laugh, Angry, Gloomy, LightBulb

        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<bool> HostEnabled;
        public static ConfigEntry<float> Duration;

        // [set, slot]
        private static readonly ConfigEntry<int>[,] emotes = new ConfigEntry<int>[2, 4];

        public static void Init(ConfigFile config)
        {
            Enabled = config.Bind(
                "Emotes", "Enabled", true,
                "Show an emotion bubble over your character on the overworld. Press a face button for the first set, or hold the right trigger and press a face button for the second set.");

            HostEnabled = config.Bind(
                "Emotes", "Host Emotes", false,
                "Also allow controller 1 (host) to emote. The host only uses the trigger set, because the game still reads the host's face buttons for talking and confirming.");

            Duration = config.Bind(
                "Emotes", "Duration Seconds", 1.5f,
                new ConfigDescription("How long an emotion bubble stays up.", new AcceptableValueRange<float>(0.5f, 5f)));

            var range = new AcceptableValueRange<int>(0, 16);
            for (int slot = 0; slot < 4; slot++)
            {
                emotes[0, slot] = config.Bind(
                    FaceSection, ButtonNames[slot], FaceDefaults[slot],
                    new ConfigDescription("Emotion shown when pressing this face button. " + EmoteList, range));

                emotes[1, slot] = config.Bind(
                    TriggerSection, ButtonNames[slot], TriggerDefaults[slot],
                    new ConfigDescription("Emotion shown when holding the right trigger and pressing this face button. " + EmoteList, range));
            }
        }

        /// <summary>set: 0 = face buttons, 1 = trigger + face buttons. slot: 0 = South, 1 = East, 2 = West, 3 = North.</summary>
        public static UIDefine.EmotionType GetEmotion(int set, int slot)
        {
            return (UIDefine.EmotionType)emotes[set, slot].Value;
        }
    }
}
