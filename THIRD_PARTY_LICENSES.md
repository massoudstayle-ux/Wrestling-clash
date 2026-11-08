# Third-Party Assets and Licenses

This file tracks third-party components and assets used by Wrestling-clash.
It is an inventory, not a grant of rights.

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
| Unity Editor and Unity Runtime 2022.3.62f1 | Unity Technologies | Unity Editor Software Terms and applicable Unity Terms of Service; not an MIT-licensed component | Use Unity under an eligible plan and comply with terms applicable to the build and distribution. |
| glTFast `com.unity.cloud.gltfast` 6.17.0 | Unity Technologies and glTFast authors | Apache License 2.0 | Preserve the upstream license and applicable third-party notices when redistributing covered code. |
| Newtonsoft Json Unity package `com.unity.nuget.newtonsoft-json` 3.2.1 (corresponds to Json.NET 13.0.2) | James Newton-King / Newtonsoft | MIT | Preserve the Json.NET copyright and MIT license notice when redistributing covered copies. |
| Unity built-in modules: AI, Physics, UI, Animation | Unity Technologies | Governed by applicable Unity terms | Do not treat these modules as project-owned or relicense them under this repository's MIT license. |

Official references:
- Unity Editor Software Terms: https://activation.unity3d.com/legal/editor-terms-of-service/software
- Unity glTFast source and Apache-2.0 license: https://github.com/Unity-Technologies/com.unity.cloud.gltfast
- Newtonsoft Json Unity package documentation: https://docs.unity.cn/Packages/com.unity.nuget.newtonsoft-json%403.2/manual/index.html
- Json.NET MIT license: https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md

Note: this covers direct dependencies declared in the manifest, not every
transitive package resolved by Unity Package Manager. The repository currently
does not contain `UnityProject/Packages/packages-lock.json`; inspect the
resolved package list and each package's notices before shipping.

## Asset audit — findings from the checked-in source archive

The GitHub Actions inspection of `RingClash3D_Unity.zip` confirmed that the
archive is a valid ZIP with 98 entries. It contains four GLB wrestler models
and their JPG textures, eleven Mixamo FBX animation files, and four JSON
animation libraries. The archive documentation and animation QA report were
also read.

| Component / asset | Evidence and source stated in project files | Rights status | Required action |
| --- | --- | --- | --- |
| AJ Styles model and textures | `Godot_Source_README_AR.md` identifies the supplied AJ model as a RigModels asset | **Personal / student educational use only**, according to the project's own source documentation | Do not include in public or commercial releases unless the rightsholder grants broader written permission. Replace for distributable builds if permission is unavailable. |
| Randy Orton model and textures | Same RigModels notice as AJ in `Godot_Source_README_AR.md` | **Personal / student educational use only**, according to the project's own source documentation | Same restriction: do not include in public or commercial releases without broader written permission. |
| Boxer model and textures | `boxer.glb` and `boxer_0.jpg` are present in the archive | Unknown / verification required | Identify the creator and obtain the exact license or permission. |
| Runner model and textures | `runner.glb` and `runner_*.jpg` are present in the archive | Unknown / verification required | Identify the creator and obtain the exact license or permission. |
| Mixamo animation FBX files | Eleven FBX files under `Assets/RingClash3D/Art/Animations/Mixamo/` | Mixamo is named as the source, but the acquisition account, applicable terms, and redistribution permission for these exact files were not included in the archive | Verify the applicable Adobe Mixamo terms and whether the intended packaged redistribution is allowed. Keep required notices. Do not assume MIT applies. |
| WWE2011 animation libraries | Four JSON files under `Assets/RingClash3D/Data/Animations/`; the included QA report says active clips are tagged `WWE2011_NEW` and retargeted from WWE2011 banks | Game-derived animation data; original rightsholder and redistribution permission are not established by the archive | Treat as restricted / not cleared for public redistribution. Obtain permission or replace with independently created or appropriately licensed motion data. |
| Motifect and Haley BVH sources | Mentioned as historical animation sources in `Godot_Source_README_AR.md`; their source files are not listed in the current Unity ZIP inventory | Not verified as included in this Unity archive; exact licenses not established here | If reintroduced, record exact package, author, license, and permitted use before redistribution. |
| Ring, arena, UI, sound, music, fonts, other imported assets | No complete item-by-item rights inventory was supplied by the inspected archive | Unknown / verification required | Audit each asset and retain its original license or permission. |

### Release decision

**The current asset set is not cleared for public or commercial redistribution.**
The clearest blocker is the documented personal/student-only restriction on
the AJ and Orton RigModels models. The WWE2011-derived animation libraries,
Mixamo FBX files, Boxer/Runner models, and any other imported materials also
need their exact permissions verified. This is a rights-status finding, not a
claim that any specific rightsholder has denied permission.

Until those checks are complete, keep the repository private if appropriate
and do not publish a build containing assets whose redistribution rights are
unclear. A safer release path is to replace restricted models and motion data
with assets created by the project or obtained under licenses that explicitly
allow the intended distribution.

## Asset register maintenance

For every third-party component, record its exact source URL, creator or
rightsholder, license version, date obtained, required attribution, and whether
commercial use and redistribution inside an APK are permitted. Use
`Unknown / verification required` rather than guessing.

Keep original license texts and required notices with the relevant asset.
If permission is unclear or unavailable, exclude the asset from public
releases or replace it with an appropriately licensed alternative.

## Contributors

Only add assets when their license permits the intended use and distribution.
