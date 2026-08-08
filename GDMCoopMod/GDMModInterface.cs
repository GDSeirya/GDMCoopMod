using BepInEx.Unity.IL2CPP.Configuration;
using System;
using System.Text;
using UnityEngine;
using GDMCoopMod;

public class GDMModInterface : MonoBehaviour
{
    private KeyboardShortcut f1Key = new KeyboardShortcut(KeyCode.F1); //P1C1 Assignment Key
    private KeyboardShortcut f2Key = new KeyboardShortcut(KeyCode.F2); //P2C2 Assignment Key
    private KeyboardShortcut f3Key = new KeyboardShortcut(KeyCode.F3); //P3C3 Assignment Key
    private KeyboardShortcut f4Key = new KeyboardShortcut(KeyCode.F4); //P4C4 Assignment Key
    private KeyboardShortcut f5Key = new KeyboardShortcut(KeyCode.F5); //Unassignment Key
    private KeyboardShortcut f6Key = new KeyboardShortcut(KeyCode.F6); //Unassign all Key
    private KeyboardShortcut f7Key = new KeyboardShortcut(KeyCode.F7); //Display Key
    private bool isSelectingCharacter;
    private int controllerIndexToAssign;
    private int partyIndexToAssign;

    private KeyboardShortcut enableAi = new KeyboardShortcut(KeyCode.F9); //Enable AI Key
    private KeyboardShortcut disableAi = new KeyboardShortcut(KeyCode.F10); //Disable AI Key

    public void Start()
    {
        isSelectingCharacter = false;
        controllerIndexToAssign = -1;
        partyIndexToAssign = 0;
    }

    public void AssignCharacterToController(int selectedPartyIndex, int selectedControllerIndex)
    {
        GDMControllerRouting.AssignController(selectedPartyIndex, selectedControllerIndex);
        GDMCoopPlugin.OverlayHost.Init($"Controller {selectedControllerIndex + 1} is now assigned to character {selectedPartyIndex + 1}.");
    }

    public void UnassignSelectedCharacter(int selectedPartyIndex)
    {
        GDMControllerRouting.UnassignCharacter(selectedPartyIndex);
        GDMCoopPlugin.OverlayHost.Init($"Character {selectedPartyIndex + 1} is now unassigned.");
    }

    public void Update()
    {
        if (enableAi.IsDown())
        {
            if (BattleAIControllerPatch.IsPartyAIEnabled())
            {
                BattleAIControllerPatch.SetOtherPlayerAI(true);
                GDMCoopPlugin.OverlayHost.Init("Party AI enabled.", 5);
            }
            else
            {
                GDMCoopPlugin.OverlayHost.Init("Party AI is already enabled.", 5);
            }
        }

        if (disableAi.IsDown())
        {
            if (BattleAIControllerPatch.IsPartyAIEnabled())
            {
                BattleAIControllerPatch.SetOtherPlayerAI(false);
                GDMCoopPlugin.OverlayHost.Init("Party AI disabled.", 5);
            }
            else
            {
                GDMCoopPlugin.OverlayHost.Init("Party AI is already disabled.", 5);
            }
        }

        if (!isSelectingCharacter)
        {
            if (f1Key.IsDown()) controllerIndexToAssign = 0;
            else if (f2Key.IsDown()) controllerIndexToAssign = 1;
            else if (f3Key.IsDown()) controllerIndexToAssign = 2;
            else if (f4Key.IsDown()) controllerIndexToAssign = 3;
            else if (f5Key.IsDown())
            {
                GDMCoopPlugin.OverlayHost.Init($"Select a controller first to clear.");
            }
            if (f1Key.IsDown() || f2Key.IsDown() || f3Key.IsDown() || f4Key.IsDown())
            {
                isSelectingCharacter = true;
                GDMCoopPlugin.OverlayHost.Init($"Selected controller {controllerIndexToAssign+1}.");
            }
        }
        else
        {
            if (f1Key.IsDown()) partyIndexToAssign = 0;
            else if (f2Key.IsDown()) partyIndexToAssign = 1;
            else if (f3Key.IsDown()) partyIndexToAssign = 2;
            else if (f4Key.IsDown()) partyIndexToAssign = 3;
            else if (f5Key.IsDown()) partyIndexToAssign = -1;
            if (f1Key.IsDown() || f2Key.IsDown() || f3Key.IsDown() || f4Key.IsDown() || f5Key.IsDown())
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

        if (f6Key.IsDown())
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

        if (f7Key.IsDown())
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                int selectedController = GDMControllerRouting.GetControllerForParty(i);
                if (selectedController != -1)
                {
                    sb.AppendLine($"Character {i + 1} is assigned to controller {selectedController + 1}.");
                }
                else
                {
                    sb.AppendLine($"Character {i + 1} is not assigned to any controllers.");
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
                GDMCoopPlugin.OverlayHost.Init($"No controllers detected.");
            }
        }

    }


}