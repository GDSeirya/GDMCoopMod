using BepInEx.Unity.IL2CPP.Configuration;
using System;
using System.Text;
using UnityEngine;
using GDMCoopMod;
using Game;

public class GDMModInterface : MonoBehaviour
{
    private KeyboardShortcut f1Key = new KeyboardShortcut(KeyCode.F1); //P1C1 Assignment Key
    private KeyboardShortcut f2Key = new KeyboardShortcut(KeyCode.F2); //P2C2 Assignment Key
    private KeyboardShortcut f3Key = new KeyboardShortcut(KeyCode.F3); //P3C3 Assignment Key
    private KeyboardShortcut f4Key = new KeyboardShortcut(KeyCode.F4); //P4C4 Assignment Key
    private KeyboardShortcut f5Key = new KeyboardShortcut(KeyCode.F5); //Display Key
    private KeyboardShortcut f6Key = new KeyboardShortcut(KeyCode.F6); //Unassignment Key
    private KeyboardShortcut f7Key = new KeyboardShortcut(KeyCode.F7); //Unassign all Key
    private bool isSelectingCharacter;
    private int controllerIndexToAssign;
    private int partyIndexToAssign;

    private KeyboardShortcut toggleAi = new KeyboardShortcut(KeyCode.F9); //Enable AI Key

    public void Start()
    {
        isSelectingCharacter = false;
        controllerIndexToAssign = -1;
        partyIndexToAssign = 0;
    }

    public void AssignCharacterToController(int selectedPartyIndex, int selectedControllerIndex)
    {
        GDMControllerRouting.AssignController(selectedPartyIndex, selectedControllerIndex);
        //GDMCoopPlugin.OverlayHost.Init($"Controller {selectedControllerIndex + 1} is now assigned to character {selectedPartyIndex + 1}.");
        GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("ControllerAssigned", selectedControllerIndex + 1, selectedPartyIndex + 1));
        //ControllerAssigned
    }

    public void UnassignSelectedCharacter(int selectedPartyIndex)
    {
        GDMControllerRouting.UnassignCharacter(selectedPartyIndex);
        //GDMCoopPlugin.OverlayHost.Init($"Character {selectedPartyIndex + 1} is now unassigned.");
        GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("CharacterUnassigned", selectedPartyIndex + 1));
    }

    public void Update()
    {
        if (toggleAi.IsDown())
        {
            if (!BattleAIControllerPatch.IsPartyAIEnabled())
            {
                BattleAIControllerPatch.SetOtherPlayerAI(true);
                //GDMCoopPlugin.OverlayHost.Init("Party AI enabled.", 5);
                GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("PartyAIEnabled"), 5);
            }
            else
            {
                BattleAIControllerPatch.SetOtherPlayerAI(false);
                //GDMCoopPlugin.OverlayHost.Init("Party AI disabled.", 5);
                GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("PartyAIDisabled"), 5);
            }
        }

        if (!isSelectingCharacter)
        {
            if (f1Key.IsDown()) controllerIndexToAssign = 0;
            else if (f2Key.IsDown()) controllerIndexToAssign = 1;
            else if (f3Key.IsDown()) controllerIndexToAssign = 2;
            else if (f4Key.IsDown()) controllerIndexToAssign = 3;
            else if (f6Key.IsDown())
            {
                //GDMCoopPlugin.OverlayHost.Init($"Select a controller first to clear.");
                GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("SelectControllerFirst"));
            }
            if (f1Key.IsDown() || f2Key.IsDown() || f3Key.IsDown() || f4Key.IsDown())
            {
                isSelectingCharacter = true;
                //GDMCoopPlugin.OverlayHost.Init($"Selected controller {controllerIndexToAssign+1}.");
                GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("SelectedController", controllerIndexToAssign + 1));
            }
        }
        else
        {
            if (f1Key.IsDown()) partyIndexToAssign = 0;
            else if (f2Key.IsDown()) partyIndexToAssign = 1;
            else if (f3Key.IsDown()) partyIndexToAssign = 2;
            else if (f4Key.IsDown()) partyIndexToAssign = 3;
            else if (f6Key.IsDown()) partyIndexToAssign = -1;
            if (f1Key.IsDown() || f2Key.IsDown() || f3Key.IsDown() || f4Key.IsDown() || f6Key.IsDown())
            {
                isSelectingCharacter = false;
                
                if (partyIndexToAssign >= 0 && partyIndexToAssign <= 3)
                {
                    AssignCharacterToController(partyIndexToAssign, controllerIndexToAssign);
                }
                else
                {
                    UnassignSelectedCharacter(controllerIndexToAssign);
                }
                
            }
        }

        if (f7Key.IsDown())
        {
            for (int i = 0; i < 4; i++)
            {
                int selectedController = GDMControllerRouting.GetControllerForParty(i);
                if (selectedController != -1)
                {
                    UnassignSelectedCharacter(i);
                }
            }
        }

        if (f5Key.IsDown())
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                int selectedController = GDMControllerRouting.GetControllerForParty(i);
                if (selectedController != -1)
                {
                    //sb.AppendLine($"Character {i + 1} is assigned to controller {selectedController + 1}.");
                    sb.AppendLine(LanguageManager.Get("CharacterAssignedToController", i + 1, selectedController + 1));
                }
                else
                {

                    //sb.AppendLine($"Character {i + 1} is not assigned to any controllers.");
                    sb.AppendLine(LanguageManager.Get("CharacterNotAssigned", i + 1));
                }
            }
            if (sb.ToString().Length > 0)
            {
                string nl = Environment.NewLine;
                if (sb.Length >= nl.Length &&
                    sb.ToString(sb.Length - nl.Length, nl.Length) == nl)
                {
                    sb.Length -= nl.Length;
                }
                GDMCoopPlugin.OverlayHost.Init(sb.ToString());
            }
            else
            {
                //GDMCoopPlugin.OverlayHost.Init($"No controllers detected.");
                GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("NoControllersDetected"));
            }
        }

        for (int i = 1; i < 4; i++)
        {
            if (GDMCoopPlugin.VirtualControllers.GetState(i).SpellPreviousPressed)
            {
                BattleAIControllerPatch.PreviousSpell(i);
            }

            if (GDMCoopPlugin.VirtualControllers.GetState(i).SpellNextPressed)
            {
                BattleAIControllerPatch.NextSpell(i);
            }

            if (GDMCoopPlugin.VirtualControllers.GetState(i).TargetingModePressed)
            {
                BattleAIControllerPatch.SetHostTargetingMode(i, !BattleAIControllerPatch.GetHostTargetingMode(i));
                if (BattleAIControllerPatch.GetHostTargetingMode(i))
                {
                    //GDMCoopPlugin.OverlayHost.Init($"Controller {i+1} is targeting host's target.");
                    GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("TargetingHost", i+1));
                }
                else
                {
                    //GDMCoopPlugin.OverlayHost.Init($"Controller {i+1} is targeting their closest target.");
                    GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("TargetingClosest", i + 1));
                }

            }
            if (GDMCoopPlugin.VirtualControllers.GetState(i).ChangeToSlot1Pressed)
            {
                if (GDMControllerRouting.GetControllerForParty(0) == i)
                {
                    // Already assigned to this character: toggle off.
                    UnassignSelectedCharacter(0);
                }
                else
                {
                    // Remove this controller from any other character first.
                    if (GDMControllerRouting.GetControllerForParty(1) == i) UnassignSelectedCharacter(1);
                    if (GDMControllerRouting.GetControllerForParty(2) == i) UnassignSelectedCharacter(2);
                    if (GDMControllerRouting.GetControllerForParty(3) == i) UnassignSelectedCharacter(3);
                    // Now assign it to the requested character.
                    AssignCharacterToController(0, i);
                }
            }
            else if (GDMCoopPlugin.VirtualControllers.GetState(i).ChangeToSlot2Pressed)
            {
                if (GDMControllerRouting.GetControllerForParty(1) == i)
                {
                    UnassignSelectedCharacter(1);
                }
                else
                {
                    if (GDMControllerRouting.GetControllerForParty(0) == i) UnassignSelectedCharacter(0);
                    if (GDMControllerRouting.GetControllerForParty(2) == i) UnassignSelectedCharacter(2);
                    if (GDMControllerRouting.GetControllerForParty(3) == i) UnassignSelectedCharacter(3);
                    AssignCharacterToController(1, i);
                }
            }
            else if (GDMCoopPlugin.VirtualControllers.GetState(i).ChangeToSlot3Pressed)
            {
                if (GDMControllerRouting.GetControllerForParty(2) == i)
                {
                    UnassignSelectedCharacter(2);
                }
                else
                {
                    if (GDMControllerRouting.GetControllerForParty(0) == i) UnassignSelectedCharacter(0);
                    if (GDMControllerRouting.GetControllerForParty(1) == i) UnassignSelectedCharacter(1);
                    if (GDMControllerRouting.GetControllerForParty(3) == i) UnassignSelectedCharacter(3);
                    AssignCharacterToController(2, i);
                }
            }
            else if (GDMCoopPlugin.VirtualControllers.GetState(i).ChangeToSlot4Pressed)
            {
                if (GDMControllerRouting.GetControllerForParty(3) == i)
                {
                    UnassignSelectedCharacter(3);
                }
                else
                {
                    if (GDMControllerRouting.GetControllerForParty(0) == i) UnassignSelectedCharacter(0);
                    if (GDMControllerRouting.GetControllerForParty(1) == i) UnassignSelectedCharacter(1);
                    if (GDMControllerRouting.GetControllerForParty(2) == i) UnassignSelectedCharacter(2);
                    AssignCharacterToController(3, i);
                }
            }
        }

    }


}