using Godot;
using RoadArmed.Game.Character.Nodes;

namespace RoadArmed.Game.UI.Debug;

public partial class ControllerDebugOverlay : CanvasLayer
{
    [Export]
    public NodePath ActorPath { get; set; } = new();

    [Export]
    public NodePath LabelPath { get; set; } = new("Panel/Margin/DebugLabel");

    private PlayerActor? _actor;
    private Label? _label;

    public override void _Ready()
    {
        _actor = !ActorPath.IsEmpty ? GetNodeOrNull<PlayerActor>(ActorPath) : null;
        _label = GetNodeOrNull<Label>(LabelPath);
    }

    public override void _Process(double delta)
    {
        if (_actor == null || _label == null)
        {
            return;
        }

        var runtime = _actor.RuntimeContext;
        _label.Text = string.Join("\n", new[]
        {
            $"Mode: {runtime.ControlMode}",
            $"Source: {runtime.ControlSourceName}",
            $"Motion: {runtime.ActiveMotionModel}",
            $"Action: {runtime.ActiveActionCoordinator}",
            $"Anim: {runtime.ActiveAnimationDriver}",
            $"Locomotion: {runtime.LocomotionState} / {runtime.StanceMode}",
            $"Grounded: {runtime.IsGrounded}  Landed: {runtime.JustLanded}",
            $"Speed: {runtime.HorizontalSpeed:F2}  Ratio: {runtime.SpeedRatio:F2}",
            $"Local Vel: {runtime.LocalPlanarVelocity}",
            $"View Yaw/Pitch: {Mathf.RadToDeg(runtime.ViewYaw):F1} / {Mathf.RadToDeg(runtime.ViewPitch):F1}",
            $"Fire: {runtime.WantsFire}  Reload: {runtime.WantsReload}",
            $"Command Point: {runtime.CommandPoint}"
        });
    }
}
