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
        public bool EmoteModifierHeld; //right trigger
        public bool WestHeld;
        public bool RightStickClickHeld; //R3

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
        public bool WestPressed;
        public bool RightStickClickPressed;

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
        public bool PrevWest;
        public bool PrevRightStickClick;

        // Right stick (analog)
        public Vector2 RightStick;

        // Spell scrolling (right stick up/down)
        public bool SpellPreviousPressed;
        public bool SpellNextPressed;

        // Ally target scrolling (right stick left/right)
        public bool AllyPreviousPressed;
        public bool AllyNextPressed;

        // Right-stick scrolling state. Each axis keeps its own flick/hold/repeat tracking.
        private readonly StickAxisScroller verticalScroller = new StickAxisScroller();
        private readonly StickAxisScroller horizontalScroller = new StickAxisScroller();

        public void UpdateEvents(float deltaTime)
        {
            // Reset one-frame events.
            SpellPreviousPressed = false;
            SpellNextPressed = false;
            AllyPreviousPressed = false;
            AllyNextPressed = false;

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

            WestPressed = WestHeld && !PrevWest;
            RightStickClickPressed = RightStickClickHeld && !PrevRightStickClick;

            UpdateStickScrolling(deltaTime);

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
            PrevWest = WestHeld;
            PrevRightStickClick = RightStickClickHeld;
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
            EmoteModifierHeld = false;
            WestHeld = false;
            RightStickClickHeld = false;

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
            WestPressed = false;
            RightStickClickPressed = false;

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
            PrevWest = false;
            PrevRightStickClick = false;

            SpellPreviousPressed = false;
            SpellNextPressed = false;
            AllyPreviousPressed = false;
            AllyNextPressed = false;

            verticalScroller.Reset();
            horizontalScroller.Reset();
        }

        private void UpdateStickScrolling(float deltaTime)
        {
            float absX = Mathf.Abs(RightStick.x);
            float absY = Mathf.Abs(RightStick.y);

            // Only the dominant axis may scroll, so a diagonal push doesn't change
            // the spell and the target at the same time.
            bool verticalDominant = absY > absX;
            bool horizontalDominant = absX > absY;

            // Up = previous spell, Down = next spell.
            int spellDirection = verticalScroller.Update(-RightStick.y, verticalDominant, deltaTime);
            if (spellDirection < 0)
                SpellPreviousPressed = true;
            else if (spellDirection > 0)
                SpellNextPressed = true;

            // Left = previous ally, Right = next ally.
            int allyDirection = horizontalScroller.Update(RightStick.x, horizontalDominant, deltaTime);
            if (allyDirection < 0)
                AllyPreviousPressed = true;
            else if (allyDirection > 0)
                AllyNextPressed = true;
        }

        /// <summary>
        /// Turns one stick axis into repeated "previous/next" events:
        /// an immediate event on the first flick, then auto-repeat (speeding up) while held.
        /// </summary>
        private class StickAxisScroller
        {
            private const float FlickThreshold = 0.7f;
            private const float ResetThreshold = 0.3f;
            private const float HoldDelay = 0.4f;

            private bool scrolling;
            private float holdTime;
            private float repeatTimer;
            private int direction;

            public void Reset()
            {
                scrolling = false;
                holdTime = 0f;
                repeatTimer = 0f;
                direction = 0;
            }

            /// <summary>
            /// Returns -1 (negative direction), +1 (positive direction) or 0 (no event this frame).
            /// </summary>
            public int Update(float axisValue, bool isDominantAxis, float deltaTime)
            {
                // Stick returned to center (or the other axis took over).
                if (!isDominantAxis || Mathf.Abs(axisValue) < ResetThreshold)
                {
                    Reset();
                    return 0;
                }

                int newDirection = 0;

                if (axisValue > FlickThreshold)
                    newDirection = 1;
                else if (axisValue < -FlickThreshold)
                    newDirection = -1;

                if (newDirection == 0)
                    return 0;

                // First flick.
                if (!scrolling)
                {
                    scrolling = true;
                    direction = newDirection;
                    holdTime = 0f;
                    repeatTimer = 0f;
                    return newDirection;
                }

                // Direction changed while the stick is still held.
                if (direction != newDirection)
                {
                    direction = newDirection;
                    holdTime = 0f;
                    repeatTimer = 0f;
                    return newDirection;
                }

                holdTime += deltaTime;

                // Wait before starting automatic scrolling.
                if (holdTime < HoldDelay)
                    return 0;

                repeatTimer -= deltaTime;

                if (repeatTimer > 0f)
                    return 0;

                // Gradually increase scrolling speed.
                float repeatInterval = Mathf.Lerp(
                    0.15f,
                    0.07f,
                    Mathf.Clamp01((holdTime - HoldDelay) / 2f)
                );

                repeatTimer = repeatInterval;
                return newDirection;
            }
        }
    }
}