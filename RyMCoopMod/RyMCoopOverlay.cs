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

public class RyMCoopOverlay : MonoBehaviour
{
    private bool showOverlay = true;
    private Gamepad playerOneController;
    private KeyboardShortcut toggleKey = new KeyboardShortcut(KeyCode.F1);
    private RyMOverlayEntry overlayEntry;
    public void Start()
    {
        overlayEntry = new RyMOverlayEntry(250, 25);
        RyMOverlayManager.Register(overlayEntry);
    }

    public void Update()
    {
        if (toggleKey.IsDown())
        {
            showOverlay = !showOverlay;
        }

        // Refresh controller reference
        if (Gamepad.all.Count > 0)
        {
            playerOneController = Gamepad.all[0];
        }
        else
        {
            playerOneController = null;
        }
    }

    public void OnDestroy()
    {
        if (overlayEntry != null)
        {
            RyMOverlayManager.Unregister(overlayEntry);
        }
    }

    private void DrawButton(float x, float y, string name, bool pressed)
    {
        GUI.color = pressed ? Color.green : Color.white;

        GUI.Box(
            new Rect(x, y, 70, 25),
            name
        );

        GUI.color = Color.white;
    }

    private void DrawAnalogButton(float x, float y, string name, float value)
    {
        bool pressed = value > 0.1f;

        GUI.color = pressed ? Color.green : Color.white;

        GUI.Box(
            new Rect(x, y, 70, 25),
            $"{name}: {value:F2}"
        );

        GUI.color = Color.white;
    }

    public void OnGUI()
    {
        if (!showOverlay)
            return;

        Rect overlayRect = RyMOverlayManager.GetRect(overlayEntry);

        /*
        GUI.Box(
            overlayRect,
            "RyM Coop Debug"
        );

        GUI.Label(
            new Rect(
                overlayRect.x + 10,
                overlayRect.y + 30,
                200,
                25
            ),
            $"Money: {playerMoney}"
        );
        */
        GUI.Box(
            overlayRect,
            "Player 1 Controller"
        );

        if (playerOneController == null)
        {
            GUI.Label(
                new Rect(overlayRect.x + 10, overlayRect.y + 30, 250, 25),
                "No controller detected"
            );
            return;
        }

        float x = overlayRect.x + 10;
        float y = overlayRect.y + 30;

        // Face buttons
        DrawButton(x + 90, y, "Y",
            playerOneController.buttonNorth.isPressed);

        DrawButton(x + 180, y + 30, "B",
            playerOneController.buttonEast.isPressed);

        DrawButton(x, y + 30, "X",
            playerOneController.buttonWest.isPressed);

        DrawButton(x + 90, y + 30, "A",
            playerOneController.buttonSouth.isPressed);


        // Shoulders
        DrawButton(x, y + 80, "L1",
            playerOneController.leftShoulder.isPressed);

        DrawButton(x + 180, y + 80, "R1",
            playerOneController.rightShoulder.isPressed);


        // Triggers (analog)
        DrawAnalogButton(
            x,
            y + 115,
            "L2",
            playerOneController.leftTrigger.ReadValue()
        );

        DrawAnalogButton(
            x + 180,
            y + 115,
            "R2",
            playerOneController.rightTrigger.ReadValue()
        );


        // DPad
        DrawButton(x + 80, y + 160, "UP",
            playerOneController.dpad.up.isPressed);

        DrawButton(x + 80, y + 210, "DOWN",
            playerOneController.dpad.down.isPressed);

        DrawButton(x + 30, y + 185, "LEFT",
            playerOneController.dpad.left.isPressed);

        DrawButton(x + 130, y + 185, "RIGHT",
            playerOneController.dpad.right.isPressed);


        // Stick buttons
        DrawButton(x + 250, y + 180, "L3",
            playerOneController.leftStickButton.isPressed);

        DrawButton(x + 250, y + 215, "R3",
            playerOneController.rightStickButton.isPressed);


        // Stick values
        Vector2 leftStick = playerOneController.leftStick.ReadValue();
        Vector2 rightStick = playerOneController.rightStick.ReadValue();

        GUI.Label(
            new Rect(x, y + 260, 250, 20),
            $"Left: {leftStick.x:F2}, {leftStick.y:F2}"
        );

        GUI.Label(
            new Rect(x, y + 280, 250, 20),
            $"Right: {rightStick.x:F2}, {rightStick.y:F2}"
        );
    }
}