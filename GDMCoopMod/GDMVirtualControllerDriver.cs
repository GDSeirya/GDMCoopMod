using UnityEngine;
using UnityEngine.InputSystem;

//This file is so that on every update, the virtual controllers are updated!

namespace GDMCoopMod
{
    public class GDMVirtualControllerDriver : MonoBehaviour
    {
        void Update()
        {
            UpdateVirtualInputs();
        }

        private void UpdateVirtualInputs()
        {
            for (int partyIndex = 0; partyIndex < 4; partyIndex++)
            {
                var state = GDMCoopPlugin.VirtualControllers.GetState(partyIndex);

                // If there are NOT enough controllers, return zero movement and false buttons
                if (partyIndex >= Gamepad.all.Count)
                {
                    state.Move = Vector2.zero;

                    state.AttackHeld = false;
                    state.EvadeHeld = false;
                    state.LeftSkillHeld = false;
                    state.RightSkillHeld = false;
                    state.ChangeToSlot1Held = false;
                    state.ChangeToSlot2Held = false;
                    state.ChangeToSlot3Held = false;
                    state.ChangeToSlot4Held = false;

                    // Update press/release events
                    state.UpdateEvents();

                    GDMCoopPlugin.VirtualControllers.SetState(partyIndex, state);
                    continue;
                }

                // Otherwise read from the actual gamepad
                Gamepad pad = Gamepad.all[partyIndex];

                // Movement
                state.Move = pad.leftStick.ReadValue();

                // HELD states
                state.AttackHeld = pad.buttonEast.isPressed;
                state.EvadeHeld = pad.buttonSouth.isPressed;
                state.LeftSkillHeld = pad.leftShoulder.isPressed;
                state.RightSkillHeld = pad.rightShoulder.isPressed;
                state.ChangeToSlot1Held = pad.dpad.up.isPressed;
                state.ChangeToSlot2Held = pad.dpad.right.isPressed;
                state.ChangeToSlot3Held = pad.dpad.down.isPressed;
                state.ChangeToSlot4Held = pad.dpad.left.isPressed;

                // Compute press/release events
                state.UpdateEvents();

                GDMCoopPlugin.VirtualControllers.SetState(partyIndex, state);
            }
        }
    }
}