using UnityEngine;

public class GDMCoopCredits : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float displayTime = 15f;
    private GDMOverlayEntry overlayEntry;

    public void Start()
    {
        overlayEntry = new GDMOverlayEntry(300, 60);
        GDMOverlayManager.Register(overlayEntry);
    }

    public void Update()
    {
        if (elapsedTime < displayTime) elapsedTime += Time.deltaTime;
    }

    public void OnDestroy()
    {
        GDMOverlayManager.Unregister(overlayEntry);
    }

    public void OnGUI()
    {
        if (elapsedTime > displayTime)
        {
            Destroy(this);
            return;
        }
        Rect rect = GDMOverlayManager.GetRect(overlayEntry);

        GUI.Box(
            rect,
            "GDMCoop Mod Loaded"
        );

        GUI.Label(
            new Rect(
                rect.x + 10,
                rect.y + 25,
                280,
                20
            ),
            "Developed by GD Seirya & Mithras Seirya"
        );
    }
}