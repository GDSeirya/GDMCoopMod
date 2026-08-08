using System.Collections.Generic;
using UnityEngine;

public class GDMPopupOverlayHost : MonoBehaviour
{
    private List<GDMPopupOverlay> gdmPopupOverlays = new List<GDMPopupOverlay>();

    public void Init(string message, int duration = 10)
    {
        gdmPopupOverlays.Add(new GDMPopupOverlay(message, duration));
    }

    private void Update()
    {
        for (int i = gdmPopupOverlays.Count-1; i >= 0; i--)
        {
            gdmPopupOverlays[i].Update();
            
            if (gdmPopupOverlays[i].IsExpired)
            {
                gdmPopupOverlays[i].Destroy();
                gdmPopupOverlays.RemoveAt(i);
            }
            
        }
    }

    private void OnGUI()
    {
        foreach (var overlay in gdmPopupOverlays)
        {
            overlay.OnGUI();
        }
    }
}