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

        // PRESS events (onPress)
        public bool AttackPressed;
        public bool EvadePressed;
        public bool LeftSkillPressed;
        public bool RightSkillPressed;

        // RELEASE events (onRelease)
        public bool AttackReleased;
        public bool EvadeReleased;
        public bool LeftSkillReleased;
        public bool RightSkillReleased;

        // Previous frame states
        public bool PrevAttack;
        public bool PrevEvade;
        public bool PrevLeftSkill;
        public bool PrevRightSkill;

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

            // Update previous frame states
            PrevAttack = AttackHeld;
            PrevEvade = EvadeHeld;
            PrevLeftSkill = LeftSkillHeld;
            PrevRightSkill = RightSkillHeld;
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

            // Press events
            AttackPressed = false;
            EvadePressed = false;
            LeftSkillPressed = false;
            RightSkillPressed = false;

            // Release events
            AttackReleased = false;
            EvadeReleased = false;
            LeftSkillReleased = false;
            RightSkillReleased = false;

            // Previous frame states
            PrevAttack = false;
            PrevEvade = false;
            PrevLeftSkill = false;
            PrevRightSkill = false;
        }
    }
}