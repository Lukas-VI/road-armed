using Godot;
using RoadArmed.Game.Character.Resources;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public partial class CharacterMotor : Node, IActorMotionModel
{
    [Export]
    public CharacterMotorSettings? Settings { get; set; }

    [Export]
    public NodePath VisualRootPath { get; set; } = new();

    [Export]
    public float VisualYawOffsetDegrees { get; set; }

    private CharacterBody3D? _body;
    private Node3D? _visualRoot;
    private CharacterMotorSettings _settings = new();
    private CharacterRuntimeContext? _runtime;

    public string DebugName => "CharacterMotor";

    public bool SupportsMode(ControlMode mode)
    {
        return mode is ControlMode.ThirdPerson or ControlMode.PrecisionAim or ControlMode.TacticalCommand;
    }

    public void Setup(CharacterBody3D actorBody, CharacterRuntimeContext runtime)
    {
        _body = actorBody;
        _runtime = runtime;
        _settings = Settings ?? new CharacterMotorSettings();
        _visualRoot = !VisualRootPath.IsEmpty ? GetNodeOrNull<Node3D>(VisualRootPath) : GetNodeOrNull<Node3D>("../VisualRoot");

        if (_visualRoot != null)
        {
            Vector3 rotation = _visualRoot.Rotation;
            rotation.Y = Mathf.DegToRad(VisualYawOffsetDegrees);
            _visualRoot.Rotation = rotation;
        }
    }

    public void Simulate(ActorIntent intent, double delta)
    {
        if (_body == null || _runtime == null)
        {
            return;
        }

        bool wasGrounded = _body.IsOnFloor();
        float dt = (float)delta;
        float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

        Vector3 velocity = _body.Velocity;
        if (wasGrounded && velocity.Y < 0f)
        {
            velocity.Y = 0f;
        }
        else if (!wasGrounded)
        {
            velocity.Y -= gravity * dt;
        }

        if (intent.WantsJump && wasGrounded)
        {
            velocity.Y = _settings.JumpVelocity;
        }

        float targetSpeed = ResolveTargetSpeed(intent);
        Vector3 targetHorizontal = intent.WorldMove * targetSpeed;
        Vector3 currentHorizontal = new(velocity.X, 0f, velocity.Z);
        float acceleration = currentHorizontal.LengthSquared() <= targetHorizontal.LengthSquared()
            ? _settings.Acceleration
            : _settings.Deceleration;

        if (!wasGrounded)
        {
            acceleration = _settings.AirControl;
        }

        currentHorizontal = currentHorizontal.MoveToward(targetHorizontal, acceleration * dt);
        velocity.X = currentHorizontal.X;
        velocity.Z = currentHorizontal.Z;

        _body.Velocity = velocity;
        RotateBody(intent, wasGrounded, dt);
        _body.MoveAndSlide();

        Vector3 localVelocity = _body.GlobalBasis.Inverse() * _body.Velocity;
        _runtime.JustLanded = !wasGrounded && _body.IsOnFloor();
        _runtime.JustLeftGround = wasGrounded && !_body.IsOnFloor();
        _runtime.IsGrounded = _body.IsOnFloor();
        _runtime.Velocity = _body.Velocity;
        _runtime.LocalPlanarVelocity = new Vector2(localVelocity.X, -localVelocity.Z);
        _runtime.HorizontalSpeed = new Vector2(_body.Velocity.X, _body.Velocity.Z).Length();
        _runtime.SpeedRatio = targetSpeed > 0.001f ? Mathf.Clamp(_runtime.HorizontalSpeed / targetSpeed, 0f, 2f) : 0f;
        _runtime.BodyYaw = _body.Rotation.Y;
        _runtime.ActiveMotionModel = DebugName;
    }

    private float ResolveTargetSpeed(ActorIntent intent)
    {
        float stanceScale = _runtime?.StanceMode == StanceMode.Crouched ? 0.5f : 1f;

        return intent.ControlMode switch
        {
            ControlMode.PrecisionAim => _settings.PrecisionAimSpeed * stanceScale,
            ControlMode.TacticalCommand => 0f,
            _ when intent.WantsSprint => _settings.SprintSpeed,
            _ => _settings.WalkSpeed * stanceScale
        };
    }

    private void RotateBody(ActorIntent intent, bool wasGrounded, float delta)
    {
        if (_body == null)
        {
            return;
        }

        Vector3 facingDirection = Vector3.Zero;
        if (intent.WantsAim || intent.ControlMode == ControlMode.PrecisionAim)
        {
            facingDirection = intent.AimDirection;
        }
        else if (intent.WorldMove.LengthSquared() > 0.0001f)
        {
            facingDirection = intent.WorldMove;
        }

        if (facingDirection.LengthSquared() <= 0.0001f)
        {
            return;
        }

        facingDirection.Y = 0f;
        facingDirection = facingDirection.Normalized();
        float targetYaw = Mathf.Atan2(facingDirection.X, facingDirection.Z);
        float sharpness = intent.WantsAim || intent.ControlMode == ControlMode.PrecisionAim
            ? _settings.AimRotationSharpness
            : _settings.RotationSharpness;

        if (!wasGrounded)
        {
            sharpness = _settings.AirRotationSharpness;
        }

        Vector3 rotation = _body.Rotation;
        float weight = 1f - Mathf.Exp(-sharpness * delta);
        rotation.Y = Mathf.LerpAngle(rotation.Y, targetYaw, weight);
        _body.Rotation = rotation;
    }
}
