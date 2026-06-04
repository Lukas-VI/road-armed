using Godot;
using RoadArmed.Game.Character.Nodes;

namespace RoadArmed.Game.UI.Debug;

public partial class ControllerDebugOverlay : CanvasLayer
{
    [Export]
    public NodePath ActorPath { get; set; } = new();

    [Export]
    public NodePath OutputPath { get; set; } = new("Panel/Margin/DebugOutput");

    [Export]
    public NodePath CrosshairPath { get; set; } = new("Crosshair");

    private PlayerActor? _actor;
    private RichTextLabel? _output;
    private ColorRect? _crosshair;

    public override void _Ready()
    {
        _actor = !ActorPath.IsEmpty ? GetNodeOrNull<PlayerActor>(ActorPath) : null;
        _output = GetNodeOrNull<RichTextLabel>(OutputPath);
        _crosshair = GetNodeOrNull<ColorRect>(CrosshairPath);
    }

    public override void _Process(double delta)
    {
        if (_actor == null || _output == null)
        {
            return;
        }

        var runtime = _actor.RuntimeContext;
        _output.Text = string.Join("\n", new[]
        {
            $"Mode: {runtime.ControlMode}",
            $"Source: {runtime.ControlSourceName}",
            $"Motion: {runtime.ActiveMotionModel}",
            $"Action: {runtime.ActiveActionCoordinator}",
            $"Combat: {runtime.ActiveCombatCoordinator}",
            $"Anim: {runtime.ActiveAnimationDriver}",
            $"Clip: {runtime.ActiveAnimationClip}",
            $"Locomotion: {runtime.LocomotionState} / {runtime.StanceMode}",
            $"Grounded: {runtime.IsGrounded}  Landed: {runtime.JustLanded}",
            $"Speed: {runtime.HorizontalSpeed:F2}  Ratio: {runtime.SpeedRatio:F2}",
            $"Local Vel: {runtime.LocalPlanarVelocity}",
            $"Equipped: [{runtime.EquippedSlotIndex}] {runtime.EquippedItemName}",
            $"Fire: {runtime.WantsFire}  CanFire: {runtime.CanFire}  Reloading: {runtime.IsReloading}",
            $"Muzzle: {runtime.WeaponMuzzlePath}",
            $"Muzzle Pos: {runtime.WeaponMuzzlePosition}",
            $"Shot Hit: {runtime.LastShotHitPosition}",
            $"Hit Collider: {runtime.LastShotHitCollider}",
            $"Blocked: {runtime.LastShotBlocked}",
            $"Override: {runtime.OverrideLayerName} ({runtime.OverrideRemainingTime:F2}s)",
            $"Last Fire: {runtime.LastFiredItemId} / {runtime.LastFireAnimationKey}"
        });

        if (_crosshair != null)
        {
            var viewportRect = GetViewport().GetVisibleRect();
            Vector2 center = viewportRect.Size * 0.5f;
            _crosshair.Position = center - (_crosshair.Size * 0.5f);
        }
    }
}
