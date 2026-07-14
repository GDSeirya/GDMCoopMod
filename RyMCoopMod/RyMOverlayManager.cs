using System.Collections.Generic;
using UnityEngine;

public static class RyMOverlayManager
{
    private static List<RyMOverlayEntry> overlays = new();

    public static void Register(RyMOverlayEntry overlay)
    {
        overlays.Add(overlay);
    }

    public static void Unregister(RyMOverlayEntry overlay)
    {
        overlays.Remove(overlay);
    }

    public static Rect GetRect(RyMOverlayEntry target)
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

public class RyMOverlayEntry
{
    public float Width;
    public float Height;

    public RyMOverlayEntry(float width, float height)
    {
        Width = width;
        Height = height;
    }
}