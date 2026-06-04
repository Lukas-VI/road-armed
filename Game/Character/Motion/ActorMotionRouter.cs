using Godot;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public partial class ActorMotionRouter : Node
{
    [Export]
    public Godot.Collections.Array<NodePath> MotionModelPaths { get; set; } = new();

    private readonly System.Collections.Generic.List<IActorMotionModel> _models = new();
    private CharacterRuntimeContext? _runtime;
    private IActorMotionModel? _activeModel;

    public string ActiveModelName => _activeModel?.DebugName ?? "None";

    public void Setup(CharacterBody3D actorBody, CharacterRuntimeContext runtime)
    {
        _runtime = runtime;
        _models.Clear();

        if (MotionModelPaths.Count > 0)
        {
            foreach (var path in MotionModelPaths)
            {
                if (GetNodeOrNull(path) is IActorMotionModel model)
                {
                    model.Setup(actorBody, runtime);
                    _models.Add(model);
                }
            }
        }
        else
        {
            foreach (Node child in GetChildren())
            {
                if (child is IActorMotionModel model)
                {
                    model.Setup(actorBody, runtime);
                    _models.Add(model);
                }
            }
        }

        _activeModel = ResolveModel(runtime.ControlMode);
        runtime.ActiveMotionModel = ActiveModelName;
    }

    public void Simulate(ActorIntent intent, double delta)
    {
        if (_runtime == null)
        {
            return;
        }

        if (_activeModel == null || !_activeModel.SupportsMode(intent.ControlMode))
        {
            _activeModel = ResolveModel(intent.ControlMode);
        }

        _runtime.ActiveMotionModel = ActiveModelName;
        _activeModel?.Simulate(intent, delta);
    }

    private IActorMotionModel? ResolveModel(ControlMode mode)
    {
        return _models.Find(model => model.SupportsMode(mode));
    }
}
