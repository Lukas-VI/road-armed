using Godot;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public interface IActorMotionModel
{
    string DebugName { get; }

    void Setup(CharacterBody3D actorBody, CharacterRuntimeContext runtime);

    bool SupportsMode(ControlMode mode);

    void Simulate(ActorIntent intent, double delta);
}
