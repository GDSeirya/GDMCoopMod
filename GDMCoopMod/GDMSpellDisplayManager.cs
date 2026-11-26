using Game;
using GDMCoopMod;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GDMSpellDisplayManager : MonoBehaviour
{
    Vector2[] hpBarRef = new Vector2[4]
    {
        new Vector2(1560, 375),
        new Vector2(1560, 475),
        new Vector2(1560, 575),
        new Vector2(1560, 675)
    };

    // ------------------------------------------------------------------
    // Ally target indicator (arrow + player number) for support spells.
    // ------------------------------------------------------------------
    private const string ArrowGlyph = ">";
    private const float IndicatorWidth = 55f;     // width reserved for one "> P2" label
    private const float IndicatorHeight = 30f;
    private const float IndicatorGap = 120f;      // gap between the indicator and the party member's HUD slot
    private const float IndicatorStackGap = 2f;   // gap between indicators when several players target the same ally
    private const float IndicatorOffsetY = 6f;    // vertical offset from the top of the party member's HUD slot

    // Colour per controller index (controller 1 = Player 2, etc.) so each player's marker is easy to tell apart.
    private static readonly Color[] playerColors = new Color[4]
    {
        new Color(1.00f, 1.00f, 1.00f), // Player 1 White
        new Color(0.35f, 0.85f, 1.00f), // Player 2 Blue
        new Color(1.00f, 0.90f, 0.30f), // Player 3 Yellow
        new Color(0.55f, 1.00f, 0.45f)  // Player 4 Green
    };

    /// <summary>
    /// Draws "> P#" to the left of the party member's HUD slot that this player's support spell will hit.
    /// Nothing is drawn while the player is using the game's normal targeting.
    /// </summary>
    private void DrawAllyTargetIndicator(
        int casterPartyIndex,
        int controllerIndex,
        int[] stackCounts,
        int partyCount,
        float scaleX,
        float scaleY,
        GUIStyle baseStyle)
    {
        // Normal targeting has no chosen ally, so there is nothing to point at.
        if (BattleAIControllerPatch.IsNormalSupportTargeting(casterPartyIndex))
        {
            return;
        }

        // Check if is valid, otherwise dip out
        int target = BattleAIControllerPatch.GetAllyTargetIndex()[casterPartyIndex];
        if (target < 0 || target >= partyCount || target >= hpBarRef.Length)
        {
            return;
        }

        // Players aiming at the same ally are lined up side by side, further to the left.
        float right = hpBarRef[target].x - IndicatorGap - stackCounts[target] * (IndicatorWidth + IndicatorStackGap);
        stackCounts[target]++;

        float x = (right - IndicatorWidth) * scaleX;
        float y = (hpBarRef[target].y + IndicatorOffsetY) * scaleY;

        Color playerColor = playerColors[Mathf.Clamp(controllerIndex, 0, playerColors.Length - 1)];

        GUIStyle labelStyle = new GUIStyle(baseStyle);
        labelStyle.normal.textColor = playerColor;
        labelStyle.alignment = TextAnchor.MiddleRight;

        string text = ArrowGlyph + " P" + (controllerIndex + 1);

        DrawOutlinedText(
            x,
            y,
            IndicatorWidth * scaleX,
            IndicatorHeight * scaleY,
            text,
            labelStyle,
            new Color(0.03f, 0.12f, 0.35f)
        );
    }

    /// <summary>
    /// Takes a BattleSkillID and returns it as a string.
    /// </summary>
    /// <returns>
    /// A string representing derived from BattleSkillID is returned.
    /// </returns>
    private string GetSkillString(BattleSkillID skillId)
    {
        return ParameterManager.Instance.GetConstBattleSkillMessage(skillId).name;
    }

    /// <summary>
    /// Draws a list of all spells enabled in the party management on a selected party index.
    /// </summary>
    private void DrawSpellSelection(
        List<BattleSkillID> spells,
        int selectedIndex,
        float x,
        float y,
        float spacing,
        float scaleX,
        float scaleY,
        GUIStyle style,
        Color shadowColor)
    {
        if (spells == null || spells.Count == 0)
        {
            return;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, spells.Count - 1);

        int previousIndex = (selectedIndex - 1 + spells.Count) % spells.Count;
        int nextIndex = (selectedIndex + 1) % spells.Count;

        float width = 300 * scaleX;
        float height = 30 * scaleY;
        float scaledSpacing = spacing * scaleY;

        // Only one spell: draw just the current spell.
        if (spells.Count == 1)
        {
            DrawOutlinedText(
                x,
                y,
                width,
                height,
                GetSkillString(spells[0]),
                style,
                shadowColor
            );
            return;
        }
        
        // Create a darker and more transparent style for previous/next.
        GUIStyle adjacentStyle = new GUIStyle(style);
        Color currentColor = style.normal.textColor;

        adjacentStyle.normal.textColor = new Color(
            currentColor.r * 0.5f,
            currentColor.g * 0.5f,
            currentColor.b * 0.5f,
            currentColor.a * 0.5f
        );

        Color adjacentShadow = new Color(
            shadowColor.r * 0.5f,
            shadowColor.g * 0.5f,
            shadowColor.b * 0.5f,
            shadowColor.a * 0.5f
        );

        // Previous spell - above current
        DrawOutlinedText(
            x,
            y - scaledSpacing,
            width,
            height,
            GetSkillString(spells[previousIndex]),
            adjacentStyle,
            adjacentShadow
        );

        // Next spell - below current
        DrawOutlinedText(
            x,
            y + scaledSpacing,
            width,
            height,
            GetSkillString(spells[nextIndex]),
            adjacentStyle,
            adjacentShadow
        );

        // Current spell - drawn last so it appears on top.
        DrawOutlinedText(
            x,
            y,
            width,
            height,
            GetSkillString(spells[selectedIndex]),
            style,
            shadowColor
        );
    }

    private void DrawOutlinedText(float x, float y, float width, float height, string text, GUIStyle style, Color shadowColor)
    {
        // Copy the style so we don't mutate the original
        GUIStyle outlineStyle = new GUIStyle(style);

        // Shadow/outline color
        outlineStyle.normal.textColor = shadowColor;

        // Outline offsets (1 pixel)
        GUI.Label(new Rect(x - 1, y, width, height), text, outlineStyle);
        GUI.Label(new Rect(x + 1, y, width, height), text, outlineStyle);
        GUI.Label(new Rect(x, y - 1, width, height), text, outlineStyle);
        GUI.Label(new Rect(x, y + 1, width, height), text, outlineStyle);

        // Main text (uses the GUIStyle's own color)
        GUI.Label(new Rect(x, y, width, height), text, style);
    }

    private void OnGUI()
    {
        float scaleX = Screen.width / 1920f;
        float scaleY = Screen.height / 1080f;
        float fontScale = Mathf.Min(scaleX, scaleY);

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(20 * fontScale),
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        StringBuilder sb = new StringBuilder();
        // How many players already point at each party member's slot (to stack their markers).
        int[] allyIndicatorStack = new int[4];
        if (BattleManager.GetInstance() != null)
        {
            if ((BattleState)BattleManager.GetInstance().stateMachine.currentState == BattleState.Playable &&
                !BattleManager.GetInstance().IsDirectingFlag())
            {
                for (int i = 0; i < BattleManager.GetInstance().battlePlayerList.Count; i++)
                {
                    if (GDMControllerRouting.GetControllerForParty(i) != -1)
                    {
                        BattlePlayer battlePlayer = BattleManager.GetInstance().battlePlayerList[i];
                        if (
                            (PlayerID)battlePlayer.CharacterID == PlayerID.RENA ||
                            (PlayerID)battlePlayer.CharacterID == PlayerID.CELINE ||
                            (PlayerID)battlePlayer.CharacterID == PlayerID.LEON ||
                            (PlayerID)battlePlayer.CharacterID == PlayerID.NOEL)
                        {
                            List<BattleSkillID> listOfBattleSkills = BattleAIControllerPatch.GetListOfCharacterSpells()[i];
                            if (listOfBattleSkills.Count > 0)
                            {
                                int selectedSpellIndex = BattleAIControllerPatch.GetSpellIndex()[i];
                                if (selectedSpellIndex != -1)
                                {
                                    BattleSkillID skillId = listOfBattleSkills[selectedSpellIndex];
                                    float x = hpBarRef[i].x * scaleX;
                                    float y = hpBarRef[i].y * scaleY;
                                    DrawSpellSelection(BattleAIControllerPatch.GetListOfCharacterSpells()[i], BattleAIControllerPatch.GetSpellIndex()[i], x, y - (25 * scaleY), 13f, scaleX, scaleY, style, new Color(0.03f, 0.12f, 0.35f));

                                    //Support spell selected: show who it will be cast on (host uses the game's own targeting)
                                    int controllerIndex = GDMControllerRouting.GetControllerForParty(i);
                                    if (controllerIndex >= 1 && !BattleAIControllerPatch.IsHostControlled(i) && BattleAIControllerPatch.IsSupportSpell(skillId))
                                    {
                                        DrawAllyTargetIndicator(
                                            i,
                                            controllerIndex,
                                            allyIndicatorStack,
                                            BattleManager.GetInstance().battlePlayerList.Count,
                                            scaleX,
                                            scaleY,
                                            style);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}