using UnityEngine;

public class RyMCoopCredits : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float displayTime = 15f;
    private RyMOverlayEntry overlayEntry;

    public void Start()
    {
        overlayEntry = new RyMOverlayEntry(300, 60);
        RyMOverlayManager.Register(overlayEntry);
    }

    public void Update()
    {
        if (elapsedTime < displayTime) elapsedTime += Time.deltaTime;
    }

    public void OnDestroy()
    {
        RyMOverlayManager.Unregister(overlayEntry);
    }

    public void OnGUI()
    {
        if (elapsedTime > displayTime)
        {
            Destroy(this);
            return;
        }
        Rect rect = RyMOverlayManager.GetRect(overlayEntry);

        GUI.Box(
            rect,
            "RyM Coop Mod Loaded!"
        );

        GUI.Label(
            new Rect(
                rect.x + 10,
                rect.y + 25,
                280,
                20
            ),
            "By: Ryuria & Mithras"
        );
    }
}