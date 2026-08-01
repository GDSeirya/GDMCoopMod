using BepInEx.Logging;
using BepInEx.Unity.IL2CPP.Configuration;
using Common;
using Game;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;
using static Common.InputManager;

[HarmonyPatch(typeof(BattleAIController), "AIRun")]
public static class BattleAIControllerPatch
{
    private static KeyboardShortcut enablePlayerAi = new KeyboardShortcut(KeyCode.F5);
    private static KeyboardShortcut disablePlayerAi = new KeyboardShortcut(KeyCode.F6);
    private static bool isPartyAiEnabled = false;

    [HarmonyPrefix]
    public static void Update()
    {
        if (enablePlayerAi.IsDown())
        {
            isPartyAiEnabled = true;
            RyMCoopPlugin.StaticLog.LogInfo($"Enabled Party AI {isPartyAiEnabled.ToString()}");
        }
        if (disablePlayerAi.IsDown())
        {
            isPartyAiEnabled = false;
            RyMCoopPlugin.StaticLog.LogInfo($"Disabled Party AI {isPartyAiEnabled.ToString()}");
        }
    }

    static class AiInitTracker
    {
        public static readonly HashSet<BattleAIController> Initialized
            = new HashSet<BattleAIController>();
    }


    [HarmonyPrefix]
    public static bool Prefix(ref BattleAIController __instance)
    {
        //If playerAI is Enabled
        if (!isPartyAiEnabled)
        {
            //Check if Player Battler
            if (__instance.OwnerObject.GetBattleCharacterType() == BattleCharacterType.Player)
            {
                if (!AiInitTracker.Initialized.Contains(__instance))
                {
                    AiInitTracker.Initialized.Add(__instance);
                    return true; // allow original AIRun ONCE
                }
                if (__instance.OwnerObject.indexInParty == BattleManager.GetInstance().ControlPlayerIndex)
                {
                    return true; //If Player 1, run as usual
                }
                //get controller based on partyIndex assigned earlier
                int controllerIndex = RyMCoopMod.RyMControllerRouting.GetControllerForParty(__instance.OwnerObject.indexInParty);

                if (controllerIndex >= 0 && controllerIndex < 4)
                {
                    //Reset combo link if guard, or damaged
                    if (__instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Guard || __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Damage)
                    {
                        if (__instance.OwnerObject.GetCharacterController().battleSkillLeftIndex != 0) __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex = 0;
                        if (__instance.OwnerObject.GetCharacterController().battleSkillRightIndex != 0) __instance.OwnerObject.GetCharacterController().battleSkillRightIndex = 0;
                        if (__instance.OwnerObject.GetCharacterController().normalAttackIndex != 0) __instance.OwnerObject.GetCharacterController().normalAttackIndex = 0;
                    }

                    //Init Variables
                    BattleCharacterController battleCharacterController = __instance.OwnerObject.GetCharacterController();
                    if (battleCharacterController != null)
                    {

                        //Move Logic
                        Vector2 moveVector = RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).Move;
                        float magnitude = moveVector.magnitude;
                        if (magnitude >= 0.1f) //deadzone
                        {
                            Vector2 normalized = moveVector.normalized;
                            if (__instance.enableAIMove) __instance.enableAIMove = false;
                            battleCharacterController.OnMove(new Vector3(normalized.x, 0, normalized.y), __instance.aiMoveController.moveSpeedRate);
                        }

                        //Attack Logic
                        if (RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).AttackPressed)
                        {
                            // INVALID=0, CLAUDE=1, RENA=2 CELINE=3, BOWMAN=4, DIAS=5, PRECIS=6, ASHTON=7, LEON=8, OPERA=9, ERNEST=10, NOEL=11, CHISATO=12, WELCH=13, MAX=14
                            //GET PLAYER ID so we know what character
                            PlayerID playerId = (PlayerID)__instance.OwnerObject.CharacterID;

                            //get skill in reserve
                            //BattleSkillID reservedSkillId = __instance.OwnerObject.GetCharacterController().reserveBattleSkillID;
                            //get next normal attack skill
                            BattleSkillID skillId = __instance.OwnerObject.GetCharacterController().GetNextNormalAttackSkillID();
                            //if invalid, get default normal attack
                            if (skillId == BattleSkillID.INVALID)
                            {
                                switch (playerId)
                                {
                                    case PlayerID.CLAUDE:
                                        skillId = BattleSkillID.CLAUDE_NORMAL_ATTACK_01; //has combo
                                        break;
                                    case PlayerID.RENA:
                                        skillId = BattleSkillID.RENA_NORMAL_ATTACK_01;
                                        break;
                                    case PlayerID.CELINE:
                                        skillId = BattleSkillID.CELINE_NORMAL_ATTACK_01;
                                        break;
                                    case PlayerID.BOWMAN:
                                        skillId = BattleSkillID.BOWMAN_NORMAL_ATTACK_01; //has combo
                                        break;
                                    case PlayerID.DIAS:
                                        skillId = BattleSkillID.DIAS_NORMAL_ATTACK_01; //has combo
                                        break;
                                    case PlayerID.PRECIS:
                                        skillId = BattleSkillID.PRECIS_NORMAL_ATTACK_01; //has combo
                                        break;
                                    case PlayerID.ASHTON:
                                        skillId = BattleSkillID.ASHTON_NORMAL_ATTACK_01; //has combo
                                        break;
                                    case PlayerID.LEON:
                                        skillId = BattleSkillID.LEON_NORMAL_ATTACK_01;
                                        break;
                                    case PlayerID.OPERA:
                                        skillId = BattleSkillID.OPERA_NORMAL_ATTACK_01; //has combo
                                        break;
                                    case PlayerID.ERNEST:
                                        skillId = BattleSkillID.ERNEST_NORMAL_ATTACK_01; //has combo
                                        break;
                                    case PlayerID.NOEL:
                                        skillId = BattleSkillID.NOEL_NORMAL_ATTACK_01;
                                        break;
                                    case PlayerID.CHISATO:
                                        skillId = BattleSkillID.CHISATO_NORMAL_ATTACK_01; //has combo
                                        break;
                                    case PlayerID.WELCH:
                                        skillId = BattleSkillID.WELCH_NORMAL_ATTACK_01; //has combo
                                        break;
                                    default:
                                        break;
                                }
                            }
                            //if valid, do action
                            if (skillId != BattleSkillID.INVALID)
                            {
                                bool rangeFinder = BattleUtility.IsLongRange(__instance.OwnerObject, BattleManager.GetInstance().GetControlPlayerTarget());
                                //__instance.SetActionParameter(BattleManager.GetInstance().GetControlPlayerTarget(), skillId, rangeFinder, false);
                                battleCharacterController.ReserveAction(skillId, BattleManager.GetInstance().GetControlPlayerTarget());
                                __instance.rootBehavior.AIRun();
                                return true;
                            }
                        }

                        //Left Skill Logic
                        if (RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).LeftSkillPressed)
                        {
                            //get get left skill index
                            if (battleCharacterController.battleSkillLeftIndex > 1)
                                battleCharacterController.battleSkillLeftIndex = 0;
                            //store left skill index
                            int battleSkillComboIndex = battleCharacterController.battleSkillLeftIndex;
                            //get left skill id
                            BattleSkillID skillId = (BattleSkillID)battleCharacterController.GetNextBattleSkillID(BattleDefine.RootType.Left);

                            //get range type of current target
                            bool rangeFinder = BattleUtility.IsLongRange(__instance.OwnerObject, BattleManager.GetInstance().GetControlPlayerTarget());

                            //if skill isn't invalid, perform skill
                            if (skillId != BattleSkillID.INVALID)
                            {
                                //send skill battle character controller
                                battleCharacterController.OnBattleSkill(skillId, BattleManager.GetInstance().GetControlPlayerTarget(), BattleDefine.RootType.Left);
                                //execute battle character skill
                                __instance.rootBehavior.AIRun();
                                return true;
                            }
                        }

                        //Right Skill Logic
                        if (RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).RightSkillPressed)
                        {
                            RyMCoopPlugin.StaticLog.LogInfo($"{controllerIndex} RIGHT SKILL PRESSEd");
                            //get get left skill index
                            if (battleCharacterController.battleSkillRightIndex > 1)
                                battleCharacterController.battleSkillRightIndex = 0;
                            //store left skill index
                            int battleSkillComboIndex = battleCharacterController.battleSkillRightIndex;
                            //get left skill id
                            BattleSkillID skillId = (BattleSkillID)battleCharacterController.GetNextBattleSkillID(BattleDefine.RootType.Right);

                            //get range type of current target
                            bool rangeFinder = BattleUtility.IsLongRange(__instance.OwnerObject, BattleManager.GetInstance().GetControlPlayerTarget());

                            //if skill isn't invalid, perform skill
                            if (skillId != BattleSkillID.INVALID)
                            {
                                //send skill battle character controller
                                battleCharacterController.OnBattleSkill(skillId, BattleManager.GetInstance().GetControlPlayerTarget(), BattleDefine.RootType.Right);
                                //execute battle character skill
                                __instance.rootBehavior.AIRun();
                                return true;
                            }
                        }

                        //Evade logic
                        if (RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).EvadePressed)
                        {
                            RyMCoopPlugin.StaticLog.LogInfo($"{controllerIndex}: EVADE pressed");
                            //TODO

                            battleCharacterController.OnStepAvoid();
                            __instance.rootBehavior.AIRun();
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    //business as usual if no controller is returned
                    return true;
                }
            }
        }
        //Don't modify anything, run AI controller as usual
        return true;
    }
}