using Godot;
using RoadArmed.Game.Combat;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public partial class AnimationOverrideCoordinator : Node
{
    [Export]
    public string SocialAnimationKey { get; set; } = "social_wave";

    [Export]
    public float SocialAnimationDuration { get; set; } = 2.4f;

    private CharacterRuntimeContext? _runtime;
    private float _remainingTime;

    public string DebugName => nameof(AnimationOverrideCoordinator);

    public void Setup(CharacterRuntimeContext runtime)
    {
        _runtime = runtime;
        runtime.OverrideLayerName = string.Empty;
        runtime.OverrideRemainingTime = 0f;
    }

    public void Process(ActorIntent intent, double delta)
    {
        if (_runtime == null)
        {
            return;
        }

        if (_remainingTime > 0f)
        {
            _remainingTime = Mathf.Max(0f, _remainingTime - (float)delta);
            _runtime.OverrideRemainingTime = _remainingTime;
            if (_remainingTime <= 0f)
            {
                _runtime.OverrideLayerName = string.Empty;
            }
        }

        if (intent.WantsSocialAction)
        {
            RequestOverride(SocialAnimationKey, SocialAnimationDuration);
        }
    }

    public void RequestOverride(string key, float duration)
    {
        if (_runtime == null)
        {
            return;
        }

        _runtime.OverrideLayerName = key;
        _remainingTime = Mathf.Max(0f, duration);
        _runtime.OverrideRemainingTime = _remainingTime;
    }
}
