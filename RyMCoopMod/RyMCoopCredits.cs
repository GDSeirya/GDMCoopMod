using UnityEngine;

public class RyMCoopCredits : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float displayTime = 15f;

    public void Update()
    {
        if (elapsedTime < displayTime) elapsedTime += Time.deltaTime;
    }

    public void OnGUI()
    {
        if (elapsedTime > displayTime)
            return;

        GUI.Box(
            new Rect(10, 10, 300, 60),
            "RyM Coop Mod Loaded!"
        );

        GUI.Label(
            new Rect(20, 35, 280, 20),
            "By: Ryuria & Mithras"
        );
    }
}