using BepInEx.Unity.IL2CPP.Configuration;
using Common;
using Game;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class RyMDebugOverlay : MonoBehaviour
{
    private bool showOverlay = true;
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
    }

    public void OnDestroy()
    {
        if (overlayEntry != null)
        {
            RyMOverlayManager.Unregister(overlayEntry);
        }
    }


    public void OnGUI()
    {
        const int lineHeight = 18;
        const int numberOfRows = 25;
        if (!showOverlay) return;
        Rect overlayRect = RyMOverlayManager.GetRect(overlayEntry);
        GUI.Box(
            overlayRect,
            "Battle Instance Debug"
        );
        if (BattleManager.GetInstance() != null)
        {
            if (BattleManager.GetInstance().BattlePlayerList != null)
            {
                /*
                GUI.Label(
                new Rect(overlayRect.x + 10, overlayRect.y + 60, 250, 25),
                "Battle Instance Detected");
                */
                if (BattleManager.GetInstance().BattlePlayerList.Count > 0)
                {
                    for (int i = 0; i < BattleManager.GetInstance().BattlePlayerList.Count; i++)
                    {
                        
                        BattlePlayer battlePlayer = BattleManager.GetInstance().BattlePlayerList[i]; //good

                        
                        BattleParameterBase battleCharParam = battlePlayer.battleCharacterParameter;


                        BattleCharacterAnimatorController battleCharAnimController = battlePlayer.battleSkillAnimatorController;

                        CharacterAnimation charAnimation = battlePlayer.characterAnimation;
                        GameCharacterController gameCharController = battlePlayer.characterController;
                        BattleCharacterIndividual battleCharIndividual = battlePlayer.characterIndividual;
                        BattleCharacterController battleCharController = battlePlayer.GetCharacterController();

                        AIBehavior<BattleCharacter> aiBehavior = null;
                        BattleAIController battleAiController = null;
                        BattleCharacter battleCharacter = null;
                        AIParameter<BattleCharacter> aiParameter = null;
                        BattleCharacterHistoryParameter battleHistoryParam = null;
                        AISearcher<BattleCharacter> aiSearcher = null;
                        AISenseParameter aiSenseParam = null;
                        CharacterParameter charParam = null;

                        BattleAIActionParameter battleAiActionParam = null;
                        BattleAIActionRequestParameter battleAiActionRequestParam = null;
                        BattleAIMoveAvoidance battleAiMoveAvoidance = null;
                        AIMoveController<BattleCharacter> aiMoveController = null;

                        AIMoveResult aiMoveResult = null;

                        CharacterShadowAnimation charShadowAnim = null;
                        AIController<BattleCharacter> aiController = null;

                        if (battlePlayer != null)
                        {
                            battleAiController = battlePlayer.battleAIController;
                        }
                        if (battleAiController != null)
                        {
                            battleCharacter = battlePlayer.battleAIController.OwnerObject;
                        }
                        if (aiBehavior != null)
                        {
                            aiParameter = aiBehavior.aiParameter;
                        }
                        if (aiParameter != null)
                        {
                            aiSearcher = aiParameter.aiSearcher;
                            aiController = aiParameter.aiController;
                        }
                        if (aiMoveController != null)
                        {
                            aiMoveResult = aiMoveController.aiMoveResult;
                        }
                        if (aiSearcher != null)
                        {
                            aiSenseParam = aiSearcher.aiSenseParameter;
                        }
                        if (battleCharParam != null)
                        {
                            battleHistoryParam = battleCharParam.historyParameter;
                            charParam = battleCharParam.characterParameter;
                        }
                        if (battleAiController != null)
                        {
                            battleAiActionParam = battleAiController.actionParameter;
                            battleAiActionRequestParam = battleAiController.actionRequestParameter;
                            battleAiMoveAvoidance = battleAiController.aiMoveAvoidance;
                            aiMoveController = battleAiController.aiMoveController;
                        }
                        if (charAnimation != null)
                        {
                            charShadowAnim = charAnimation.CharacterShadowAnimation;
                        }
                        
                        StringBuilder sb = new StringBuilder();
                        if (battlePlayer != null)
                        {
                            sb.AppendLine($"P{i}:{battlePlayer.indexInParty} {(PlayerID)battlePlayer.CharacterID}: Position ({battlePlayer.transform.position.x:0.00},{battlePlayer.transform.position.y:0.00},{battlePlayer.transform.position.z:0.00}){Environment.NewLine}");
                            sb.AppendLine($"charDirection {battlePlayer.characterDirection}");
                            sb.AppendLine($"isRun {battlePlayer.IsRun}");
                            sb.AppendLine($"dontAddPauser {battlePlayer.dontAddPauser}");
                            sb.AppendLine($"generateCount {battlePlayer.generateCount}");
                            sb.AppendLine($"emissionColor {battlePlayer.emissionColor}");
                            sb.AppendLine($"isUpdateModelColor {battlePlayer.isUpdateModelColor}");
                            sb.AppendLine($"modelColor {battlePlayer.modelColor}");
                        }
                        /*
                        if (aiBehavior != null)
                        {
                            sb.AppendLine($"a{aiBehavior.ToString()}");
                        }
                        */
                        /*
                        if (battleCharacter != null)
                        {
                            sb.AppendLine($"=====");

                        }
                        */
                        if (battleCharParam != null)
                        {
                            sb.AppendLine($"=====");
                            sb.AppendLine($"attackRate {battleCharParam.AttackRate:0.00}");
                            sb.AppendLine($"counterRate {battleCharParam.CounterRate:0.00}");
                            sb.AppendLine($"maxRecastTime {battleCharParam.maxRecastTime:0.00}");
                            sb.AppendLine($"recastTimer {battleCharParam.recastTimer:0.00}");
                            sb.AppendLine($"moveType {battleCharParam.MoveType}");
                            //sb.AppendLine($"specialFxTimer {battleCharParam.specialEffectsTimer}");
                            sb.AppendLine($"speed {battleCharParam.Speed}");
                        }
                        if (aiMoveController != null)
                        {
                            sb.AppendLine($"=====");
                            sb.AppendLine($"moveSpeedRate {aiMoveController.moveSpeedRate:0.00}");
                            sb.AppendLine($"moveState {aiMoveController.moveState}");
                            sb.AppendLine($"isWalk {aiMoveController.isWalk}");
                            sb.AppendLine($"state {aiMoveController.State}");
                            sb.AppendLine($"isDisableRun {aiMoveController.isDisableRun}");
                        }
                        if (aiMoveResult != null)
                        {
                            sb.AppendLine($"=====");
                            sb.AppendLine($"moveRate {aiMoveResult.moveRate:0.00}");
                            sb.AppendLine($"moveDir {aiMoveResult.moveDir}");
                            
                        }

                        if (gameCharController != null)
                        {
                            sb.AppendLine($"=====");
                            sb.AppendLine($"prevState {(BattleCharacterState)gameCharController.prevState}");
                            sb.AppendLine($"currentState {(BattleCharacterState)gameCharController.currentState}");
                            sb.AppendLine($"nextState {(BattleCharacterState)gameCharController.nextState}");
                        }

                        if (battleAiController != null)
                        {
                            sb.AppendLine($"=====");
                            AIBehavior<BattleCharacter> currentBehavior = battleAiController.rootBehavior;
                            sb.Append(currentBehavior.ToString() + ", ");
                            while (currentBehavior.childBehavior != null)
                            {
                                currentBehavior = currentBehavior.childBehavior;
                                sb.Append(currentBehavior.ToString());
                            }
                        }

                        //var a = (BattleCharacter)battlePlayer;
                        
                        GUI.Label(
                            new Rect(
                            overlayRect.x + 10,
                            overlayRect.y + 25 + (lineHeight * numberOfRows * i),
                            1000,
                            lineHeight * numberOfRows), sb.ToString());
                        /*
                        $"====={Environment.NewLine}" +
                        $"moveSpeedRate {battlePlayer.battleAIController.aiMoveController.moveSpeedRate}{Environment.NewLine}" +
                        $"moveRate {battlePlayer.battleAIController.aiMoveController.aiMoveResult.moveRate:0.00}{Environment.NewLine}" +
                        $"speed {battlePlayer.battleCharacterParameter.Speed:0.00}{Environment.NewLine}" +
                        $"buffdebuffSpeedvalue {battlePlayer.battleCharacterParameter.BuffDebuffSpeedValue:0.00}{Environment.NewLine}" +
                        $"isWalk {battlePlayer.battleAIController.aiMoveController.isWalk}{Environment.NewLine}" +
                        $"isRun {battlePlayer.battleAIController.aiMoveController.aiMoveResult.isRun}{Environment.NewLine}" +
                        $"====={Environment.NewLine}" +
                        //$"taskList {taskList}{Environment.NewLine}" +
                        //$"renderInfoList {renderInfoList}{Environment.NewLine}" +
                        $"shadowColor {battlePlayer.shadowController.enabled}{Environment.NewLine}" +
                        $"shadowCount {battlePlayer.shadowController.generateCount}{Environment.NewLine}" +
                        $"shadowHideFlag {battlePlayer.shadowController.hideFlags}{Environment.NewLine}" +
                        //BattleCharacterActionResult.
                        $"startRunningDistance {battlePlayer.battleAIController.aiMoveController.startRunningDistance:0.00}{Environment.NewLine}" +
                        $"stopRunningDistance {battlePlayer.battleAIController.aiMoveController.stopRunningDistance:0.00}{Environment.NewLine}" +
                        $"====={Environment.NewLine}" +
                        //$"aiState {player.battleAIController.aiMoveController.State}{Environment.NewLine}" +
                        //$"aiMoveState {player.battleAIController.aiMoveController.moveState}{Environment.NewLine}" +
                        //$"turnState {player.battleAIController.aiMoveController.turnState}{Environment.NewLine}" +

                        $"=====#####====={Environment.NewLine}" +

                        //$"isAiMoveAvoidnaceEnabled {player.BattleAIController.aiMoveAvoidance.isEnable}{Environment.NewLine}" +
                        /*
                        $"ParamSpeed {player.battleCharacterParameter.Speed}{Environment.NewLine}" +
                        $"NormalAttackSkillId {string.Join(", ", player.battleCharacterParameter.NormalAttackSkillID.ToArray())}{Environment.NewLine}" +
                        $"JumpAttackSkillId {player.battleCharacterParameter.JumpAttackSkillID}{Environment.NewLine}" +
                        $"AntiAirAttackSkillId {player.battleCharacterParameter.AntiAirAttackSkillID}{Environment.NewLine}" +
                        $"AtkRate {player.battleCharacterParameter.AttackRate}{Environment.NewLine}" +
                        */

                        //$"CounterRate {player.battleCharacterParameter.CounterRate}{Environment.NewLine}" +
                        //$"RecastTimer {player.battleCharacterParameter.recastTimer}{Environment.NewLine}" +
                        //$"AiInterval {player.BattleAIController.aiInterval}{Environment.NewLine}" +
                        //$"ActionParamBattleSkillId {battlePlayer.BattleAIController.actionParameter.BattleSkillID}{Environment.NewLine}" +
                        //$"IsForceTurning {player.BattleAIController.actionParameter.IsForceTurning}{Environment.NewLine}" +

                        //$"IsIgnoreConsume {player.BattleAIController.actionParameter.IsIgnoreConsume}{Environment.NewLine}" +
                        //$"IsLong {player.BattleAIController.actionParameter.IsLong}{Environment.NewLine}" +
                        //$"IsReservedAction {player.BattleAIController.actionParameter.IsReservedAction}{Environment.NewLine}" +
                        //$"RootType {player.BattleAIController.actionParameter.RootType}{Environment.NewLine}" +

                        //$"isReflection {player.BattleAIController.aiMoveAvoidance.isReflection}{Environment.NewLine}" +
                        //$"isAvoidance {player.BattleAIController.aiMoveAvoidance.IsAvoidance}{Environment.NewLine}" +
                        //__instance.OwnerObject.GetCharacterController().currentState


                        //$"isOtherTargetCancelAttack {player.GetCharacterController().isOtherTargetCancelAttack}{Environment.NewLine}" +
                        //$"lastAnimationKickerName {player.animationKicker.lastAnimationName}{Environment.NewLine}" +
                        //$"IsReserveAction {player.BattleAIController.actionParameter.IsReservedAction}{Environment.NewLine}" +
                        //$"lastAnimationKickerName {player}{Environment.NewLine}" +
                        //$"taskListCount {player.GetCharacterController().taskList.Clear}{Environment.NewLine}" +
                        //$"specialEffectsTimer {string.Join("-",player.battleCharacterParameter.specialEffectsTimer.ToArray())}{Environment.NewLine}" +
                        //$"fadeTime {player.battleCharacterFadeTask.fadeTime}{Environment.NewLine}" +
                        //$"alphaInterpIsActive {player.battleCharacterFadeTask.alphaInterpolator.isActive}{Environment.NewLine}" +
                        //$"startAlpha {player.battleCharacterFadeTask.startAlpha}{Environment.NewLine}" +
                        //$"transformationPattern {player.battleCharacterParameter.transformationPattern}{Environment.NewLine}" +
                        //$"attackNotifyTaskIsInit {player.attackNotifyTask.isInitialized}{Environment.NewLine}" +
                        //$"vd_pointerSize {player.GetCharacterController().variableData.pointerSize}{Environment.NewLine}" +
                        //$"vd_type {player.GetCharacterController().variableData.GetType()}{Environment.NewLine}" +
                        //$"vd_string {player.GetCharacterController().variableData.ToString()}{Environment.NewLine}" +
                        //$"vd_objectClass {player.GetCharacterController().variableData.}{Environment.NewLine}" +
                        //$"currentTask {characterTasks}{Environment.NewLine}" +
                        //$"checkCombatSkillHighSpeed {player.CheckCombatSkill(CombatSkillID.HIGH_SPEED)}{Environment.NewLine}" +
                        //$"highSpeedEffectValue {player.GetCombatSkillEffectValue(CombatSkillID.HIGH_SPEED)}{Environment.NewLine}" +
                        //$"emissionColor {player.emissionColor.ToString()}{Environment.NewLine}" +
                        //$"getLayerMaskWall {player.GetLayerMaskWall()}{Environment.NewLine}" +
                        //$"GetMoveAnimationName {player.GetMoveAnimationName(player.IsRun)}{Environment.NewLine}" +
                        //$"pauseBehaviourList {behaviourList}{Environment.NewLine}" +
                    }
                }
                else
                {
                    GUI.Label(
                    new Rect(overlayRect.x + 10, overlayRect.y + 60, 250, 25),
                    "No players found");
                }
            }
        }
        
    }
}