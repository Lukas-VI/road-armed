using Godot;
using RoadArmed.Game.Camera;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public static class PlayerIntentProcessor
{
    public static void BuildIntent(ControlFrame frame, CharacterRuntimeContext runtime, PlayerCameraRig? cameraRig, ActorIntent intent)
    {
        intent.Clear(runtime.ControlMode);

        Vector3 planarForward = cameraRig?.PlanarForward ?? Vector3.Forward;
        if (planarForward.LengthSquared() < 0.0001f)
        {
            planarForward = Vector3.Forward;
        }

        Vector3 planarRight = cameraRig?.PlanarRight ?? Vector3.Right;
        if (planarRight.LengthSquared() < 0.0001f)
        {
            planarRight = Vector3.Right;
        }

        Vector3 worldMove = (planarRight * frame.Move.X) + (planarForward * -frame.Move.Y);
        if (worldMove.LengthSquared() > 1f)
        {
            worldMove = worldMove.Normalized();
        }

        intent.ControlMode = runtime.ControlMode;
        intent.WorldMove = runtime.ControlMode == ControlMode.TacticalCommand ? Vector3.Zero : worldMove;
        intent.ViewForward = cameraRig?.Forward ?? Vector3.Forward;
        intent.AimDirection = intent.ViewForward;
        intent.CommandPoint = cameraRig?.LastGroundPoint ?? Vector3.Zero;
        intent.HotbarSlotRequested = frame.HotbarSlotRequested;
        intent.WantsJump = (runtime.ControlMode is ControlMode.ThirdPerson or ControlMode.PrecisionAim) && frame.JumpPressed;
        intent.WantsSprint = runtime.ControlMode == ControlMode.ThirdPerson && frame.SprintHeld && worldMove.LengthSquared() > 0.0001f;
        intent.WantsAim = frame.AimHeld || runtime.ControlMode == ControlMode.PrecisionAim;
        intent.WantsFire = frame.FireHeld || frame.FirePressed;
        intent.WantsInteract = frame.InteractPressed;
        intent.WantsReload = frame.ReloadPressed;
        intent.WantsCrouchToggle = frame.CrouchPressed;
        intent.WantsShoulderSwap = frame.ShoulderSwapPressed;
        intent.WantsSocialAction = frame.SocialActionPressed;
        intent.Throttle = frame.Throttle - frame.Brake;
        intent.AngularInput = frame.AngularInput;

        runtime.MoveInput = frame.Move;
        runtime.LookInput = frame.Look;
        runtime.WorldMoveDirection = intent.WorldMove;
        runtime.AimDirection = intent.AimDirection;
        runtime.CommandPoint = intent.CommandPoint;
    }
}
