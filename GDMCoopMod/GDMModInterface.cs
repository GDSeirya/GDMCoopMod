using BepInEx.Unity.IL2CPP.Configuration;
using GDMCoopMod;
using System;
using System.Text;
using UnityEngine;

public class GDMModInterface : MonoBehaviour
{
    private KeyboardShortcut f1Key = new KeyboardShortcut(KeyCode.F1); //P1C1 Assignment Key
    private KeyboardShortcut f2Key = new KeyboardShortcut(KeyCode.F2); //P2C2 Assignment Key
    private KeyboardShortcut f3Key = new KeyboardShortcut(KeyCode.F3); //P3C3 Assignment Key
    private KeyboardShortcut f4Key = new KeyboardShortcut(KeyCode.F4); //P4C4 Assignment Key
    private KeyboardShortcut f5Key = new KeyboardShortcut(KeyCode.F5); //Display Key
    private KeyboardShortcut f6Key = new KeyboardShortcut(KeyCode.F6); //Unassignment Key
    private KeyboardShortcut f7Key = new KeyboardShortcut(KeyCode.F7); //Unassign all Key
#if DEBUG
    private KeyboardShortcut f8Key = new KeyboardShortcut(KeyCode.F8); //Debug Key
#endif
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
        GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("ControllerAssigned", selectedControllerIndex + 1, selectedPartyIndex + 1));
    }

    public void UnassignSelectedCharacter(int selectedPartyIndex)
    {
        GDMControllerRouting.UnassignCharacter(selectedPartyIndex);
        GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("CharacterUnassigned", selectedPartyIndex + 1));
    }

    public void Update()
    {
        if (toggleAi.IsDown())
        {
            if (!BattleAIControllerPatch.IsPartyAIEnabled())
            {
                BattleAIControllerPatch.SetOtherPlayerAI(true);
                GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("PartyAIEnabled"), 5);
                GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
            }
            else
            {
                BattleAIControllerPatch.SetOtherPlayerAI(false);
                GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("PartyAIDisabled"), 5);
                GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
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
                GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("SelectControllerFirst"));
                GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
            }
            if (f1Key.IsDown() || f2Key.IsDown() || f3Key.IsDown() || f4Key.IsDown())
            {
                isSelectingCharacter = true;
                GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("SelectedController", controllerIndexToAssign + 1));
                GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
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
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }
                else
                {
                    UnassignSelectedCharacter(controllerIndexToAssign);
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuClose);
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
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuClose);
                }
            }
        }
#if DEBUG
        if (f8Key.IsDown())
        {
            GDMCoopPlugin.OverlayHost.Init("DEBUG");
        }
#endif
        if (f5Key.IsDown())
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                int selectedController = GDMControllerRouting.GetControllerForParty(i);
                if (selectedController != -1)
                {
                    sb.AppendLine(LanguageManager.Get("CharacterAssignedToController", i + 1, selectedController + 1));
                }
                else
                {
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
                GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuOpen);
            }
            else
            {
                GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("NoControllersDetected"));
                GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
            }
        }

        for (int i = 1; i < 4; i++)
        {
            if (GDMCoopPlugin.VirtualControllers.GetState(i).SpellPreviousPressed)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (GDMControllerRouting.GetControllerForParty(j) == i)
                    {
                        BattleAIControllerPatch.PreviousSpell(j);
                    }
                }
            }

            if (GDMCoopPlugin.VirtualControllers.GetState(i).SpellNextPressed)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (GDMControllerRouting.GetControllerForParty(j) == i)
                    {
                        BattleAIControllerPatch.NextSpell(j);
                    }
                }
            }

            if (GDMCoopPlugin.VirtualControllers.GetState(i).TargetingModePressed)
            {
                BattleAIControllerPatch.SetHostTargetingMode(i, !BattleAIControllerPatch.GetHostTargetingMode(i));
                if (BattleAIControllerPatch.GetHostTargetingMode(i))
                {
                    //GDMCoopPlugin.OverlayHost.Init($"Controller {i+1} is targeting host's target.");
                    GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("TargetingHost", i+1));
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }
                else
                {
                    //GDMCoopPlugin.OverlayHost.Init($"Controller {i+1} is targeting their closest target.");
                    GDMCoopPlugin.OverlayHost.Init(LanguageManager.Get("TargetingClosest", i + 1));
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }

            }
            if (GDMCoopPlugin.VirtualControllers.GetState(i).ChangeToSlot1Pressed)
            {
                if (GDMControllerRouting.GetControllerForParty(0) == i)
                {
                    // Already assigned to this character: toggle off.
                    UnassignSelectedCharacter(0);
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }
                else
                {
                    // Remove this controller from any other character first.
                    if (GDMControllerRouting.GetControllerForParty(1) == i) UnassignSelectedCharacter(1);
                    if (GDMControllerRouting.GetControllerForParty(2) == i) UnassignSelectedCharacter(2);
                    if (GDMControllerRouting.GetControllerForParty(3) == i) UnassignSelectedCharacter(3);
                    // Now assign it to the requested character.
                    AssignCharacterToController(0, i);
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }
            }
            else if (GDMCoopPlugin.VirtualControllers.GetState(i).ChangeToSlot2Pressed)
            {
                if (GDMControllerRouting.GetControllerForParty(1) == i)
                {
                    UnassignSelectedCharacter(1);
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }
                else
                {
                    if (GDMControllerRouting.GetControllerForParty(0) == i) UnassignSelectedCharacter(0);
                    if (GDMControllerRouting.GetControllerForParty(2) == i) UnassignSelectedCharacter(2);
                    if (GDMControllerRouting.GetControllerForParty(3) == i) UnassignSelectedCharacter(3);
                    AssignCharacterToController(1, i);
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }
            }
            else if (GDMCoopPlugin.VirtualControllers.GetState(i).ChangeToSlot3Pressed)
            {
                if (GDMControllerRouting.GetControllerForParty(2) == i)
                {
                    UnassignSelectedCharacter(2);
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }
                else
                {
                    if (GDMControllerRouting.GetControllerForParty(0) == i) UnassignSelectedCharacter(0);
                    if (GDMControllerRouting.GetControllerForParty(1) == i) UnassignSelectedCharacter(1);
                    if (GDMControllerRouting.GetControllerForParty(3) == i) UnassignSelectedCharacter(3);
                    AssignCharacterToController(2, i);
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }
            }
            else if (GDMCoopPlugin.VirtualControllers.GetState(i).ChangeToSlot4Pressed)
            {
                if (GDMControllerRouting.GetControllerForParty(3) == i)
                {
                    UnassignSelectedCharacter(3);
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }
                else
                {
                    if (GDMControllerRouting.GetControllerForParty(0) == i) UnassignSelectedCharacter(0);
                    if (GDMControllerRouting.GetControllerForParty(1) == i) UnassignSelectedCharacter(1);
                    if (GDMControllerRouting.GetControllerForParty(2) == i) UnassignSelectedCharacter(2);
                    AssignCharacterToController(3, i);
                    GDMSoundRegistry.PlaySe(GDMSoundRegistry.ModSfx.MenuSelect);
                }
            }
        }

    }


}