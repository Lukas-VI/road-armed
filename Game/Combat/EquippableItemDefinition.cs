using Godot;
using RoadArmed.Game.Shared;

namespace RoadArmed.Game.Combat;

[GlobalClass]
public partial class EquippableItemDefinition : ItemDefinition
{
    [Export]
    public PackedScene? WorldModelScene { get; set; }

    [Export]
    public StringName GripSocketName { get; set; } = new(AssetNaming.CharacterSockets.RightHand);

    [Export]
    public StringName MuzzleSocketName { get; set; } = new(AssetNaming.CharacterSockets.Muzzle);

    [Export]
    public Vector3 HoldPositionOffset { get; set; } = Vector3.Zero;

    [Export]
    public Vector3 HoldRotationDegrees { get; set; } = Vector3.Zero;

    [Export]
    public Vector3 HoldScale { get; set; } = Vector3.One;

    [Export]
    public string EquipAnimationKey { get; set; } = AssetNaming.AnimationKeys.EquipDefault;

    [Export]
    public string IdleAnimationKey { get; set; } = AssetNaming.AnimationKeys.IdleUnarmed;

    [Export]
    public string UnequipAnimationKey { get; set; } = AssetNaming.AnimationKeys.UnequipDefault;
}
