using Game;
using UnityEngine;

namespace GDMCoopMod
{
    /// <summary>
    /// Overworld emotes for coop players.
    /// Press South / East / West / North for the first set of emotes, or hold the right trigger and press
    /// one of them for the second set. The bubble appears over whichever character(s) that controller is
    /// assigned to. The host controller only uses the trigger set, so it never hijacks the game's own
    /// talk/confirm presses.
    /// </summary>
    public class GDMFieldEmoteManager : MonoBehaviour
    {
        private void Update()
        {
            if (GDMEmoteSettings.Enabled == null || !GDMEmoteSettings.Enabled.Value) return;
            if (!IsOverworldFree()) return;

            int firstController = GDMEmoteSettings.HostEnabled.Value ? 0 : 1;
            for (int controller = firstController; controller < 4; controller++)
            {
                GDMVirtualControllerState state = GDMCoopPlugin.VirtualControllers.GetState(controller);
                bool triggerHeld = state.EmoteModifierHeld;

                // The game still reads the host's face buttons (talk/confirm), so the host only emotes with the trigger.
                if (controller == 0 && !triggerHeld) continue;

                int slot = -1;
                if (state.EvadePressed) slot = 0;          // South
                else if (state.AttackPressed) slot = 1;    // East
                else if (state.WestPressed) slot = 2;      // West
                else if (state.UseMagicPressed) slot = 3;  // North
                if (slot < 0) continue;

                ShowForController(controller, GDMEmoteSettings.GetEmotion(triggerHeld ? 1 : 0, slot));
            }
        }

        private static bool IsOverworldFree()
        {
            if (BattleManager.GetInstance() != null)
            {
                if (BattleManager.GetInstance().battlePlayerList.Count > 0)
                {
                    return false;
                }
            }

            FieldManager field = FieldManager.Instance;
            if (field == null) return false;

            return field.IsInputOperation();
        }

        private static void ShowForController(int controller, UIDefine.EmotionType emotion)
        {
            float duration = GDMEmoteSettings.Duration.Value;
            bool shown = false;

            for (int party = 0; party < 4; party++)
            {
                if (GDMControllerRouting.GetControllerForParty(party) != controller) continue;
                shown |= GDMFieldEmote.Show(GetFieldPlayerForParty(party), emotion, duration);
            }

            // Host controller with no explicit assignment controls the party leader.
            if (!shown && controller == 0 && !IsControllerAssigned(0))
            {
                FieldManager field = FieldManager.Instance;
                if (field != null) GDMFieldEmote.Show(field.GetControlPlayer(), emotion, duration);
            }
        }

        private static bool IsControllerAssigned(int controller)
        {
            for (int party = 0; party < 4; party++)
                if (GDMControllerRouting.GetControllerForParty(party) == controller) return true;
            return false;
        }

        // Party order on the overworld is the same order the battle uses: PartyManager's battle members.
        private static FieldPlayer GetFieldPlayerForParty(int partyIndex)
        {
            PartyManager party = PartyManager.Instance;
            FieldManager field = FieldManager.Instance;
            if (party == null || field == null) return null;

            var members = party.GetBattleMembers();
            if (members == null || partyIndex < 0 || partyIndex >= members.Length) return null;

            PlayerID id = members[partyIndex];
            if (id == PlayerID.INVALID) return null;

            return field.GetFieldPlayer(id);
        }
    }
}
