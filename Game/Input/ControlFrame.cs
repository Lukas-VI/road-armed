using Godot;

namespace RoadArmed.Game.Input;

public sealed class ControlFrame
{
    public Vector2 Move { get; set; }
    public Vector2 Look { get; set; }
    public float ZoomDelta { get; set; }
    public float Throttle { get; set; }
    public float Brake { get; set; }
    public Vector3 AngularInput { get; set; }
    public int HotbarSlotRequested { get; set; } = -1;
    public bool JumpPressed { get; set; }
    public bool SprintHeld { get; set; }
    public bool AimHeld { get; set; }
    public bool FirePressed { get; set; }
    public bool FireHeld { get; set; }
    public bool InteractPressed { get; set; }
    public bool ReloadPressed { get; set; }
    public bool CrouchPressed { get; set; }
    public bool ShoulderSwapPressed { get; set; }
    public bool SocialActionPressed { get; set; }
    public ControlMode RequestedMode { get; set; } = ControlMode.ThirdPerson;

    public void Clear(ControlMode requestedMode)
    {
        Move = Vector2.Zero;
        Look = Vector2.Zero;
        ZoomDelta = 0f;
        Throttle = 0f;
        Brake = 0f;
        AngularInput = Vector3.Zero;
        HotbarSlotRequested = -1;
        JumpPressed = false;
        SprintHeld = false;
        AimHeld = false;
        FirePressed = false;
        FireHeld = false;
        InteractPressed = false;
        ReloadPressed = false;
        CrouchPressed = false;
        ShoulderSwapPressed = false;
        SocialActionPressed = false;
        RequestedMode = requestedMode;
    }

    public void CopyFrom(ControlFrame other)
    {
        Move = other.Move;
        Look = other.Look;
        ZoomDelta = other.ZoomDelta;
        Throttle = other.Throttle;
        Brake = other.Brake;
        AngularInput = other.AngularInput;
        HotbarSlotRequested = other.HotbarSlotRequested;
        JumpPressed = other.JumpPressed;
        SprintHeld = other.SprintHeld;
        AimHeld = other.AimHeld;
        FirePressed = other.FirePressed;
        FireHeld = other.FireHeld;
        InteractPressed = other.InteractPressed;
        ReloadPressed = other.ReloadPressed;
        CrouchPressed = other.CrouchPressed;
        ShoulderSwapPressed = other.ShoulderSwapPressed;
        SocialActionPressed = other.SocialActionPressed;
        RequestedMode = other.RequestedMode;
    }

    public void ClearTransientSignals()
    {
        Look = Vector2.Zero;
        ZoomDelta = 0f;
        HotbarSlotRequested = -1;
        JumpPressed = false;
        FirePressed = false;
        InteractPressed = false;
        ReloadPressed = false;
        CrouchPressed = false;
        ShoulderSwapPressed = false;
        SocialActionPressed = false;
    }
}
