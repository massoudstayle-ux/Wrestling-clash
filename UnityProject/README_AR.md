# RingClash3D — Unity starter project

This is the first Unity gameplay prototype assembled from the existing repository archive.

## Open
1. Download the GitHub Actions artifact named `RingClash3D-Unity-Project`.
2. Extract it.
3. Open the extracted `UnityProject` folder in Unity Hub using Unity 2022.3 LTS or a compatible 2022.3 editor.
4. Press Play.

## Current prototype
- Ring, ropes, posts, arena lighting and follow camera are generated at runtime.
- Player movement and an AI opponent.
- Punch, kick, grapple and block inputs.
- On-screen touch buttons for Android.

## Important status
The current fighters are temporary primitive placeholders. Original character models, FBX clips and JSON animation libraries are preserved under `Assets/ImportedOriginal`; they have not yet been retargeted or connected to the new Unity animator. The first Actions job packages the project and validates its structure; it does not run the Unity Editor or produce an APK. Visual/gameplay testing is still required in Unity.
