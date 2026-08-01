using SimpleSpritePacker;
using UnityEngine;
using UnityEngine.InputSystem;

//This file is so that on every update, the virtual controllers are updated!

namespace RyMCoopMod
{
    public class RyMCoopUpdateDriver : MonoBehaviour
    {
        void Update()
        {
            UpdateVirtualInputs();
        }

        private void UpdateVirtualInputs()
        {
            for (int partyIndex = 0; partyIndex < 4; partyIndex++)
            {
                var state = RyMCoopPlugin.VirtualControllers.GetState(partyIndex);

                // If there are NOT enough controllers, return zero movement and false buttons
                if (partyIndex >= Gamepad.all.Count)
                {
                    state.Move = Vector2.zero;

                    state.AttackHeld = false;
                    state.EvadeHeld = false;
                    state.LeftSkillHeld = false;
                    state.RightSkillHeld = false;

                    // Update press/release events
                    state.UpdateEvents();

                    RyMCoopPlugin.VirtualControllers.SetState(partyIndex, state);
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

                // Compute press/release events
                state.UpdateEvents();

                RyMCoopPlugin.VirtualControllers.SetState(partyIndex, state);
            }
        }
    }
}