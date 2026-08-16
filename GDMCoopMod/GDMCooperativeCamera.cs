using Game;
using GDMCoopMod;
using Il2CppSystem.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GDMCooperativeCamera : MonoBehaviour
{
    public enum CameraState
    {
        NotInBattle,
        IsDirecting,
        SoloMode,
        CoopMode
    }

    private static CameraState state;

    public CameraState GetCameraState()
    {
        return state;
    }

    public void Start()
    {
        state = CameraState.NotInBattle;
    }

    private int GetNumberOfPlayableCharacters()
    {
        if (BattleManager.GetInstance() != null)
        {
            if (BattleManager.GetInstance().battlePlayerList.Count > 0)
            {
                int numberOfPlayables = 1; //player 1 always exist
                for (int i = 0; i < BattleManager.GetInstance().battlePlayerList.Count; i++)
                {
                    if (GDMControllerRouting.GetControllerForParty(BattleManager.GetInstance().battlePlayerList[i].indexInParty) >= 0 &&
                        GDMControllerRouting.GetControllerForParty(BattleManager.GetInstance().battlePlayerList[i].indexInParty) <= 3)
                    {
                        numberOfPlayables++;
                    }
                }
                return numberOfPlayables;
            }
        }
        return -1;
    }

    private void FocusCameraOnCharacters(List<BattlePlayer> listOfBattlePlayers)
    {
        if (BattleManager.GetInstance() != null)
        {
            List<BattleCharacter> resultBattleCharacters = new List<BattleCharacter>();
            for (int i = 0; i < listOfBattlePlayers.Count; i++)
            {
                resultBattleCharacters.Add(listOfBattlePlayers[i]);
            }
            BattleManager.GetInstance().StartCameraTargetSelect(BattleManager.GetInstance().GetControlPlayer(), resultBattleCharacters);
        }
    }

    private List<BattlePlayer> GetListOfFocusableCharacters(List<BattlePlayer> listOfBattleCharacters)
    {
        List<BattlePlayer> resultBattleCharacters = new List<BattlePlayer>();
        if (BattleManager.GetInstance() != null)
        {
            for (int i = 0; i < listOfBattleCharacters.Count; i++)
            {
                if (listOfBattleCharacters[i].IsControlPlayer())
                {
                    resultBattleCharacters.Add(listOfBattleCharacters[i]);
                }
                else if (GDMControllerRouting.GetControllerForParty(i) != -1 && listOfBattleCharacters[i].GetCharacterController().GetCurrentState() != BattleCharacterState.Dead) 
                {
                    resultBattleCharacters.Add(listOfBattleCharacters[i]);
                }
            }
        }
        return resultBattleCharacters;
    }

    private void ResetBattleCamera()
    {
        BattleManager.GetInstance().EndCameraTargetSelect(true);
    }

    public void Update()
    {
        //Check if BattleManager instance exists
        if (BattleManager.GetInstance() != null)
        {
            //If player count detected, enter battle
            if (BattleManager.GetInstance().battlePlayerList.Count > 0)
            {
                //State manager
                if (state == CameraState.NotInBattle)
                {
                    if (GetNumberOfPlayableCharacters() == 1)
                    {
                        state = CameraState.SoloMode;
                    }
                    else if (GetNumberOfPlayableCharacters() > 1)
                    {
                        state = CameraState.CoopMode;
                    }
                }
                else if (BattleManager.GetInstance().IsDirectingFlag() && state != CameraState.IsDirecting)
                {
                    if (state == CameraState.CoopMode)
                    {
                        ResetBattleCamera();
                    }
                    state = CameraState.IsDirecting;
                }
                //In solo mode, check if players are added
                else if (state == CameraState.SoloMode)
                {
                    if (GetNumberOfPlayableCharacters() > 1)
                    {
                        state = CameraState.CoopMode;
                    }
                }
                //In coop mode, check if camera is required to be reset
                else if (state == CameraState.CoopMode)
                {
                    if (GetNumberOfPlayableCharacters() == 1)
                    {
                        ResetBattleCamera();
                        state = CameraState.SoloMode;
                    }
                }
                else if (state == CameraState.IsDirecting)
                {
                    if (!BattleManager.GetInstance().IsDirectingFlag())
                    {
                        //In solo mode, check if players are added
                        if (GetNumberOfPlayableCharacters() > 1)
                        {
                            state = CameraState.CoopMode;
                        }
                        //In coop mode, check if camera is required to be reset
                        else if (GetNumberOfPlayableCharacters() == 1)
                        {
                            ResetBattleCamera();
                            state = CameraState.SoloMode;
                        }
                    }
                }
                //always logic
                if (state == CameraState.CoopMode && !BattleManager.GetInstance().IsDirectingFlag())
                {
                    FocusCameraOnCharacters(GetListOfFocusableCharacters(BattleManager.GetInstance().battlePlayerList));
                }
                //end of camera logic
            }
            //default null
        }
        //If no players detected and state is still in battle, set to not in battle
        else if (state != CameraState.NotInBattle) state = CameraState.NotInBattle;
    }
}