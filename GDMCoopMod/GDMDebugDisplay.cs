#if DEBUG
using Game;
using SimpleSpritePacker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GDMCoopMod
{
    public class GDMDebugDisplay : MonoBehaviour
    {
        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                normal = { textColor = Color.yellow }
            };

            int startX = 20;
            int startY = 20;
            int rowHeight = 30;


            if (BattleManager.GetInstance() != null)
            {
                for (int i = 0; i < BattleManager.GetInstance().battlePlayerList.Count; i++)
                {
                    BattlePlayer battlePlayer = BattleManager.GetInstance().battlePlayerList[i];
                    if (battlePlayer != null)
                    {
                        
                        GUI.Label(
                        new Rect(startX, startY + (i * rowHeight), 1000, rowHeight),
                        $"[BATTLESTATE [{(PlayerID)battlePlayer.CharacterID}]]" +
                        $"[Player [{(PlayerID)battlePlayer.CharacterID}]]" +
                        //$"controllingId {BattleManager.GetInstance().controlPlayerIndex}, " +
                        //$"indexInParty {battlePlayer.indexInParty}, " +
                        $"[charState {battlePlayer.GetCharacterController().GetCurrentState()}]" +
                        $"[battleSkillId {battlePlayer.battleAIController.actionParameter.BattleSkillID}]" +
                        $"[reserveSkill {battlePlayer.GetCharacterController().reserveBattleSkillID}]",
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