using BepInEx.Unity.IL2CPP.Configuration;
using Common;
using Game;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;

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
                        if (battleAiActionParam != null)
                        {
                            sb.AppendLine($"=====");
                            sb.AppendLine($"battleSkillId {battleAiActionParam.BattleSkillID}");
                        }
                        if (battleCharController != null)
                        {
                            sb.AppendLine($"=====");
                            sb.AppendLine($"battleSkillIndex {battleCharController.battleSkillLeftIndex}:{battleCharController.battleSkillRightIndex}");
                            sb.AppendLine($"reserveBattleSkillId {battleCharController.reserveBattleSkillID}");
                            sb.AppendLine($"reserveIsLong {battleCharController.reserveIsLong}");
                            sb.AppendLine($"normalAttackIndex {battleCharController.normalAttackIndex}");
                            sb.AppendLine($"battleSkillRootType {battleCharController.BattleSkillRootType}");
                            sb.AppendLine($"isOtherTargetCancelAttack {battleCharController.isOtherTargetCancelAttack}");
                            sb.AppendLine($"wasFirstBattleSkill {battleCharController.wasFirstBattleSkill}");
                            
                            
                            sb.AppendLine($"prevState {(BattleCharacterState)battleCharController.prevState}");
                            sb.AppendLine($"currentState {(BattleCharacterState)battleCharController.currentState}");
                            sb.AppendLine($"nextState {(BattleCharacterState)battleCharController.nextState}");
                            
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
                        if (battleAiController != null)
                        {
                            sb.AppendLine($"=====");
                            sb.AppendLine($"disableInterval {battleAiController.disableAIInterval}");
                            sb.AppendLine($"enableAi {battleAiController.enableAI}");
                            sb.AppendLine($"enableAiFromSystem {battleAiController.enableAIFromSystem}");
                            sb.AppendLine($"enableAiMove {battleAiController.enableAIMove}");
                            AIBehavior<BattleCharacter> currentBehavior = battleAiController.rootBehavior;
                            sb.Append(currentBehavior.ToString() + ", ");
                            while (currentBehavior.childBehavior != null)
                            {
                                currentBehavior = currentBehavior.childBehavior;
                                sb.Append(currentBehavior.ToString());
                            }
                            
                        }
                        Rect labelRect = new Rect(
                            overlayRect.x + 10,
                            overlayRect.y + 25 + (lineHeight * numberOfRows * i),
                            1000,
                            lineHeight * numberOfRows);
                        GUI.Label(
                            new Rect(
                            overlayRect.x + 10,
                            overlayRect.y + 25 + (lineHeight * numberOfRows * i),
                            1000,
                            lineHeight * numberOfRows), sb.ToString(), new GUIStyle()
                            {
                                clipping = TextClipping.Overflow,
                                fontSize = 9,
                                fontStyle = FontStyle.Bold,
                                normal = new GUIStyleState()
                                {
                                    textColor = Color.white
                                }

                            });
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