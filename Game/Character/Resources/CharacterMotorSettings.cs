using Godot;

namespace RoadArmed.Game.Character.Resources;

[GlobalClass]
public partial class CharacterMotorSettings : Resource
{
    [Export]
    public float WalkSpeed { get; set; } = 4.5f;

    [Export]
    public float SprintSpeed { get; set; } = 7.5f;

    [Export]
    public float PrecisionAimSpeed { get; set; } = 3f;

    [Export]
    public float Acceleration { get; set; } = 18f;

    [Export]
    public float Deceleration { get; set; } = 24f;

    [Export]
    public float AirControl { get; set; } = 8f;

    [Export]
    public float JumpVelocity { get; set; } = 5.5f;

    [Export]
    public float RotationSharpness { get; set; } = 14f;

    [Export]
    public float AimRotationSharpness { get; set; } = 18f;

    [Export]
    public float AirRotationSharpness { get; set; } = 8f;
}
