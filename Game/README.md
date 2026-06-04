# Game Folder Guide / 游戏目录说明

中文：这个目录承载新的 Godot 原生玩法代码，用来替代当前已经废弃的 `BBBN` 原型。目标是让输入、相机、角色、战斗、交互、调试都拥有清晰边界，并且方便在后续接入载具、飞机和 AI。

English: This folder contains the new Godot-native gameplay code that replaces the discarded `BBBN` prototype. The goal is to keep input, camera, character, combat, interaction, and debug systems clearly separated and ready for later vehicle, aircraft, and AI integration.

- `Bootstrap`: startup scenes and wiring / 启动场景与装配
- `Camera`: camera rigs and transitions / 相机 rig 与模式切换
- `Character/Core`: runtime context and character enums / 运行时上下文与角色状态枚举
- `Character/Motion`: motion models and locomotion routing / 运动模型与移动路由
- `Character/Processing`: input-to-intent translation / 输入到意图的翻译
- `Character/Actions`: character-side action gating / 角色侧动作仲裁
- `Character/Animation`: animation drivers and override layer / 动画驱动与覆盖层
- `Character/Equipment`: inventory, sockets, weapon visuals, combat glue / 物品栏、挂点、武器可视化、战斗胶水层
- `Character/Debug`: validation and asset diagnostics / 校验与资产诊断
- `Character/Nodes`: actor root nodes / 角色根节点
- `Character/Resources`: tunable settings / 可调参数资源
- `Combat`: weapon and item definitions / 武器与物品定义
- `Input`: player, AI, replay, and shared input contracts / 玩家、AI、回放与共享输入协议
- `Interaction`: world interaction and command picking / 世界交互与指令拾取
- `Shared`: reusable common code / 共享公共代码
- `UI/Debug`: runtime debug overlays / 运行时调试界面
- `World`: environment-side systems / 场景环境侧系统
