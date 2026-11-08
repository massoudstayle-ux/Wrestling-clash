using UnityEngine;

public class WrestlingGameBootstrap : MonoBehaviour
{
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

        // The real GLB characters are imported by glTFast from Resources/Wrestlers.
        // AJ and Orton are the first match; Boxer and Runner remain available in the roster assets.
        var player = CreateFighter("AJ", "aj", new Vector3(-1.25f, 0, 0), new Color(.12f, .35f, .85f));
        var rival = CreateFighter("ORTON", "orton", new Vector3(1.25f, 0, 0), new Color(.8f, .12f, .12f));
        player.AddComponent<WrestlerController>().isPlayer = true;
        rival.AddComponent<WrestlerController>().isPlayer = false;

        var cameraObject = new GameObject("MatchCamera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.fieldOfView = 55;
        cameraObject.AddComponent<AudioListener>();
        var follow = cameraObject.AddComponent<MatchCamera>();
        follow.target = player.transform;
        follow.secondaryTarget = rival.transform;

        var manager = new GameObject("MatchManager").AddComponent<MatchManager>();
        manager.Setup(player, rival);
        new GameObject("MobileControls").AddComponent<MobileControls>();
    }

    private void CreateLight()
    {
        var lightObject = new GameObject("ArenaLight");
        lightObject.transform.rotation = Quaternion.Euler(50, -30, 0);
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
    }

    private void CreateRing()
    {
        MakeCube("ArenaFloor", new Vector3(0, -.35f, 0), new Vector3(18, .5f, 18), new Color(.08f, .08f, .1f));
        MakeCube("RingMat", new Vector3(0, .15f, 0), new Vector3(8, .35f, 8), new Color(.12f, .25f, .42f));

        foreach (float x in new float[] { -3.8f, 3.8f })
        foreach (float z in new float[] { -3.8f, 3.8f })
            MakeCube("RingPost", new Vector3(x, 1.15f, z), new Vector3(.22f, 2.3f, .22f), Color.gray);

        for (int i = 0; i < 3; i++)
        {
            float y = .65f + i * .48f;
            MakeCube("RopeNorth", new Vector3(0, y, 3.8f), new Vector3(7.6f, .07f, .07f), Color.white);
            MakeCube("RopeSouth", new Vector3(0, y, -3.8f), new Vector3(7.6f, .07f, .07f), Color.white);
            MakeCube("RopeEast", new Vector3(3.8f, y, 0), new Vector3(.07f, .07f, 7.6f), Color.white);
            MakeCube("RopeWest", new Vector3(-3.8f, y, 0), new Vector3(.07f, .07f, 7.6f), Color.white);
        }
    }

    private GameObject CreateFighter(string label, string modelKey, Vector3 position, Color fallbackColor)
    {
        var root = new GameObject(label);
        root.transform.position = position;

        GameObject model = Resources.Load<GameObject>("Wrestlers/" + modelKey);
        if (model != null)
        {
            var visual = Instantiate(model, root.transform);
            visual.name = modelKey + "_Visual";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            // The source GLB character meshes are authored at roughly 250 cm tall.
            visual.transform.localScale = Vector3.one * .01f;
        }
        else
        {
            Debug.LogWarning("Missing GLB model Resources/Wrestlers/" + modelKey + ". Using a temporary placeholder.");
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = Vector3.up;
            body.transform.localScale = new Vector3(.72f, 1, .72f);
            body.GetComponent<Renderer>().material.color = fallbackColor;
            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = Vector3.up * 2.05f;
            head.transform.localScale = Vector3.one * .48f;
            head.GetComponent<Renderer>().material.color = new Color(.75f, .58f, .43f);
        }

        var bodyCollider = root.AddComponent<CapsuleCollider>();
        bodyCollider.center = new Vector3(0, 1.05f, 0);
        bodyCollider.height = 2.1f;
        bodyCollider.radius = .42f;
        root.AddComponent<Rigidbody>().isKinematic = true;
        return root;
    }

    private void MakeCube(string name, Vector3 position, Vector3 scale, Color color)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
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
        if (secondaryTarget) focus = (target.position + secondaryTarget.position) * .5f;
        float distance = secondaryTarget ? Vector3.Distance(target.position, secondaryTarget.position) : 2f;
        Vector3 desired = focus + new Vector3(0, 5.8f + distance * .18f, -9.5f - distance * .25f);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * 3f);
        transform.LookAt(focus + Vector3.up * 1.05f);
    }
}
