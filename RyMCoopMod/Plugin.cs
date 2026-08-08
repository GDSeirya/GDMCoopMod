using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using GDMCoopMod;

[BepInPlugin("gdm.coopmod", "GDM Coop Plugin", "1.0")]
public class GDMCoopPlugin : BasePlugin
{
    public static ManualLogSource StaticLog;
    public static GDMVirtualControllerManager VirtualControllers;

    public override void Load()
    {
        StaticLog = Log;

        VirtualControllers = new GDMVirtualControllerManager();

        Harmony harmony = new Harmony("com.gd.gdmcoop");
        harmony.PatchAll();

        AddComponent<GDMCoopCredits>();
        AddComponent<GDMMainOverlay>();
        AddComponent<GDMCoopUpdateDriver>();

        //playerIndex, controllerIndex
        GDMControllerRouting.AssignController(0, 0);
        GDMControllerRouting.AssignController(1, 1);

        StaticLog.LogInfo($"GDM Coop Plugin loaded");
        
    }
}
