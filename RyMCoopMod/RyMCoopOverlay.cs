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
using UnityEngine.Playables;
using static Common.InputManager;

[HarmonyPatch(typeof(BattleAIController), "AIRun")]
public static class BattleAIControllerPatch
{
    private static KeyboardShortcut enablePlayerAi = new KeyboardShortcut(KeyCode.F5);
    private static KeyboardShortcut disablePlayerAi = new KeyboardShortcut(KeyCode.F6);
    private static KeyboardShortcut testAttackButton1 = new KeyboardShortcut(KeyCode.F7);
    private static KeyboardShortcut testAttackButton2 = new KeyboardShortcut(KeyCode.F8);
    private static KeyboardShortcut testAttackButton3 = new KeyboardShortcut(KeyCode.F9);
    
    private static KeyboardShortcut testMoveLeftButton1 = new KeyboardShortcut(KeyCode.F10);
    private static KeyboardShortcut testMoveLeftButton2 = new KeyboardShortcut(KeyCode.F12);
    private static bool isPlayerAiEnabled = false;
    private static bool isFakeMoving = false;

    [HarmonyPrefix]
    public static void Update()
    {
        if (enablePlayerAi.IsDown())
        {
            isPlayerAiEnabled = true;
            RyMCoopPlugin.StaticLog.LogInfo($"Enable Player AI {isPlayerAiEnabled.ToString()}");
        }
        if (disablePlayerAi.IsDown())
        {
            isPlayerAiEnabled = false;
            RyMCoopPlugin.StaticLog.LogInfo($"Disable Player AI {isPlayerAiEnabled.ToString()}");
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
        if (!isPlayerAiEnabled)
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
                // Simple movement test
                if (testMoveLeftButton1.IsDown())
                {
                    isFakeMoving = true;
                }
                if (testMoveLeftButton2.IsDown())
                {
                    isFakeMoving = false;
                }

                if (isFakeMoving)
                {
                    BattleCharacterController controller = __instance.OwnerObject.GetCharacterController();

                    if (controller != null)
                    {
                        // World-space left
                        controller.OnMove(Vector3.left, 1.0f);
                        //controller.OnMove(new Vector3(2f, 0, 2f), 1.0f);
                    }

                    return false;
                }
                // If test attack button is pressed, run it
                if (testAttackButton1.IsDown())
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
                        __instance.SetActionParameter(BattleManager.GetInstance().GetControlPlayerTarget(), skillId, false, false, BattleDefine.RootType.Invalid);
                        __instance.rootBehavior.AIRun();
                        RyMCoopPlugin.StaticLog.LogInfo($"2Battler Index {__instance.OwnerObject.indexInParty}, SkillId {skillId}");

                    }


                    //__instance.SetActionParameter(BattleManager.GetInstance().GetControlPlayer().BattleAIController.Target, BattleSkillID.DIAS_NORMAL_ATTACK_01, true, false, BattleDefine.RootType.Invalid);
                    //BattleSkillID skillID = __instance.OwnerObject.GetCharacterController().re;                
                    //BattleSkillID skillID = BattleSkillID.DIAS_NORMAL_ATTACK_01;
                    //BattleSkillID skillID = __instance.OwnerObject.GetCharacterController().normalAtta ;
                    //BattleSkillID skillID = __instance.OwnerObject.GetCharacterController().GetNextNormalAttackSkillID();
                    /*
                    if (skillID != BattleSkillID.INVALID)
                    {
                        RyMCoopPlugin.StaticLog.LogInfo($"Attack with id {skillID}");
                        __instance.SetActionParameter(BattleManager.GetInstance().GetControlPlayer().BattleAIController.Target, skillID, true, false, BattleDefine.RootType.Invalid);
                        __instance.rootBehavior.AIRun();
                    }
                    */
                    //BattleCharacterController
                    //BELOW DOES NOTHING
                    //__instance.SetActionParameter(BattleManager.GetInstance().GetControlPlayer().BattleAIController.Target, __instance.OwnerObject.GetCharacterController().GetNextNormalAttackSkillID(), __instance.OwnerObject.GetCharacterController().reserveIsLong, false, __instance.OwnerObject.GetCharacterController().BattleSkillRootType);

                }
                else if (testAttackButton2.IsDown())
                {
                    // INVALID=0, CLAUDE=1, RENA=2 CELINE=3, BOWMAN=4, DIAS=5, PRECIS=6, ASHTON=7, LEON=8, OPERA=9, ERNEST=10, NOEL=11, CHISATO=12, WELCH=13, MAX=14
                    //GET PLAYER ID so we know what character
                    PlayerID playerId = (PlayerID)__instance.OwnerObject.CharacterID;

                    
                    //get get left skill index
                    int battleSkillComboIndex = __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex;
                    BattleSkillID skillId = (BattleSkillID)__instance.OwnerObject.GetCharacterController().GetNextBattleSkillID(BattleDefine.RootType.Left);
                    
                    if (skillId == BattleSkillID.INVALID && battleSkillComboIndex != 0)
                    {
                        __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex = 0;
                        skillId = (BattleSkillID)__instance.OwnerObject.GetCharacterController().GetNextBattleSkillID(BattleDefine.RootType.Left);
                    }

                    bool rangeFinder = BattleUtility.IsLongRange(__instance.OwnerObject, BattleManager.GetInstance().GetControlPlayerTarget());

                    //if invalid, get default normal attack
                    RyMCoopPlugin.StaticLog.LogInfo($"3Battler Index {__instance.OwnerObject.indexInParty}, SkillId {skillId}, IsLong {rangeFinder}, LeftSkillIndex {battleSkillComboIndex}");
                    //if valid, do action

                    if (skillId != BattleSkillID.INVALID)
                    {
                        __instance.SetActionParameter(BattleManager.GetInstance().GetControlPlayerTarget(), skillId, rangeFinder, false, BattleDefine.RootType.Invalid);
                        __instance.rootBehavior.AIRun();
                    }
                }
                /*
                else if (testAttackButton2.IsDown())
                {
                    // INVALID=0, CLAUDE=1, RENA=2 CELINE=3, BOWMAN=4, DIAS=5, PRECIS=6, ASHTON=7, LEON=8, OPERA=9, ERNEST=10, NOEL=11, CHISATO=12, WELCH=13, MAX=14
                    //GET PLAYER ID so we know what character
                    PlayerID playerId = (PlayerID)__instance.OwnerObject.CharacterID;

                    //get get left skill index
                    BattleSkillID skillId = (BattleSkillID)__instance.OwnerObject.GetCharacterController().GetNextBattleSkillID(BattleDefine.RootType.Left);

                    //if invalid, get default normal attack
                    RyMCoopPlugin.StaticLog.LogInfo($"3Battler Index {__instance.OwnerObject.indexInParty}, SkillId {skillId}");
                    //if valid, do action
                    if (skillId != BattleSkillID.INVALID)
                    {
                        __instance.SetActionParameter(BattleManager.GetInstance().GetControlPlayerTarget(), skillId, false, false, BattleDefine.RootType.Invalid);
                        __instance.rootBehavior.AIRun();


                    }
                }
                */
                else if (testAttackButton3.IsDown())
                {
                    var controller = __instance.OwnerObject.GetCharacterController();

                    BattleSkillID id =
                    controller.GetNextNormalAttackSkillID();

                    controller.ReserveAction(
                        id,
                        BattleManager.GetInstance().GetControlPlayerTarget()
                    );
                    controller.ReserveAction(id, BattleManager.GetInstance().GetControlPlayerTarget());

                    __instance.SetActionParameter(
                        BattleManager.GetInstance().GetControlPlayerTarget(),
                        id,
                        false,
                        false,
                        BattleDefine.RootType.Invalid);

                    __instance.rootBehavior.AIRun();

                }
                return false;
            }
        }
        //Don't modify anything, run AI controller as usual
        return true;
    }
}

/*
[HarmonyPatch(typeof(BattleAIController), "AIRun")]
public static class BattleAIControllerPatch
{
    private static KeyboardShortcut holyOn = new KeyboardShortcut(KeyCode.F5);
    private static KeyboardShortcut holyOff = new KeyboardShortcut(KeyCode.F6);
    private static KeyboardShortcut holyAction = new KeyboardShortcut(KeyCode.F7);
    private static KeyboardShortcut unholyAction = new KeyboardShortcut(KeyCode.F8);
    private static bool holyToggleFlag = false;
    private static bool isInit = false;
    private static int healthAmount;

    public static int HealthAmount { get => healthAmount; set => healthAmount = value; }

    [HarmonyPrefix]
    public static void Update()
    {
        if (holyOn.IsDown())
        {
            holyToggleFlag = true;
            RyMCoopPlugin.StaticLog.LogInfo($"Toggled Holy1 {holyToggleFlag.ToString()}");
        }
        if (holyOff.IsDown())
        {
            holyToggleFlag = false;
            RyMCoopPlugin.StaticLog.LogInfo($"Toggled Holy2 {holyToggleFlag.ToString()}");
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
        if (__instance.OwnerObject.GetBattleCharacterType() == BattleCharacterType.Player)
        {
            if (!AiInitTracker.Initialized.Contains(__instance))
            {
                AiInitTracker.Initialized.Add(__instance);
                return true; // allow original AIRun ONCE
            }
            if (holyAction.IsDown())
            {
                __instance.SetActionParameter(BattleManager.GetInstance().GetPlayer().target, BattleSkillID.DIAS_NORMAL_ATTACK_01, false, false, __instance.actionParameter.RootType);
                __instance.rootBehavior.AIRun();
                RyMCoopPlugin.StaticLog.LogInfo($"Holy3");
            }
            if (unholyAction.IsDown())
            {
                //GameInputManager.instance.;
                //var a = new BattleObject();
                var a = new BattleCharacterController();
                RyMCoopPlugin.StaticLog.LogInfo($"Player{__instance.OwnerObject.indexInParty}: Wow!");
                
                
            }
            return holyToggleFlag;
        }
        else return true;
        
    }
}
*/


/*
[HarmonyPatch(typeof(BattleManager), nameof(BattleManager.GetControlPlayer))]
public static class BattleManagerPatcher
{
    
    private static int counter = 0;
    public static bool Prefix(ref BattleManager __instance, ref BattleCharacter __result)
    {
        RyMCoopPlugin.StaticLog.LogInfo($"Counter: {counter.ToString()}");
        int playerCount = 0;
        for (int i = 0; i < BattleManager.Instance.battlePlayerList.Count; i++)
        {
            if (BattleManager.Instance.battlePlayerList[i].GetBattleCharacterType() == BattleCharacterType.Player)
            {
                playerCount++;
            }
        }
        if (counter > playerCount) counter = 0;
        for (int i = 0; i < BattleManager.Instance.battlePlayerList.Count; i++)
        {
            if (BattleManager.Instance.battlePlayerList[i].GetBattleCharacterType() == BattleCharacterType.Player)
            {
                if (BattleManager.Instance.battlePlayerList[i].indexInParty == counter)
                {
                    __result = BattleManager.Instance.battlePlayerList[i];
                    counter++;
                    return false;
                }
            }
            
        }
        return true;
    }
}

[HarmonyPatch(typeof(BattleCharacterController), "Run")]
public static class BattleCharacterControllerPatch
{
    private static KeyboardShortcut holyOn = new KeyboardShortcut(KeyCode.F5);
    private static KeyboardShortcut holyOff = new KeyboardShortcut(KeyCode.F6);
    private static KeyboardShortcut holyAction = new KeyboardShortcut(KeyCode.F7);
    private static KeyboardShortcut unholyAction = new KeyboardShortcut(KeyCode.F8);
    private static bool holyToggleFlag = true;
    private static int healthAmount;

    public static int HealthAmount { get => healthAmount; set => healthAmount = value; }

    [HarmonyPrefix]
    public static void Update()
    {
        if (holyOn.IsDown())
        {
            holyToggleFlag = true;
            RyMCoopPlugin.StaticLog.LogInfo($"Toggled Holy1 {holyToggleFlag.ToString()}");
        }
        if (holyOff.IsDown())
        {
            holyToggleFlag = false;
            RyMCoopPlugin.StaticLog.LogInfo($"Toggled Holy2 {holyToggleFlag.ToString()}");
        }
    }

    [HarmonyPrefix]
    public static bool Prefix(ref BattleCharacterController __instance)
    {
        if (__instance.battleCharacter.GetBattleCharacterType() == BattleCharacterType.Player)
        {
            if (__instance.battleCharacter.indexInParty != BattleManager.GetInstance().ControlPlayerIndex)
            {
                //BattleManager.GetInstance().GetControlPlayer();
            }
            if (holyAction.IsDown())
            {
                holyToggleFlag = true;
                RyMCoopPlugin.StaticLog.LogInfo($"Player{__instance.battleCharacter.indexInParty}: SkillID:{__instance.GetNextNormalAttackSkillID()}");
            }

            if (__instance.battleCharacter.indexInParty != BattleManager.GetInstance().ControlPlayerIndex)
            {
                return holyToggleFlag;
            }
            else
            {
                return true;
            }
        }
        return true;
    }
}
*/
//@@
public class RyMCoopOverlay : MonoBehaviour
{
    private bool showOverlay = true;
    private Gamepad playerOneController;
    private KeyboardShortcut toggleKey = new KeyboardShortcut(KeyCode.F1);
    private KeyboardShortcut debugKey = new KeyboardShortcut(KeyCode.F2);
    private KeyboardShortcut tiertiaryKey = new KeyboardShortcut(KeyCode.F3);
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
        if (debugKey.IsDown())
        {
            RyMCoopPlugin.StaticLog.LogInfo($"Debug Triggered");

            if (BattleManager.Instance != null)
            {
                RyMCoopPlugin.StaticLog.LogInfo($"ControlPlayerIndex: {BattleManager.Instance.ControlPlayerIndex}");
                RyMCoopPlugin.StaticLog.LogInfo($"IsEnabled: {BattleManager.Instance.enabled}");
                RyMCoopPlugin.StaticLog.LogInfo($"PlayerCount: {BattleManager.Instance.battlePlayerList.Count}");

                if (BattleManager.Instance.battlePlayerList.Count > 0)
                {
                    for (int i = 0; i < BattleManager.Instance.battlePlayerList.Count; i++)
                    {

                        //BattleManager.Instance.battlePlayerList[0].CharacterID

                        //RyMCoopPlugin.StaticLog.LogInfo($"Player[{i}], {BattleManager.Instance.battlePlayerList[i].battleAIController.aiMoveController.destination}, {BattleManager.Instance.battlePlayerList[i].battleAIController.aiMoveController.finalDestination}, {BattleManager.Instance.battlePlayerList[i].battleAIController.aiMoveController.finalDestination + new Vector3(1, 0, 1)}");
                        /* TRIED: 
                         * enableAIMove
                         * EnableAIFromSystem
                         * 
                         */
                        /*
                        RyMCoopPlugin.StaticLog.LogInfo($"AI_INERVAL[{i}]:{BattleManager.Instance.battlePlayerList[i].BattleAIController.aiInterval}");
                        RyMCoopPlugin.StaticLog.LogInfo($"ITEM_ID[{i}]:{BattleManager.Instance.battlePlayerList[i].BattleAIController.actionParameter.ItemID}");
                        RyMCoopPlugin.StaticLog.LogInfo($"BATTLE_SKILL[{i}]:{BattleManager.Instance.battlePlayerList[i].BattleAIController.actionParameter.BattleSkillID}");
                        */


                        //RyMCoopPlugin.StaticLog.LogInfo($"CHAR_CONTROLLER_CURR_STATE[{i}]:{BattleManager.Instance.battlePlayerList[1].characterController}");


                        if (i > -1)
                        {
                            //BattleManager.Instance.battlePlayerList[i].BattleAIController.enableAIMove = false;
                        }
                        if (i == 1)
                        {
                            //BattleManager.Instance.battlePlayerList[i].characterController
                            //RyMCoopPlugin.StaticLog.LogInfo($"MOVE_COMMAND");
                            //BattleManager.Instance.battlePlayerList[i].AddPosition = BattleManager.Instance.battlePlayerList[i].Position + new Vector3(0.1f, 0, 0.1f);
                            //BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.aiMoveResult.moveDir;
                            /*
                            RyMCoopPlugin.StaticLog.LogInfo($"########");
                            RyMCoopPlugin.StaticLog.LogInfo($"MOVEDIR:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.aiMoveResult.moveDir}");
                            RyMCoopPlugin.StaticLog.LogInfo($"ISRUN:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.aiMoveResult.isRun}");
                            RyMCoopPlugin.StaticLog.LogInfo($"MOVERATE:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.aiMoveResult.moveRate}");
                            RyMCoopPlugin.StaticLog.LogInfo($"TURNRATE:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.aiMoveResult.turnRate}");
                            RyMCoopPlugin.StaticLog.LogInfo($"DIRECTION_VECOTR:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.Direction}");
                            RyMCoopPlugin.StaticLog.LogInfo($"HOME_POSITION:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.AIParameter.HomePosition}");
                            RyMCoopPlugin.StaticLog.LogInfo($"DESTINATION_VECTOR:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.destination}");
                            RyMCoopPlugin.StaticLog.LogInfo($"FINELDESTIN_VECTOR:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.finalDestination}");
                            RyMCoopPlugin.StaticLog.LogInfo($"IS_DISABLE_RUN:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.isDisableRun}");
                            RyMCoopPlugin.StaticLog.LogInfo($"IS_WALK:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.isWalk}");
                            RyMCoopPlugin.StaticLog.LogInfo($"MOVESPEED_RATE:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.moveSpeedRate}");
                            RyMCoopPlugin.StaticLog.LogInfo($"MOVE_STATE:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.moveState}");
                            RyMCoopPlugin.StaticLog.LogInfo($"AI_MOVE_STATE:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.State}");
                            RyMCoopPlugin.StaticLog.LogInfo($"AI_MOVE_MOVE_STATE:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIMoveController.moveState}");
                            RyMCoopPlugin.StaticLog.LogInfo($"AI_PARAM_FLAG_COUNT:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIParameter.aiFlag.flagCount}");
                            */
                            /*
                            for(int j = 0; i < BattleManager.Instance.battlePlayerList[i].BattleAIController.AIParameter.aiFlag.bitFlag.Count; i++)
                            {
                                RyMCoopPlugin.StaticLog.LogInfo($"AI_FLAGS{j}:{BattleManager.Instance.battlePlayerList[i].BattleAIController.AIParameter.aiFlag.bitFlag[j].ToString()}");
                            }
                            RyMCoopPlugin.StaticLog.LogInfo($"########");
                            */
                            /*
                            RyMCoopPlugin.StaticLog.LogInfo($"########");
                            Vector3 initVector = new Vector3(BattleManager.Instance.battlePlayerList[1].Position.x, BattleManager.Instance.battlePlayerList[1].Position.y, BattleManager.Instance.battlePlayerList[1].Position.z);
                            RyMCoopPlugin.StaticLog.LogInfo($"Init Move Vector{initVector.ToString()}");
                            Vector3 finalVector = new Vector3(initVector.x - 5f, 0, initVector.z + 5f);
                            RyMCoopPlugin.StaticLog.LogInfo($"Final Move Vector{finalVector.ToString()}");
                            BattleManager.Instance.battlePlayerList[1].battleAIController.aiMoveController.destination = initVector;
                            BattleManager.Instance.battlePlayerList[1].battleAIController.aiMoveController.finalDestination = finalVector;
                            BattleManager.Instance.battlePlayerList[1].CharacterController.




                            RyMCoopPlugin.StaticLog.LogInfo($"########");
                            */
                            //var arga = new BattleChangeControlPlayerInputTask();
                            //arga.exe
                        }
                    }
                    //RyMCoopPlugin.StaticLog.LogInfo($"List Completed");
                }


            }
            else
            {
                RyMCoopPlugin.StaticLog.LogInfo($"No Battle Instance");
            }
        }
        if (tiertiaryKey.IsDown())
        {
            RyMCoopPlugin.StaticLog.LogInfo($"TiertiaryKeyPressed");


            /*
            if (PartyManager.Instance != null)
            {
                if (PartyManager.Instance.GetPartyMemberCount() < 7)
                {
                    PartyManager.instance.JoinMember(PlayerID.DIAS, false);
                    PartyManager.instance.JoinMember(PlayerID.LEON, false);
                    //PartyManager.instance.AddUpdatedMember(PlayerID.OPERA, true, true);
                    //PartyManager.instance.BreakawayMember(PlayerID.OPERA);
                    //BattleManager.instance.GetPlayer(PlayerID.CLAUDE).characterController = null;
                    //RyMCoopPlugin.StaticLog.LogInfo($"Added Ashton to Party Members");
                }
                else
                {
                    RyMCoopPlugin.StaticLog.LogInfo($"Cannot add more party members");
                }
            }
            else
            {
                RyMCoopPlugin.StaticLog.LogInfo($"No Party Instance");
            }
            */
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