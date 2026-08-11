using UnityEngine;

namespace GDMCoopMod
{
    public class GDMVirtualControllerState
    {
        // Movement (analog)
        public Vector2 Move;

        // HELD states
        public bool AttackHeld;
        public bool EvadeHeld;
        public bool LeftSkillHeld;
        public bool RightSkillHeld;
        public bool ChangeToSlot1Held;
        public bool ChangeToSlot2Held;
        public bool ChangeToSlot3Held; 
        public bool ChangeToSlot4Held;

        // PRESS events (onPress)
        public bool AttackPressed;
        public bool EvadePressed;
        public bool LeftSkillPressed;
        public bool RightSkillPressed;
        public bool ChangeToSlot1Pressed;
        public bool ChangeToSlot2Pressed;
        public bool ChangeToSlot3Pressed;
        public bool ChangeToSlot4Pressed;

        // RELEASE events (onRelease)
        public bool AttackReleased;
        public bool EvadeReleased;
        public bool LeftSkillReleased;
        public bool RightSkillReleased;
        public bool ChangeToSlot1Released;
        public bool ChangeToSlot2Released;
        public bool ChangeToSlot3Released;
        public bool ChangeToSlot4Released;
        
        // Previous frame states
        public bool PrevAttack;
        public bool PrevEvade;
        public bool PrevLeftSkill;
        public bool PrevRightSkill;
        public bool PrevChangetoSlot1;
        public bool PrevChangeToSlot2;
        public bool PrevChangeToSlot3;
        public bool PrevChangeToSlot4;

        public void UpdateEvents()
        {
            AttackPressed = AttackHeld && !PrevAttack;
            AttackReleased = !AttackHeld && PrevAttack;

            EvadePressed = EvadeHeld && !PrevEvade;
            EvadeReleased = !EvadeHeld && PrevEvade;

            LeftSkillPressed = LeftSkillHeld && !PrevLeftSkill;
            LeftSkillReleased = !LeftSkillHeld && PrevLeftSkill;
            
            RightSkillPressed = RightSkillHeld && !PrevRightSkill;
            RightSkillReleased = !RightSkillHeld && PrevRightSkill;

            ChangeToSlot1Pressed = ChangeToSlot1Held && !PrevChangetoSlot1;
            ChangeToSlot1Released = !ChangeToSlot1Held && PrevChangetoSlot1;

            ChangeToSlot2Pressed = ChangeToSlot2Held && !PrevChangeToSlot2;
            ChangeToSlot2Released = !ChangeToSlot2Held && PrevChangeToSlot2;

            ChangeToSlot3Pressed = ChangeToSlot3Held && !PrevChangeToSlot3;
            ChangeToSlot3Released = !ChangeToSlot3Held && PrevChangeToSlot3;

            ChangeToSlot4Pressed = ChangeToSlot4Held && !PrevChangeToSlot4;
            ChangeToSlot4Released = !ChangeToSlot4Held && PrevChangeToSlot4;

            // Update previous frame states
            PrevAttack = AttackHeld;
            PrevEvade = EvadeHeld;
            PrevLeftSkill = LeftSkillHeld;
            PrevRightSkill = RightSkillHeld;
            PrevChangetoSlot1 = ChangeToSlot1Held;
            PrevChangeToSlot2 = ChangeToSlot2Held;
            PrevChangeToSlot3 = ChangeToSlot3Held;
            PrevChangeToSlot4 = ChangeToSlot4Held;
        }

        public void Clear()
        {
            // Movement
            Move = Vector2.zero;

            // Held states
            AttackHeld = false;
            EvadeHeld = false;
            LeftSkillHeld = false;
            RightSkillHeld = false;
            ChangeToSlot1Held = false;
            ChangeToSlot2Held = false;
            ChangeToSlot3Held = false;
            ChangeToSlot4Held = false;

            // Press events
            AttackPressed = false;
            EvadePressed = false;
            LeftSkillPressed = false;
            RightSkillPressed = false;
            ChangeToSlot1Pressed = false;
            ChangeToSlot2Pressed = false;
            ChangeToSlot3Pressed = false;
            ChangeToSlot4Pressed = false;

            // Release events
            AttackReleased = false;
            EvadeReleased = false;
            LeftSkillReleased = false;
            RightSkillReleased = false;
            ChangeToSlot1Released = false;
            ChangeToSlot2Released = false;
            ChangeToSlot3Released = false;
            ChangeToSlot4Released = false;

            // Previous frame states
            PrevAttack = false;
            PrevEvade = false;
            PrevLeftSkill = false;
            PrevRightSkill = false;
            PrevChangetoSlot1 = false;
            PrevChangeToSlot2 = false;
            PrevChangeToSlot3 = false;
            PrevChangeToSlot4 = false;
        }
    }
}