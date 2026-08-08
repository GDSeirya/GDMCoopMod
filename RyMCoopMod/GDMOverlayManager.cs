using System.Collections.Generic;
using UnityEngine;

public static class GDMOverlayManager
{
    private static List<GDMOverlayEntry> overlays = new();

    public static void Register(GDMOverlayEntry overlay)
    {
        overlays.Add(overlay);
    }

    public static void Unregister(GDMOverlayEntry overlay)
    {
        overlays.Remove(overlay);
    }

    public static Rect GetRect(GDMOverlayEntry target)
    {
        float y = 10;

        foreach (var overlay in overlays)
        {
            if (overlay == target)
                break;

            y += overlay.Height + 10;
        }

        return new Rect(
            10,
            y,
            target.Width,
            target.Height
        );
    }
}

public class GDMOverlayEntry
{
    public float Width;
    public float Height;

    public GDMOverlayEntry(float width, float height)
    {
        Width = width;
        Height = height;
    }
}