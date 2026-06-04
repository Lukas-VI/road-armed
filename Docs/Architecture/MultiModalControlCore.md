# Multi-Modal Control Core / 多操作模式控制核心

## How Input Decoupling Works in `BBB-Nexus` / `BBB-Nexus` 中的输入解耦是怎么做的

中文：参考项目的关键并不是“输入类和控制器分开”这么简单，而是把输入拆成了多层。`InputSourceBase` 面向硬件，负责采样键鼠或 AI。`InputPipeline` 把原始输入整理成一帧快照。`MainProcessorPipeline` 再把快照翻译成角色意图和参数，写入 `RuntimeData`。最后，状态机、动作层、动画层、IK 层都只读取 `RuntimeData`，而不是直接读取硬件。

English: The important part is not merely separating an input class from the controller. The reference project uses multiple layers. `InputSourceBase` reads hardware or AI. `InputPipeline` converts raw input into a frame snapshot. `MainProcessorPipeline` translates that snapshot into intentions and parameters stored in `RuntimeData`. State machines, action layers, animation, and IK then consume `RuntimeData` instead of touching hardware directly.

## Three Reference Features Worth Preserving / 三个值得保留的参考特性

### 1. Authored movement clips / 动画驱动的起步与转身

中文：参考项目不仅依赖普通循环动画，还会从动画中烘焙位移和旋转数据，用于八向起步、刹停、转身、闪避和翻越。这类内容值得保留，但不应该在当前 MVP 里立刻全面启用。更合理的路线是：先用代码权威位移把基础手感和状态接口做稳定，再为起步/刹停/原地转身引入“动画授权位移”层，或者局部 root motion/warp 机制。

English: The reference project does more than loop locomotion clips. It bakes displacement and rotation from clips for eight-way starts, stops, turns, dodges, and vaults. This is worth preserving, but it should not be turned on globally in the current MVP. The better path is to stabilize code-authoritative locomotion first, then add an animation-authored motion layer or local root-motion or warp support for starts, stops, and turn-in-place clips.

### 2. Global override layer / 全局最高覆盖层

中文：这个特性很有必要保留。社交动作、处决、受击硬直、上下车、过场强制动作，都适合走一条高优先级覆盖层，而不是塞进普通 locomotion 或 upper-body 层。当前项目已经加入了 `AnimationOverrideCoordinator` 骨架，用于承载这种最高优先级的动作请求。

English: This feature should be preserved. Social emotes, executions, hard hit reactions, enter or exit vehicle clips, and scripted moments all fit a high-priority override layer better than the normal locomotion or upper-body layers. The current project now includes an `AnimationOverrideCoordinator` skeleton for this purpose.

### 3. Inventory and hotbar / 物品栏与快捷栏

中文：这个思路也值得保留，因为装备状态本身就是动画、战斗、IK、交互的上游条件。当前项目已经加入 `InventoryComponent` 和基础 `WeaponDefinition` 资源，后面可以自然扩展到背包、堆叠、换武器、消耗品和装备驱动模型实例化。

English: This should also be preserved because the equipped state is an upstream condition for animation, combat, IK, and interaction. The current project now includes `InventoryComponent` and a base `WeaponDefinition` resource, which can later expand into a full inventory, stacking, weapon swap, consumables, and driven equipment instantiation.

## Why That Is Valuable / 这套设计为什么有价值

- player, AI, replay, and networking can share the same downstream gameplay code
- state logic becomes deterministic and easier to debug
- camera and locomotion can consume the same intent from different sources
- the system can add preprocessors without rewriting the controller root

- 玩家、AI、回放、联机都能共享同一套下游玩法逻辑
- 状态逻辑更可预测，也更容易调试
- 相机和移动可以消费同一份意图，而不关心来源
- 可以不断增加预处理器，而不必重写控制器根节点

## Why We Should Still Change It for Godot / 为什么到了 Godot 仍然要重做

中文：Unity 版本高度依赖 Unity 的生命周期、组件引用方式，以及它自己的表现层约束。Godot 更适合把运行时所有权做得更显式：输入事件在 `_UnhandledInput` 里积累，权威移动在 `_PhysicsProcess` 里执行，相机用节点层级与 `SpringArm3D` 解决遮挡，配置数据用 `Resource` 管理，而不是把所有事情都塞进一个大 MonoBehaviour。

English: The Unity version is strongly shaped by Unity lifecycle rules, component references, and presentation constraints. In Godot we can make runtime ownership more explicit: collect event deltas in `_UnhandledInput`, run authoritative movement in `_PhysicsProcess`, solve camera obstruction with node hierarchy and `SpringArm3D`, and store tuning data in `Resource` assets instead of stuffing everything into one large `MonoBehaviour`.

## The Godot Version in This Project / 本项目中的 Godot 版本实现

### 1. Control sources / 控制源

中文：`ControlSourceNode` 是统一入口。`PlayerInputRouter` 从输入映射采样玩家输入，`AiControlSource` 则允许 AI、回放、网络同步直接构造同样的输入帧。`ControlSourceRouter` 让角色在多个输入源之间切换，而不需要重写角色根节点。

English: `ControlSourceNode` is the unified entry point. `PlayerInputRouter` samples the player from Godot input actions, while `AiControlSource` lets AI, replay, or network sync feed the exact same frame contract. `ControlSourceRouter` lets the actor switch between multiple sources without changing the actor root.

### 2. Shared control contract / 共享控制协议

中文：`ControlFrame` 不是“角色专用输入”，而是更广义的控制协议。它不仅有移动、视角、跳跃、射击，还预留了油门、刹车、角速度、快捷栏选择和社交动作触发，这就是为载具、飞机和覆盖动画留出来的接口。

English: `ControlFrame` is not a character-only input object. It is a broader control contract. Besides movement, look, jump, and fire, it already reserves throttle, brake, angular input, hotbar selection, and social-action triggers for vehicles, aircraft, and animation overrides.

### 3. Intent translation / 意图翻译

中文：`PlayerIntentProcessor` 根据当前控制模式和相机朝向，把 `ControlFrame` 翻译成 `ActorIntent`。也就是说，原始输入先被模式化，再被世界化，然后才交给动作、战斗和运动层。

English: `PlayerIntentProcessor` translates `ControlFrame` into `ActorIntent` using the active control mode and the camera basis. Raw input is first interpreted by mode, then transformed into world-space intent, and only then passed to action, combat, and movement.

### 4. Runtime blackboard / 运行时黑板

中文：`CharacterRuntimeContext` 承担了黑板职责。相机、输入、动作、战斗、动画和运动模型都可以把结果写入这里，调试覆盖层也只读这里。这是整个系统最关键的共享边界。

English: `CharacterRuntimeContext` is the runtime blackboard. Camera, input, action, combat, animation, and motion layers can write results here, and the debug overlay reads only from this context. This is the most important shared boundary in the system.

### 5. Motion models / 运动模型

中文：`IActorMotionModel` 是为多模式准备的接口，当前已有 `CharacterMotor` 作为第一种实现。以后可以继续加入 `GroundVehicleMotor`、`AircraftMotor`、`TurretMotor`，甚至 `SpectatorMotor`，而不需要推翻输入层。

English: `IActorMotionModel` is the extension seam for multiple modes. `CharacterMotor` is the first implementation. Later we can add `GroundVehicleMotor`, `AircraftMotor`, `TurretMotor`, or even `SpectatorMotor` without replacing the input layer.

### 6. Combat, inventory, and override / 战斗、物品栏与覆盖层

中文：`InventoryComponent` 负责快捷栏装备状态，`CombatCoordinator` 负责武器可开火、换弹、射击冷却和动画 key，`AnimationOverrideCoordinator` 负责社交动作与后续的高优先级强制动作。它们都不直接控制位移，而是只改运行时上下文。

English: `InventoryComponent` handles hotbar equipment state, `CombatCoordinator` handles weapon fire validity, reload, fire cooldown, and animation keys, and `AnimationOverrideCoordinator` handles social emotes and future high-priority forced actions. None of them moves the actor directly; they only update the runtime context.

## How AI Connects In / AI 如何接入

中文：AI 不应该绕过控制核心直接改角色位置。更合理的做法是让 AI 也实现一个“控制源”。它可以是行为树、GOAP、Utility AI、导航系统或者战术脚本，但最终都写入 `ControlFrame`，与玩家走同一条后处理链。这样 AI 和玩家就天然共享同一套移动限制、动作门槛、瞄准逻辑和状态规则。

English: AI should not bypass the control core by writing transform data directly. A better design is to let AI be another control source. Whether it comes from a behavior tree, GOAP, utility AI, navigation, or tactical scripting, it should write into `ControlFrame` and then pass through the same downstream processors. That gives AI and players the same movement limits, action gating, aiming logic, and state rules.

## How Vehicle and Aircraft Modes Can Grow / 载具与飞机模式如何扩展

### Ground vehicles / 地面载具

中文：地面载具更适合使用“油门 + 转向 + 炮塔/镜头分离”的控制模型。`ControlFrame` 里已经预留了 `Throttle`、`Brake` 和 `AngularInput`。未来可以让底盘运动、炮塔旋转、炮镜视角成为三个相互协作但职责分离的子系统。

English: Ground vehicles fit a throttle-plus-steering model, with chassis movement separated from turret and optics. `ControlFrame` already reserves `Throttle`, `Brake`, and `AngularInput`. Later the chassis, turret, and gunnery view can become cooperating but distinct subsystems.

### Aircraft / 飞机

中文：飞机控制和人形角色完全不同，但输入协议不需要重做。飞机可以把 `AngularInput.X/Y/Z` 映射到 pitch, yaw, roll，再结合 throttle 构成飞行动力学输入。这样玩家、AI、录制回放、辅助驾驶都能共享同一条控制入口。

English: Aircraft control is completely different from a humanoid character, but the input contract does not need to be reinvented. Aircraft can map `AngularInput.X/Y/Z` to pitch, yaw, and roll, then combine that with throttle for flight dynamics. Player, AI, replay, and assist systems all keep the same control entry point.

## Suggested Future Split / 我建议的后续拆分

- `ControlSourceNode`: where commands come from / 命令从哪里来
- `IntentProcessor`: what those commands mean in the current mode / 当前模式下这些命令意味着什么
- `RuntimeContext`: what the actor currently believes / 当前角色的权威认知
- `MotionModel`: how the actor or vehicle physically moves / 角色或载具如何物理运动
- `ActionCoordinator`: how stance and locomotion gating work / 姿态与移动门槛如何仲裁
- `CombatCoordinator`: how equip, fire, reload, and item use are resolved / 装备、开火、换弹、道具使用如何处理
- `AnimationOverrideCoordinator`: how high-priority full-body actions preempt lower layers / 最高优先级全身动作如何覆盖低层
- `CameraRig`: how the view interprets the same runtime state / 视图如何解释同一份运行时状态

## Practical Recommendation / 实际建议

中文：不要把“多模式支持”理解成一个越来越大的万能控制器类。正确方向是“共享协议 + 可替换执行器”。输入协议尽量稳定，执行层按模式拆分，运行时黑板作为中间边界。这样系统才会越做越强，而不是越做越乱。

English: Do not treat multi-mode support as one ever-growing universal controller class. The right direction is a stable shared protocol with replaceable executors. Keep the input contract stable, split executors by mode, and use the runtime blackboard as the middle boundary. That is how the system becomes stronger instead of more tangled.
