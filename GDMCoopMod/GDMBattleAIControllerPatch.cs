using Common;
using Game;
using GDMCoopMod;
using HarmonyLib;
using JetBrains.Annotations;
using SimpleSpritePacker;
using System;
using System.Collections.Generic;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

//Patch do attack notify to store information about attack warnings
[HarmonyPatch(typeof(BattleCharacter), nameof(BattleCharacter.DoAttackNotify))]
class NotifyPatch
{
    private static bool partnerNotify;
    private static BattleCharacter notifyTarget;

    public static bool GetPartnerNotify()
    {
        return partnerNotify;
    }

    public static BattleCharacter GetNotifyTarget()
    {
        return notifyTarget;
    }

    static void Prefix(BattleCharacter target)
    {
        notifyTarget = target;
        //Check if valid, controlled character that is not leader
        partnerNotify = (target != null && !target.IsControlPlayer() && GDMControllerRouting.GetControllerForParty(target.indexInParty) >= 0);
    }

    static void Finalizer()
    {
        partnerNotify = false;
        notifyTarget = null;
    }
}

//Patches enemy attacking to create a red aura notification by forcing return that this is a target to notify
[HarmonyPatch(typeof(BattleCharacter), nameof(BattleCharacter.IsControlPlayer))]
class ControlPlayerPatch
{
    static void Postfix(BattleCharacter __instance, ref bool __result)
    {
        if (NotifyPatch.GetPartnerNotify() && __instance == NotifyPatch.GetNotifyTarget()) __result = true;
    }
}

//Patch MP Presenter
[HarmonyPatch(typeof(UIBattleWarningMPPresenter), nameof(UIBattleWarningMPPresenter.Show))]
class WarningMpShowPatch
{
    public static BattleCharacter PartnerWarningTarget; // set only around a partner's call

    static void Prefix(UIBattleWarningMPPresenter __instance)
    {
        BattleCharacter owner = PartnerWarningTarget;
        if (owner == null) owner = BattleManager.GetInstance().GetControlPlayer();
        PartnerWarningTarget = null; // Reset partner warning target

        if (owner == null) return; // Use original code
        var ui = GameUIManager.GetInstance().UIBattleController;
        var offset = UIBattleController.StaminaWorldOffset;
        __instance.Set(owner.transform, ui.myCanvas, ref offset);
    }
}

//Patch AIRun
[HarmonyPatch(typeof(BattleAIController), "AIRun")]
public static class BattleAIControllerPatch
{
    private static bool isPartyAiEnabled = false;
    private static bool[] useHostTarget = new bool[4] { true, true, true, true };
    private static int[] spellIndex = new int[4] { -1, -1, -1, -1 };
    private static List<BattleSkillID>[] listOfCharacterSpells = new List<BattleSkillID>[4]
    {
        new List<BattleSkillID>(),
        new List<BattleSkillID>(),
        new List<BattleSkillID>(),
        new List<BattleSkillID>()
    };
    private static bool[] isRightSkillActive = new bool[4] { false, false, false, false };
    private static bool[] isLeftSkillActive = new bool[4] { false, false, false, false };

    /// <summary>
    /// Modulos to the next spell slot
    /// </summary>
    public static void NextSpell(int partyIndex)
    {
        if (partyIndex < 0 || partyIndex >= spellIndex.Length)
            return;

        List<BattleSkillID> spells = listOfCharacterSpells[partyIndex];

        if (spells == null || spells.Count == 0)
        {
            spellIndex[partyIndex] = -1;
            return;
        }

        spellIndex[partyIndex] =
            (spellIndex[partyIndex] + 1 + spells.Count) % spells.Count;
    }

    /// <summary>
    /// Modulos to the previous spell slot
    /// </summary>
    public static void PreviousSpell(int partyIndex)
    {
        if (partyIndex < 0 || partyIndex >= spellIndex.Length)
            return;

        List<BattleSkillID> spells = listOfCharacterSpells[partyIndex];

        if (spells == null || spells.Count == 0)
        {
            spellIndex[partyIndex] = -1;
            return;
        }

        spellIndex[partyIndex] =
            (spellIndex[partyIndex] - 1 + spells.Count) % spells.Count;
    }

    // ------------------------------------------------------------------
    // Ally targeting for single-target support spells identified by system
    // ------------------------------------------------------------------

    // Index = casting party member. Value = list/party index of the ally that caster will target.
    // Defaults to the caster themselves; reset at the start of every battle.
    private static int[] allyTargetIndex = new int[4] { 0, 1, 2, 3 };

    // True = this caster uses the game's normal targeting for support spells (toggled with R3).
    private static bool[] useNormalSupportTargeting = new bool[4] { false, false, false, false };

    // Cache of "is this spell a single-ally support spell?" so we only ask the game once per spell.
    private static Dictionary<BattleSkillID, bool> supportSpellCache = new Dictionary<BattleSkillID, bool>();

    /// <summary>
    /// Determines who the target is based on the index.
    /// </summary>
    /// <returns>
    /// Returns the selected ally index for each casting party member.
    /// </returns>
    public static int[] GetAllyTargetIndex()
    {
        return allyTargetIndex;
    }

    /// <summary>
    /// True if the spell targets a single ally (the game's own TargetType.PLAYER),
    /// e.g. Heal, Cure Light, Antidote, Haste, Bless. Self-only and party-wide spells return false
    /// because there is nothing to choose for them.
    /// </summary>
    /// <returns>
    /// Returns the definition if the spell counts as a support spell based on the game's definition.
    /// </returns>
    public static bool IsSupportSpell(BattleSkillID skillId)
    {
        if (skillId == BattleSkillID.INVALID)
        {
            return false;
        }

        // Revive spells always count, so they can be aimed at a downed ally.
        if (IsReviveSpell(skillId))
        {
            return true;
        }

        bool isSupport;
        if (supportSpellCache.TryGetValue(skillId, out isSupport))
        {
            return isSupport;
        }

        // If it's not found in support spell cache, potentially enter in a new one so we don't have to again.
        try
        {
            ConstBattleSkillParameter parameter = ParameterManager.Instance.GetBattleSkillParameter(skillId);
            if (parameter == null)
            {
                return false; // Not cached, parameters may not be loaded yet.
            }
            isSupport = parameter.TargetType == TargetType.PLAYER;
            supportSpellCache[skillId] = isSupport;
            return isSupport;
        }
        catch (Exception e)
        {
            GDMCoopPlugin.StaticLog.LogWarning("Could not read target type for " + skillId + ": " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// True if this caster is using the game's normal targeting for support spells, toggled with R3 or
    /// because Raise Dead is selected and nobody is down (nothing valid to pick).
    /// While Raise Dead is selected and someone is down, this also moves the selection onto a downed ally.
    /// </summary>
    /// <returns>
    /// Returns a boolean value suggesting if we are using the default casting method, otherwise false.
    /// </returns>
    public static bool IsNormalSupportTargeting(int partyIndex)
    {
        // Check if target is valid and within the bounds of the party index.
        if (partyIndex < 0 || partyIndex >= useNormalSupportTargeting.Length)
        {
            return false;
        }
            
        // Check if the party index selected is using normal targeting.
        if (useNormalSupportTargeting[partyIndex])
        {
            return true;
        }
            
        // Check if the spell is not a revive spell from the selected party index.
        if (!IsReviveSpell(GetSelectedSpell(partyIndex)))
        {
            return false;
        }
        
        // Check if a battle manager exists.
        BattleManager battleManager = BattleManager.GetInstance();
        if (battleManager == null)
        {
            return false;
        }
        
        // Check if there are no valid players.
        var players = battleManager.battlePlayerList;
        if (players == null || players.Count == 0)
        {
            return false;
        }

        // Clamping logic.
        int count = players.Count;
        int current = allyTargetIndex[partyIndex];
        if (current < 0 || current >= count)
        {
            current = partyIndex < count ? partyIndex : 0;
        }

        // Keep the current pick if it's already someone who is down, otherwise take the next downed ally.
        for (int i = 0; i < count; i++)
        {
            int candidate = (current + i) % count;
            BattlePlayer candidatePlayer = players[candidate];

            if (candidatePlayer != null && candidatePlayer.IsDead())
            {
                allyTargetIndex[partyIndex] = candidate;
                return false;
            }
        }

        // Nobody is down: fall back to the game's normal targeting.
        return true;
    }

    /// <summary>
    /// Switches a caster between manual ally targeting and the game's normal targeting.
    /// Does nothing unless a support spell is selected. Returns true if the mode changed.
    /// </summary>
    /// <returns>
    /// Returns a boolean value determining whether to use the default target system or the mod's.
    /// </returns>
    public static bool ToggleNormalSupportTargeting(int partyIndex)
    {
        if (partyIndex < 0 || partyIndex >= useNormalSupportTargeting.Length)
        {
            return false;
        }

        if (IsHostControlled(partyIndex) || !IsSupportSpellSelected(partyIndex))
        {
            return false;
        }

        useNormalSupportTargeting[partyIndex] = !useNormalSupportTargeting[partyIndex];
        return true;
    }

    /// <summary>
    /// True if this party member is the character the host is currently controlling.
    /// The host uses the game's own menus and targeting, so the co-op ally selection never applies to them.
    /// </summary>
    /// <returns>
    /// Returns a boolean value determining if the party member in question is controlled by the host.
    /// </returns>
    public static bool IsHostControlled(int partyIndex)
    {
        BattleManager battleManager = BattleManager.GetInstance();
        return battleManager != null && battleManager.ControlPlayerIndex == partyIndex;
    }

    /// <summary>
    /// True for spells that bring a downed ally back. These can only target downed allies.
    /// </summary>
    /// <returns>
    /// Returns a boolean value determining the spell passed can target downed allies.
    /// </returns>
    public static bool IsReviveSpell(BattleSkillID skillId)
    {
        return skillId == BattleSkillID.M_RAISEDEAD;
    }

    /// <summary>
    /// Returns the spell this party member currently has selected, else return invalid.
    /// </summary>
    /// <returns>
    /// Returns a battle skill id for the selected spell of a selected party.
    /// </returns>
    public static BattleSkillID GetSelectedSpell(int partyIndex)
    {
        if (partyIndex < 0 || partyIndex >= spellIndex.Length)
        {
            return BattleSkillID.INVALID;
        }

        List<BattleSkillID> spells = listOfCharacterSpells[partyIndex];
        int index = spellIndex[partyIndex];

        if (spells == null || index < 0 || index >= spells.Count)
        {
            return BattleSkillID.INVALID;
        }

        return spells[index];
    }

    /// <summary>
    /// True if the party member currently has a single-ally support spell selected.
    /// </summary>
    /// <returns>
    /// Returns a boolean value determining if the party index has a support spell selected.
    /// </returns>
    public static bool IsSupportSpellSelected(int partyIndex)
    {
        return IsSupportSpell(GetSelectedSpell(partyIndex));
    }

    /// <summary>
    /// Moves a caster's ally selection to the previous (-1) or next (+1) living party member.
    /// Does nothing unless a support spell is selected.
    /// </summary>
    /// <returns>
    /// Returns true if the selection changes.
    /// </returns>
    public static bool CycleAllyTarget(int partyIndex, int direction)
    {
        if (partyIndex < 0 || partyIndex >= allyTargetIndex.Length || direction == 0)
        {
            return false;
        }

        if (IsHostControlled(partyIndex) || !IsSupportSpellSelected(partyIndex))
        {
            return false;
        }

        BattleManager battleManager = BattleManager.GetInstance();
        if (battleManager == null)
        {
            return false;
        }

        var players = battleManager.battlePlayerList;
        if (players == null || players.Count == 0)
        {
            return false;
        }

        int count = players.Count;
        int current = allyTargetIndex[partyIndex];
        if (current < 0 || current >= count)
        {
            current = partyIndex < count ? partyIndex : 0;
        }

        int step = direction > 0 ? 1 : -1;

        // Revive spells can only be aimed at downed allies; everything else only at living ones.
        bool wantDead = IsReviveSpell(GetSelectedSpell(partyIndex));

        // Moving the stick left/right while on normal targeting switches back to manual targeting.
        useNormalSupportTargeting[partyIndex] = false;

        // Walk around the party, skipping anyone who doesn't fit the spell.
        for (int i = 1; i <= count; i++)
        {
            int candidate = (((current + step * i) % count) + count) % count;
            BattlePlayer candidatePlayer = players[candidate];

            if (candidatePlayer != null && candidatePlayer.IsDead() == wantDead)
            {
                allyTargetIndex[partyIndex] = candidate;
                return candidate != current;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the ally this caster picked for a support spell, or null if the spell isn't a
    /// single-ally support spell or the picked ally is gone/down which is the default system.
    /// </summary>
    private static BattleCharacter GetSelectedAllyTarget(int partyIndex, BattleSkillID skillId)
    {
        if (partyIndex < 0 || partyIndex >= allyTargetIndex.Length)
        {
            return null;
        }

        if (!IsSupportSpell(skillId))
        {
            return null;
        }

        // Normal targeting (toggled, or Raise Dead with nobody down): let the game pick.
        if (IsNormalSupportTargeting(partyIndex))
        {
            return null;
        }

        BattleManager battleManager = BattleManager.GetInstance();
        if (battleManager == null)
        {
            return null;
        }

        var players = battleManager.battlePlayerList;
        int target = allyTargetIndex[partyIndex];

        if (players == null || target < 0 || target >= players.Count)
        {
            return null;
        }

        BattlePlayer targetPlayer = players[target];
        if (targetPlayer == null)
        {
            return null;
        }

        // Revive spells need a downed target, other spells a living one. Otherwise the game's own targeting is used.
        if (targetPlayer.IsDead() != IsReviveSpell(skillId))
        {
            return null;
        }

        return targetPlayer;
    }

#if DEBUG
    /// <summary>
    /// Writes each spell's target type to the BepInEx log once per battle start, which makes it easy to check
    /// which spells the mod treats as support spells.
    /// </summary>
    private static void LogSpellList(int partyIndex)
    {
        try
        {
            List<BattleSkillID> spells = listOfCharacterSpells[partyIndex];
            for (int i = 0; i < spells.Count; i++)
            {
                GDMCoopPlugin.StaticLog.LogInfo(
                    "Party " + (partyIndex + 1) + " spell " + spells[i] +
                    (IsSupportSpell(spells[i]) ? " -> ally target selectable" : " -> default targeting"));
            }
        }
        catch (Exception e)
        {
            GDMCoopPlugin.StaticLog.LogWarning("Could not log spell list: " + e.Message);
        }
    }
#endif

    /// <summary>
    /// Returns 4 lists of BattleSkillIds
    /// </summary>
    public static List<BattleSkillID>[] GetListOfCharacterSpells()
    {
        return listOfCharacterSpells;
    }

    /// <summary>
    /// Clamps the selected skill if it exceeds the skill list
    /// </summary>
    public static void ClampSpellIndex(int partyIndex)
    {
        int characterIndex = GDMControllerRouting.GetControllerForParty(partyIndex);
        if (characterIndex >= 0 && characterIndex <= 3)
        {
            if (listOfCharacterSpells[characterIndex].Count == 0)
            {
                spellIndex[characterIndex] = -1;
            }

            if (spellIndex[characterIndex] >= listOfCharacterSpells[characterIndex].Count)
            {
                spellIndex[characterIndex] = listOfCharacterSpells[characterIndex].Count - 1;
            }
        }
    }

    public struct BattleSkillState
    {
        public BattleSkillID SkillId;
        public int SkillIndex;

        public BattleSkillState(BattleSkillID skillId, int skillIndex)
        {
            SkillId = skillId;
            SkillIndex = skillIndex;
        }
    }

    private static BattleSkillState[] lastLeftBattleSkill = new BattleSkillState[4]
    {
    new BattleSkillState(BattleSkillID.INVALID, 0),
    new BattleSkillState(BattleSkillID.INVALID, 0),
    new BattleSkillState(BattleSkillID.INVALID, 0),
    new BattleSkillState(BattleSkillID.INVALID, 0)
    };

    private static BattleSkillState[] lastRightBattleSkill = new BattleSkillState[4]
    {
    new BattleSkillState(BattleSkillID.INVALID, 0),
    new BattleSkillState(BattleSkillID.INVALID, 0),
    new BattleSkillState(BattleSkillID.INVALID, 0),
    new BattleSkillState(BattleSkillID.INVALID, 0)
    };

    /// <summary>
    /// Sets the index controller to target host or not
    /// </summary>
    public static void SetHostTargetingMode(int index, bool isEnabled)
    {
        if (index >= 0 && index <= 3)
        {
            useHostTarget[index] = isEnabled;
        }
    }

    /// <summary>
    /// Gets from memory if the index controller targets host or not
    /// </summary>
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
    /// Returns 4 selected spell index
    /// </summary>
    public static int[] GetSpellIndex()
    {
        return spellIndex;
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

    private static void ClearPendingAI(BattleAIController ai)
    {
        ai.ActionParameter.Reset();
        ai.ActionRequestParameter.Reset();
    }
    private static void NoMPNotification(BattleCharacter battleCharacter)
    {
        WarningMpShowPatch.PartnerWarningTarget = battleCharacter;
        try
        {
            GameUIManager uiManager = GameUIManager.GetInstance();
            UIBattleController uiBattleController = uiManager.UIBattleController;
            uiBattleController.ShowWarningMP();
        }
        finally
        {
            WarningMpShowPatch.PartnerWarningTarget = null;
        }
    }

    //Start of AIRun
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
                    //Init special case for spellcasters
                    listOfCharacterSpells[__instance.OwnerObject.indexInParty].Clear();
                    //Default ally target is the caster themselves at the start of each battle
                    allyTargetIndex[__instance.OwnerObject.indexInParty] = __instance.OwnerObject.indexInParty;
                    useNormalSupportTargeting[__instance.OwnerObject.indexInParty] = false;
                    if (        
                        (PlayerID)__instance.OwnerObject.CharacterID == PlayerID.RENA ||
                        (PlayerID)__instance.OwnerObject.CharacterID == PlayerID.CELINE ||
                        (PlayerID)__instance.OwnerObject.CharacterID == PlayerID.LEON ||
                        (PlayerID)__instance.OwnerObject.CharacterID == PlayerID.NOEL)
                    {
                        
                        var casterBehavior = new BattleAISpellCasterBehavior();
                        casterBehavior.Initialize(__instance.aiParameter);
                        TacticsID originalId = __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID;
                        casterBehavior.AIRun();
                        __instance.OwnerObject.GetCharacterController().Reset();

                        for (int i = 0; i < casterBehavior.battleSkillList.Count; i++)
                        {
                            listOfCharacterSpells[__instance.OwnerObject.indexInParty].Add(casterBehavior.battleSkillList[i].battleSkillID);
                        }
                        if (listOfCharacterSpells[__instance.OwnerObject.indexInParty].Count > 0)
                        {
                            if (spellIndex[__instance.OwnerObject.indexInParty] == -1)
                            {
                                spellIndex[__instance.OwnerObject.indexInParty] = 0;
                            }
                            //Clamp spell index so it's not out of bounds
                            ClampSpellIndex(__instance.OwnerObject.indexInParty);
#if DEBUG
                            LogSpellList(__instance.OwnerObject.indexInParty);
#endif
                        }
                        casterBehavior = null;
                    }
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
                    if (isLeftSkillActive[__instance.OwnerObject.IndexInParty]) isLeftSkillActive[__instance.OwnerObject.IndexInParty] = false;
                    if (isRightSkillActive[__instance.OwnerObject.IndexInParty]) isRightSkillActive[__instance.OwnerObject.IndexInParty] = false;

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
                        if (isLeftSkillActive[__instance.OwnerObject.indexInParty]) isLeftSkillActive[__instance.OwnerObject.indexInParty] = false;
                        if (isRightSkillActive[__instance.OwnerObject.indexInParty]) isRightSkillActive[__instance.OwnerObject.indexInParty] = false;

                    }
                    //Diable inputs if you are dead
                    if (__instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Dead ||
                        __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Incapacitated)
                    {
                        return false;
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
                                    ClearPendingAI(__instance);
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
                        if (GDMCoopPlugin.VirtualControllers.GetState(controllerIndex).LeftSkillPressed && !isRightSkillActive[__instance.OwnerObject.indexInParty])
                        {
                            if (!isLeftSkillActive[__instance.OwnerObject.indexInParty]) isLeftSkillActive[__instance.OwnerObject.indexInParty] = true;
                            
                            if (GetDeepestBehavior(__instance.rootBehavior).ToString() == "Game.BattleAINullBehavior" || __instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Move)
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
                                lastLeftBattleSkill[__instance.OwnerObject.indexInParty].SkillIndex = 0;
                                lastLeftBattleSkill[__instance.OwnerObject.indexInParty].SkillId = BattleSkillID.INVALID;
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
                            if (lastLeftBattleSkill[__instance.OwnerObject.indexInParty].SkillIndex == 1 &&
                                __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex == 0 &&
                                skillId == BattleSkillID.INVALID)
                            {
                                __instance.OwnerObject.GetCharacterController().Reset();
                                skillId = __instance.OwnerObject.GetCharacterController().GetNextBattleSkillID(BattleDefine.RootType.Left);
                            }
                            //Remember last skillIndex and Skill
                            lastLeftBattleSkill[__instance.OwnerObject.indexInParty].SkillIndex = __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex;
                            lastLeftBattleSkill[__instance.OwnerObject.indexInParty].SkillId = skillId;

                            BattleCharacterActionResult actionResult = BattleCharacterActionResult.Invalid;
                            //Check if can proceed to skill
                            if (skillId != BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().reserveBattleSkillID == BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().battleSkillLeftIndex < 2 && !__instance.OwnerObject.GetCharacterController().IsLinkComboAction())
                            {
                                //Check if have MP
                                if (__instance.OwnerObject.battleCharacterParameter.CanUseBattleSkill(skillId))
                                {
                                    //Targeting Logic
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
                                else
                                {
                                    //Play buzzer if no MP
                                    NoMPNotification(__instance.OwnerObject);
                                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.ActionBuzzer);
                                }
                            }
                            //Return original tactics to AI to prevent strategy menu from being bad
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = originalId;
                        }

                        //Right Skill Logic
                        if (GDMCoopPlugin.VirtualControllers.GetState(controllerIndex).RightSkillPressed && !isLeftSkillActive[__instance.OwnerObject.indexInParty])
                        {
                            if (!isRightSkillActive[__instance.OwnerObject.indexInParty]) isRightSkillActive[__instance.OwnerObject.indexInParty] = true;
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
                                lastRightBattleSkill[__instance.OwnerObject.indexInParty].SkillIndex = 0;
                                lastRightBattleSkill[__instance.OwnerObject.indexInParty].SkillId = BattleSkillID.INVALID;
                                __instance.OwnerObject.GetCharacterController().Reset();
                            }
                            TacticsID originalId = __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID;
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = TacticsID.INVALID;
                            if (!__instance.enableAIMove) __instance.enableAIMove = true;
                            BattleCharacterActionResult actionResult = BattleCharacterActionResult.Invalid;

                            BattleSkillID skillId = __instance.OwnerObject.GetCharacterController().GetNextBattleSkillID(BattleDefine.RootType.Right);
                            if (lastRightBattleSkill[__instance.OwnerObject.indexInParty].SkillIndex == 1 &&
                                __instance.OwnerObject.GetCharacterController().battleSkillRightIndex == 0 &&
                                skillId == BattleSkillID.INVALID)
                            {
                                __instance.OwnerObject.GetCharacterController().Reset();
                                skillId = __instance.OwnerObject.GetCharacterController().GetNextBattleSkillID(BattleDefine.RootType.Right);
                            }
                            lastRightBattleSkill[__instance.OwnerObject.indexInParty].SkillIndex = __instance.OwnerObject.GetCharacterController().battleSkillRightIndex;
                            lastRightBattleSkill[__instance.OwnerObject.indexInParty].SkillId = skillId;
                            if (skillId != BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().reserveBattleSkillID == BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().battleSkillRightIndex < 2 && !__instance.OwnerObject.GetCharacterController().IsLinkComboAction())
                            {
                                if (__instance.OwnerObject.battleCharacterParameter.CanUseBattleSkill(skillId))
                                {
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
                                else
                                {
                                    NoMPNotification(__instance.OwnerObject);
                                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.ActionBuzzer);
                                }
                                
                            }
                            __instance.OwnerObject.battleCharacterParameter.characterParameter.TacticsID = originalId;
                        }

                        //Spellcaster Button
                        if (GDMCoopPlugin.VirtualControllers.GetState(controllerIndex).UseMagicPressed)
                        {
                            if (__instance.OwnerObject.GetCharacterController().GetCurrentState() == BattleCharacterState.Move)
                            {
                                if (__instance.OwnerObject.battleAIController.actionParameter.BattleSkillID == BattleSkillID.INVALID &&
                                ((PlayerID)__instance.OwnerObject.CharacterID == PlayerID.RENA ||
                                (PlayerID)__instance.OwnerObject.CharacterID == PlayerID.CELINE ||
                                (PlayerID)__instance.OwnerObject.CharacterID == PlayerID.LEON ||
                                (PlayerID)__instance.OwnerObject.CharacterID == PlayerID.NOEL))
                                {
                                    BattleCharacterActionResult actionResult = BattleCharacterActionResult.Invalid;
                                    BattleSkillID skillId = BattleSkillID.INVALID;
                                    if (listOfCharacterSpells[__instance.OwnerObject.indexInParty].Count > 0)
                                    {
                                        //Assign skill
                                        skillId = listOfCharacterSpells[__instance.OwnerObject.indexInParty][spellIndex[__instance.OwnerObject.indexInParty]];
                                    }

                                    //Do skill like any other skill but without root define left or right
                                    if (skillId != BattleSkillID.INVALID && __instance.OwnerObject.GetCharacterController().reserveBattleSkillID == BattleSkillID.INVALID)
                                    {
                                        if (__instance.OwnerObject.battleCharacterParameter.CanUseBattleSkill(skillId))
                                        {
                                            BattleCharacter targetToCheck = __instance.OwnerObject.GetCharacterController().GetTargetOnAction(skillId);
                                            //Support spells go to the ally picked with right stick left/right
                                            BattleCharacter allyTarget = GetSelectedAllyTarget(__instance.OwnerObject.indexInParty, skillId);
                                            if (allyTarget != null)
                                            {
                                                actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, allyTarget);
                                            }
                                            else if (targetToCheck.IsPlayerSide() || useHostTarget[controllerIndex])
                                            {
                                                actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId);
                                            }
                                            else
                                            {
                                                BattleEnemy closestEnemy = GetClosestEnemy(__instance.OwnerObject);
                                                if (closestEnemy != null)
                                                {
                                                    actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId, closestEnemy);
                                                }
                                                else
                                                {
                                                    actionResult = __instance.OwnerObject.GetCharacterController().OnBattleSkill(skillId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            NoMPNotification(__instance.OwnerObject);
                                            GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.ActionBuzzer);
                                        }
                                    }
                                }
                            }
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