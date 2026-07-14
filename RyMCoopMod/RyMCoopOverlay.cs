using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Configuration;
using BepInEx.Unity.IL2CPP.Utils;
using UnityEngine;

public class RyMCoopOverlay : MonoBehaviour
{
    private bool showOverlay = true;
    private int playerMoney = 12345; // Temporary test value
    private KeyboardShortcut toggleKey = new KeyboardShortcut(KeyCode.F1);

    public void Update()
    {
        if (toggleKey.IsDown())
        {
            showOverlay = !showOverlay;
        }
    }

    public void OnGUI()
    {
        if (!showOverlay)
            return;

        GUI.Box(
            new Rect(10, 10, 250, 100),
            "RyM Coop Debug"
        );

        GUI.Label(
            new Rect(20, 40, 200, 25),
            $"Money: {playerMoney}"
        );
    }
}