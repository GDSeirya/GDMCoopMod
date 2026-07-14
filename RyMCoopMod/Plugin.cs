using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine;

[BepInPlugin("rym.coopmod", "RyM Coop Plugin", "1.0")]
public class RyMCoopPlugin : BasePlugin
{
    public override void Load()
    {
        AddComponent<RyMCoopCredits>();
        AddComponent<RyMCoopOverlay>();
        Log.LogInfo("Rym Plugin loaded");
        
    }

}
