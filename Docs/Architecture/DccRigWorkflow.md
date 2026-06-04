# DCC Rig Workflow / DCC 绑定工作流

## Core Principle / 核心原则

中文：在这个项目里，不建议把“动画师用的制作骨架”和“游戏运行时使用的导出骨架”视为同一个东西。最稳妥的做法是双层结构：

1. Authoring Rig：给 Blender 动画制作使用，可以是 Rigify、Auto-Rig Pro，甚至自定义控制器。
2. Export Game Rig：给 Godot 导出使用，只保留稳定的 deform bones、socket 和动画轨道。

English: In this project, the animation-authoring rig and the runtime export rig should not be treated as the same thing. The safest workflow is a two-layer setup:

1. Authoring Rig: for Blender animation work, using Rigify, Auto-Rig Pro, or a custom controller rig.
2. Export Game Rig: for Godot export, containing only stable deform bones, sockets, and gameplay-relevant animation tracks.

## Why This Matters / 为什么这很重要

中文：Rigify 和 Auto-Rig Pro 都很适合做动画，但它们的控制器命名、约束层级、辅助骨数量并不适合直接作为引擎运行骨架。Godot 真正关心的是最终导出的骨架层级、骨骼朝向、socket、动画轨道和可重定向性，而不是你在 Blender 里如何控骨。

English: Rigify and Auto-Rig Pro are both good for animation authoring, but their controller naming, constraint hierarchy, and helper-bone counts are not ideal as direct runtime rigs. Godot ultimately cares about the exported skeleton hierarchy, bone orientation, sockets, animation tracks, and retargetability, not how the rig is controlled inside Blender.

## Rigify vs Auto-Rig Pro / Rigify 与 Auto-Rig Pro

### Rigify

中文：如果你已经熟悉 Rigify，它完全可以继续用。它的优势是免费、成熟、生态广、作者多。缺点是导出到游戏引擎时需要更自觉地整理 deform bones、辅助骨、约束烘焙和命名，不然导出骨架会比较脏。

English: If you are already comfortable with Rigify, it is completely usable. Its strengths are being free, mature, and widely adopted. Its weakness is that export to game engines requires more discipline around deform bones, helper bones, baked constraints, and naming, otherwise the exported rig becomes noisy.

### Auto-Rig Pro

中文：Auto-Rig Pro 通常更偏向“从 DCC 到游戏引擎”的整洁导出流程，尤其在游戏导出、重定向、统一骨架输出方面往往更省心。如果你后面会大量做角色，并且希望更稳定地导出游戏骨架，它通常会比 Rigify 更省时间。

English: Auto-Rig Pro is generally more oriented toward a cleaner DCC-to-engine export workflow, especially for game export, retargeting, and stable output skeletons. If you expect to build many characters and want more predictable game-rig export, it will often save time compared with Rigify.

## Recommendation for This Project / 对本项目的建议

中文：短期内，不需要因为 Godot 就立刻放弃 Rigify。更合理的建议是：

- 如果你目前最熟悉 Rigify，就继续用 Rigify 做 authoring rig。
- 但请尽快建立一套单独的 export game rig 规范。
- 如果后面发现 Rigify 导出整理成本太高，再考虑转向 Auto-Rig Pro。

English: In the short term, you do not need to abandon Rigify just because you are using Godot.

- If Rigify is your most familiar tool, keep using it for the authoring rig.
- But establish a separate export game rig standard as soon as possible.
- If Rigify export cleanup becomes too expensive later, then consider moving to Auto-Rig Pro.

## Godot and Humanoid Retargeting / Godot 与 Humanoid 重定向

中文：Godot 4 确实有“人形骨架标准化”和“骨骼映射”的能力，主要是 `SkeletonProfileHumanoid` 和 `BoneMap`，再配合导入器里的 Retarget 选项来做动画共享和重定向。但它不是 Unity Mecanim 那种完整意义上的 `Humanoid Avatar` 运行时系统。

English: Godot 4 does provide humanoid standardization and bone mapping through `SkeletonProfileHumanoid` and `BoneMap`, plus importer retarget options for animation sharing. But it is not the same as Unity Mecanim's full runtime `Humanoid Avatar` system.

## Practical Conclusion / 实际结论

中文：对你来说，最好的策略不是强迫 Rigify 控制骨直接符合引擎命名，而是让导出骨架符合项目标准，并在必要时为 Godot 建立 BoneMap。也就是说：作者骨架可以自由，导出骨架必须严格。

English: The best strategy is not to force Rigify control bones to match engine naming directly. Instead, keep the exported game skeleton aligned with the project standard and add a BoneMap for Godot when necessary. In practice: authoring rig can be flexible, export rig must be strict.


## Export Bone Naming / 导出骨骼命名

中文：对于人形角色，导出到 Godot 的 game rig 建议尽量接近 SkeletonProfileHumanoid 的标准名，例如 Hips、Spine、LeftUpperArm、RightFoot。这样更容易利用 BoneMap 自动映射。

English: For humanoid characters, the exported game rig should stay close to SkeletonProfileHumanoid names such as Hips, Spine, LeftUpperArm, and RightFoot. This makes BoneMap auto-mapping much easier.

