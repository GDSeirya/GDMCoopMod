using UnityEngine;
using UnityEngine.InputSystem;

// This file updates the virtual controllers every frame.
// Physical controllers are automatically registered/deregistered
// while the game is running.

namespace GDMCoopMod
{
    public class GDMVirtualControllerDriver : MonoBehaviour
    {
        // Fixed player slots.
        // Index 0 = Player 1
        // Index 1 = Player 2
        // Index 2 = Player 3
        // Index 3 = Player 4
        private readonly Gamepad[] registeredGamepads = new Gamepad[4];
        private readonly string[] slotIdentity = new string[4];   // survives disconnects
        private readonly Gamepad[] slotLastPad = new Gamepad[4];  // survives disconnects
        private readonly float[] slotLeftAt = new float[4];

        private static string GetIdentity(Gamepad pad)
        {
            var d = pad.description;
            return $"{d.interfaceName}|{d.manufacturer}|{d.product}|{d.version}|{d.serial}";
        }

        private int ChooseSlot(Gamepad pad)
        {
            string id = GetIdentity(pad);

            // 1. Same device object coming back (Input System often reuses it)
            for (int s = 0; s < 4; s++)
                if (registeredGamepads[s] == null && slotLastPad[s] == pad) return s;

            // 2. Same kind/serial of controller as one that left
            int best = -1;
            for (int s = 0; s < 4; s++)
                if (registeredGamepads[s] == null && slotIdentity[s] == id)
                    if (best == -1 || slotLeftAt[s] < slotLeftAt[best]) best = s;
            if (best != -1) return best;

            // 3. Genuinely new pad: take an empty slot nobody is waiting on
            for (int s = 0; s < 4; s++)
                if (registeredGamepads[s] == null && slotIdentity[s] == null) return s;

            // 4. Only reserved slots left: use the one that has been away longest
            for (int s = 0; s < 4; s++)
                if (registeredGamepads[s] == null)
                    if (best == -1 || slotLeftAt[s] < slotLeftAt[best]) best = s;
            return best;   // -1 means no room
        }



        private void Update()
        {
            UpdateGamepads();
            UpdateVirtualInputs();
        }

        private void UpdateGamepads()
        {
            // Look for newly connected controllers.
            for (int gamepadIndex = 0; gamepadIndex < Gamepad.all.Count; gamepadIndex++)
            {
                Gamepad gamepad = Gamepad.all[gamepadIndex];

                if (IsRegistered(gamepad))
                    continue;

                // Dynamically choose the next available slot
                int slot = ChooseSlot(gamepad); if (slot >= 0) RegisterGamepad(gamepad, slot);
            }

            // Look for disconnected controllers.
            for (int playerIndex = 0; playerIndex < registeredGamepads.Length; playerIndex++)
            {
                Gamepad gamepad = registeredGamepads[playerIndex];

                if (gamepad == null)
                    continue;

                if (!IsGamepadConnected(gamepad))
                {
                    DeregisterGamepad(playerIndex);
                }
            }
        }

        private bool IsRegistered(Gamepad gamepad)
        {
            for (int playerIndex = 0; playerIndex < registeredGamepads.Length; playerIndex++)
            {
                if (registeredGamepads[playerIndex] == gamepad)
                    return true;
            }

            return false;
        }

        private bool IsGamepadConnected(Gamepad gamepad)
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (Gamepad.all[i] == gamepad)
                    return true;
            }

            return false;
        }

        private void RegisterGamepad(Gamepad gamepad, int playerIndex)
        {
            registeredGamepads[playerIndex] = gamepad;
            slotIdentity[playerIndex] = GetIdentity(gamepad);
            GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("ControllerRegistered", gamepad.displayName, playerIndex + 1));
        }

        private void DeregisterGamepad(int playerIndex)
        {
            Gamepad gamepad = registeredGamepads[playerIndex];
            GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("ControllerDeregistered", gamepad.displayName, playerIndex + 1));
            registeredGamepads[playerIndex] = null;
            ClearVirtualInput(playerIndex);
            slotLastPad[playerIndex] = gamepad;
        }

        private void UpdateVirtualInputs()
        {
            for (int partyIndex = 0; partyIndex < 4; partyIndex++)
            {
                var state =
                    GDMCoopPlugin.VirtualControllers.GetState(partyIndex);

                Gamepad pad = registeredGamepads[partyIndex];

                // No controller assigned to this player.
                if (pad == null)
                {
                    ClearInputState(state);

                    state.UpdateEvents(Time.deltaTime);

                    GDMCoopPlugin.VirtualControllers.SetState(
                        partyIndex,
                        state
                    );

                    continue;
                }

                // Movement
                state.Move = pad.leftStick.ReadValue();
                state.RightStick = pad.rightStick.ReadValue();

                // Held states
                state.AttackHeld = pad.buttonEast.isPressed;
                state.EvadeHeld = pad.buttonSouth.isPressed;

                state.LeftSkillHeld = pad.leftShoulder.isPressed;
                state.RightSkillHeld = pad.rightShoulder.isPressed;

                state.ChangeToSlot1Held = pad.dpad.up.isPressed;
                state.ChangeToSlot2Held = pad.dpad.right.isPressed;
                state.ChangeToSlot3Held = pad.dpad.down.isPressed;
                state.ChangeToSlot4Held = pad.dpad.left.isPressed;

                state.TargetingModeHeld = pad.leftTrigger.isPressed;
                state.UseMagicHeld = pad.buttonNorth.isPressed;

                // Overworld emotes: right trigger is the modifier, West is the 4th emote button.
                state.EmoteModifierHeld = pad.rightTrigger.isPressed;
                state.WestHeld = pad.buttonWest.isPressed;

                // Calculate press/release events.
                state.UpdateEvents(Time.deltaTime);

                GDMCoopPlugin.VirtualControllers.SetState(
                    partyIndex,
                    state
                );
            }
        }

        private void ClearVirtualInput(int partyIndex)
        {
            var state =
                GDMCoopPlugin.VirtualControllers.GetState(partyIndex);

            ClearInputState(state);

            state.UpdateEvents(Time.deltaTime);

            GDMCoopPlugin.VirtualControllers.SetState(
                partyIndex,
                state
            );
        }

        private void ClearInputState(GDMVirtualControllerState state)
        {
            state.Move = Vector2.zero;
            state.RightStick = Vector2.zero;

            state.AttackHeld = false;
            state.EvadeHeld = false;

            state.LeftSkillHeld = false;
            state.RightSkillHeld = false;

            state.ChangeToSlot1Held = false;
            state.ChangeToSlot2Held = false;
            state.ChangeToSlot3Held = false;
            state.ChangeToSlot4Held = false;

            state.TargetingModeHeld = false;
            state.UseMagicHeld = false;

            state.EmoteModifierHeld = false;
            state.WestHeld = false;
        }
    }
}