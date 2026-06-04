# Socket Guide / Socket 说明

## What Is a Socket? / 什么是 Socket

中文：Socket 本质上是一个“约定好的空间锚点”。它通常是骨骼、辅助骨、`Marker3D` 或 `Node3D`，用来告诉代码和动画系统：某个功能应该附着、发射、观察或对齐到哪里。

English: A socket is a predefined spatial anchor. It is usually a bone, helper bone, `Marker3D`, or `Node3D` that tells gameplay and animation systems where something should attach, fire from, look from, or align to.

## Character Sockets / 人物 Socket

### `socket_hand_r`

中文：这是人物右手握持挂点。它属于人物，不属于枪。武器模型通常会被挂到这里，或者至少以这里为对齐基准。

English: This is the right-hand grip socket on the character. It belongs to the character, not the weapon. Weapons are typically attached here, or at least aligned against it.

Typical use / 典型用途:
- attach rifle or pistol model / 挂接枪械模型
- align equipped item pose / 对齐装备姿态
- support upper-body hold states / 支撑持枪上半身状态

### `socket_hand_l`

中文：这是人物左手辅助挂点。它通常用于双手持枪时的左手目标参考，也可作为 IK 目标或对齐辅助。

English: This is the left-hand support socket on the character. It is typically used as a reference for two-hand weapon support, either as an IK target or as an alignment helper.

Typical use / 典型用途:
- left-hand IK support / 左手 IK 支撑
- two-hand aiming stabilization / 双手持枪稳定

### `socket_camera`

中文：这是人物相机参考点，属于人物。它一般放在胸口上方、肩线附近、头后或头部附近，但不要直接放到眼球中心，否则第三人称视角会太不稳定。当前项目里相机会优先跟随这个点；如果没有，则退回到角色根节点位置，所以你现在看到相机贴地，就是因为这个点缺失。

English: This is the camera anchor on the character. It usually sits around the upper chest, shoulder line, back of the head, or near the head, but not exactly inside the eyeballs, otherwise third-person camera motion becomes unstable. The current project now prefers to follow this point; if it is missing, it falls back to the actor root, which is why your camera ends up too low.

Recommended position / 推荐位置:
- near upper chest or collarbone height / 接近上胸或锁骨高度
- slightly behind the face plane / 略微位于面部平面之后
- stable during locomotion / 移动中尽量稳定

### `socket_aim`

中文：这是人物瞄准参考点，属于人物。它通常用于把准星、枪口方向、上半身朝向或瞄准 IK 对齐到人物正前方的某个“权威朝向参考”。如果只有一个瞄准参考点，它一般放在胸口前方或头前方。

English: This is the aim reference anchor on the character. It is commonly used to align reticles, muzzle direction, upper-body facing, or aim IK to a stable forward reference on the character. If you only use one aim helper, place it slightly in front of the chest or head.

## Weapon Sockets / 武器 Socket

### `socket_muzzle`

中文：这是枪械上的枪口 socket，属于武器，不属于人物。它应该放在真正的枪口位置，用来决定子弹、射线、枪口火焰、弹道轨迹从哪里发射。

English: This is the muzzle socket on the weapon, not the character. It should sit at the real muzzle position and is used for bullets, hitscan traces, muzzle flashes, and projectile spawn orientation.

Typical use / 典型用途:
- hitscan origin / 射线起点
- projectile spawn / 投射物生成点
- muzzle flash / 枪口火焰
- recoil reference / 后坐参考点

Recommended position / 推荐位置:
- exactly at the barrel exit / 尽量在枪管出口
- forward axis aligned with barrel direction / 前向轴对准枪管方向

### `socket_magazine`

中文：这是武器上的换弹参考点。它通常用于手与弹匣的对位、换弹特效或拆装动画参考。

English: This is the reload reference on the weapon. It is often used for hand-magazine alignment, reload effects, or disassembly animation references.

## Who Owns What? / 谁拥有哪个 Socket

- character owns: `socket_hand_r`, `socket_hand_l`, `socket_camera`, `socket_aim`, `socket_interact`
- weapon owns: `socket_muzzle`, `socket_magazine`
- vehicle owns: `socket_camera_driver`, `socket_turret_yaw`, `socket_barrel_muzzle`
- aircraft owns: `socket_camera_cockpit`, `socket_pylon_l`, `socket_gun_muzzle`

- 人物拥有：`socket_hand_r`、`socket_hand_l`、`socket_camera`、`socket_aim`、`socket_interact`
- 武器拥有：`socket_muzzle`、`socket_magazine`
- 载具拥有：`socket_camera_driver`、`socket_turret_yaw`、`socket_barrel_muzzle`
- 飞机拥有：`socket_camera_cockpit`、`socket_pylon_l`、`socket_gun_muzzle`

## What You Need Right Now / 你当前最需要补什么

中文：对你现在的 `jj` 人物模型来说，下一步最值得补的是 `socket_camera`。它能直接改善第三人称相机位置。之后再补武器上的 `socket_muzzle`，这样射击和枪口火焰链路才能成立。

English: For your current `jj` character model, the most important next socket is `socket_camera`. It will immediately improve third-person camera placement. After that, add `socket_muzzle` on the weapon so firing and muzzle effects can work correctly.


## Bone Socket Support / 骨骼 Socket 支持

中文：当前项目的 socket 系统已经同时支持两种形式：
- 作为导出的 Node3D / 空物体节点
- 作为导入到 Skeleton3D 里的骨骼名

如果找不到同名节点，系统会尝试在骨架中查找名称包含 socket_camera、socket_hand_r、socket_muzzle 等关键字的骨骼，并自动通过 BoneAttachment3D 生成可用锚点。

English: The current socket system supports both exported Node3D/empty helper nodes and bone names imported into Skeleton3D. If a matching node is not found, the system will search the skeleton for bone names containing keys such as socket_camera, socket_hand_r, or socket_muzzle, then generate a usable anchor through BoneAttachment3D.

