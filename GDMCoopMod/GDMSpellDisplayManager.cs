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

    private string GetSkillString(BattleSkillID skillId)
    {
        return ParameterManager.Instance.GetConstBattleSkillMessage(skillId).name;
    }

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
            return;

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
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}