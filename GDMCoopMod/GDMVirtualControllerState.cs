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
        public bool TargetingModeHeld;
        public bool UseMagicHeld;

        // PRESS events (onPress)
        public bool AttackPressed;
        public bool EvadePressed;
        public bool LeftSkillPressed;
        public bool RightSkillPressed;
        public bool ChangeToSlot1Pressed;
        public bool ChangeToSlot2Pressed;
        public bool ChangeToSlot3Pressed;
        public bool ChangeToSlot4Pressed;
        public bool TargetingModePressed;
        public bool UseMagicPressed;

        // RELEASE events (onRelease)
        public bool AttackReleased;
        public bool EvadeReleased;
        public bool LeftSkillReleased;
        public bool RightSkillReleased;
        public bool ChangeToSlot1Released;
        public bool ChangeToSlot2Released;
        public bool ChangeToSlot3Released;
        public bool ChangeToSlot4Released;
        public bool TargetingModeReleased;
        public bool UseMagicReleased;
        
        // Previous frame states
        public bool PrevAttack;
        public bool PrevEvade;
        public bool PrevLeftSkill;
        public bool PrevRightSkill;
        public bool PrevChangetoSlot1;
        public bool PrevChangeToSlot2;
        public bool PrevChangeToSlot3;
        public bool PrevChangeToSlot4;
        public bool PrevTargetingMode;
        public bool PrevUseMagic;

        // Right stick (analog)
        public Vector2 RightStick;

        // Spell scrolling
        public bool SpellPreviousPressed;
        public bool SpellNextPressed;

        // Right-stick scrolling state
        private bool RightStickScrolling;
        private float RightStickHoldTime;
        private float RightStickRepeatTimer;
        private int RightStickDirection;

        public void UpdateEvents(float deltaTime)
        {
            // Reset one-frame events.
            SpellPreviousPressed = false;
            SpellNextPressed = false;

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

            TargetingModePressed = TargetingModeHeld && !PrevTargetingMode;
            TargetingModeReleased = !TargetingModeHeld && PrevTargetingMode;

            UseMagicPressed = UseMagicHeld && !PrevUseMagic;
            UseMagicReleased = !UseMagicHeld && PrevUseMagic;

            UpdateSpellScrolling(deltaTime);

            // Update previous frame states
            PrevAttack = AttackHeld;
            PrevEvade = EvadeHeld;
            PrevLeftSkill = LeftSkillHeld;
            PrevRightSkill = RightSkillHeld;
            PrevChangetoSlot1 = ChangeToSlot1Held;
            PrevChangeToSlot2 = ChangeToSlot2Held;
            PrevChangeToSlot3 = ChangeToSlot3Held;
            PrevChangeToSlot4 = ChangeToSlot4Held;
            PrevTargetingMode = TargetingModeHeld;
            PrevUseMagic = UseMagicHeld;
        }

        public void Clear()
        {
            // Movement
            Move = Vector2.zero;
            RightStick = Vector2.zero;

            // Held states
            AttackHeld = false;
            EvadeHeld = false;
            LeftSkillHeld = false;
            RightSkillHeld = false;
            ChangeToSlot1Held = false;
            ChangeToSlot2Held = false;
            ChangeToSlot3Held = false;
            ChangeToSlot4Held = false;
            TargetingModeHeld = false;
            UseMagicHeld = false;

            // Press events
            AttackPressed = false;
            EvadePressed = false;
            LeftSkillPressed = false;
            RightSkillPressed = false;
            ChangeToSlot1Pressed = false;
            ChangeToSlot2Pressed = false;
            ChangeToSlot3Pressed = false;
            ChangeToSlot4Pressed = false;
            TargetingModePressed = false;
            UseMagicPressed = false;

            // Release events
            AttackReleased = false;
            EvadeReleased = false;
            LeftSkillReleased = false;
            RightSkillReleased = false;
            ChangeToSlot1Released = false;
            ChangeToSlot2Released = false;
            ChangeToSlot3Released = false;
            ChangeToSlot4Released = false;
            TargetingModeReleased = false;
            UseMagicReleased = false;

            // Previous frame states
            PrevAttack = false;
            PrevEvade = false;
            PrevLeftSkill = false;
            PrevRightSkill = false;
            PrevChangetoSlot1 = false;
            PrevChangeToSlot2 = false;
            PrevChangeToSlot3 = false;
            PrevChangeToSlot4 = false;
            PrevTargetingMode = false;
            PrevUseMagic = false;

            RightStickScrolling = false;
            RightStickHoldTime = 0f;
            RightStickRepeatTimer = 0f;
            RightStickDirection = 0;
        }

        private void UpdateSpellScrolling(float deltaTime)
        {
            const float flickThreshold = 0.7f;
            const float resetThreshold = 0.3f;
            const float holdDelay = 0.4f;

            // Stick returned to center.
            if (Mathf.Abs(RightStick.y) < resetThreshold)
            {
                RightStickScrolling = false;
                RightStickHoldTime = 0f;
                RightStickRepeatTimer = 0f;
                RightStickDirection = 0;

                return;
            }

            int direction = 0;

            // Up = previous
            if (RightStick.y > flickThreshold)
                direction = -1;

            // Down = next
            else if (RightStick.y < -flickThreshold)
                direction = 1;

            if (direction == 0)
                return;

            // First flick.
            if (!RightStickScrolling)
            {
                RightStickScrolling = true;
                RightStickDirection = direction;
                RightStickHoldTime = 0f;
                RightStickRepeatTimer = 0f;

                TriggerSpellScroll(direction);

                return;
            }

            // Direction changed while the stick is still held.
            if (RightStickDirection != direction)
            {
                RightStickDirection = direction;
                RightStickHoldTime = 0f;
                RightStickRepeatTimer = 0f;

                TriggerSpellScroll(direction);

                return;
            }

            RightStickHoldTime += deltaTime;

            // Wait before starting automatic scrolling.
            if (RightStickHoldTime < holdDelay)
                return;

            RightStickRepeatTimer -= deltaTime;

            if (RightStickRepeatTimer > 0f)
                return;

            TriggerSpellScroll(direction);

            // Gradually increase scrolling speed.
            float repeatInterval = Mathf.Lerp(
                0.15f,
                0.07f,
                Mathf.Clamp01((RightStickHoldTime - holdDelay) / 2f)
            );

            RightStickRepeatTimer = repeatInterval;
        }

        private void TriggerSpellScroll(int direction)
        {
            if (direction < 0)
                SpellPreviousPressed = true;
            else
                SpellNextPressed = true;
        }

    }
}