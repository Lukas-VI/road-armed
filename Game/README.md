# Game Folder Guide / 游戏目录说明

中文：这个目录承载新的 Godot 原生玩法代码，用来替代当前已经废弃的 `BBBN` 原型。目标是让输入、相机、角色、战斗、交互、调试都拥有清晰边界，并且方便在后续接入载具、飞机和 AI。

English: This folder contains the new Godot-native gameplay code that replaces the discarded `BBBN` prototype. The goal is to keep input, camera, character, combat, interaction, and debug systems clearly separated and ready for later vehicle, aircraft, and AI integration.

- `Bootstrap`: startup scenes and wiring / 启动场景与装配
- `Camera`: camera rigs and transitions / 相机 rig 与模式切换
- `Character`: actor logic, runtime data, motion models / 角色逻辑、运行时数据、运动模型
- `Combat`: weapons and firing systems / 武器与射击系统
- `Input`: player, AI, replay, and shared input contracts / 玩家、AI、回放与共享输入协议
- `Interaction`: world interaction and command picking / 世界交互与指令拾取
- `Shared`: reusable common code / 共享公共代码
- `UI/Debug`: runtime debug overlays / 运行时调试界面
- `World`: environment-side systems / 场景环境侧系统
