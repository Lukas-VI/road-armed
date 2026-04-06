using Godot;
using BBBN.Character.Core;
using BBBN.Character.Input;
using BBBN.Character.States;
using BBBN.Camera;

namespace BBBN.Character.Controller
{
    [Tool]
    public partial class GodotCharacterController : CharacterBody3D
    {
        [Export]
        public float MoveSpeed { get; set; } = 6f;

        [Export]
        public float SprintSpeed { get; set; } = 10f;

        [Export]
        public float JumpStrength { get; set; } = 12f;

        [Export]
        public NodePath AnimationTreePath { get; set; }

        [Export]
        public NodePath CameraPath { get; set; }

        public ThirdPersonCamera _camera;

        private CharacterControllerBase _controllerCore;

        private GodotPlayerInputSource _inputSource;
        private GodotCharacterMotionDriver _motionDriver;
        private CharacterRuntimeData _runtimeData;

        public override void _Ready()
        {
            _inputSource = new GodotPlayerInputSource();
            _motionDriver = new GodotCharacterMotionDriver(this);
            
            // 获取相机
            if (!string.IsNullOrEmpty(CameraPath))
            {
                _camera = GetNodeOrNull<ThirdPersonCamera>(CameraPath);
            }
            
            // AnimationTree is optional for basic movement demo
            AnimationTree animationTree = null;
            if (!string.IsNullOrEmpty(AnimationTreePath))
            {
                animationTree = GetNodeOrNull<AnimationTree>(AnimationTreePath);
            }
            
            ICharacterAnimationDriver animationDriver = animationTree != null 
                ? new GodotCharacterAnimationDriver(animationTree)
                : new NullCharacterAnimationDriver();
                
            _runtimeData = new CharacterRuntimeData();

            _controllerCore = new GodotCharacterControllerCore(
                _inputSource,
                _motionDriver,
                animationDriver,
                _runtimeData,
                MoveSpeed,
                SprintSpeed,
                JumpStrength);

            _controllerCore.Initialize(new CharacterIdleState());
        }

        public override void _Process(double delta)
        {
            _controllerCore?.Update((float)delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            _controllerCore?.PhysicsUpdate((float)delta);
        }

        private sealed class GodotCharacterControllerCore : CharacterControllerBase
        {
            public GodotCharacterControllerCore(
                IInputSource inputSource,
                ICharacterMotionDriver motionDriver,
                ICharacterAnimationDriver animationDriver,
                ICharacterRuntimeData runtimeData,
                float moveSpeed,
                float sprintSpeed,
                float jumpStrength)
                : base(inputSource, motionDriver, animationDriver, runtimeData, moveSpeed, sprintSpeed, jumpStrength)
            {
            }

            public override string ToString() => nameof(GodotCharacterControllerCore);
        }

        private sealed class NullCharacterAnimationDriver : ICharacterAnimationDriver
        {
            public void SetMovementSpeed(float speed) { }
            public void SetIsGrounded(bool isGrounded) { }
            public void SetIsMoving(bool isMoving) { }
            public void SetIsSprinting(bool isSprinting) { }
            public void SetJumpTriggered(bool jumpTriggered) { }
            public void SetActionTriggered(bool actionTriggered) { }
            public void SetIsJumping(bool isJumping) { }
            public void SetAction(bool isActive) { }
        }
    }
}
