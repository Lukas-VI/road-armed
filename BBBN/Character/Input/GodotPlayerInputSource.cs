using Godot;
using BBBN.Character.Controller;

namespace BBBN.Character.Input
{
    public sealed class GodotPlayerInputSource : InputSourceBase
    {
        private GodotCharacterController _controller;

        public GodotPlayerInputSource()
        {
            // 尝试获取控制器引用（如果在场景中）
            var sceneTree = (SceneTree)Engine.GetMainLoop();
            if (sceneTree != null && sceneTree.CurrentScene != null)
            {
                _controller = sceneTree.CurrentScene.GetNodeOrNull<GodotCharacterController>("root/CharacterBody3D");
            }
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            var input2d = Godot.Input.GetVector("move_left", "move_right", "move_backward", "move_forward");
            
            // 如果有相机，根据相机方向转换输入
            if (_controller != null && _controller._camera != null)
            {
                var cameraRotation = _controller._camera.Rotation.Y;
                var rotatedInput = input2d.Rotated(-cameraRotation);
                Movement = new Vector3(rotatedInput.X, 0f, -rotatedInput.Y);
            }
            else
            {
                Movement = new Vector3(input2d.X, 0f, -input2d.Y);
            }
            
            JumpPressed = Godot.Input.IsActionJustPressed("jump");
            Sprinting = Godot.Input.IsActionPressed("sprint");
            ActionPressed = Godot.Input.IsActionJustPressed("action");
        }
    }
}
