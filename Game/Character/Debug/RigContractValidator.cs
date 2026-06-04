using Godot;
using RoadArmed.Game.Shared;

namespace RoadArmed.Game.Character.Components;

public partial class RigContractValidator : Node
{
    [Export]
    public NodePath SkeletonPath { get; set; } = new();

    [Export]
    public NodePath SearchRootPath { get; set; } = new("../VisualRoot");

    [Export]
    public bool ValidateOnReady { get; set; } = true;

    [Export]
    public bool RequireSocketsNode { get; set; } = true;

    [Export]
    public NodePath SocketsPath { get; set; } = new("../EquipmentSockets");

    public override void _Ready()
    {
        if (ValidateOnReady)
        {
            ValidateContract();
        }
    }

    public string[] ValidateContract()
    {
        var issues = new System.Collections.Generic.List<string>();
        var skeleton = ResolveSkeleton();

        if (skeleton == null)
        {
            issues.Add("Missing Skeleton3D reference for rig validation.");
            EmitIssues(issues);
            return issues.ToArray();
        }

        string[] requiredBones =
        {
            AssetNaming.CharacterBones.Root,
            AssetNaming.CharacterBones.Hips,
            AssetNaming.CharacterBones.Spine,
            AssetNaming.CharacterBones.Chest,
            AssetNaming.CharacterBones.Head,
            AssetNaming.CharacterBones.LeftHand,
            AssetNaming.CharacterBones.RightHand,
            AssetNaming.CharacterBones.LeftFoot,
            AssetNaming.CharacterBones.RightFoot
        };

        foreach (string boneName in requiredBones)
        {
            if (skeleton.FindBone(boneName) < 0)
            {
                issues.Add($"Missing required bone: {boneName}");
            }
        }

        if (RequireSocketsNode)
        {
            var socketsRoot = !SocketsPath.IsEmpty ? GetNodeOrNull<Node>(SocketsPath) : null;

            string[] requiredSockets =
            {
                AssetNaming.CharacterSockets.RightHand,
                AssetNaming.CharacterSockets.LeftHand,
                AssetNaming.CharacterSockets.Muzzle,
                AssetNaming.CharacterSockets.Camera
            };

            foreach (string socketName in requiredSockets)
            {
                bool foundAsNode = socketsRoot != null && FindDescendantByName(socketsRoot, socketName) != null;
                bool foundAsBone = skeleton.FindBone(socketName) >= 0;

                if (!foundAsNode && !foundAsBone)
                {
                    issues.Add($"Missing required socket: {socketName}");
                }
            }
        }

        EmitIssues(issues);
        return issues.ToArray();
    }

    private Skeleton3D? ResolveSkeleton()
    {
        if (!SkeletonPath.IsEmpty)
        {
            var direct = GetNodeOrNull<Skeleton3D>(SkeletonPath);
            if (direct != null)
            {
                return direct;
            }
        }

        var searchRoot = !SearchRootPath.IsEmpty ? GetNodeOrNull<Node>(SearchRootPath) : this;
        return searchRoot != null ? FindDescendantSkeleton(searchRoot) : null;
    }

    private static Skeleton3D? FindDescendantSkeleton(Node root)
    {
        foreach (Node child in root.GetChildren())
        {
            if (child is Skeleton3D skeleton)
            {
                return skeleton;
            }

            var nested = FindDescendantSkeleton(child);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

    private static Node3D? FindDescendantByName(Node root, string nodeName)
    {
        foreach (Node child in root.GetChildren())
        {
            if (child is Node3D node3D && child.Name == nodeName)
            {
                return node3D;
            }

            var nested = FindDescendantByName(child, nodeName);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

    private void EmitIssues(System.Collections.Generic.List<string> issues)
    {
        if (issues.Count == 0)
        {
            GD.Print("[RigContractValidator] Rig contract validation passed.");
            return;
        }

        foreach (string issue in issues)
        {
            GD.PushWarning($"[RigContractValidator] {issue}");
        }
    }
}
