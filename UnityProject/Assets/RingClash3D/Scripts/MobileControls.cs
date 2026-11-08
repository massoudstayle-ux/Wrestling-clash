using UnityEngine;

public class MobileControls : MonoBehaviour
{
    private int joystickFinger = -1;
    private Vector2 joystickOrigin;
    private Vector2 joystickValue;
    private const float JoystickRadius = 82f;

    private Rect PunchRect => ButtonRect(0);
    private Rect KickRect => ButtonRect(1);
    private Rect GrappleRect => ButtonRect(2);
    private Rect BlockRect => ButtonRect(3);

    private void Update()
    {
        WrestlerController player = FindPlayer();
        if (player == null)
            return;

        joystickValue = Vector2.zero;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 point = new Vector2(touch.position.x, Screen.height - touch.position.y);

            if (touch.phase == TouchPhase.Began)
            {
                if (point.x < Screen.width * 0.48f && joystickFinger == -1)
                {
                    joystickFinger = touch.fingerId;
                    joystickOrigin = point;
                }
                else if (PunchRect.Contains(point)) player.Punch();
                else if (KickRect.Contains(point)) player.Kick();
                else if (GrappleRect.Contains(point)) player.Grapple();
                else if (BlockRect.Contains(point)) player.Block();
            }

            if (touch.fingerId == joystickFinger)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    Vector2 delta = point - joystickOrigin;
                    joystickValue = Vector2.ClampMagnitude(delta / JoystickRadius, 1f);
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    joystickFinger = -1;
                    joystickValue = Vector2.zero;
                }
            }
        }

        player.SetMobileMove(joystickValue);
    }

    private WrestlerController FindPlayer()
    {
        WrestlerController[] wrestlers = FindObjectsOfType<WrestlerController>();
        foreach (WrestlerController wrestler in wrestlers)
            if (wrestler.isPlayer)
                return wrestler;
        return null;
    }

    private Rect ButtonRect(int index)
    {
        float width = Mathf.Clamp(Screen.width * 0.19f, 112f, 168f);
        float height = Mathf.Clamp(Screen.height * 0.075f, 52f, 76f);
        float right = Screen.width - width - 24f;
        float bottom = Screen.height - height - 26f;
        float gap = height + 12f;

        if (index == 0) return new Rect(right, bottom - gap * 3f, width, height);
        if (index == 1) return new Rect(right, bottom - gap * 2f, width, height);
        if (index == 2) return new Rect(right, bottom - gap, width, height);
        return new Rect(right - width - 12f, bottom - gap, width, height);
    }

    private void OnGUI()
    {
        float scale = Mathf.Clamp(Screen.width / 900f, 0.85f, 1.35f);
        GUIStyle button = new GUIStyle(GUI.skin.button);
        button.fontSize = Mathf.RoundToInt(18f * scale);
        button.fontStyle = FontStyle.Bold;

        Vector2 center = joystickFinger == -1
            ? new Vector2(125f, Screen.height - 125f)
            : joystickOrigin;

        GUI.color = new Color(1f, 1f, 1f, 0.22f);
        GUI.DrawTexture(new Rect(center.x - JoystickRadius, center.y - JoystickRadius,
            JoystickRadius * 2f, JoystickRadius * 2f), Texture2D.whiteTexture);
        Vector2 knob = center + joystickValue * JoystickRadius;
        GUI.color = new Color(0.2f, 0.65f, 1f, 0.75f);
        GUI.DrawTexture(new Rect(knob.x - 27f, knob.y - 27f, 54f, 54f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUI.Button(PunchRect, "PUNCH", button);
        GUI.Button(KickRect, "KICK", button);
        GUI.Button(GrappleRect, "GRAPPLE", button);
        GUI.Button(BlockRect, "BLOCK", button);
    }
}
