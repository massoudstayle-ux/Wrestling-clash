using UnityEngine;

public class WrestlingGameBootstrap : MonoBehaviour
{
    private Transform player;
    private Transform rival;
    private Camera gameCamera;
    private float playerHealth = 100f;
    private float rivalHealth = 100f;
    private float playerAttackCooldown;
    private float rivalAttackCooldown;
    private bool blocking;
    private GUIStyle labelStyle;
    private GUIStyle buttonStyle;

    private void Start()
    {
        CreateArena();
        player = CreateFighter("PLAYER", new Vector3(-1.4f, 1.15f, 0f), new Color(0.08f, 0.35f, 0.95f)).transform;
        rival = CreateFighter("RIVAL", new Vector3(1.4f, 1.15f, 0f), new Color(0.85f, 0.12f, 0.12f)).transform;
        CreateCamera();
    }

    private void Update()
    {
        if (player == null || rival == null) return;
        playerAttackCooldown -= Time.deltaTime;
        rivalAttackCooldown -= Time.deltaTime;

        Vector3 input = Vector3.zero;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input.x -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input.x += 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input.z += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) input.z -= 1f;
        MovePlayer(input);

        Vector3 toPlayer = player.position - rival.position;
        toPlayer.y = 0f;
        if (toPlayer.magnitude > 1.35f)
            rival.position += toPlayer.normalized * 1.15f * Time.deltaTime;
        rival.rotation = Quaternion.LookRotation(toPlayer.sqrMagnitude > 0.01f ? toPlayer : Vector3.forward);

        if (toPlayer.magnitude < 1.65f && rivalAttackCooldown <= 0f)
        {
            rivalAttackCooldown = 1.8f;
            if (!blocking) playerHealth = Mathf.Max(0f, playerHealth - 7f);
        }

        player.position = new Vector3(Mathf.Clamp(player.position.x, -2.7f, 2.7f), 1.15f,
            Mathf.Clamp(player.position.z, -2.7f, 2.7f));
        rival.position = new Vector3(Mathf.Clamp(rival.position.x, -2.7f, 2.7f), 1.15f,
            Mathf.Clamp(rival.position.z, -2.7f, 2.7f));

        if (gameCamera != null)
        {
            Vector3 midpoint = (player.position + rival.position) * 0.5f;
            Vector3 desired = midpoint + new Vector3(0f, 7.5f, -9.5f);
            gameCamera.transform.position = Vector3.Lerp(gameCamera.transform.position, desired, Time.deltaTime * 3f);
            gameCamera.transform.LookAt(midpoint);
        }
    }

    private void MovePlayer(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;
        direction = Vector3.ClampMagnitude(direction, 1f);
        player.position += new Vector3(direction.x, 0f, direction.z) * 3.4f * Time.deltaTime;
        Vector3 face = rival.position - player.position;
        face.y = 0f;
        if (face.sqrMagnitude > 0.01f) player.rotation = Quaternion.LookRotation(face);
    }

    private void Punch()
    {
        if (playerAttackCooldown > 0f || player == null || rival == null) return;
        playerAttackCooldown = 0.55f;
        if (Vector3.Distance(player.position, rival.position) < 2.15f)
            rivalHealth = Mathf.Max(0f, rivalHealth - 6f);
    }

    private void Grapple()
    {
        if (playerAttackCooldown > 0f || player == null || rival == null) return;
        playerAttackCooldown = 1.2f;
        if (Vector3.Distance(player.position, rival.position) < 2.0f)
            rivalHealth = Mathf.Max(0f, rivalHealth - 13f);
    }

    private GameObject CreateFighter(string fighterName, Vector3 position, Color color)
    {
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = fighterName;
        body.transform.position = position;
        body.transform.localScale = new Vector3(0.85f, 1.15f, 0.85f);
        Renderer renderer = body.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = color;

        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = fighterName + "_Head";
        head.transform.SetParent(body.transform);
        head.transform.localPosition = new Vector3(0f, 0.92f, 0f);
        head.transform.localScale = new Vector3(0.58f, 0.58f, 0.58f);
        head.GetComponent<Renderer>().material = renderer.material;
        Destroy(head.GetComponent<Collider>());
        return body;
    }

    private void CreateArena()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Wrestling Ring Mat";
        floor.transform.position = new Vector3(0f, 0f, 0f);
        floor.transform.localScale = new Vector3(7f, 0.35f, 7f);
        floor.GetComponent<Renderer>().material = MakeMaterial(new Color(0.12f, 0.22f, 0.48f));

        CreatePost(-3.25f, -3.25f); CreatePost(-3.25f, 3.25f);
        CreatePost(3.25f, -3.25f); CreatePost(3.25f, 3.25f);
        for (int level = 0; level < 3; level++)
        {
            float y = 0.65f + level * 0.55f;
            CreateRope(new Vector3(-3.25f, y, -3.25f), new Vector3(-3.25f, y, 3.25f));
            CreateRope(new Vector3(3.25f, y, -3.25f), new Vector3(3.25f, y, 3.25f));
            CreateRope(new Vector3(-3.25f, y, -3.25f), new Vector3(3.25f, y, -3.25f));
            CreateRope(new Vector3(-3.25f, y, 3.25f), new Vector3(3.25f, y, 3.25f));
        }

        GameObject lightObject = new GameObject("Arena Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.62f);
    }

    private void CreatePost(float x, float z)
    {
        GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        post.name = "Ring Corner Post";
        post.transform.position = new Vector3(x, 1.15f, z);
        post.transform.localScale = new Vector3(0.16f, 1.25f, 0.16f);
        post.GetComponent<Renderer>().material = MakeMaterial(new Color(0.12f, 0.12f, 0.14f));
    }

    private void CreateRope(Vector3 a, Vector3 b)
    {
        GameObject rope = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rope.name = "Ring Rope";
        Vector3 delta = b - a;
        rope.transform.position = (a + b) * 0.5f;
        rope.transform.up = delta.normalized;
        rope.transform.localScale = new Vector3(0.045f, delta.magnitude * 0.5f, 0.045f);
        rope.GetComponent<Renderer>().material = MakeMaterial(new Color(0.8f, 0.08f, 0.08f));
    }

    private Material MakeMaterial(Color color)
    {
        Material material = new Material(Shader.Find("Standard"));
        material.color = color;
        return material;
    }

    private void CreateCamera()
    {
        gameCamera = Camera.main;
        if (gameCamera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            gameCamera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }
        gameCamera.transform.position = new Vector3(0f, 8f, -10f);
        gameCamera.fieldOfView = 55f;
        gameCamera.clearFlags = CameraClearFlags.SolidColor;
        gameCamera.backgroundColor = new Color(0.035f, 0.045f, 0.075f);
    }

    private void OnGUI()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(18, Screen.height / 28), alignment = TextAnchor.MiddleCenter };
            labelStyle.normal.textColor = Color.white;
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = Mathf.Max(16, Screen.height / 34), fontStyle = FontStyle.Bold };
        }

        float w = Screen.width;
        float h = Screen.height;
        GUI.Label(new Rect(w * 0.28f, 8f, w * 0.44f, h * 0.07f),
            "PLAYER  " + Mathf.CeilToInt(playerHealth) + "     VS     RIVAL  " + Mathf.CeilToInt(rivalHealth), labelStyle);

        float size = Mathf.Min(w * 0.12f, h * 0.19f);
        float left = 18f;
        float bottom = h - size * 2.45f;
        if (GUI.RepeatButton(new Rect(left + size, bottom, size, size), "UP", buttonStyle)) MovePlayer(Vector3.forward);
        if (GUI.RepeatButton(new Rect(left + size, bottom + size, size, size), "DOWN", buttonStyle)) MovePlayer(Vector3.back);
        if (GUI.RepeatButton(new Rect(left, bottom + size, size, size), "LEFT", buttonStyle)) MovePlayer(Vector3.left);
        if (GUI.RepeatButton(new Rect(left + size * 2f, bottom + size, size, size), "RIGHT", buttonStyle)) MovePlayer(Vector3.right);

        float right = w - size * 3.3f;
        if (GUI.Button(new Rect(right + size, bottom, size * 1.2f, size), "PUNCH", buttonStyle)) Punch();
        if (GUI.Button(new Rect(right + size, bottom + size * 1.05f, size * 1.2f, size), "GRAPPLE", buttonStyle)) Grapple();
        blocking = GUI.Toggle(new Rect(right, bottom + size * 2.1f, size * 3.2f, size * 0.7f), blocking, "BLOCK", buttonStyle);

        if (rivalHealth <= 0f)
            GUI.Label(new Rect(w * 0.3f, h * 0.25f, w * 0.4f, h * 0.12f), "YOU WIN!", labelStyle);
        else if (playerHealth <= 0f)
            GUI.Label(new Rect(w * 0.3f, h * 0.25f, w * 0.4f, h * 0.12f), "YOU LOST", labelStyle);
    }
}
