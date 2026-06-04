# Character Scene Integration / 人物场景集成方案

## Recommended Structure / 推荐结构

中文：对于可调试、可重导入、可维护的人物场景，推荐使用下面这种结构：

- 角色控制器场景自己持有 `CharacterBody3D`、碰撞、输入、战斗、动画驱动。
- 导入的 `jj.glb` 作为 `VisualRoot` 下的直接子场景实例存在。
- `AnimationTree`、`AnimationPlayer` 的引用由角色场景去搜索导入模型中的节点。

English: For a debuggable, reimport-safe, and maintainable character scene, use this structure:

- The character controller scene owns `CharacterBody3D`, collision, input, combat, and animation driver nodes.
- The imported `jj.glb` exists as a direct child scene instance under `VisualRoot`.
- `AnimationTree` and `AnimationPlayer` references are resolved by the character scene from nodes inside the imported model scene.

## Why This Is Better / 为什么这更好

- the model is visible in the editor / 模型在编辑器中可见
- imported scene remains reimportable / 导入场景仍可安全重导入
- gameplay nodes stay outside the imported asset / 玩法节点不污染导入资产
- AnimationTree can still live in the gameplay scene / AnimationTree 仍然可以放在玩法场景里

## Official Basis / 官方依据

中文：Godot 官方在 `Using AnimationTree` 文档里明确说明，导入的 3D 场景会带有 `AnimationPlayer`，而实际游戏中通常不会直接使用导入场景本身，而是创建一个新的角色场景，把导入场景实例化进去，并在新场景里放置 `AnimationTree`。

English: In the `Using AnimationTree` documentation, Godot explicitly explains that imported 3D scenes contain the `AnimationPlayer`, and in actual gameplay you usually don't use the imported scene directly. Instead, you create a new character scene, instance the imported scene inside it, and place the `AnimationTree` in the new scene.

Source / 来源:
https://docs.godotengine.org/en/stable/tutorials/animation/animation_tree.html

