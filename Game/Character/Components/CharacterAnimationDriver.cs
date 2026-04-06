using Godot;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public partial class CharacterAnimationDriver : Node, ICharacterAnimationDriver
{
    [Export]
    public NodePath AnimationTreePath { get; set; } = new();

    [Export]
    public string BlendXPath { get; set; } = "parameters/Locomotion/blend_position";

    [Export]
    public string SpeedScalePath { get; set; } = "parameters/LocomotionSpeed/scale";

    private AnimationTree? _animationTree;

    public string DebugName => nameof(CharacterAnimationDriver);

    public void Setup(CharacterRuntimeContext runtime)
    {
        _animationTree = !AnimationTreePath.IsEmpty ? GetNodeOrNull<AnimationTree>(AnimationTreePath) : null;
        runtime.ActiveAnimationDriver = DebugName;
    }

    public void Apply(CharacterRuntimeContext runtime, ActorIntent intent, double delta)
    {
        runtime.ActiveAnimationDriver = DebugName;

        if (_animationTree == null)
        {
            return;
        }

        _animationTree.Active = true;

        Vector2 blend = runtime.LocalPlanarVelocity;
        if (blend.LengthSquared() > 1f)
        {
            blend = blend.Normalized();
        }

        TrySet(BlendXPath, blend);
        TrySet(SpeedScalePath, runtime.SpeedRatio);
    }

    private void TrySet(string propertyPath, Variant value)
    {
        if (string.IsNullOrWhiteSpace(propertyPath))
        {
            return;
        }

        _animationTree?.Set(propertyPath, value);
    }
}
