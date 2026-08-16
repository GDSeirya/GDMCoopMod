using Game;
using GDMCoopMod;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

public class GDMCooperativeCamera : MonoBehaviour
{
    private float previousBattleTimer = -1f;
    private int frozenBattleTimerFrames = 0;

    public enum CameraState
    {
        NotInBattle,
        NotPlayable,
        IsPaused,
        IsDirecting,
        SoloMode,
        CoopMode
    }

    private static CameraState state;

    private bool IsBattleTimerFrozen()
    {
        BattleManager battleManager = BattleManager.GetInstance();

        if (battleManager == null)
        {
            previousBattleTimer = -1f;
            frozenBattleTimerFrames = 0;
            return false;
        }

        float currentBattleTimer = battleManager.battleTime;

        if (previousBattleTimer < 0f)
        {
            previousBattleTimer = currentBattleTimer;
            frozenBattleTimerFrames = 0;
            return false;
        }

        if (Mathf.Approximately(currentBattleTimer, previousBattleTimer))
        {
            frozenBattleTimerFrames++;
        }
        else
        {
            frozenBattleTimerFrames = 0;
        }

        previousBattleTimer = currentBattleTimer;

        return frozenBattleTimerFrames >= 2;
    }

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

    private void FocusCameraOnCharacters(List<BattleCharacter> listOfBattlePlayers)
    {
        if (BattleManager.GetInstance() != null)
        {
            List<BattleCharacter> resultBattleCharacters = new List<BattleCharacter>();
            for (int i = 0; i < listOfBattlePlayers.Count; i++)
            {
                resultBattleCharacters.Add(listOfBattlePlayers[i]);
            }
            if (BattleManager.GetInstance().controlPlayerTarget != null)
            resultBattleCharacters.Add(BattleManager.GetInstance().controlPlayerTarget);
            BattleManager.GetInstance().StartCameraTargetSelect(BattleManager.GetInstance().GetControlPlayer(), resultBattleCharacters);
        }
    }

    private List<BattleCharacter> GetListOfFocusableCharacters(List<BattlePlayer> listOfBattleCharacters)
    {
        List<BattleCharacter> resultBattleCharacters = new List<BattleCharacter>();
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
            resultBattleCharacters.Add(BattleManager.GetInstance().controlPlayerTarget);
        }
        return resultBattleCharacters;
    }

    private void ResetBattleCamera()
    {
        BattleManager.GetInstance().EndCameraTargetSelect(true);
    }

    public void Update()
    {
        BattleManager battleManager = BattleManager.GetInstance();

        // No BattleManager = not in battle
        if (battleManager == null)
        {
            state = CameraState.NotInBattle;
            previousBattleTimer = -1f;
            return;
        }

        // ---------------------------------------------------------
        // Camera-blocking states
        // ---------------------------------------------------------

        // Non-playable state, such as a cutscene.
        if ((BattleState)battleManager.stateMachine.currentState != BattleState.Playable)
        {
            if (state == CameraState.CoopMode)
            {
                ResetBattleCamera();
            }

            state = CameraState.NotPlayable;
            return;
        }

        // Battle timer has stopped advancing, meaning the game is paused.
        if (IsBattleTimerFrozen())
        {
            if (state == CameraState.CoopMode)
            {
                ResetBattleCamera();
            }

            state = CameraState.IsPaused;
            return;
        }

        // ---------------------------------------------------------
        // Return from camera-blocking states
        // ---------------------------------------------------------

        if (state == CameraState.NotPlayable ||
            state == CameraState.IsPaused)
        {
            state = CameraState.NotInBattle;
        }

        // ---------------------------------------------------------
        // Normal battle state logic
        // ---------------------------------------------------------

        if (battleManager.battlePlayerList.Count > 0)
        {
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
            else if (battleManager.IsDirectingFlag() &&
                     state != CameraState.IsDirecting)
            {
                if (state == CameraState.CoopMode)
                {
                    ResetBattleCamera();
                }

                state = CameraState.IsDirecting;
            }
            else if (state == CameraState.SoloMode)
            {
                if (GetNumberOfPlayableCharacters() > 1)
                {
                    state = CameraState.CoopMode;
                }
            }
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
                if (!battleManager.IsDirectingFlag())
                {
                    if (GetNumberOfPlayableCharacters() > 1)
                    {
                        state = CameraState.CoopMode;
                    }
                    else if (GetNumberOfPlayableCharacters() == 1)
                    {
                        ResetBattleCamera();
                        state = CameraState.SoloMode;
                    }
                }
            }

            // Only override the camera during cooperative playable gameplay.
            if (state == CameraState.CoopMode &&
                !battleManager.IsDirectingFlag())
            {
                FocusCameraOnCharacters(
                    GetListOfFocusableCharacters(
                        battleManager.battlePlayerList
                    )
                );
            }
        }
    }
}