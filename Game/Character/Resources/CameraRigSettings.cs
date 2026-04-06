using Godot;

namespace RoadArmed.Game.Character.Resources;

[GlobalClass]
public partial class CameraRigSettings : Resource
{
    [Export]
    public float MouseSensitivity { get; set; } = 0.0024f;

    [Export(PropertyHint.Range, "-89,0,0.1")]
    public float MinPitchDegrees { get; set; } = -70f;

    [Export(PropertyHint.Range, "0,89,0.1")]
    public float MaxPitchDegrees { get; set; } = 55f;

    [Export]
    public Vector3 ThirdPersonPivotOffset { get; set; } = new(0f, 1.55f, 0f);

    [Export]
    public Vector3 ThirdPersonShoulderOffset { get; set; } = new(0.55f, 0f, 0f);

    [Export]
    public float ThirdPersonDistance { get; set; } = 4.5f;

    [Export]
    public Vector3 PrecisionPivotOffset { get; set; } = new(0f, 1.55f, 0f);

    [Export]
    public Vector3 PrecisionShoulderOffset { get; set; } = new(0.2f, 0f, 0f);

    [Export]
    public float PrecisionDistance { get; set; } = 1.2f;

    [Export]
    public Vector3 TacticalPivotOffset { get; set; } = new(0f, 4f, 0f);

    [Export]
    public Vector3 TacticalShoulderOffset { get; set; } = Vector3.Zero;

    [Export]
    public float TacticalDistance { get; set; } = 14f;

    [Export(PropertyHint.Range, "-89,0,0.1")]
    public float TacticalPitchDegrees { get; set; } = -35f;

    [Export]
    public float ZoomStep { get; set; } = 0.75f;

    [Export]
    public float MinZoomDistance { get; set; } = 1.25f;

    [Export]
    public float MaxZoomDistance { get; set; } = 18f;

    [Export]
    public float DistanceSmoothing { get; set; } = 12f;
}
