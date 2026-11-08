using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class WrestlerAnimationPlayer : MonoBehaviour
{
    private sealed class Track { public Transform bone; public Quaternion[] rotations; }
    private sealed class Clip { public string name; public float fps; public int frameCount; public Track[] tracks; }
    private readonly Dictionary<string, Clip> clips = new Dictionary<string, Clip>(StringComparer.OrdinalIgnoreCase);
    private Clip activeClip;
    private float elapsed;
    private bool playing;

    public void LoadLibrary(string resourceKey)
    {
        TextAsset asset = Resources.Load<TextAsset>("AnimationLibraries/" + resourceKey);
        if (asset == null) { Debug.LogWarning("Missing animation library: " + resourceKey); return; }

        JObject root;
        try { root = JObject.Parse(asset.text); }
        catch (Exception ex) { Debug.LogError("Invalid animation JSON " + resourceKey + ": " + ex.Message); return; }

        JArray skeleton = root["bones"] as JArray;
        JObject clipTable = root["clips"] as JObject;
        if (skeleton == null || clipTable == null) { Debug.LogError("Expected bones[] and clips{} in " + resourceKey); return; }

        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
            if (!boneMap.ContainsKey(child.name)) boneMap.Add(child.name, child);

        foreach (JProperty entry in clipTable.Properties())
        {
            JObject source = entry.Value as JObject;
            if (source == null) continue;
            int frames = source.Value<int?>("n") ?? 0;
            float fps = source.Value<float?>("fps") ?? 30f;
            JObject sourceTracks = source["bones"] as JObject;
            if (frames < 2 || fps <= 0f || sourceTracks == null) continue;

            List<Track> parsed = new List<Track>();
            foreach (JProperty boneEntry in sourceTracks.Properties())
            {
                Transform target;
                JArray values = boneEntry.Value as JArray;
                if (!boneMap.TryGetValue(boneEntry.Name, out target) || values == null || values.Count != frames * 4) continue;
                Quaternion[] rotations = new Quaternion[frames];
                bool valid = true;
                for (int f = 0; f < frames; f++)
                {
                    int i = f * 4;
                    float x=(float)values[i], y=(float)values[i+1], z=(float)values[i+2], w=(float)values[i+3];
                    if (float.IsNaN(x)||float.IsNaN(y)||float.IsNaN(z)||float.IsNaN(w)) { valid=false; break; }
                    Quaternion q = new Quaternion(x,y,z,w);
                    rotations[f] = q.sqrMagnitude < 0.0001f ? Quaternion.identity : Quaternion.Normalize(q);
                }
                if (valid) parsed.Add(new Track { bone=target, rotations=rotations });
            }
            if (parsed.Count > 0)
                clips[entry.Name] = new Clip { name=entry.Name, fps=fps, frameCount=frames, tracks=parsed.ToArray() };
        }
        Debug.Log("Animation library " + resourceKey + ": " + clips.Count + " clips; mapped transforms " + boneMap.Count);
    }

    public bool HasClip(string name) => clips.ContainsKey(name);
    public float GetClipDuration(string name) => clips.TryGetValue(name, out Clip c) ? (c.frameCount - 1f) / c.fps : 0f;

    public string FindClipStartingWith(string prefix, string keyword)
    {
        foreach (string name in clips.Keys)
            if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return name;
        return null;
    }

    public bool PlayClip(string name)
    {
        if (!clips.TryGetValue(name, out activeClip)) return false;
        elapsed=0f; playing=true; ApplyFrame(0f); return true;
    }

    private void Update()
    {
        if (!playing || activeClip == null) return;
        elapsed += Time.deltaTime;
        float frame=elapsed*activeClip.fps;
        if (frame >= activeClip.frameCount-1) { frame=activeClip.frameCount-1; playing=false; }
        ApplyFrame(frame);
    }

    private void ApplyFrame(float frame)
    {
        int a=Mathf.Clamp(Mathf.FloorToInt(frame),0,activeClip.frameCount-1);
        int b=Mathf.Min(a+1,activeClip.frameCount-1);
        float blend=frame-a;
        foreach (Track track in activeClip.tracks)
            if (track.bone != null)
                track.bone.localRotation=Quaternion.Slerp(track.rotations[a],track.rotations[b],blend);
    }
}