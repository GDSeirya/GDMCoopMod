using UnityEngine;

public class GDMPopupOverlay : GDMOverlayEntry
{
    private float elapsedTime = 0f;
    private float displayTime;

    private readonly string _message;
    private GUIStyle _style;
    private bool _initialized = false;

    public GDMPopupOverlay(string message) : this(message, 10) { }

    public GDMPopupOverlay(string message, int duration) : base(0, 0)
    {
        _message = message;
        displayTime = duration;
        GDMPopupOverlayManager.Register(this);
    }

    public void Update()
    {
        if (elapsedTime < displayTime)
            elapsedTime += Time.deltaTime;
    }

    public void OnGUI()
    {
        if (!_initialized)
        {
            // GUI calls are allowed here
            _style = new GUIStyle(GUI.skin.box)
            {
                fontSize = 18,
                padding = new RectOffset()
                {
                    top = 10,
                    right = 10,
                    bottom = 10,
                    left = 10
                },
                alignment = TextAnchor.MiddleLeft
            };

            Vector2 size = _style.CalcSize(new GUIContent(_message));
            Width = size.x;
            Height = size.y;

            _initialized = true;
        }

        if (elapsedTime > displayTime)
        {
            Destroy();
            return;
        }

        Rect rect = GDMPopupOverlayManager.GetRect(this);
        GUI.Box(rect, _message, _style);
    }

    public void Destroy()
    {
        GDMPopupOverlayManager.Unregister(this);
    }

    public bool IsExpired => elapsedTime > displayTime;
}