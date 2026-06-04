using Godot;
using System.Linq;

namespace RoadArmed.Game.Character.Components;

public partial class AssetDiagnostics : Node
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

        var root = !SearchRootPath.IsEmpty ? GetNodeOrNull<Node>(SearchRootPath) : null;
        if (root == null)
        {
            GD.Print("[AssetDiagnostics] Search root missing.");
            return;
        }

        GD.Print($"[AssetDiagnostics] Root: {root.Name} ({root.GetClass()})");

        foreach (var animationPlayer in FindChildrenOfType<AnimationPlayer>(root))
        {
            var clips = string.Join(", ", animationPlayer.GetAnimationList());
            GD.Print($"[AssetDiagnostics] AnimationPlayer: {animationPlayer.GetPath()} | Clips: {clips}");
        }

        foreach (var skeleton in FindChildrenOfType<Skeleton3D>(root))
        {
            var bones = Enumerable.Range(0, skeleton.GetBoneCount())
                .Select(i => skeleton.GetBoneName(i).ToString());
            GD.Print($"[AssetDiagnostics] Skeleton: {skeleton.GetPath()} | Bones: {string.Join(", ", bones)}");
        }

        foreach (Node3D node in FindChildrenOfType<Node3D>(root))
        {
            string lower = node.Name.ToString().ToLowerInvariant();
            if (lower.Contains("socket") || lower.Contains("muzzle") || lower.Contains("camera"))
            {
                GD.Print($"[AssetDiagnostics] Candidate node: {node.GetPath()} | Position: {node.Position}");
            }
        }
    }

    private static System.Collections.Generic.IEnumerable<T> FindChildrenOfType<T>(Node root) where T : Node
    {
        foreach (Node child in root.GetChildren())
        {
            if (child is T typed)
            {
                yield return typed;
            }

            foreach (var nested in FindChildrenOfType<T>(child))
            {
                yield return nested;
            }
        }
    }
}
