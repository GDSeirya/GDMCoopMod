using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GDMCoopMod;
using HarmonyLib;

[BepInPlugin("gdm.coopmod", "GDM Coop Plugin", "1.3.2")]
public class GDMCoopPlugin : BasePlugin
{
    public static ManualLogSource StaticLog;
    public static GDMVirtualControllerManager VirtualControllers;
    public static GDMPopupOverlayHost OverlayHost;
    public static GDMModInterface modInterface;

    // Config entries
    private ConfigEntry<int> player1Character;
    private ConfigEntry<int> player2Character;
    private ConfigEntry<int> player3Character;
    private ConfigEntry<int> player4Character;

    public override void Load()
    {
        StaticLog = Log;

        // -----------------------------
        // Configuration
        // -----------------------------

        player1Character = Config.Bind(
            "Controller Assignments",
            "Player 1 Character",
            -1,
            "Party/character index assigned to controller 0. Set to -1 to disable. Set to 0 for character 1."
        );

        player2Character = Config.Bind(
            "Controller Assignments",
            "Player 2 Character",
            1,
            "Party/character index assigned to controller 1. Set to -1 to disable. Set to 1 for character 2."
        );

        player3Character = Config.Bind(
            "Controller Assignments",
            "Player 3 Character",
            -1,
            "Party/character index assigned to controller 2. Set to -1 to disable. Set to 2 for character 3."
        );

        player4Character = Config.Bind(
            "Controller Assignments",
            "Player 4 Character",
            -1,
            "Party/character index assigned to controller 3. Set to -1 to disable. Set to 3 for character 4."
        );

        // -----------------------------
        // Mod initialization
        // -----------------------------
        VirtualControllers = new GDMVirtualControllerManager();

        Harmony harmony = new Harmony("com.gd.gdmcoop");
        harmony.PatchAll();

        // IMPORTANT:
        // Assign the component to the static field.
        modInterface = AddComponent<GDMModInterface>();
        var updateDriver = AddComponent<GDMVirtualControllerDriver>();
        OverlayHost = AddComponent<GDMPopupOverlayHost>();


        // -----------------------------
        // Startup messages
        // -----------------------------

        OverlayHost.Init("GDM Coop Plugin Loaded", 15);

        OverlayHost.Init("Developed by GD Seirya & Mithras Seirya", 15);


        // -----------------------------
        // Apply controller assignments
        // -----------------------------

        AssignConfiguredController(0,player1Character.Value);
        AssignConfiguredController(1,player2Character.Value);
        AssignConfiguredController(2,player3Character.Value);
        AssignConfiguredController(3,player4Character.Value);


        StaticLog.LogInfo("GDM Coop Plugin loaded");
    }


    private void AssignConfiguredController(
        int controllerIndex,
        int partyIndex)
    {
        // -1 means don't assign this controller.
        if (partyIndex < 0)
        {
            OverlayHost.Init($"Controller {controllerIndex + 1} is disabled.", 5);
            return;
        }

        // Safety check.
        if (modInterface == null)
        {
            OverlayHost.Init($"GDMModInterface is null. Cannot assign controller.", 5);
            return;
        }
        modInterface.AssignCharacterToController(partyIndex,controllerIndex);
    }
}