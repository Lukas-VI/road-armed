using Godot;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Input;

public partial class ControlSourceRouter : ControlSourceNode
{
    [Export]
    public Godot.Collections.Array<NodePath> SourcePaths { get; set; } = new();

    [Export]
    public NodePath ActiveSourcePath { get; set; } = new("PlayerInput");

    private readonly System.Collections.Generic.List<ControlSourceNode> _sources = new();
    private ControlSourceNode? _activeSource;

    public string ActiveSourceName => _activeSource?.Name ?? "None";

    public override void _Ready()
    {
        RebuildSources();
        TryAssignActiveSource();
    }

    public override bool IsAvailable => _activeSource?.IsAvailable ?? false;

    public override void Capture(ControlMode currentMode, ControlFrame frame, double delta)
    {
        EnsureActiveSource();
        if (_activeSource == null)
        {
            frame.Clear(currentMode);
            return;
        }

        _activeSource.Capture(currentMode, frame, delta);
    }

    public override void HandleInput(InputEvent @event)
    {
        EnsureActiveSource();
        _activeSource?.HandleInput(@event);
    }

    public bool SetActiveSourceByName(string nodeName)
    {
        EnsureActiveSource();
        var source = _sources.Find(candidate => candidate.Name == nodeName);
        if (source == null)
        {
            return false;
        }

        _activeSource = source;
        return true;
    }

    private void RebuildSources()
    {
        _sources.Clear();

        if (SourcePaths.Count > 0)
        {
            foreach (var path in SourcePaths)
            {
                var source = GetNodeOrNull<ControlSourceNode>(path);
                if (source != null && source != this)
                {
                    _sources.Add(source);
                }
            }

            return;
        }

        foreach (Node child in GetChildren())
        {
            if (child is ControlSourceNode source && source != this)
            {
                _sources.Add(source);
            }
        }
    }

    private void EnsureActiveSource()
    {
        if (_activeSource != null && _activeSource.IsAvailable)
        {
            return;
        }

        TryAssignActiveSource();
    }

    private void TryAssignActiveSource()
    {
        if (!ActiveSourcePath.IsEmpty)
        {
            _activeSource = GetNodeOrNull<ControlSourceNode>(ActiveSourcePath);
            if (_activeSource != null)
            {
                return;
            }
        }

        _activeSource = _sources.Find(candidate => candidate.IsAvailable);
    }
}
