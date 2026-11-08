using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class WrestlerAnimationPlayer : MonoBehaviour
{
    private sealed class Track
    {
        public Transform bone;
        public Quaternion[] rotations;
    }

    private sealed class Clip
    {
        public string name;
        public float fps;
        public int frameCount;
        public bool loop;
        public Track[] tracks;
    }

    private readonly Dictionary<string, Clip> clips =
        new Dictionary<string, Clip>(StringComparer.OrdinalIgnoreCase);

    private Clip activeClip;
    private Clip idleClip;
    private float elapsed;
    private bool playing;

    public void LoadLibrary(string resourceKey)
    {
        TextAsset asset = Resources.Load<TextAsset>("AnimationLibraries/" + resourceKey);
        if (asset == null)
        {
            Debug.LogWarning("Missing animation library: " + resourceKey);
            return;
        }

        JObject root;
        try
        {
            root = JObject.Parse(asset.text);
        }
        catch (Exception ex)
        {
            Debug.LogError("Invalid animation JSON " + resourceKey + ": " + ex.Message);
            return;
        }

        JArray skeleton = root["bones"] as JArray;
        JObject clipTable = root["clips"] as JObject;
        if (skeleton == null || clipTable == null)
        {
            Debug.LogError("Expected bones[] and clips{} in " + resourceKey);
            return;
        }

        Dictionary<string, Transform> boneMap =
            new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (!boneMap.ContainsKey(child.name))
                boneMap.Add(child.name, child);
        }

        clips.Clear();
        foreach (JProperty entry in clipTable.Properties())
        {
            JObject source = entry.Value as JObject;
            if (source == null) continue;

            int frames = source.Value<int?>("n") ?? 0;
            float fps = source.Value<float?>("fps") ?? 30f;
            bool loop = source.Value<bool?>("loop") ?? false;
            JObject sourceTracks = source["bones"] as JObject;
            if (frames < 2 || fps <= 0f || sourceTracks == null) continue;

            List<Track> parsed = new List<Track>();
            foreach (JProperty boneEntry in sourceTracks.Properties())
            {
                Transform target;
                JArray values = boneEntry.Value as JArray;
                if (!boneMap.TryGetValue(boneEntry.Name, out target) ||
                    values == null || values.Count != frames * 4)
                    continue;

                Quaternion[] rotations = new Quaternion[frames];
                bool valid = true;
                for (int f = 0; f < frames; f++)
                {
                    int i = f * 4;
                    float x = (float)values[i];
                    float y = (float)values[i + 1];
                    float z = (float)values[i + 2];
                    float w = (float)values[i + 3];

                    if (float.IsNaN(x) || float.IsNaN(y) ||
                        float.IsNaN(z) || float.IsNaN(w))
                    {
                        valid = false;
                        break;
                    }

                    Quaternion q = new Quaternion(x, y, z, w);
                    rotations[f] = q.sqrMagnitude < 0.0001f
                        ? Quaternion.identity
                        : Quaternion.Normalize(q);
                }

                if (valid)
                    parsed.Add(new Track { bone = target, rotations = rotations });
            }

            if (parsed.Count > 0)
            {
                clips[entry.Name] = new Clip
                {
                    name = entry.Name,
                    fps = fps,
                    frameCount = frames,
                    loop = loop,
                    tracks = parsed.ToArray()
                };
            }
        }

        clips.TryGetValue("idle", out idleClip);
        Debug.Log("Animation library " + resourceKey + ": " + clips.Count +
                  " clips; mapped transforms " + boneMap.Count);

        if (idleClip != null)
            PlayClip("idle");
        else
            Debug.LogWarning("Animation library " + resourceKey + " has no idle clip.");
    }

    public bool HasClip(string name)
    {
        return clips.ContainsKey(name);
    }

    public float GetClipDuration(string name)
    {
        Clip clip;
        return clips.TryGetValue(name, out clip)
            ? (clip.frameCount - 1f) / clip.fps
            : 0f;
    }

    public string FindClipStartingWith(string prefix, string keyword)
    {
        foreach (string name in clips.Keys)
        {
            if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return name;
        }
        return null;
    }

    public bool PlayClip(string name)
    {
        Clip requested;
        if (!clips.TryGetValue(name, out requested))
            return false;

        if (activeClip == requested && playing)
            return true;

        activeClip = requested;
        elapsed = 0f;
        playing = true;
        ApplyFrame(0f);
        return true;
    }

    public void PlayLocomotion(bool moving, bool running)
    {
        // Do not interrupt a punch, reaction, or paired wrestling move.
        if (playing && activeClip != null && !activeClip.loop)
            return;

        string wanted = !moving ? "idle" : (running ? "run" : "walk");
        if (!HasClip(wanted) && moving && !running)
            wanted = "walk_b";
        if (!HasClip(wanted) && running)
            wanted = "run";
        if (HasClip(wanted))
            PlayClip(wanted);
    }

    private void Update()
    {
        if (!playing || activeClip == null)
            return;

        elapsed += Time.deltaTime;
        float frame = elapsed * activeClip.fps;

        if (activeClip.loop)
        {
            frame %= activeClip.frameCount;
        }
        else if (frame >= activeClip.frameCount - 1)
        {
            if (idleClip != null && activeClip != idleClip)
            {
                activeClip = idleClip;
                elapsed = 0f;
                frame = 0f;
            }
            else
            {
                frame = activeClip.frameCount - 1;
                playing = false;
            }
        }

        ApplyFrame(frame);
    }

    private void ApplyFrame(float frame)
    {
        if (activeClip == null) return;

        int a = Mathf.Clamp(Mathf.FloorToInt(frame), 0, activeClip.frameCount - 1);
        int b = activeClip.loop
            ? (a + 1) % activeClip.frameCount
            : Mathf.Min(a + 1, activeClip.frameCount - 1);
        float blend = frame - a;

        foreach (Track track in activeClip.tracks)
        {
            if (track.bone != null)
                track.bone.localRotation =
                    Quaternion.Slerp(track.rotations[a], track.rotations[b], blend);
        }
    }
}
