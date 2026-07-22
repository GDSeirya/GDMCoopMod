using BepInEx.Logging;
using BepInEx.Unity.IL2CPP.Configuration;
using Common;
using Game;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

[HarmonyPatch(typeof(BattleCharacterController), nameof(BattleCharacterController.OnMove),
              new[] { typeof(Vector3), typeof(float) })]
/*class Log_OnMove
{
    static void Prefix(BattleCharacterController __instance, Vector3 moveDir, float moveRate)
    {
        RyMCoopPlugin.StaticLog.LogInfo($"[OnMove] {__instance.battleCharacter.CharacterID} Dir={moveDir} Rate={moveRate}");
    }
}
[HarmonyPatch(typeof(BattleCharacterController), nameof(BattleCharacterController.OnNormalAttack))]
class Log_OnNormalAttack
{
    static void Prefix(BattleCharacterController __instance, BattleCharacter target)
    {
        string tgt = target != null ? target.ToString() : "null";
        RyMCoopPlugin.StaticLog.LogInfo($"[OnNormalAttack] {__instance.battleCharacter.CharacterID} Target={tgt}");
    }
}
[HarmonyPatch(typeof(BattleCharacterController),
     nameof(BattleCharacterController.OnBattleSkill),
     new[] { typeof(BattleSkillID), typeof(BattleDefine.RootType) })]
class Log_OnBattleSkill
{
    static void Prefix(BattleCharacterController __instance, BattleSkillID battleSkillID, BattleDefine.RootType root)
    {
        RyMCoopPlugin.StaticLog.LogInfo($"[OnBattleSkill] {__instance.battleCharacter.CharacterID} Skill={battleSkillID}");
    }
}
[HarmonyPatch(typeof(BattleCharacterController),
     nameof(BattleCharacterController.OnAction),
     new[] { typeof(BattleAIActionParameter) })]
class Log_OnActionParam
{
    static void Prefix(BattleCharacterController __instance, BattleAIActionParameter ap)
    {
        string target = ap?.Target != null ? ap.Target.ToString() : "null";
        RyMCoopPlugin.StaticLog.LogInfo($"[OnActionParam] {__instance.battleCharacter.CharacterID} Skill={ap?.BattleSkillID} Target={target} Long={ap?.IsLong}");
    }
}
[HarmonyPatch(typeof(BattleCharacterController), nameof(BattleCharacterController.ReserveAction))]
class Log_ReserveAction
{
    static void Prefix(BattleCharacterController __instance, BattleSkillID battleSkillID, BattleCharacter target)
    {
        string tgt = target != null ? target.ToString() : "null";
        RyMCoopPlugin.StaticLog.LogInfo($"[ReserveAction] {__instance.battleCharacter.CharacterID} Skill={battleSkillID} Target={tgt}");
    }
}

[HarmonyPatch(typeof(InputComponent), "SetInputTask")]
class Log_SetInputTask
{
    static void Prefix(TaskComponent component, InputTask inputTask)
    {
        if (component is InputComponent ic && ic.InputTask != inputTask)
            RyMCoopPlugin.StaticLog.LogInfo($"[SetInputTask] Component={ic} NewTask={inputTask}");
    }
}*/

/*[HarmonyPatch(typeof(BattleManager))]
public class BattleManagerPatch
{
    [HarmonyPatch("OnUpdate")]
    [HarmonyPostfix]
    public static void Postfix()
    {
        RyMCoopPlugin.StaticLog.LogInfo("BattleManager Update!");
    }
}*/

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
    /*[HarmonyPrefix]
    public static bool Prefix(BattleCharacter __instance)
    {
        if (Keyboard.current != null && Keyboard.current.f4Key.isPressed)
        {
            RyMCoopPlugin.StaticLog.LogInfo("BattleCharacter");
            return false;
        }
        return true;
    }*/

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
                        RyMCoopPlugin.StaticLog.LogInfo($"Player[{i}]");
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
[HarmonyPatch(typeof(BattleCharacter), "OnUpdate")]
public static class Log0
{
    [HarmonyPrefix]
    public static bool Prefix(BattleCharacter __instance)
    {
        if (Keyboard.current != null && Keyboard.current.f4Key.isPressed)
        {
            RyMCoopPlugin.StaticLog.LogInfo("BattleCharacter");
            return false;
        }
        return true;
    }
}
/*[HarmonyPatch(typeof(BattleCharacterController))]
public static class BattleCharacterController1
{
    [HarmonyPrefix]
    public static bool Prefix(BattleCharacterController __instance)
    {
            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                BattleManager.controlPlayerIndex) = 4;
            }

            if (Keyboard.current.f6Key.wasPressedThisFrame)
            {
                battleManager.ControlPlayerIndex = 5;
            }
    }
}*/

[HarmonyPatch(typeof(Common.InputManager), "OnUpdate")]
public static class Log1
{
    [HarmonyPrefix]
    static bool Prefix()
    {
        if (Keyboard.current != null && Keyboard.current.f5Key.isPressed)
        {
            return false;
        }
        return true;
    }
    
}

[HarmonyPatch(typeof(Common.InputManager), "GetGamepad")]
public static class Log2
{
    [HarmonyPrefix]
    static bool Prefix(ref Gamepad __result)
    {
        if (Keyboard.current != null && Keyboard.current.f6Key.isPressed)
        {
            __result = null;
            return false;
        }
        return true;
    }

}

/*[HarmonyPatch(typeof(BattleManager), "controlPlayerIndex")]
public static class Log3
{
    static bool Prefix(BattleManager __instance, int set_ControlPlayerIndex)
    {

        // Example: character 0 = controller 1, character 1 = controller 2
        //__result = MyInputRouter.IsCharacterControlled(index);
        if (Keyboard.current != null && Keyboard.current.f7Key.isPressed)
        {
            int value = set_ControlPlayerIndex;
            RyMCoopPlugin.StaticLog.LogInfo("BM.controlPlayerIndex");
            set_ControlPlayerIndex = value++;
            return false; // skip original
        }
        return true;
    }
}*/

[HarmonyPatch(typeof(BattleManager), "set_ControlPlayerIndex")]
public static class Log3
{
    [HarmonyPrefix]
    static void Prefix(ref int value)
    {
        RyMCoopPlugin.StaticLog.LogInfo($"SET ControlPlayerIndex = {value}");

        if (Keyboard.current.f7Key.wasPressedThisFrame)
        {
            value++;
            RyMCoopPlugin.StaticLog.LogInfo($"Changed to {value}");
        }
    }
}

/*[HarmonyPatch(typeof(BattleManager), "get_ControlPlayerIndex")]
public static class Log3b
{
    static bool Prefix(BattleManager __instance, int get_ControlPlayerIndex)
    {

        // Example: character 0 = controller 1, character 1 = controller 2
        //__result = MyInputRouter.IsCharacterControlled(index);
            RyMCoopPlugin.StaticLog.LogInfo(get_ControlPlayerIndex);
            return false; // skip original
    }
}*/

[HarmonyPatch(typeof(BattleManager), "get_ControlPlayerIndex")]
public static class Log3b
{
    [HarmonyPostfix]
    static void Postfix(int __result)
    {
        RyMCoopPlugin.StaticLog.LogInfo($"GET ControlPlayerIndex = {__result}");
    }
}

[HarmonyPatch(typeof(BattleManager), "SetControlPlayerTarget")]
public static class Log4
{
    static bool Prefix(BattleManager __instance, BattleCharacter target, bool isCameraFocus)
    {

        // Example: character 0 = controller 1, character 1 = controller 2
        //__result = MyInputRouter.IsCharacterControlled(index);
        if (Keyboard.current != null && Keyboard.current.f8Key.isPressed)
        {
            RyMCoopPlugin.StaticLog.LogInfo(
            $"SetControlPlayerTarget(target={target}, cameraFocus={isCameraFocus})");
        }
        return true;
    }
}
