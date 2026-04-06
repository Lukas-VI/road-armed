using Godot;

namespace RoadArmed.Game.Input;

public partial class PlayerInputRouter : ControlSourceNode
{
    [Export]
    public bool CaptureMouseOnReady { get; set; } = true;

    [Export]
    public bool ReCaptureMouseOnClick { get; set; } = true;

    private Vector2 _pendingLookInput = Vector2.Zero;
    private float _pendingZoomDelta;

    public override void _Ready()
    {
        if (CaptureMouseOnReady)
        {
            Godot.Input.MouseMode = Godot.Input.MouseModeEnum.Captured;
        }
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && Godot.Input.MouseMode == Godot.Input.MouseModeEnum.Captured)
        {
            _pendingLookInput += mouseMotion.Relative;
            return;
        }

        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                _pendingZoomDelta += 1f;
                return;
            }

            if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                _pendingZoomDelta -= 1f;
                return;
            }

            if (ReCaptureMouseOnClick && Godot.Input.MouseMode != Godot.Input.MouseModeEnum.Captured)
            {
                Godot.Input.MouseMode = Godot.Input.MouseModeEnum.Captured;
                GetViewport().SetInputAsHandled();
            }
        }

        if (InputMap.HasAction("ui_cancel") && @event.IsActionPressed("ui_cancel"))
        {
            Godot.Input.MouseMode = Godot.Input.MouseModeEnum.Visible;
            GetViewport().SetInputAsHandled();
        }
    }

    public override void Capture(ControlMode currentMode, ControlFrame frame, double delta)
    {
        frame.Clear(currentMode);
        frame.Move = Godot.Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        frame.Look = _pendingLookInput;
        frame.ZoomDelta = _pendingZoomDelta;
        frame.JumpPressed = IsActionJustPressed("jump");
        frame.SprintHeld = IsActionPressed("sprint");
        frame.AimHeld = IsActionPressed("aim");
        frame.FirePressed = IsActionJustPressed("fire") || IsActionJustPressed("action");
        frame.FireHeld = IsActionPressed("fire") || IsActionPressed("action");
        frame.InteractPressed = IsActionJustPressed("interact");
        frame.ReloadPressed = IsActionJustPressed("reload");
        frame.CrouchPressed = IsActionJustPressed("crouch");
        frame.ShoulderSwapPressed = IsActionJustPressed("shoulder_swap");
        frame.Throttle = IsActionPressed("throttle_up") ? 1f : 0f;
        frame.Brake = IsActionPressed("throttle_down") ? 1f : 0f;
        frame.AngularInput = new Vector3(
            GetAxis("pitch_down", "pitch_up"),
            GetAxis("yaw_left", "yaw_right"),
            GetAxis("roll_left", "roll_right"));

        if (IsActionJustPressed("precision_mode"))
        {
            frame.RequestedMode = currentMode == ControlMode.PrecisionAim
                ? ControlMode.ThirdPerson
                : ControlMode.PrecisionAim;
        }
        else if (IsActionJustPressed("tactical_mode"))
        {
            frame.RequestedMode = currentMode == ControlMode.TacticalCommand
                ? ControlMode.ThirdPerson
                : ControlMode.TacticalCommand;
        }

        _pendingLookInput = Vector2.Zero;
        _pendingZoomDelta = 0f;
    }

    private static float GetAxis(string negativeAction, string positiveAction)
    {
        return (IsActionPressed(positiveAction) ? 1f : 0f) - (IsActionPressed(negativeAction) ? 1f : 0f);
    }

    private static bool IsActionPressed(string action)
    {
        return InputMap.HasAction(action) && Godot.Input.IsActionPressed(action);
    }

    private static bool IsActionJustPressed(string action)
    {
        return InputMap.HasAction(action) && Godot.Input.IsActionJustPressed(action);
    }
}

