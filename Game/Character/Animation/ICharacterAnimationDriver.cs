using Godot;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public interface ICharacterAnimationDriver
{
    string DebugName { get; }

    void Setup(CharacterRuntimeContext runtime);

    void Apply(CharacterRuntimeContext runtime, ActorIntent intent, double delta);
}
