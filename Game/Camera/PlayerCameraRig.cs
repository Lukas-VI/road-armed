using Godot;
using RoadArmed.Game.Character.Components;
using RoadArmed.Game.Character.Resources;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Camera;

public partial class PlayerCameraRig : Node3D
{
    [Export]
    public CameraRigSettings? Settings { get; set; }
    [Export]
    public NodePath YawPivotPath { get; set; } = new("YawPivot");
    [Export]
    public NodePath PitchPivotPath { get; set; } = new("YawPivot/PitchPivot");
    [Export]
    public NodePath SpringArmPath { get; set; } = new("YawPivot/PitchPivot/SpringArm3D");
    [Export]
    public NodePath CameraPath { get; set; } = new("YawPivot/PitchPivot/SpringArm3D/Camera3D");

    private Node3D? _followTarget;
    private CharacterRuntimeContext? _runtime;
    private Node3D? _yawPivot;
    private Node3D? _pitchPivot;
    private SpringArm3D? _springArm;
    private Camera3D? _camera;
    private float _yaw;
    private float _pitch;
    private float _thirdPersonDistance;
    private float _tacticalDistance;
    private int _shoulderSide = 1;

    public float Yaw => _yaw;
    public float Pitch => _pitch;
    public Vector3 Forward => _camera != null ? -_camera.GlobalBasis.Z : Vector3.Forward;
    public Vector3 PlanarForward => Forward.Slide(Vector3.Up).Normalized();
    public Vector3 PlanarRight => PlanarForward.Cross(Vector3.Up).Normalized();
    public Vector3 LastGroundPoint { get; private set; } = Vector3.Zero;

    public void Setup(Node3D followTarget, CharacterRuntimeContext runtime)
    {
        _followTarget = followTarget;
        _runtime = runtime;
        _yawPivot = GetNodeOrNull<Node3D>(YawPivotPath);
        _pitchPivot = GetNodeOrNull<Node3D>(PitchPivotPath);
        _springArm = GetNodeOrNull<SpringArm3D>(SpringArmPath);
        _camera = GetNodeOrNull<Camera3D>(CameraPath);
        Settings ??= new CameraRigSettings();
        _thirdPersonDistance = Settings.ThirdPersonDistance;
        _tacticalDistance = Settings.TacticalDistance;
        _yaw = runtime.BodyYaw;
        if (_springArm != null && followTarget is CollisionObject3D collisionObject)
        {
            _springArm.AddExcludedObject(collisionObject.GetRid());
        }
    }

    public void UpdateRig(ControlFrame frame, double delta)
    {
        if (_followTarget == null || _runtime == null || Settings == null || _yawPivot == null || _pitchPivot == null || _springArm == null) return;
        float dt = (float)delta;
        _yaw -= frame.Look.X * Settings.MouseSensitivity;
        _pitch -= frame.Look.Y * Settings.MouseSensitivity;
        float minPitch = Mathf.DegToRad(Settings.MinPitchDegrees);
        float maxPitch = Mathf.DegToRad(Settings.MaxPitchDegrees);
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        if (frame.ShoulderSwapPressed) _shoulderSide *= -1;
        switch (_runtime.ControlMode)
        {
            case ControlMode.ThirdPerson:
                _thirdPersonDistance = Mathf.Clamp(_thirdPersonDistance - frame.ZoomDelta * Settings.ZoomStep, Settings.MinZoomDistance, Settings.MaxZoomDistance);
                break;
            case ControlMode.TacticalCommand:
                _tacticalDistance = Mathf.Clamp(_tacticalDistance - frame.ZoomDelta * Settings.ZoomStep, Settings.ThirdPersonDistance, Settings.MaxZoomDistance + 8f);
                break;
        }
        GlobalPosition = ResolveFollowOrigin();
        _yawPivot.Rotation = new Vector3(0f, _yaw, 0f);
        _pitchPivot.Position = ResolvePivotOffset(_runtime.ControlMode);
        _pitchPivot.Rotation = new Vector3(ResolvePitch(_runtime.ControlMode), 0f, 0f);
        _springArm.Position = ResolveShoulderOffset(_runtime.ControlMode);
        _springArm.SpringLength = Mathf.Lerp(_springArm.SpringLength, ResolveDistance(_runtime.ControlMode), 1f - Mathf.Exp(-Settings.DistanceSmoothing * dt));
        UpdateAimPoint();
        _runtime.ViewYaw = _yaw;
        _runtime.ViewPitch = _pitchPivot.Rotation.X;
    }

    private Vector3 ResolveFollowOrigin()
    {
        if (_followTarget == null || Settings == null) return GlobalPosition;
        var sockets = _followTarget.GetNodeOrNull<EquipmentSockets>("EquipmentSockets");
        if (sockets?.CameraAnchor != null) return sockets.CameraAnchor.GlobalPosition;
        return _followTarget.GlobalPosition + Settings.ThirdPersonPivotOffset;
    }

    private Vector3 ResolvePivotOffset(ControlMode mode) => mode switch
    {
        ControlMode.PrecisionAim => Settings!.PrecisionPivotOffset,
        ControlMode.TacticalCommand => Settings!.TacticalPivotOffset,
        _ => Vector3.Zero
    };

    private Vector3 ResolveShoulderOffset(ControlMode mode)
    {
        Vector3 offset = mode switch
        {
            ControlMode.PrecisionAim => Settings!.PrecisionShoulderOffset,
            ControlMode.TacticalCommand => Settings!.TacticalShoulderOffset,
            _ => Settings!.ThirdPersonShoulderOffset
        };
        offset.X *= _shoulderSide;
        return offset;
    }

    private float ResolvePitch(ControlMode mode) => mode == ControlMode.TacticalCommand ? Mathf.DegToRad(Settings!.TacticalPitchDegrees) : _pitch;
    private float ResolveDistance(ControlMode mode) => mode switch
    {
        ControlMode.PrecisionAim => Settings!.PrecisionDistance,
        ControlMode.TacticalCommand => _tacticalDistance,
        _ => _thirdPersonDistance
    };

    private void UpdateAimPoint()
    {
        Vector3 planarForward = PlanarForward;
        if (planarForward.LengthSquared() <= 0.0001f) planarForward = Vector3.Forward;
        float y = _followTarget?.GlobalPosition.Y ?? 0f;
        LastGroundPoint = (_followTarget?.GlobalPosition ?? Vector3.Zero) + planarForward * 12f;
        LastGroundPoint = new Vector3(LastGroundPoint.X, y, LastGroundPoint.Z);
    }
}
