#if DEBUG
using Game;
using System.Text;
using UnityEngine;

namespace GDMCoopMod
{
    public class GDMDebugDisplay : MonoBehaviour
    {
        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                normal = { textColor = Color.yellow }
            };

            GUIStyle styleShadow = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                normal = { textColor = Color.black }
            };

            int startX = 20;
            int startY = 20;
            int rowHeight = 30 * 10;

            
            if (BattleManager.GetInstance() != null)
            {
                for (int i = 0; i < BattleManager.GetInstance().battlePlayerList.Count; i++)
                {
                    BattlePlayer battlePlayer = BattleManager.GetInstance().battlePlayerList[i];
                    if (battlePlayer != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"[Player [{(PlayerID)battlePlayer.CharacterID}]");
                        sb.AppendLine($"[charState {battlePlayer.GetCharacterController().GetCurrentState()}]");
                        sb.AppendLine($"[isLinkComboAction {battlePlayer.GetCharacterController().IsLinkComboAction()}]");
                        sb.AppendLine($"[normalAtkIndex {battlePlayer.GetCharacterController().normalAttackIndex}]");
                        sb.AppendLine($"[bSkillRootType {battlePlayer.GetCharacterController().BattleSkillRootType}]");
                        sb.AppendLine($"[battleSkillId {battlePlayer.battleAIController.actionParameter.BattleSkillID}]");
                        sb.AppendLine($"[reserveSkill {battlePlayer.GetCharacterController().reserveBattleSkillID}]");
                        sb.AppendLine($"[bskillIndex {battlePlayer.GetCharacterController().battleSkillLeftIndex} - {battlePlayer.GetCharacterController().battleSkillRightIndex}]");
                        sb.AppendLine($"[lastSkillLeft {BattleAIControllerPatch.GetLastBattleSkill(BattleDefine.RootType.Left, i).SkillIndex} - {BattleAIControllerPatch.GetLastBattleSkill(BattleDefine.RootType.Left, i).SkillId}]");
                        sb.AppendLine($"[lastSkillRight {BattleAIControllerPatch.GetLastBattleSkill(BattleDefine.RootType.Right, i).SkillIndex} - {BattleAIControllerPatch.GetLastBattleSkill(BattleDefine.RootType.Right, i).SkillId}]");
                        
                        GUI.Label(
                        new Rect(startX + 1, startY + (i * rowHeight) + 1, 1080, rowHeight),
                        sb.ToString(),
                        styleShadow
                    );
                        GUI.Label(
                        new Rect(startX, startY + (i * rowHeight), 1080, rowHeight),
                        sb.ToString(),
                        style
                    );
                    }
                    


                }
                if (BattleManager.GetInstance().battlePlayerList.Count == 0)
                {
                    GUI.Label(
                        new Rect(startX, startY + (rowHeight), 1000, rowHeight),
                        $"No Battle",
                        style
                    );
                }
            }
            else
            {
                GUI.Label(
                        new Rect(startX, startY + (rowHeight), 1000, rowHeight),
                        $"Definitely No Battle",
                        style
                    );
            }
            
        }
    }
}
#endif