# Asset Naming Convention / 资产命名规范

## Goal / 目标

中文：这份规范的目标是让 Blender、导出文件、Godot 场景、动画树、武器资源和后续代码使用同一套命名体系。这样可以减少绑定错误、socket 失配、动画 key 混乱，以及多人协作时的接口漂移。

English: The goal of this convention is to keep Blender rigs, exported files, Godot scenes, animation trees, weapon resources, and gameplay code on the same naming system. This reduces binding errors, socket mismatches, animation-key drift, and integration problems across collaborators.

## General Rules / 通用规则

- use lowercase snake_case for sockets, animation clips, exported helper nodes, and gameplay identifiers / socket、动画片段、导出辅助节点、玩法标识统一用小写 snake_case
- for humanoid export bones, prefer Godot humanoid-compatible English names in PascalCase / 人形导出骨骼优先使用与 Godot humanoid 兼容的英文 PascalCase 名称
- use suffix `_l` and `_r` for left and right on sockets and helper nodes / socket 与辅助节点的左右统一使用 `_l` 与 `_r`
- keep prefixes semantic and stable, not tool-specific / 前缀按语义命名，不按软件临时习惯命名
- avoid spaces and localized names inside exported assets / 导出资产内部避免空格和中文命名
- one concept, one canonical name / 一个概念只保留一个标准名

## Character Bone Standard / 人物骨骼标准

中文：如果你希望利用 Godot 的 `SkeletonProfileHumanoid` 和 `BoneMap`，导出游戏骨架最好直接贴近它的人形标准名。

English: If you want to benefit from Godot's `SkeletonProfileHumanoid` and `BoneMap`, the exported game skeleton should stay close to its humanoid naming standard.

### Required base bones / 必备基础骨骼

- `Root`
- `Hips`
- `Spine`
- `Chest`
- `UpperChest`
- `Neck`
- `Head`
- `LeftShoulder`
- `LeftUpperArm`
- `LeftLowerArm`
- `LeftHand`
- `RightShoulder`
- `RightUpperArm`
- `RightLowerArm`
- `RightHand`
- `LeftUpperLeg`
- `LeftLowerLeg`
- `LeftFoot`
- `LeftToes`
- `RightUpperLeg`
- `RightLowerLeg`
- `RightFoot`
- `RightToes`

### Optional helper bones / 可选辅助骨骼

- `ik_hand_gun`
- `ik_hand_l`
- `ik_hand_r`
- `ik_foot_l`
- `ik_foot_r`
- `aim_chest`
- `weapon_attach`

## Socket Standard / Socket 标准

中文：socket 可以是独立 helper bone，也可以是导出到场景里的 `Marker3D` 或 `Node3D`。无论使用哪种形式，名字都要统一。

English: Sockets can be helper bones or exported `Marker3D` or `Node3D` nodes. Regardless of representation, the names must remain consistent.

### Character sockets / 人物 socket

- `socket_hand_r`
- `socket_hand_l`
- `socket_muzzle`
- `socket_magazine`
- `socket_camera`
- `socket_aim`
- `socket_interact`
- `socket_foot_ik_l`
- `socket_foot_ik_r`

### Vehicle sockets / 载具 socket

- `socket_camera_driver`
- `socket_camera_gunner`
- `socket_camera_commander`
- `socket_turret_yaw`
- `socket_barrel_pitch`
- `socket_barrel_muzzle`

### Aircraft sockets / 飞机 socket

- `socket_camera_cockpit`
- `socket_camera_chase`
- `socket_pylon_l`
- `socket_pylon_r`
- `socket_gun_muzzle`

## Animation Key Standard / 动画 Key 标准

### Locomotion / 移动

- `locomotion_idle_unarmed`
- `locomotion_idle_rifle`
- `locomotion_walk_fwd`
- `locomotion_walk_bwd`
- `locomotion_walk_left`
- `locomotion_walk_right`
- `locomotion_run_fwd`
- `locomotion_crouch_idle`
- `locomotion_jump_start`
- `locomotion_jump_loop`
- `locomotion_land`
- `locomotion_turn_l_90`
- `locomotion_turn_r_90`

### Equipment / 装备

- `equip_default`
- `equip_rifle`
- `unequip_default`
- `unequip_rifle`

### Combat / 战斗

- `combat_fire_rifle`
- `combat_reload_rifle`
- `combat_fire_pistol`
- `combat_reload_pistol`
- `combat_melee_light`

### Override / 覆盖层

- `override_social_wave`
- `override_social_salute`
- `override_hit_front`
- `override_execute`
- `override_enter_vehicle`
- `override_exit_vehicle`

## Authoring Rig vs Export Rig / 制作骨架与导出骨架

中文：Rigify 和 Auto-Rig Pro 的控制骨名称不需要强行改成这里的标准。真正必须遵循本规范的是导出到 Godot 的 game rig，而不是 Blender 内部的控制器层。

English: You do not need to force Rigify or Auto-Rig Pro control bones to follow this convention. The mandatory target is the exported game rig for Godot, not the internal controller layer inside Blender.

## Scene and File Naming / 场景与文件命名

- character scene: `PlayerActor.tscn`, `EnemySoldier.tscn`
- weapon resource: `DebugRifle.tres`, `AssaultRifle_Mk1.tres`
- animation library or controller scene: `HumanAnimationRig.tscn`
- blender file: `chr_soldier_a.blend`, `wpn_rifle_debug.blend`, `veh_tank_light_a.blend`

- 角色场景：`PlayerActor.tscn`、`EnemySoldier.tscn`
- 武器资源：`DebugRifle.tres`、`AssaultRifle_Mk1.tres`
- 动画控制相关场景：`HumanAnimationRig.tscn`
- Blender 文件：`chr_soldier_a.blend`、`wpn_rifle_debug.blend`、`veh_tank_light_a.blend`

## Code Contract / 代码契约

中文：当前代码里的标准名常量已经集中在 [AssetNaming.cs](e:/Gogot/road-armed/Game/Shared/AssetNaming.cs)。如果你要改规范，应该优先改这个文件，再同步 Blender 和资源。

English: The current canonical constants live in [AssetNaming.cs](e:/Gogot/road-armed/Game/Shared/AssetNaming.cs). If the convention changes, update that file first, then sync Blender and resources.

## Validation / 校验

中文：当前提供了一个基础校验器 [RigContractValidator.cs](e:/Gogot/road-armed/Game/Character/Components/RigContractValidator.cs)，可以在后续接入 `Skeleton3D` 后检查关键骨骼和 socket 是否齐全。

English: A basic validator is now available at [RigContractValidator.cs](e:/Gogot/road-armed/Game/Character/Components/RigContractValidator.cs). Once a `Skeleton3D` is connected, it can verify required bones and sockets.

## Recommendation / 建议

中文：你现在在 Blender 里最该锁定的是导出骨架、socket 名和动画 key。对于人形角色，如果后续想更方便地走 Godot retargeting，导出骨骼名最好直接贴近 `SkeletonProfileHumanoid`。

English: What you should lock down now in Blender is the exported skeleton, socket names, and animation keys. For humanoid characters, if you want smoother Godot retargeting later, export bone names should stay close to `SkeletonProfileHumanoid`.
