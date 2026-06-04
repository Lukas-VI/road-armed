using Godot;

namespace RoadArmed.Game.Character.Debug;

public partial class AnimationTrackDiagnostics : Node
{
    [Export]
    public NodePath SearchRootPath { get; set; } = new("../VisualRoot/Model");

    [Export]
    public bool PrintOnReady { get; set; } = true;

    public override void _Ready()
    {
        if (!PrintOnReady)
        {
            return;
        }

        Node? root = !SearchRootPath.IsEmpty ? GetNodeOrNull<Node>(SearchRootPath) : null;
        if (root == null)
        {
            GD.Print("[AnimationTrackDiagnostics] Search root missing.");
            return;
        }

        var player = FindAnimationPlayer(root);
        if (player == null)
        {
            GD.Print("[AnimationTrackDiagnostics] AnimationPlayer missing.");
            return;
        }

        foreach (string clipName in player.GetAnimationList())
        {
            var animation = player.GetAnimation(clipName);
            if (animation == null)
            {
                continue;
            }

            var paths = new System.Collections.Generic.List<string>();
            for (int i = 0; i < animation.GetTrackCount(); i++)
            {
                paths.Add(animation.TrackGetPath(i).ToString());
            }

            GD.Print($"[AnimationTrackDiagnostics] {clipName}: {string.Join(" | ", paths)}");
        }
    }

    private static AnimationPlayer? FindAnimationPlayer(Node root)
    {
        foreach (Node child in root.GetChildren())
        {
            if (child is AnimationPlayer animationPlayer)
            {
                return animationPlayer;
            }

            var nested = FindAnimationPlayer(child);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }
}
