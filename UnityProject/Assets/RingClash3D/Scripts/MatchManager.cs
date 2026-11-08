using UnityEngine;

public class MatchManager : MonoBehaviour
{
    private GameObject player;
    private GameObject rival;
    private bool matchEnded;
    private string resultText = "";
    private string playerLabel = "PLAYER";
    private string rivalLabel = "OPPONENT";

    public void Setup(GameObject p, GameObject e, string pLabel, string eLabel)
    {
        player = p;
        rival = e;
        playerLabel = pLabel;
        rivalLabel = eLabel;

        WrestlerController playerController = player.GetComponent<WrestlerController>();
        WrestlerController rivalController = rival.GetComponent<WrestlerController>();
        playerController.opponent = rival.transform;
        rivalController.opponent = player.transform;

        player.AddComponent<WrestlerHealth>();
        rival.AddComponent<WrestlerHealth>();

        PairedMoveController paired = player.AddComponent<PairedMoveController>();
        paired.Setup(player, rival);
    }

    private void Update()
    {
        if (matchEnded || player == null || rival == null)
            return;

        WrestlerHealth playerHealth = player.GetComponent<WrestlerHealth>();
        WrestlerHealth rivalHealth = rival.GetComponent<WrestlerHealth>();

        if (playerHealth != null && playerHealth.isKnockedOut)
            EndMatch(rivalLabel + " WINS");
        else if (rivalHealth != null && rivalHealth.isKnockedOut)
            EndMatch(playerLabel + " WINS");

        if (Input.GetKeyDown(KeyCode.R) && matchEnded)
            RestartMatch();
    }

    private void EndMatch(string message)
    {
        matchEnded = true;
        resultText = message;

        WrestlerController a = player.GetComponent<WrestlerController>();
        WrestlerController b = rival.GetComponent<WrestlerController>();
        if (a != null) a.SetMobileMove(Vector2.zero);
        if (b != null) b.SetMobileMove(Vector2.zero);
    }

    private void RestartMatch()
    {
        matchEnded = false;
        resultText = "";
        player.transform.position = new Vector3(-1.25f, 0f, 0f);
        rival.transform.position = new Vector3(1.25f, 0f, 0f);

        foreach (GameObject fighter in new[] { player, rival })
        {
            WrestlerHealth health = fighter.GetComponent<WrestlerHealth>();
            if (health != null)
            {
                health.value = 100f;
                health.guard = false;
                health.isKnockedOut = false;
            }

            WrestlerController controller = fighter.GetComponent<WrestlerController>();
            if (controller != null)
                controller.ResetAfterMatch();

            WrestlerAnimationPlayer animationPlayer = fighter.GetComponent<WrestlerAnimationPlayer>();
            if (animationPlayer != null)
                animationPlayer.PlayClip("idle");
        }
    }

    private void OnGUI()
    {
        if (player == null || rival == null)
            return;

        float width = Mathf.Min(300f, Screen.width * 0.34f);
        float height = 24f;
        float top = 22f;

        DrawHealthBar(new Rect(20f, top, width, height),
            player.GetComponent<WrestlerHealth>(), playerLabel);
        DrawHealthBar(new Rect(Screen.width - width - 20f, top, width, height),
            rival.GetComponent<WrestlerHealth>(), rivalLabel);

        if (!matchEnded)
            return;

        GUIStyle title = new GUIStyle(GUI.skin.label);
        title.alignment = TextAnchor.MiddleCenter;
        title.fontSize = Mathf.Clamp(Screen.width / 16, 28, 54);
        title.fontStyle = FontStyle.Bold;
        title.normal.textColor = Color.white;

        GUI.Box(new Rect(Screen.width * 0.2f, Screen.height * 0.34f,
            Screen.width * 0.6f, Screen.height * 0.25f), "");
        GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.36f,
            Screen.width * 0.6f, 65f), resultText, title);

        GUIStyle button = new GUIStyle(GUI.skin.button);
        button.fontSize = 20;
        button.fontStyle = FontStyle.Bold;
        if (GUI.Button(new Rect(Screen.width * 0.35f, Screen.height * 0.53f,
            Screen.width * 0.3f, 52f), "REMATCH", button))
            RestartMatch();
    }

    private void DrawHealthBar(Rect rect, WrestlerHealth health, string label)
    {
        if (health == null)
            return;

        GUI.Box(rect, "");
        float ratio = Mathf.Clamp01(health.value / 100f);
        GUI.color = ratio > 0.5f ? Color.green : (ratio > 0.25f ? Color.yellow : Color.red);
        GUI.DrawTexture(new Rect(rect.x + 3f, rect.y + 3f,
            (rect.width - 6f) * ratio, rect.height - 6f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle text = new GUIStyle(GUI.skin.label);
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        text.normal.textColor = Color.white;
        GUI.Label(rect, label + "  " + Mathf.CeilToInt(health.value) + " / 100", text);
    }
}
