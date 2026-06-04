using Godot;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public sealed class CharacterRuntimeContext
{
    public ControlMode ControlMode { get; set; } = ControlMode.ThirdPerson;
    public LocomotionState LocomotionState { get; set; } = LocomotionState.Idle;
    public StanceMode StanceMode { get; set; } = StanceMode.Standing;
    public ActionPhase ActionPhase { get; set; } = ActionPhase.None;
    public Vector2 MoveInput { get; set; } = Vector2.Zero;
    public Vector2 LookInput { get; set; } = Vector2.Zero;
    public Vector3 WorldMoveDirection { get; set; } = Vector3.Zero;
    public Vector3 Velocity { get; set; } = Vector3.Zero;
    public Vector2 LocalPlanarVelocity { get; set; } = Vector2.Zero;
    public float HorizontalSpeed { get; set; }
    public float SpeedRatio { get; set; }
    public bool IsGrounded { get; set; }
    public bool JustLanded { get; set; }
    public bool JustLeftGround { get; set; }
    public bool IsAiming { get; set; }
    public bool IsSprinting { get; set; }
    public bool IsMoving { get; set; }
    public bool IsFiring { get; set; }
    public bool IsReloading { get; set; }
    public bool CanFire { get; set; }
    public bool WantsJump { get; set; }
    public bool WantsFire { get; set; }
    public bool WantsInteract { get; set; }
    public bool WantsReload { get; set; }
    public float ViewYaw { get; set; }
    public float ViewPitch { get; set; }
    public float BodyYaw { get; set; }
    public Vector3 AimDirection { get; set; } = Vector3.Forward;
    public Vector3 CommandPoint { get; set; } = Vector3.Zero;
    public Vector3 WeaponMuzzlePosition { get; set; } = Vector3.Zero;
    public string WeaponMuzzlePath { get; set; } = string.Empty;
    public Vector3 LastShotHitPosition { get; set; } = Vector3.Zero;
    public string LastShotHitCollider { get; set; } = string.Empty;
    public bool LastShotBlocked { get; set; }
    public int EquippedSlotIndex { get; set; } = -1;
    public string EquippedItemId { get; set; } = string.Empty;
    public string EquippedItemName { get; set; } = "Unarmed";
    public string LastFiredItemId { get; set; } = string.Empty;
    public string LastFireAnimationKey { get; set; } = string.Empty;
    public string OverrideLayerName { get; set; } = string.Empty;
    public float OverrideRemainingTime { get; set; }
    public float ReloadRemaining { get; set; }
    public string ActiveAnimationClip { get; set; } = string.Empty;
    public string ControlSourceName { get; set; } = "None";
    public string ActiveMotionModel { get; set; } = "None";
    public string ActiveAnimationDriver { get; set; } = "None";
    public string ActiveActionCoordinator { get; set; } = "None";
    public string ActiveCombatCoordinator { get; set; } = "None";
    public string ActiveAimController { get; set; } = "None";
}
