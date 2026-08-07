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
                                BattleCharacterActionResult actionResult = __instance.OwnerObject.GetCharacterController().OnNormalAttack(BattleManager.GetInstance().GetControlPlayerTarget());

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
                            RyMCoopPlugin.StaticLog.LogInfo($"{controllerIndex}: EVADE pressed");

                            if (GetDeepestBehavior(__instance.rootBehavior).ToString() == "Game.BattleAIActionBehavior")
                            {
                                RemoveAllChildBehaviors(__instance.rootBehavior);
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAINullBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                            }
                            __instance.OwnerObject.StopPersistence();
                            __instance.OwnerObject.ResetBattleCharacterFlag();
                            __instance.OwnerObject.GetCharacterController().UpdateState();
                            if (__instance.enableAIMove) __instance.enableAIMove = false;
                            TacticsID originalId = __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID;
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = TacticsID.INVALID;
                            
                            __instance.rootBehavior.AIRun();
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = originalId;
                            battleCharController.OnStepAvoid();
                            

                            /*
                            var bp = (BattlePlayer)__instance.OwnerObject;
                            var bcc = (BattleCharacterController)bp.characterController;
                            bcc.OnNormalAttack();
                            */
                            //return true;
                        }

                        //Left Skill Logic
                        if (RyMCoopPlugin.VirtualControllers.GetState(controllerIndex).LeftSkillPressed)
                        {
                            //get get left skill index
                            if (battleCharController.battleSkillLeftIndex > 1)
                                battleCharController.battleSkillLeftIndex = 0;
                            //store left skill index
                            int battleSkillComboIndex = battleCharController.battleSkillLeftIndex;
                            //get left skill id
                            BattleSkillID skillId = (BattleSkillID)battleCharController.GetNextBattleSkillID(BattleDefine.RootType.Left);

                            //get range type of current target
                            bool rangeFinder = BattleUtility.IsLongRange(__instance.OwnerObject, BattleManager.GetInstance().GetControlPlayerTarget());

                            //if skill isn't invalid, perform skill
                            if (skillId != BattleSkillID.INVALID)
                            {
                                //send skill battle character controller
                                battleCharController.OnBattleSkill(skillId, BattleManager.GetInstance().GetControlPlayerTarget(), BattleDefine.RootType.Left);
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
                            if (battleCharController.battleSkillRightIndex > 1)
                                battleCharController.battleSkillRightIndex = 0;
                            //store left skill index
                            int battleSkillComboIndex = battleCharController.battleSkillRightIndex;
                            //get left skill id
                            BattleSkillID skillId = (BattleSkillID)battleCharController.GetNextBattleSkillID(BattleDefine.RootType.Right);

                            //get range type of current target
                            bool rangeFinder = BattleUtility.IsLongRange(__instance.OwnerObject, BattleManager.GetInstance().GetControlPlayerTarget());

                            //if skill isn't invalid, perform skill
                            if (skillId != BattleSkillID.INVALID)
                            {
                                //send skill battle character controller
                                battleCharController.OnBattleSkill(skillId, BattleManager.GetInstance().GetControlPlayerTarget(), BattleDefine.RootType.Right);
                                //execute battle character skill
                                __instance.rootBehavior.AIRun();
                                return true;
                            }
                        }

                        //return true if battling
                        if (__instance.OwnerObject.BattleAIController.actionParameter.BattleSkillID != BattleSkillID.INVALID ||
                            __instance.OwnerObject.GetCharacterController().currentState == 8 || //BattleCharacterState.Step is 8
                            __instance.OwnerObject.GetCharacterController().currentState == 9) //BattleCharacterState.StepAvoid is 9
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