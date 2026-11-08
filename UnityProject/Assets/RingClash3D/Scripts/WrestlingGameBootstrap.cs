using UnityEngine;

public class WrestlingGameBootstrap : MonoBehaviour
{
    private static readonly string[] Roster = { "aj", "orton", "boxer", "runner" };
    private string selectedPlayer = "aj";
    private string selectedOpponent = "orton";
    private bool matchStarted;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot()
    {
        if (FindObjectOfType<WrestlingGameBootstrap>() == null)
            new GameObject("WrestlingGame").AddComponent<WrestlingGameBootstrap>();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        RenderSettings.ambientLight = new Color(.48f, .48f, .52f);
        CreateLight();
        CreateRing();
    }

    private void OnGUI()
    {
        if (!matchStarted)
            DrawRosterMenu();
    }

    private void DrawRosterMenu()
    {
        float scale = Mathf.Clamp(Screen.height / 720f, .65f, 1f);
        float panelWidth = Mathf.Min(Screen.width - 24f, 920f);
        float panelHeight = 340f * scale;
        float left = (Screen.width - panelWidth) * .5f;
        float top = (Screen.height - panelHeight) * .5f;

        GUI.Box(new Rect(left, top, panelWidth, panelHeight), "");
        GUIStyle title = new GUIStyle(GUI.skin.label);
        title.alignment = TextAnchor.MiddleCenter;
        title.fontSize = Mathf.RoundToInt(28f * scale);
        title.fontStyle = FontStyle.Bold;
        title.normal.textColor = Color.white;
        GUI.Label(new Rect(left + 12f, top + 10f * scale,
            panelWidth - 24f, 42f * scale), "WRESTLING CLASH", title);

        GUIStyle button = new GUIStyle(GUI.skin.button);
        button.fontSize = Mathf.RoundToInt(17f * scale);
        button.fontStyle = FontStyle.Bold;

        DrawRosterRow("PLAYER", top + 62f * scale, ref selectedPlayer, left, panelWidth, scale, button);
        DrawRosterRow("OPPONENT", top + 158f * scale, ref selectedOpponent, left, panelWidth, scale, button);

        GUI.backgroundColor = new Color(.25f, .75f, .38f);
        if (GUI.Button(new Rect(left + panelWidth * .25f, top + 266f * scale,
            panelWidth * .5f, 52f * scale), "START MATCH", button))
            StartMatch();
        GUI.backgroundColor = Color.white;
    }

    private void DrawRosterRow(
        string heading, float y, ref string selected, float left,
        float panelWidth, float scale, GUIStyle button)
    {
        GUIStyle label = new GUIStyle(GUI.skin.label);
        label.fontSize = Mathf.RoundToInt(17f * scale);
        label.fontStyle = FontStyle.Bold;
        label.normal.textColor = Color.white;
        GUI.Label(new Rect(left + 18f, y, panelWidth - 36f, 25f * scale), heading, label);

        float gap = 8f * scale;
        float start = left + 16f;
        float width = (panelWidth - 32f - gap * 3f) / 4f;
        float buttonY = y + 29f * scale;
        float height = 48f * scale;

        for (int i = 0; i < Roster.Length; i++)
        {
            string key = Roster[i];
            Rect rect = new Rect(start + i * (width + gap), buttonY, width, height);
            GUI.backgroundColor = selected == key
                ? new Color(.25f, .65f, 1f)
                : Color.white;

            if (GUI.Button(rect, key.ToUpperInvariant(), button))
                selected = key;
        }

        GUI.backgroundColor = Color.white;
    }

    private void StartMatch()
    {
        matchStarted = true;

        GameObject player = CreateFighter(
            selectedPlayer.ToUpperInvariant(), selectedPlayer,
            new Vector3(-1.25f, 0f, 0f), new Color(.12f, .35f, .85f));
        GameObject rival = CreateFighter(
            selectedOpponent.ToUpperInvariant(), selectedOpponent,
            new Vector3(1.25f, 0f, 0f), new Color(.8f, .12f, .12f));

        player.AddComponent<WrestlerController>().isPlayer = true;
        rival.AddComponent<WrestlerController>().isPlayer = false;

        WrestlerAnimationPlayer playerAnimations = player.AddComponent<WrestlerAnimationPlayer>();
        WrestlerAnimationPlayer rivalAnimations = rival.AddComponent<WrestlerAnimationPlayer>();
        playerAnimations.LoadLibrary(selectedPlayer);
        rivalAnimations.LoadLibrary(selectedOpponent);

        GameObject cameraObject = new GameObject("MatchCamera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.fieldOfView = 55f;
        cameraObject.AddComponent<AudioListener>();

        MatchCamera follow = cameraObject.AddComponent<MatchCamera>();
        follow.target = player.transform;
        follow.secondaryTarget = rival.transform;

        MatchManager manager = new GameObject("MatchManager").AddComponent<MatchManager>();
        manager.Setup(player, rival,
            selectedPlayer.ToUpperInvariant(), selectedOpponent.ToUpperInvariant());
        new GameObject("MobileControls").AddComponent<MobileControls>();
    }

    private void CreateLight()
    {
        GameObject lightObject = new GameObject("ArenaLight");
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
    }

    private void CreateRing()
    {
        MakeCube("ArenaFloor", new Vector3(0f, -.35f, 0f),
            new Vector3(18f, .5f, 18f), new Color(.08f, .08f, .1f));
        MakeCube("RingMat", new Vector3(0f, .15f, 0f),
            new Vector3(8f, .35f, 8f), new Color(.12f, .25f, .42f));

        foreach (float x in new float[] { -3.8f, 3.8f })
        foreach (float z in new float[] { -3.8f, 3.8f })
            MakeCube("RingPost", new Vector3(x, 1.15f, z),
                new Vector3(.22f, 2.3f, .22f), Color.gray);

        for (int i = 0; i < 3; i++)
        {
            float y = .65f + i * .48f;
            MakeCube("RopeNorth", new Vector3(0f, y, 3.8f),
                new Vector3(7.6f, .07f, .07f), Color.white);
            MakeCube("RopeSouth", new Vector3(0f, y, -3.8f),
                new Vector3(7.6f, .07f, .07f), Color.white);
            MakeCube("RopeEast", new Vector3(3.8f, y, 0f),
                new Vector3(.07f, .07f, 7.6f), Color.white);
            MakeCube("RopeWest", new Vector3(-3.8f, y, 0f),
                new Vector3(.07f, .07f, 7.6f), Color.white);
        }
    }

    private GameObject CreateFighter(
        string label, string modelKey, Vector3 position, Color fallbackColor)
    {
        GameObject root = new GameObject(label);
        root.transform.position = position;

        GameObject model = Resources.Load<GameObject>("Wrestlers/" + modelKey);
        if (model != null)
        {
            GameObject visual = Instantiate(model, root.transform);
            visual.name = modelKey + "_Visual";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one * .01f;
        }
        else
        {
            Debug.LogWarning("Missing GLB model Resources/Wrestlers/" + modelKey +
                             ". Using a temporary placeholder.");
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = Vector3.up;
            body.transform.localScale = new Vector3(.72f, 1f, .72f);
            body.GetComponent<Renderer>().material.color = fallbackColor;

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = Vector3.up * 2.05f;
            head.transform.localScale = Vector3.one * .48f;
            head.GetComponent<Renderer>().material.color = new Color(.75f, .58f, .43f);
        }

        CapsuleCollider bodyCollider = root.AddComponent<CapsuleCollider>();
        bodyCollider.center = new Vector3(0f, 1.05f, 0f);
        bodyCollider.height = 2.1f;
        bodyCollider.radius = .42f;
        root.AddComponent<Rigidbody>().isKinematic = true;
        return root;
    }

    private void MakeCube(string name, Vector3 position, Vector3 scale, Color color)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().material.color = color;
    }
}

public class MatchCamera : MonoBehaviour
{
    public Transform target;
    public Transform secondaryTarget;

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 focus = target.position;
        if (secondaryTarget)
            focus = (target.position + secondaryTarget.position) * .5f;

        float distance = secondaryTarget
            ? Vector3.Distance(target.position, secondaryTarget.position)
            : 2f;
        Vector3 desired = focus +
            new Vector3(0f, 5.8f + distance * .18f, -9.5f - distance * .25f);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * 3f);
        transform.LookAt(focus + Vector3.up * 1.05f);
    }
}
