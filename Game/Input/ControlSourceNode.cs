using Godot;

namespace RoadArmed.Game.Input;

public abstract partial class ControlSourceNode : Node
{
    public virtual bool IsAvailable => IsInsideTree();

    public abstract void Capture(ControlMode currentMode, ControlFrame frame, double delta);

    public virtual void HandleInput(InputEvent @event)
    {
    }
}
