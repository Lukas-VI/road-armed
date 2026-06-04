# Blender to Godot Export Guide / Blender 到 Godot 导出指南

## Official Baseline / 官方基线

中文：Godot 官方对 3D 场景导入的建议很明确：优先使用 glTF 2.0；`.blend` 也可以直接导入，但本质上会调用 Blender 透明导出为 glTF；FBX 虽然可用，但不是优先方案。Godot 官方还建议在导出前考虑坐标朝向、三角化、应用对象变换，以及以默认姿态导出骨架。

English: Godot's official recommendation for 3D scene import is clear: prefer glTF 2.0; `.blend` can be imported directly, but it works by transparently calling Blender to export glTF; FBX is available but not the preferred path. Godot also recommends handling direction conventions, triangulation, applied object transforms, and exporting skeletons in their default pose.

## Recommended File Format / 推荐文件格式

### Default recommendation / 默认推荐

- `glTF 2.0` is the recommended format in Godot / Godot 官方推荐 `glTF 2.0`
- prefer `.glb` for most gameplay assets / 大多数玩法资产优先用 `.glb`
- use `.gltf + external textures` only when text diffing or asset inspection is important / 只有在需要文本差异查看或资产检查时再用 `.gltf + 外部纹理`

### When to use `.blend` / 什么时候用 `.blend`

中文：`.blend` 适合快速迭代和个人开发阶段，因为 Godot 会自动调用 Blender 导出。但它更像一种便利桥接，而不是最终团队管线。为了更稳定和可复用，建议后期固定成显式导出的 `.glb` 或 `.gltf`。

English: `.blend` is convenient during fast iteration and solo development because Godot will call Blender automatically. However, it is more of a convenience bridge than a final team pipeline. For stability and reuse, it is better to standardize on explicitly exported `.glb` or `.gltf` later.

### When to avoid FBX / 什么时候尽量避免 FBX

中文：除非你必须兼容某个外部工具链，否则不要优先选 FBX。Godot 官方对 glTF 的支持更直接，材质、动画和骨架路径也更稳定。

English: Unless you must integrate with an external toolchain that requires FBX, do not choose it first. Godot's support for glTF is more direct, and material, animation, and skeleton paths are usually more stable.

## Coordinate and Facing Rules / 坐标与朝向规则

中文：Godot 使用右手系、Y 轴向上、相机前方向为 `-Z`。而面向型 3D 资产的约定是“资产正面朝 `+Z`”。Godot 官方还特别指出，在 Blender 中这意味着角色资产一般应以 `-Y` 为正面。

English: Godot uses a right-handed system with Y-up and camera forward along `-Z`. For oriented 3D assets, the convention is that the asset's front points toward `+Z`. Godot specifically notes that in Blender this means a character asset generally faces `-Y`.

## Pre-Export Checklist / 导出前清单

- mesh faces forward according to Godot conventions / 模型朝向符合 Godot 约定
- object transforms are applied / 对象变换已应用
- skeleton is in rest pose or export pose / 骨架已回到默认姿态
- mesh is not deformed into a transient pose before export / 导出前网格不处于临时扭曲姿态
- quads and n-gons are triangulated, preferably in Blender / 四边面和 N 边面最好在 Blender 内先三角化
- naming follows the project naming contract / 命名符合项目契约
- only the intended export collection or objects are included / 只包含需要导出的对象

## Blender glTF Export Settings / Blender glTF 导出设置

### Recommended base settings / 推荐基础设置

Menu: `File > Export > glTF 2.0`

#### Format / 格式

- preferred: `glTF Binary (.glb)` / 默认推荐：`glTF Binary (.glb)`
- alternative: `glTF Separate (.gltf + .bin + textures)` / 备选：`glTF Separate`

#### Include / 包含

- `Selected Objects`: On when exporting specific assets / 导出单个资产时打开
- `Visible Objects`: On if your scene is disciplined / 场景管理规范时可打开
- `Renderable Objects`: On / 建议打开
- `Active Collection`: On if using export collections / 使用导出集合时建议打开
- `Custom Properties`: On only if you intentionally use extras / 只有明确要导出 extras 时才打开
- `Cameras`: usually Off for characters and weapons / 角色和武器通常关闭
- `Punctual Lights`: usually Off for gameplay assets / 玩法资产通常关闭

#### Transform / 变换

- `Y Up`: On / 打开

#### Geometry / 几何

- `Apply Modifiers`: On, except armature behavior remains special / 打开
- `UVs`: On / 打开
- `Normals`: On / 打开
- `Tangents`: On if you rely on normal maps / 使用法线贴图时打开
- `Vertex Colors`: On only if actually used / 需要时再打开
- `Materials`: Export / 导出材质

#### Animation / 动画

中文：Blender 官方 glTF 文档指出，为了确保动画被导出，应该让动作成为活动 Action，或者创建单条 NLA strip，或者将 Action stash。也就是说，如果你计划一个文件包含多段动作，请务必把每段动画整理成明确的 Action/NLA 片段。

English: Blender's official glTF exporter docs state that to ensure an animation is exported, it should either be the active Action, a single-strip NLA track, or a stashed Action. If you want multiple clips in one file, each clip must be organized as a clear Action or NLA strip.

Recommended:

- `Use Current Frame`: Off for normal export / 普通导出关闭
- `Limit to Playback Range`: On if timeline is disciplined / 时间轴管理规范时可打开
- `Sampling`: On for safety if constraints are involved / 如果有约束，建议采样
- `Always Sample Animations`: On when using Rigify or complex constraints / Rigify 或复杂约束时建议打开
- `NLA Strips`: On when exporting multiple clips via NLA / 通过 NLA 导出多动画时打开
- `Actions`: On if exporting multiple stored actions / 需要导出多个 action 时打开
- `Skinning`: On / 打开
- `Shape Keys`: On only when needed / 需要表情或形变时再打开

## Recommended Export Strategy by Asset Type / 按资产类型的导出建议

### Character / 角色

- export one clean game rig / 导出一套干净 game rig
- export sockets as helper bones or nodes / 导出 socket 辅助骨或辅助节点
- export locomotion and combat clips either in one library file or separate clip files / 移动和战斗动作可以做成动画库文件或拆分文件

### Weapon / 武器

- export with `socket_muzzle` and optional `socket_magazine` / 导出 `socket_muzzle` 和可选 `socket_magazine`
- keep origin and grip alignment stable / 原点与握持对齐要稳定

### Vehicle / 载具

- export chassis and turret pivots cleanly / 底盘和炮塔枢轴保持干净
- export driver and gunner camera anchors / 导出驾驶和炮手相机锚点

### Aircraft / 飞机

- export body root and control-surface pivots / 导出机体根与控制面枢轴
- keep weapon pylons and camera anchors explicit / 明确挂架与相机锚点

## Godot Import Notes / Godot 导入说明

### Import formats / 导入格式

- Godot supports glTF and `.blend`, but glTF is recommended / Godot 支持 glTF 和 `.blend`，但官方推荐 glTF
- `.blend` import depends on Blender being installed / `.blend` 导入依赖本机安装 Blender

### Advanced import / 高级导入

中文：Godot 的 Advanced Import Settings 可以预览场景、材质和动画，并且允许把动画保存为单独文件，还能把一条 `default` 时间线切成多个 slices。这对于后续建立动画库很有帮助。

English: Godot's Advanced Import Settings can preview scenes, materials, and animations, allow animations to be saved as files, and slice a single `default` timeline into multiple animations. This is useful for later animation-library workflows.

## Common Mistakes / 常见错误

- exporting the control rig instead of the clean export rig / 导出了控制骨架而不是干净 game rig
- forgetting to apply object transforms / 忘记应用对象变换
- exporting from a non-rest pose / 在非默认姿态下导出
- inconsistent clip naming / 动画命名不一致
- leaving triangulation to chance / 把三角化完全留给导入器
- relying on imported lights for final look / 依赖导入灯光作为最终灯光

## Recommended Team Policy / 建议的团队策略

- author in Blender with any rig you want / Blender 内部制作绑定可自由选择
- export only a clean game rig / 只导出干净 game rig
- prefer `.glb` for gameplay assets / 玩法资产优先用 `.glb`
- store exporter settings in the blend file / 将导出设置保存在 blend 文件中
- keep one export checklist for every asset type / 每种资产类型配一份导出清单

## Official References / 官方参考

- Godot: Available/imported 3D formats and scene import overview
  https://docs.godotengine.org/en/stable/tutorials/assets_pipeline/importing_3d_scenes/index.html
- Godot: Model export considerations
  https://docs.godotengine.org/en/stable/tutorials/assets_pipeline/importing_3d_scenes/model_export_considerations.html
- Godot: Advanced Import Settings
  https://docs.godotengine.org/en/stable/tutorials/assets_pipeline/importing_3d_scenes/advanced_import_settings.html
- Godot: Retargeting 3D Skeletons
  https://docs.godotengine.org/en/4.3/tutorials/assets_pipeline/retargeting_3d_skeletons.html
- Blender Manual: glTF 2.0 exporter
  https://docs.blender.org/manual/en/5.0/addons/import_export/scene_gltf2.html
