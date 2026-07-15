using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine;

[BepInPlugin("rym.coopmod", "RyM Coop Plugin", "1.0")]
public class RyMCoopPlugin : BasePlugin
{
    public static ManualLogSource StaticLog;   // your own writable field

    public override void Load()
    {
        StaticLog = Log;
        AddComponent<RyMCoopCredits>();
        AddComponent<RyMCoopOverlay>();
        StaticLog.LogInfo("Rym Plugin loaded");
        
    }

}
