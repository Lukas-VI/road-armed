using Godot;

namespace BBBN.Camera
{
    public partial class ThirdPersonCamera : Node3D
    {
        [Export]
        public NodePath TargetPath { get; set; }

        [Export]
        public float Distance { get; set; } = 5.0f;

        [Export]
        public float Height { get; set; } = 2.0f;

        [Export]
        public float RotationSpeed { get; set; } = 0.1f;

        [Export]
        public float MinAngle { get; set; } = -60.0f;

        [Export]
        public float MaxAngle { get; set; } = 60.0f;

        private Node3D _target;
        private Camera3D _camera;
        private float _rotationX = 0.0f;
        private float _rotationY = 0.0f;
        private Vector2 _lastMousePosition;

        public override void _Ready()
        {
            _target = GetNode<Node3D>(TargetPath);
            _camera = GetNode<Camera3D>("Camera3D");

            // 捕获鼠标
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion mouseMotion)
            {
                if (Input.MouseMode == Input.MouseModeEnum.Captured)
                {
                    _rotationY -= mouseMotion.Relative.X * RotationSpeed;
                    _rotationX -= mouseMotion.Relative.Y * RotationSpeed;
                    _rotationX = Mathf.Clamp(_rotationX, Mathf.DegToRad(MinAngle), Mathf.DegToRad(MaxAngle));
                }
            }
        }

        public override void _Process(double delta)
        {
            if (_target == null) return;

            // 计算相机位置
            var targetPosition = _target.GlobalPosition;
            var direction = new Vector3(
                Mathf.Cos(_rotationY) * Mathf.Cos(_rotationX),
                Mathf.Sin(_rotationX),
                Mathf.Sin(_rotationY) * Mathf.Cos(_rotationX)
            ).Normalized();

            var cameraPosition = targetPosition - direction * Distance + Vector3.Up * Height;
            GlobalPosition = cameraPosition;

            // 让相机看向目标
            LookAt(targetPosition, Vector3.Up);
        }

        public void SetTargetRotation(float rotationY)
        {
            _rotationY = rotationY;
        }
    }
}