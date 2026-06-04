using Godot;

namespace RoadArmed.Game.Input;

public sealed class ActorIntent
{
    public ControlMode ControlMode { get; set; } = ControlMode.ThirdPerson;
    public Vector3 WorldMove { get; set; } = Vector3.Zero;
    public Vector3 AimDirection { get; set; } = Vector3.Forward;
    public Vector3 ViewForward { get; set; } = Vector3.Forward;
    public Vector3 CommandPoint { get; set; } = Vector3.Zero;
    public float Throttle { get; set; }
    public Vector3 AngularInput { get; set; } = Vector3.Zero;
    public int HotbarSlotRequested { get; set; } = -1;
    public bool WantsJump { get; set; }
    public bool WantsSprint { get; set; }
    public bool WantsAim { get; set; }
    public bool WantsFire { get; set; }
    public bool WantsInteract { get; set; }
    public bool WantsReload { get; set; }
    public bool WantsCrouchToggle { get; set; }
    public bool WantsShoulderSwap { get; set; }
    public bool WantsSocialAction { get; set; }

    public void Clear(ControlMode mode)
    {
        ControlMode = mode;
        WorldMove = Vector3.Zero;
        AimDirection = Vector3.Forward;
        ViewForward = Vector3.Forward;
        CommandPoint = Vector3.Zero;
        Throttle = 0f;
        AngularInput = Vector3.Zero;
        HotbarSlotRequested = -1;
        WantsJump = false;
        WantsSprint = false;
        WantsAim = false;
        WantsFire = false;
        WantsInteract = false;
        WantsReload = false;
        WantsCrouchToggle = false;
        WantsShoulderSwap = false;
        WantsSocialAction = false;
    }
}
