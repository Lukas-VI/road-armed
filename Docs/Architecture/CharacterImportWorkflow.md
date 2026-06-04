# Importing Character Models Correctly / 正确导入人物模型

## Why It Keeps Turning Back Into a Capsule / 为什么总是又变回胶囊

中文：因为 `jj.glb` 只是“导入的模型资产”，不是完整的可玩角色场景。项目里真正带控制器、相机、碰撞、调试逻辑的是 [PlayerActor.tscn](e:/Gogot/road-armed/Scenes/Characters/PlayerActor.tscn)。这个模板默认自带一个胶囊网格作为占位可视体。如果你打开的是模板场景，或者运行的是旧的胶囊沙盒场景，就会看到胶囊，而不是你的 `jj` 模型。

English: `jj.glb` is only an imported model asset, not a complete playable character scene. The actual controller, camera, collision, and debug logic live in [PlayerActor.tscn](e:/Gogot/road-armed/Scenes/Characters/PlayerActor.tscn). That template includes a capsule mesh as a default placeholder visual. If you open the template directly or run the old capsule sandbox, you will see the capsule instead of your `jj` model.

## Correct Asset Relationship / 正确的资产关系

- `jj.glb`: imported visual asset only / 只负责导入模型与动画
- `PlayerActor.tscn`: gameplay controller template / 负责控制器逻辑
- `JjGameplayCharacter.tscn`: wrapper scene combining controller + `jj.glb` / 把控制器和 `jj.glb` 组合起来
- `JjControllerPlayground.tscn`: sandbox scene for testing the wrapped character / 用于测试组合后角色的沙盒场景

## The Scene You Should Run / 你应该运行哪个场景

中文：现在默认主场景已经切到 [JjControllerPlayground.tscn](e:/Gogot/road-armed/Scenes/Sandbox/JjControllerPlayground.tscn)。如果你想测试 `jj`，请运行这个场景，而不是旧的 [ControllerPlayground.tscn](e:/Gogot/road-armed/Scenes/Sandbox/ControllerPlayground.tscn)。

English: The default main scene is now [JjControllerPlayground.tscn](e:/Gogot/road-armed/Scenes/Sandbox/JjControllerPlayground.tscn). If you want to test `jj`, run this scene, not the old [ControllerPlayground.tscn](e:/Gogot/road-armed/Scenes/Sandbox/ControllerPlayground.tscn).

## Correct Import Workflow / 正确导入流程

1. Export your model from Blender to [jj.glb](e:/Gogot/road-armed/BlenderAsset/export/jj.glb).
2. Open the Godot editor and wait for import to finish.
3. Do not treat `jj.glb` itself as the playable character scene.
4. Open [JjGameplayCharacter.tscn](e:/Gogot/road-armed/Scenes/Characters/JjGameplayCharacter.tscn).
5. Confirm `Actor/VisualRoot/BodyMesh` is hidden.
6. Confirm `Actor/VisualRoot/Model` is the instanced `jj.glb`.
7. Run [JjControllerPlayground.tscn](e:/Gogot/road-armed/Scenes/Sandbox/JjControllerPlayground.tscn).

1. 从 Blender 导出到 [jj.glb](e:/Gogot/road-armed/BlenderAsset/export/jj.glb)。
2. 打开 Godot 编辑器并等待导入完成。
3. 不要把 `jj.glb` 本身当成可玩角色场景。
4. 打开 [JjGameplayCharacter.tscn](e:/Gogot/road-armed/Scenes/Characters/JjGameplayCharacter.tscn)。
5. 确认 `Actor/VisualRoot/BodyMesh` 已经隐藏。
6. 确认 `Actor/VisualRoot/Model` 确实是实例化的 `jj.glb`。
7. 运行 [JjControllerPlayground.tscn](e:/Gogot/road-armed/Scenes/Sandbox/JjControllerPlayground.tscn)。

## What Not To Do / 不要这样做

- do not run `PlayerActor.tscn` directly if you want to see `jj` / 如果你想看 `jj`，不要直接运行 `PlayerActor.tscn`
- do not run the old capsule-only sandbox by mistake / 不要误运行旧的胶囊沙盒
- do not edit the imported `.glb` scene as if it were your gameplay scene / 不要把导入的 `.glb` 场景当成玩法场景直接改

## Why Wrapper Scenes Matter / 为什么一定要用包装场景

中文：Godot 的导入场景会随着重导入更新，而你自己的玩法场景应该保持稳定。正确做法是：让 `.glb` 只负责“提供模型和动画”，让你自己的 `.tscn` 负责“提供控制器、相机、碰撞、挂点、校验器、调试逻辑”。

English: Imported scenes are reimported and regenerated, while your gameplay scenes should stay stable. The correct pattern is: let the `.glb` provide model and animation, and let your own `.tscn` provide controller, camera, collision, sockets, validation, and debug logic.

