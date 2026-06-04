using Godot;
using RoadArmed.Game.Shared;

namespace RoadArmed.Game.Combat;

[GlobalClass]
public partial class WeaponDefinition : EquippableItemDefinition
{
    [Export]
    public float FireRate { get; set; } = 8f;

    [Export]
    public int MagazineSize { get; set; } = 30;

    [Export]
    public float ReloadDuration { get; set; } = 1.8f;

    [Export]
    public bool IsAutomatic { get; set; } = true;

    [Export]
    public Vector3 MuzzleFallbackPosition { get; set; } = new(0f, 0f, -0.2f);

    [Export]
    public Vector3 MuzzleFallbackRotationDegrees { get; set; } = Vector3.Zero;

    [Export]
    public string FireAnimationKey { get; set; } = AssetNaming.AnimationKeys.FireRifle;

    [Export]
    public string ReloadAnimationKey { get; set; } = AssetNaming.AnimationKeys.ReloadRifle;
}
