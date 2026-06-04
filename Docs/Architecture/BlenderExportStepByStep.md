# Blender Export Step By Step / Blender 导出逐步操作说明

## Scope / 适用范围

中文：这份文档是给你实际点按钮用的，不是讲原理。默认目标是把一个角色从 Blender 导出到 Godot，并尽量适配当前项目的控制器和动画接口。

English: This document is meant for actual button-by-button execution, not theory. The default goal is to export a character from Blender to Godot and keep it compatible with the current controller and animation pipeline.

## Before You Export / 导出前准备

1. 确认你现在操作的是 export game rig，而不是 Rigify 控制骨本体。
2. 确认模型、骨架、socket 名符合项目规范。
3. 确认动画已经整理成清晰的 Action 或 NLA strip。
4. 确认角色在 Blender 中前向正确，默认推荐角色面朝 `-Y`。
5. 确认对象变换已应用：`Ctrl + A > Rotation & Scale`。
6. 确认骨架处于默认姿态或导出姿态。

## Export Characters to `.glb` / 导出角色到 `.glb`

1. 在 Outliner 里只选择你要导出的对象。
2. 点击 `File > Export > glTF 2.0`。
3. 右侧 `Format` 选择 `glTF Binary (.glb)`。
4. `Include > Selected Objects` 打开。
5. 如果你使用单独导出集合，`Include > Active Collection` 可打开。
6. `Transform > Y Up` 保持打开。
7. `Geometry > Apply Modifiers` 打开。
8. `Geometry > UVs`、`Normals` 打开。
9. 如果你依赖法线贴图，`Tangents` 打开。
10. `Animation > Skinning` 打开。
11. 如果使用 Rigify、约束或复杂控制器，`Animation > Always Sample Animations` 打开。
12. 如果你通过 NLA 管理多段动画，`Animation > NLA Strips` 打开。
13. 如果你通过多个 Action 导出动画，`Animation > Actions` 打开。
14. 选择导出路径，例如 `BlenderAsset/export/jj.glb`。
15. 点击 `Export glTF 2.0`。

## Export Weapons to `.glb` / 导出武器到 `.glb`

1. 只选择武器模型和必要 helper。
2. 确认包含 `socket_muzzle`，如果换弹动画要更准确，再加入 `socket_magazine`。
3. 保持武器原点与握持方向稳定。
4. 使用和角色相同的 glTF 导出设置。

## Organizing Multiple Animations / 多动画整理方式

### Option A: Actions / 方案 A：Action

1. 在 Action Editor 中给每段动画一个标准名。
2. 确保要导出的动画是活动 Action，或被正确 stash。
3. 导出时打开 `Actions`。

### Option B: NLA / 方案 B：NLA

1. 将每段动作推送到 NLA。
2. 每段动作单独一条 strip。
3. strip 命名与项目动画 key 保持一致或可对应。
4. 导出时打开 `NLA Strips`。

## Import into Godot / 导入到 Godot

1. 将 `.glb` 放入项目目录，例如 `BlenderAsset/export/`。
2. 打开 Godot 编辑器，等待自动导入完成。
3. 检查是否生成 `.import` 文件和 `.godot/imported/*.scn`。
4. 在 Godot 中打开对应场景或实例化它。
5. 用项目里的 `RigContractValidator` 检查关键骨骼和 socket。

## How to Use the New Template / 如何使用这次新增的模板

1. 打开 [JjGameplayCharacter.tscn](e:/Gogot/road-armed/Scenes/Characters/JjGameplayCharacter.tscn)。
2. 这个场景会把你的 [jj.glb](e:/Gogot/road-armed/BlenderAsset/export/jj.glb) 作为模型实例挂到现有控制器下。
3. `RigValidator` 会尝试自动搜索 `Skeleton3D`。
4. `EquipmentSockets` 会递归搜索 `socket_hand_r`、`socket_hand_l`、`socket_muzzle`、`socket_camera`。
5. 如果缺失，会在 Godot 输出警告。
6. 调试运行场景可使用 [JjControllerPlayground.tscn](e:/Gogot/road-armed/Scenes/Sandbox/JjControllerPlayground.tscn)。

## What Is Good Enough Right Now / 现在做到什么程度算够用

中文：对于当前阶段，你的 `jj.glb` 已经足够进入下一步集成，因为你已经有 idle、walk、run、turn、fire、reload 以及 `socket_hand_r`、`socket_hand_l`。这足够验证控制器、基础动画状态和装备逻辑。

English: For the current stage, your `jj.glb` is already good enough to move into the next integration step, because it has idle, walk, run, turn, fire, reload, and the `socket_hand_r` and `socket_hand_l` sockets. That is enough to validate the controller, baseline animation states, and equipment logic.

## What You Should Add Next / 你接下来该补什么

- `socket_muzzle` for firing and weapon traces / 用于开火和弹道的 `socket_muzzle`
- `socket_camera` or `socket_aim` for better camera and aim anchoring / 用于相机和瞄准的 `socket_camera` 或 `socket_aim`
- left and right turn variants if current turn clip is too generic / 如果当前转身太泛，补左右转变体
- jump and land clips / 起跳与落地动画
- strafe or aim locomotion clips / 横移或瞄准移动动画
- equip and unequip clips / 装备与收起动画

## Common Troubleshooting / 常见排错

- animation missing after import: check `Actions` or `NLA Strips` / 动画没进来：检查 `Actions` 或 `NLA Strips`
- skeleton mismatch: check export rig names / 骨架不匹配：检查导出骨架命名
- socket not found: check exact node or helper name / 找不到 socket：检查精确名称
- mesh twisted: check rest pose and applied transforms / 网格扭曲：检查默认姿态与应用变换
