using BepInEx.Unity.IL2CPP.Configuration;
using Game;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static Common.InputManager;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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

    /// <summary>
    /// Returns all the child behaviors chained from AIBehavior as string.
    /// </summary>
    public static string DumpBehaviorChain(AIBehavior<BattleCharacter> behavior, int depth = 0)
    {
        if (behavior == null)
            return "(null)\n";

        string indent = new string(' ', depth * 2);
        string result = indent + behavior.ToString() + "\n";

        // childBehavior is a SINGLE node, not a list
        if (behavior.childBehavior != null)
        {
            result += DumpBehaviorChain(behavior.childBehavior, depth + 1);
        }

        return result;
    }

    /// <summary>
    /// Returns AI behavior by name string, eg "Game.BattleAIActionBehavior" will return BattleAIActionBehavior behavior.
    /// </summary>
    public static AIBehavior<BattleCharacter> GetAIBehavior(AIBehavior<BattleCharacter> behavior, string behaviorName)
    {
        AIBehavior<BattleCharacter> current = behavior;
        if (current.ToString() == behaviorName) return current;
        while (current.childBehavior != null)
        {
            current = current.childBehavior;
            if (current.ToString() == behaviorName) return current;
        }
        return null;
    }

    /// <summary>
    /// Returns the lowest AI behavior. The game runs the lowest level behavior first.
    /// </summary>
    public static AIBehavior<BattleCharacter> GetDeepestBehavior(AIBehavior<BattleCharacter> behavior)
    {
        AIBehavior<BattleCharacter> current = behavior;

        while (current.childBehavior != null)
            current = current.childBehavior;

        return current;
    }

    /// <summary>
    /// Removes all child behaviors from an AIBehavior.
    /// </summary>
    public static void RemoveAllChildBehaviors(AIBehavior<BattleCharacter> root)
    {
        AIBehavior<BattleCharacter> current = root;

        while (current.childBehavior != null)
        {
            // Remove the next child
            current.childBehavior = null;
        }
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
                //Initialize Behaviors, overriding original
                if (!AiInitTracker.Initialized.Contains(__instance))
                {
                    //Required for actions to be had
                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);

                    //Think behavior lets AI get more behaviors
                    //GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerThinkBehavior();
                    //GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);

                    //Action behavior lets AI act
                    //GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIActionBehavior();
                    //GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);

                    //Battle Null Behavior is set to player 1 at the start, this is to replicate what it does
                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAINullBehavior();
                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);

                    //Add to tracker so they don't re-initialize
                    AiInitTracker.Initialized.Add(__instance);
                    
                    //Don't allow the original init to run
                    return false;
                }
                //If player is control index, do usual functions
                if (__instance.OwnerObject.indexInParty == BattleManager.GetInstance().ControlPlayerIndex)
                {
                    //If controlled player is same as index in party, run as usual, no overriding code
                    return true;
                }
                //get controller based on partyIndex assigned earlier
                int controllerIndex = RyMCoopMod.RyMControllerRouting.GetControllerForParty(__instance.OwnerObject.indexInParty);

                //if valid controler indexes
                if (controllerIndex >= 0 && controllerIndex < 4)
                {
                    //Reset combo link if any of these states are entered
                    if (__instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Guard ||
                        __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Damage ||
                        __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Escape ||
                        __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Incapacitated ||
                        __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Step ||
                        __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.StepAvoid ||
                        __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Move ||
                        __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Escape ||
                        __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.TryEscape ||
                        __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Dead
                        )
                    {
                        //Reset skill index
                        if (__instance.OwnerObject.GetCharacterController().battleSkillLeftIndex != 0) __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex = 0;
                        if (__instance.OwnerObject.GetCharacterController().battleSkillRightIndex != 0) __instance.OwnerObject.GetCharacterController().battleSkillRightIndex = 0;
                        if (__instance.OwnerObject.GetCharacterController().normalAttackIndex != 0) __instance.OwnerObject.GetCharacterController().normalAttackIndex = 0;
                    }
                    //Init Variables
                    BattleCharacterController battleCharController = __instance.OwnerObject.GetCharacterController();

                    //If battle character exist, proceed to overriding controls
                    if (battleCharController != null)
                    {
                        //Move Logic
                        Vector2 moveVector = RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).Move;
                        //Get the magnitude of the move stick
                        float magnitude = moveVector.magnitude;
                        //Deadzone of move stick, assumed to be 0.1f to give leeway for drifty sticks
                        if (magnitude >= 0.1f)
                        {
                            //Check if AI Controller exists
                            if (__instance.rootBehavior != null)
                            {
                                //Get the normalization of the move vector so that you can't walk as it's not a function in the base game
                                Vector2 normalized = moveVector.normalized;
                                //If there is a battle skill, reset it
                                if (__instance.OwnerObject.BattleAIController.actionParameter.BattleSkillID != BattleSkillID.INVALID)
                                {
                                    __instance.OwnerObject.BattleAIController.actionParameter.BattleSkillID = BattleSkillID.INVALID;
                                }
                                //If action behavior, reset it to maintain control over the AI character
                                if (GetDeepestBehavior(__instance.rootBehavior).ToString() == "Game.BattleAIActionBehavior")
                                {
                                    RemoveAllChildBehaviors(__instance.rootBehavior);
                                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAINullBehavior();
                                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                }
                                //Stop Blue Effects
                                __instance.OwnerObject.StopPersistence();
                                //Reset Godspeed Speedboost Effect
                                __instance.OwnerObject.ResetBattleCharacterFlag();
                                //Stop further movement for AI if they are allowed to and allow player to move instead
                                if (__instance.enableAIMove) __instance.enableAIMove = false;
                                //Actually move the character
                                battleCharController.OnMove(new Vector3(normalized.x, 0, normalized.y), __instance.aiMoveController.moveSpeedRate);
                            }
                        }
                        
                        //Attack Logic
                        if (RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).AttackPressed)
                        {
                            //If AIController exists, proceed to overriding to attack
                            if (__instance.rootBehavior != null)
                            {
                                if (GetDeepestBehavior(__instance.rootBehavior).ToString() == "Game.BattleAINullBehavior")
                                {
                                    RemoveAllChildBehaviors(__instance.rootBehavior);
                                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                    /*
                                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerThinkBehavior();
                                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                    */
                                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIActionBehavior();
                                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                }

                                //Perform an attack request on target
                                //BattleCharacterActionResult actionResult;
                                __instance.OwnerObject.GetCharacterController().OnNormalAttack(BattleManager.GetInstance().GetControlPlayerTarget());

                                //Store original Tactics ID
                                TacticsID originalId = __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID;

                                //Set current Tactics to Invalid to prevent AI from doing anything
                                __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = TacticsID.INVALID;

                                //Allow AI to move character
                                if (!__instance.enableAIMove) __instance.enableAIMove = true;

                                //Run AI so they actually do what they're asked to do
                                __instance.rootBehavior.AIRun();

                                //Return original tactics to AI to prevent strategy menu from being bad
                                __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = originalId;
                                
                            }
                        }

                        //Evade logic
                        if (RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).EvadePressed)
                        {
                            if (__instance.rootBehavior != null)
                            {
                                if (GetDeepestBehavior(__instance.rootBehavior).ToString() == "Game.BattleAIActionBehavior")
                                {
                                    RemoveAllChildBehaviors(__instance.rootBehavior);
                                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAINullBehavior();
                                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                }
                                if (__instance.enableAIMove) __instance.enableAIMove = false;
                                __instance.OwnerObject.StopPersistence();
                                __instance.OwnerObject.ResetBattleCharacterFlag();
                                __instance.OwnerObject.GetCharacterController().UpdateState();

                                TacticsID originalId = __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID;
                                __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = TacticsID.INVALID;
                                __instance.rootBehavior.AIRun();
                                __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = originalId;
                                battleCharController.OnStepAvoid();
                            }
                        }

                        //Left Skill Logic
                        if (RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).LeftSkillPressed)
                        {
                            if (GetDeepestBehavior(__instance.rootBehavior).ToString() == "Game.BattleAINullBehavior")
                            {
                                RemoveAllChildBehaviors(__instance.rootBehavior);
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                /*
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerThinkBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                */
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIActionBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                            }
                            //Reset if not actual combo
                            if (__instance.OwnerObject.GetCharacterController().BattleSkillRootType != BattleDefine.RootType.Left)
                            {
                                __instance.OwnerObject.GetCharacterController().battleSkillRightIndex = 0;
                                __instance.OwnerObject.GetCharacterController().normalAttackIndex = 0;
                                __instance.OwnerObject.GetCharacterController().Reset();
                            }

                            //Store original Tactics ID
                            TacticsID originalId = __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID;

                            //Set current Tactics to Invalid to prevent AI from doing anything
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = TacticsID.INVALID;

                            //Allow AI to move character
                            if (!__instance.enableAIMove) __instance.enableAIMove = true;

                            //Perform an attack request on target
                            //BattleCharacterActionResult actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill()
                            BattleSkillID skillId = __instance.OwnerObject.GetCharacterController().GetNextBattleSkillID(BattleDefine.RootType.Left);
                            BattleCharacterActionResult actionResult = BattleCharacterActionResult.Invalid;
                            if (skillId != BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().reserveBattleSkillID == BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex < 2)
                            {
                                actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, BattleDefine.RootType.Left);
                                //Run AI so they actually do what they're asked to do
                                __instance.rootBehavior.AIRun();
                            }
                            RyMCoopPlugin.StaticLog.LogInfo($"{controllerIndex}-Left: skillIndex {__instance.OwnerObject.GetCharacterController().battleSkillLeftIndex}, SkillId {skillId}, aResult {actionResult}, canUseSkill {__instance.OwnerObject.CanUseBattleSkill(skillId)}");

                            
                            

                            //Return original tactics to AI to prevent strategy menu from being bad
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = originalId;
                        }

                        //Right Skill Logic
                        if (RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).RightSkillPressed)
                        {
                            if (GetDeepestBehavior(__instance.rootBehavior).ToString() == "Game.BattleAINullBehavior")
                            {
                                RemoveAllChildBehaviors(__instance.rootBehavior);
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                /*
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerThinkBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                */
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIActionBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                            }
                            //Reset if not actual combo
                            if (__instance.OwnerObject.GetCharacterController().BattleSkillRootType != BattleDefine.RootType.Right)
                            {
                                __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex = 0;
                                __instance.OwnerObject.GetCharacterController().normalAttackIndex = 0;
                                __instance.OwnerObject.GetCharacterController().Reset();
                            }

                            //Store original Tactics ID
                            TacticsID originalId = __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID;

                            //Set current Tactics to Invalid to prevent AI from doing anything
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = TacticsID.INVALID;

                            //Allow AI to move character
                            if (!__instance.enableAIMove) __instance.enableAIMove = true;

                            //Perform an attack request on target
                            //BattleCharacterActionResult actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill()
                            BattleCharacterActionResult actionResult = BattleCharacterActionResult.Invalid;
                            
                            BattleSkillID skillId = __instance.OwnerObject.GetCharacterController().GetNextBattleSkillID(BattleDefine.RootType.Right);
                            if (skillId != BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().reserveBattleSkillID == BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().battleSkillRightIndex < 2)
                            {
                                actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, BattleDefine.RootType.Right);
                            }
                            RyMCoopPlugin.StaticLog.LogInfo($"{controllerIndex}-Right: skillIndex {__instance.OwnerObject.GetCharacterController().battleSkillRightIndex}, SkillId {skillId}, aResult {actionResult}, canUseSkill {__instance.OwnerObject.CanUseBattleSkill(skillId)}");

                            //Run AI so they actually do what they're asked to do
                            __instance.rootBehavior.AIRun();

                            //Return original tactics to AI to prevent strategy menu from being bad
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = originalId;
                        }

                        //If stepping, set state to false so that you don't move forward automatically
                        if (__instance.OwnerObject.GetCharacterController().currentState == 9 && __instance.enableAIMove)
                        {
                            __instance.enableAIMove = false;
                        }

                        //return true if battling
                        if (__instance.OwnerObject.BattleAIController.actionParameter.BattleSkillID != BattleSkillID.INVALID ||
                            __instance.OwnerObject.GetCharacterController().currentState == 8) //BattleCharacterState.Step is 8
                        {
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