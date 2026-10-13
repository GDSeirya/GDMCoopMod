#if DEBUG
using Common;
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

            if (CameraManager.instance != null && BattleManager.instance != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Extended Debug");
                sb.AppendLine($"IsBattleSkillCameraTask {BattleManager.instance.IsBattleSkillCameraTask()}");

                if (BattleManager.instance != null)
                {
                    if (BattleManager.instance.battleSkillCameraTask != null)
                    {
                        sb.AppendLine($"BattleSkillCameraTaskCurrentState {BattleManager.instance.battleSkillCameraTask.currentState}");
                        if (BattleManager.instance.battleSkillCameraTask.taskComponent != null)
                        {
                            sb.AppendLine($"Camera Task Component {BattleManager.instance.battleSkillCameraTask.taskComponent.ToString()}");
                        }


                    }

                    sb.AppendLine($"POS {BattleManager.instance.cameraTargetLocator.transform.position.x:0.00}, {BattleManager.instance.cameraTargetLocator.transform.position.y:0.00}, {BattleManager.instance.cameraTargetLocator.transform.position.z:0.00}");
                    if (BattleManager.instance.virtualCamera != null)
                    {
                        sb.AppendLine($"POSV {BattleManager.instance.virtualCamera.transform.position.x:0.00}, {BattleManager.instance.virtualCamera.transform.position.y:0.00}, {BattleManager.instance.virtualCamera.transform.position.z:0.00}");
                        sb.AppendLine($"Name {BattleManager.instance.virtualCamera.Name}");
                        sb.AppendLine($"Descriptor {BattleManager.instance.virtualCamera.Description}");
                        sb.AppendLine($"HasChanged {BattleManager.instance.virtualCamera.FollowTargetChanged}");
                        sb.AppendLine($"LookatChanged {BattleManager.instance.virtualCamera.LookAtTargetChanged}");
                        sb.AppendLine($"Priority {BattleManager.instance.virtualCamera.Priority}");
                        sb.AppendLine($"ValidPrevState {BattleManager.instance.virtualCamera.PreviousStateIsValid}");

                    }

                }



                GUI.Label(
                        new Rect(startX + 500, startY + (rowHeight), 1000, rowHeight),
                        sb.ToString(),
                        style
                    );
            }
            

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