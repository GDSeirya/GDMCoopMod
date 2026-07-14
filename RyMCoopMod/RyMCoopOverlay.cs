using BepInEx.Unity.IL2CPP.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;

public class RyMCoopOverlay : MonoBehaviour
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
        // Refresh controller reference
        if (Gamepad.all.Count > 0)
        {
            playerOneController = Gamepad.all[0];
        }
        else
        {
            playerOneController = null;
        }
    }

    public void OnDestroy()
    {
        if (overlayEntry != null)
        {
            RyMOverlayManager.Unregister(overlayEntry);
        }
    }

    private void DrawButton(float x, float y, string name, bool pressed)
    {
        GUI.color = pressed ? Color.green : Color.white;

        GUI.Box(
            new Rect(x, y, 70, 25),
            name
        );

        GUI.color = Color.white;
    }

    private void DrawAnalogButton(float x, float y, string name, float value)
    {
        bool pressed = value > 0.1f;

        GUI.color = pressed ? Color.green : Color.white;

        GUI.Box(
            new Rect(x, y, 70, 25),
            $"{name}: {value:F2}"
        );

        GUI.color = Color.white;
    }

    public void OnGUI()
    {
        if (!showOverlay)
            return;

        Rect overlayRect = RyMOverlayManager.GetRect(overlayEntry);

        /*
        GUI.Box(
            overlayRect,
            "RyM Coop Debug"
        );

        GUI.Label(
            new Rect(
                overlayRect.x + 10,
                overlayRect.y + 30,
                200,
                25
            ),
            $"Money: {playerMoney}"
        );
        */
        GUI.Box(
            overlayRect,
            "Player 1 Controller"
        );

        if (playerOneController == null)
        {
            GUI.Label(
                new Rect(overlayRect.x + 10, overlayRect.y + 30, 250, 25),
                "No controller detected"
            );
            return;
        }

        float x = overlayRect.x + 10;
        float y = overlayRect.y + 30;

        // Face buttons
        DrawButton(x + 90, y, "Y",
            playerOneController.buttonNorth.isPressed);

        DrawButton(x + 180, y + 30, "B",
            playerOneController.buttonEast.isPressed);

        DrawButton(x, y + 30, "X",
            playerOneController.buttonWest.isPressed);

        DrawButton(x + 90, y + 30, "A",
            playerOneController.buttonSouth.isPressed);


        // Shoulders
        DrawButton(x, y + 80, "L1",
            playerOneController.leftShoulder.isPressed);

        DrawButton(x + 180, y + 80, "R1",
            playerOneController.rightShoulder.isPressed);


        // Triggers (analog)
        DrawAnalogButton(
            x,
            y + 115,
            "L2",
            playerOneController.leftTrigger.ReadValue()
        );

        DrawAnalogButton(
            x + 180,
            y + 115,
            "R2",
            playerOneController.rightTrigger.ReadValue()
        );


        // DPad
        DrawButton(x + 80, y + 160, "UP",
            playerOneController.dpad.up.isPressed);

        DrawButton(x + 80, y + 210, "DOWN",
            playerOneController.dpad.down.isPressed);

        DrawButton(x + 30, y + 185, "LEFT",
            playerOneController.dpad.left.isPressed);

        DrawButton(x + 130, y + 185, "RIGHT",
            playerOneController.dpad.right.isPressed);


        // Stick buttons
        DrawButton(x + 250, y + 180, "L3",
            playerOneController.leftStickButton.isPressed);

        DrawButton(x + 250, y + 215, "R3",
            playerOneController.rightStickButton.isPressed);


        // Stick values
        Vector2 leftStick = playerOneController.leftStick.ReadValue();
        Vector2 rightStick = playerOneController.rightStick.ReadValue();

        GUI.Label(
            new Rect(x, y + 260, 250, 20),
            $"Left: {leftStick.x:F2}, {leftStick.y:F2}"
        );

        GUI.Label(
            new Rect(x, y + 280, 250, 20),
            $"Right: {rightStick.x:F2}, {rightStick.y:F2}"
        );
    }
}