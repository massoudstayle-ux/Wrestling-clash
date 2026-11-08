# Third-Party Assets and Licenses

This file tracks third-party components and assets used by Wrestling-clash.

## Scope

The MIT License in the repository's `LICENSE` file applies only to original
project code and documentation created by the project contributors, to the
extent those materials are owned by them.

It does **not** grant permission to use, copy, modify, redistribute, publish,
or commercially exploit third-party materials, including (but not limited to):

- Wrestler likenesses, names, character models, costumes, logos, and trademarks.
- Wrestling animations, motion-capture data, and game-extracted assets.
- Music, sound effects, images, textures, fonts, and other imported media.
- Third-party libraries, plugins, engines, and tools (which remain under their
  own respective licenses).

A file being present in this repository does not establish that the project
owns it or has permission to redistribute it. Do not assume that WWE, TNA,
publisher, developer, or other brand-related assets are licensed under MIT.

## Verified project dependencies

The following direct dependencies were confirmed in
`UnityProject/Packages/manifest.json`. Their presence and version are verified;
the license identification below is based on the upstream project or vendor
license information.

| Component / version | Source / rightsholder | License | Required notice / action |
| --- | --- | --- | --- |
| Unity Editor and Unity Runtime 2022.3.62f1 | Unity Technologies | Unity Editor Software Terms and applicable Unity Terms of Service; not an MIT-licensed component | Use Unity only under an eligible Unity plan and comply with the terms applicable to the build and distribution. The Unity Runtime may be distributed only as an integrated part of a Unity project, subject to those terms. |
| glTFast `com.unity.cloud.gltfast` 6.17.0 | Unity Technologies and glTFast authors | Apache License 2.0 | Preserve the upstream license and applicable third-party notices when redistributing the package or its covered code. |
| Newtonsoft Json Unity package `com.unity.nuget.newtonsoft-json` 3.2.1 (corresponds to Json.NET 13.0.2) | James Newton-King / Newtonsoft | MIT | Preserve the Json.NET copyright and MIT license notice when redistributing covered copies. |
| Unity built-in modules: AI, Physics, UI, Animation | Unity Technologies | Part of the Unity package/runtime distribution; governed by applicable Unity terms | Do not treat these modules as project-owned or relicense them under this repository's MIT license. |

Official references:
- Unity Editor Software Terms: https://activation.unity3d.com/legal/editor-terms-of-service/software
- Unity glTFast source and Apache-2.0 license: https://github.com/Unity-Technologies/com.unity.cloud.gltfast
- Newtonsoft Json Unity package documentation: https://docs.unity.cn/Packages/com.unity.nuget.newtonsoft-json%403.2/manual/index.html
- Json.NET MIT license: https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md

Note: this is a list of direct dependencies declared in the manifest, not a
complete inventory of every transitive package pulled in by Unity Package
Manager. Review the resolved package lock and each package's notices before
shipping a release.

## Asset register

Add an entry for each third-party component or asset before redistributing it.
Record its source, creator/rightsholder, exact license or permission, and any
required attribution or restrictions. Use `Unknown / verification required`
rather than guessing.

| Component / asset | Source | Creator / rightsholder | License / permission | Required action |
| --- | --- | --- | --- | --- |
| Wrestler models, likenesses, names, costumes, and logos | Not established from the inspected files | Unknown | Unknown / verification required | Confirm ownership or written permission before public or commercial distribution |
| Wrestling animations and motion data | Not established from the inspected files | Unknown | Unknown / verification required | Identify each source and confirm redistribution rights |
| Ring, arena, textures, audio, UI art, fonts, and other imported assets | Not yet inventoried | Unknown | Unknown / verification required | Audit imported assets and preserve each original license or permission |

## Contributors

Only add assets when their license permits the intended use and distribution.
Keep original license texts and required notices with the relevant asset.
If permission is unclear or unavailable, exclude the asset from public releases
or replace it with an appropriately licensed alternative.
