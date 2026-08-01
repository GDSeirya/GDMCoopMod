using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using RyMCoopMod;
using UnityEngine;
using UnityEngine.InputSystem;

[BepInPlugin("rym.coopmod", "RyM Coop Plugin", "1.0")]
public class RyMCoopPlugin : BasePlugin
{
    public static ManualLogSource StaticLog;   // your own writable field
    public static RyMVirtualControllerManager VirtualControllers;

    public override void Load()
    {
        StaticLog = Log;

        VirtualControllers = new RyMVirtualControllerManager();

        Harmony harmony = new Harmony("com.gd.rymcoop");
        harmony.PatchAll();

        AddComponent<RyMCoopCredits>();
        AddComponent<RyMCoopOverlay>();
        AddComponent<RyMCoopUpdateDriver>();

        RyMControllerRouting.AssignController(1, 1);

        

        StaticLog.LogInfo("RyM Plugin loaded");
        
    }
}
