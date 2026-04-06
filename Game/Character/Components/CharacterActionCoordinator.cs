using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public partial class CharacterActionCoordinator : Godot.Node
{
    private CharacterRuntimeContext? _runtime;

    public string DebugName => nameof(CharacterActionCoordinator);

    public void Setup(CharacterRuntimeContext runtime)
    {
        _runtime = runtime;
        runtime.ActiveActionCoordinator = DebugName;
    }

    public void Process(ActorIntent intent, double delta)
    {
        if (_runtime == null)
        {
            return;
        }

        if (intent.WantsCrouchToggle)
        {
            _runtime.StanceMode = _runtime.StanceMode == StanceMode.Standing
                ? StanceMode.Crouched
                : StanceMode.Standing;
        }

        if (_runtime.StanceMode == StanceMode.Crouched)
        {
            intent.WantsSprint = false;
        }

        _runtime.IsAiming = intent.WantsAim;
        _runtime.IsSprinting = intent.WantsSprint;
        _runtime.WantsJump = intent.WantsJump;
        _runtime.WantsFire = intent.WantsFire;
        _runtime.WantsInteract = intent.WantsInteract;
        _runtime.WantsReload = intent.WantsReload;
        _runtime.IsFiring = intent.WantsFire;
        _runtime.IsMoving = intent.WorldMove.LengthSquared() > 0.0001f;
        _runtime.LocomotionState = ResolveLocomotion(intent, _runtime);
        _runtime.ActionPhase = ResolveAction(intent);
    }

    private static LocomotionState ResolveLocomotion(ActorIntent intent, CharacterRuntimeContext runtime)
    {
        if (!runtime.IsGrounded && runtime.Velocity.Y < -0.01f)
        {
            return LocomotionState.Airborne;
        }

        if (intent.ControlMode == ControlMode.TacticalCommand)
        {
            return LocomotionState.Tactical;
        }

        if (intent.WantsSprint)
        {
            return LocomotionState.Sprinting;
        }

        if (intent.WorldMove.LengthSquared() > 0.0001f)
        {
            return LocomotionState.Moving;
        }

        return LocomotionState.Idle;
    }

    private static ActionPhase ResolveAction(ActorIntent intent)
    {
        if (intent.WantsReload)
        {
            return ActionPhase.Reloading;
        }

        if (intent.WantsInteract)
        {
            return ActionPhase.Interacting;
        }

        if (intent.WantsFire)
        {
            return ActionPhase.Firing;
        }

        if (intent.WantsAim)
        {
            return ActionPhase.Aiming;
        }

        return ActionPhase.None;
    }
}
