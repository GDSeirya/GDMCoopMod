using Game;
using HarmonyLib;
using SimpleSpritePacker;
using System.Collections.Generic;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[HarmonyPatch(typeof(BattleAIController), "AIRun")]
public static class BattleAIControllerPatch
{
    private static bool isPartyAiEnabled = false;
    private static bool[] useHostTarget = new bool[4] { true, true, true, true };

    public static void SetHostTargetingMode(int index, bool isEnabled)
    {
        if (index >= 0 && index <= 3)
        {
            useHostTarget[index] = isEnabled;
        }
    }

    public static bool GetHostTargetingMode(int index)
    {
        if (index >= 0 && index <= 3)
        {
            return useHostTarget[index];
        }
        else
        {
            return false;
        }
    }

    static private class AiInitTracker
    {
        public static readonly HashSet<BattleAIController> Initialized = new HashSet<BattleAIController>();
    }

    /// <summary>
    /// Returns all the child behaviors chained from AIBehavior as string.
    /// </summary>
    private static string DumpBehaviorChain(AIBehavior<BattleCharacter> behavior, int depth = 0)
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
    private static AIBehavior<BattleCharacter> GetAIBehavior(AIBehavior<BattleCharacter> behavior, string behaviorName)
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
    private static AIBehavior<BattleCharacter> GetDeepestBehavior(AIBehavior<BattleCharacter> behavior)
    {
        AIBehavior<BattleCharacter> current = behavior;

        while (current.childBehavior != null)
            current = current.childBehavior;

        return current;
    }

    /// <summary>
    /// Removes all child behaviors from an AIBehavior.
    /// </summary>
    private static void RemoveAllChildBehaviors(AIBehavior<BattleCharacter> root)
    {
        AIBehavior<BattleCharacter> current = root;

        while (current.childBehavior != null)
        {
            // Remove the next child
            current.childBehavior = null;
        }
    }

    /// <summary>
    /// Clears a list of hashes of detected BattleCharacters, will initialize battle ai controller again when AI is disabled.
    /// </summary>
    public static void ClearInit()
    {
        AiInitTracker.Initialized.Clear();
    }

    /// <summary>
    /// Set other player AI to either be enabled or disabled. Disabled means it may be controlled by another human.
    /// </summary>
    public static void SetOtherPlayerAI(bool setTo)
    {
        isPartyAiEnabled = setTo;
        ClearInit();
    }

    public static BattleEnemy GetClosestEnemy(BattleCharacter battleCharacter)
    {
        Vector3 home = battleCharacter.Position;
        BattleEnemy closestEnemy = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < BattleManager.Instance.battleEnemyList.Count; i++)
        {
            if (!BattleManager.Instance.battleEnemyList[i].IsDead())
            {
                float dist = (BattleManager.Instance.battleEnemyList[i].Position - home).sqrMagnitude;

                if (dist < bestDist)
                {
                    bestDist = dist;
                    closestEnemy = BattleManager.Instance.battleEnemyList[i];
                }
            }
        }
        return closestEnemy;
    }

    /// <summary>
    /// Returns the state whether if AI is enabled or not.
    /// </summary>
    public static bool IsPartyAIEnabled()
    {
        return isPartyAiEnabled;
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
                    //Do not do it if control player index
                    if (__instance.OwnerObject.indexInParty != BattleManager.GetInstance().ControlPlayerIndex)
                    {
                        RemoveAllChildBehaviors(__instance.rootBehavior);
                        //Required for actions to be had
                        GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                        GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                        //Battle Null Behavior is set to player 1 at the start, this is to replicate what it does
                        GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAINullBehavior();
                        GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                    }
                    AiInitTracker.Initialized.Add(__instance);
                    //Add to tracker so they don't re-initialize
                    if (__instance.OwnerObject.indexInParty == BattleManager.GetInstance().ControlPlayerIndex)
                    {
                        //Business as usual if you are control player index
                        return true;
                    }
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
                int controllerIndex = GDMCoopMod.GDMControllerRouting.GetControllerForParty(__instance.OwnerObject.indexInParty);
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
                        Vector2 moveVector = GDMCoopPlugin.VirtualControllers.GetState(controllerIndex).Move;
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
                        if (GDMCoopPlugin.VirtualControllers.GetState(controllerIndex).AttackPressed)
                        {
                            //If AIController exists, proceed to overriding to attack
                            if (__instance.rootBehavior != null)
                            {
                                if (GetDeepestBehavior(__instance.rootBehavior).ToString() == "Game.BattleAINullBehavior")
                                {
                                    RemoveAllChildBehaviors(__instance.rootBehavior);
                                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                    GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIActionBehavior();
                                    GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                }

                                //Perform an attack request on target
                                //BattleCharacterActionResult actionResult;

                                //target is created here
                                if (useHostTarget[controllerIndex])
                                {
                                    __instance.OwnerObject.GetCharacterController().OnNormalAttack(BattleManager.GetInstance().GetControlPlayerTarget());
                                }
                                else
                                {
                                    BattleEnemy closestEnemy = GetClosestEnemy(__instance.OwnerObject);
                                    if (closestEnemy != null)
                                    {
                                        __instance.OwnerObject.GetCharacterController().OnNormalAttack(closestEnemy);
                                    }
                                    else
                                    {
                                        //by default, target host's target
                                        __instance.OwnerObject.GetCharacterController().OnNormalAttack(BattleManager.GetInstance().GetControlPlayerTarget());
                                    }
                                }

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
                        if (GDMCoopPlugin.VirtualControllers.GetState(controllerIndex).EvadePressed)
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
                        if (GDMCoopPlugin.VirtualControllers.GetState(controllerIndex).LeftSkillPressed)
                        {
                            if (GetDeepestBehavior(__instance.rootBehavior).ToString() == "Game.BattleAINullBehavior")
                            {
                                RemoveAllChildBehaviors(__instance.rootBehavior);
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
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
                            if (skillId != BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().reserveBattleSkillID == BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex < 2 && !__instance.OwnerObject.GetCharacterController().IsLinkComboAction())
                            {
                                BattleCharacter targetToCheck = __instance.OwnerObject.GetCharacterController().GetTargetOnAction(skillId);
                                if (targetToCheck.IsPlayerSide() || useHostTarget[controllerIndex])
                                {
                                    actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, BattleDefine.RootType.Left);
                                }
                                else
                                {
                                    BattleEnemy closestEnemy = GetClosestEnemy(__instance.OwnerObject);
                                    if (closestEnemy != null)
                                    {
                                        actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, closestEnemy, BattleDefine.RootType.Left);
                                    }
                                    else
                                    {
                                        actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, BattleDefine.RootType.Left);
                                    }
                                }
                            }
                            //Return original tactics to AI to prevent strategy menu from being bad
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = originalId;
                        }

                        //Right Skill Logic
                        if (GDMCoopPlugin.VirtualControllers.GetState(controllerIndex).RightSkillPressed)
                        {
                            if (GetDeepestBehavior(__instance.rootBehavior).ToString() == "Game.BattleAINullBehavior")
                            {
                                RemoveAllChildBehaviors(__instance.rootBehavior);
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIPlayerBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                                GetDeepestBehavior(__instance.rootBehavior).childBehavior = new BattleAIActionBehavior();
                                GetDeepestBehavior(__instance.rootBehavior).Initialize(__instance.aiParameter);
                            }
                            if (__instance.OwnerObject.GetCharacterController().BattleSkillRootType != BattleDefine.RootType.Right)
                            {
                                __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex = 0;
                                __instance.OwnerObject.GetCharacterController().normalAttackIndex = 0;
                                __instance.OwnerObject.GetCharacterController().Reset();
                            }
                            TacticsID originalId = __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID;
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = TacticsID.INVALID;
                            if (!__instance.enableAIMove) __instance.enableAIMove = true;
                            BattleCharacterActionResult actionResult = BattleCharacterActionResult.Invalid;

                            BattleSkillID skillId = __instance.OwnerObject.GetCharacterController().GetNextBattleSkillID(BattleDefine.RootType.Right);
                            if (skillId != BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().reserveBattleSkillID == BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().battleSkillRightIndex < 2 && !__instance.OwnerObject.GetCharacterController().IsLinkComboAction())
                            {
                                //actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, BattleDefine.RootType.Right);


                                BattleCharacter targetToCheck = __instance.OwnerObject.GetCharacterController().GetTargetOnAction(skillId);
                                if (targetToCheck.IsPlayerSide() || useHostTarget[controllerIndex])
                                {
                                    actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, BattleDefine.RootType.Right);
                                }
                                else
                                {
                                    BattleEnemy closestEnemy = GetClosestEnemy(__instance.OwnerObject);
                                    if (closestEnemy != null)
                                    {
                                        actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, closestEnemy, BattleDefine.RootType.Right);
                                    }
                                    else
                                    {
                                        actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, BattleDefine.RootType.Right);
                                    }
                                }
                            }
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = originalId;
                        }

                        //If stepping, set state to false so that you don't move forward automatically and AI can move
                        if (__instance.enableAIMove == true && __instance.OwnerObject.GetCharacterController().currentState == 9 && __instance.enableAIMove)
                        {
                            __instance.enableAIMove = false;
                        }

                        //Return true if battling
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
                    //Business as usual if no controller is returned
                    return true;
                }
            }
        }
        else
        {
            if (__instance.OwnerObject.GetBattleCharacterType() == BattleCharacterType.Player)
            {
                //Initialize Behaviors, overriding original
                if (!AiInitTracker.Initialized.Contains(__instance))
                {
                    RemoveAllChildBehaviors(__instance.rootBehavior);
                    AiInitTracker.Initialized.Add(__instance);
                }
            }
        }
        //Don't modify anything, run AI controller as usual because AI is enabled
        return true;
    }
}