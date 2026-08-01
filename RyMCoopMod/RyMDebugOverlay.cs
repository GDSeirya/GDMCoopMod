using BepInEx.Unity.IL2CPP.Configuration;
using Game;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RyMDebugOverlay : MonoBehaviour
{
    private bool showOverlay = true;
    private Gamepad playerOneController;
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
                        int numberOfRows = 4;
                        BattlePlayer player = BattleManager.GetInstance().BattlePlayerList[i];
                        GUI.Label(
                    new Rect(overlayRect.x + 10, overlayRect.y + (25*numberOfRows) + (25*numberOfRows*i), 1000, 25*numberOfRows),
                        $"P{i} {(PlayerID)player.CharacterID}: Position ({player.transform.position.x:0.00},{player.transform.position.y:0.00},{player.transform.position.z:0.00}){Environment.NewLine}" +
                        $"IndexInParty {player.indexInParty}{Environment.NewLine}" +
                        $"CharDirection {player.characterDirection}{Environment.NewLine}" +
                        $"BattleAnimKind {player.animationKind}{Environment.NewLine}" +
                        $"");
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