using Godot;
using RoadArmed.Game.Shared;

namespace RoadArmed.Game.Character.Components;

public partial class EquipmentSockets : Node3D
{
    [Export]
    public bool DebugDrawSockets { get; set; } = true;

    [Export]
    public float DebugMarkerRadius { get; set; } = 0.08f;

    [Export]
    public NodePath SearchRootPath { get; set; } = new("../VisualRoot/Model");

    [Export]
    public NodePath RightHandSocketPath { get; set; } = new(AssetNaming.CharacterSockets.RightHand);

    [Export]
    public NodePath LeftHandSocketPath { get; set; } = new(AssetNaming.CharacterSockets.LeftHand);

    [Export]
    public NodePath MuzzleSocketPath { get; set; } = new($"{AssetNaming.CharacterSockets.RightHand}/{AssetNaming.CharacterSockets.Muzzle}");

    [Export]
    public NodePath CameraAnchorPath { get; set; } = new(AssetNaming.CharacterSockets.Camera);

    private Node? _searchRoot;
    private Skeleton3D? _skeleton;
    private SocketBinding? _rightHand;
    private SocketBinding? _leftHand;
    private SocketBinding? _muzzle;
    private SocketBinding? _camera;

    public Node3D? RightHandSocket => _rightHand?.Proxy;
    public Node3D? LeftHandSocket => _leftHand?.Proxy;
    public Node3D? MuzzleSocket => _muzzle?.Proxy;
    public Node3D? CameraAnchor => _camera?.Proxy;

    public override void _Ready()
    {
        _searchRoot = !SearchRootPath.IsEmpty ? GetNodeOrNull<Node>(SearchRootPath) : this;
        _skeleton = _searchRoot != null ? FindSkeletonRecursive(_searchRoot) : null;

        _rightHand = ResolveSocket(RightHandSocketPath, AssetNaming.CharacterSockets.RightHand, new Color(0.9f, 0.2f, 0.2f));
        _leftHand = ResolveSocket(LeftHandSocketPath, AssetNaming.CharacterSockets.LeftHand, new Color(0.2f, 0.9f, 0.2f));
        _muzzle = ResolveSocket(MuzzleSocketPath, AssetNaming.CharacterSockets.Muzzle, new Color(1f, 0.8f, 0.1f));
        _camera = ResolveSocket(CameraAnchorPath, AssetNaming.CharacterSockets.Camera, new Color(0.2f, 0.7f, 1f));

        GD.Print($"[EquipmentSockets] SearchRoot: {(_searchRoot != null ? _searchRoot.GetPath().ToString() : "None")}");
        GD.Print($"[EquipmentSockets] Skeleton: {(_skeleton != null ? _skeleton.GetPath().ToString() : "None")}");
        LogBinding("RightHand", _rightHand);
        LogBinding("LeftHand", _leftHand);
        LogBinding("Muzzle", _muzzle);
        LogBinding("Camera", _camera);
    }

    public override void _Process(double delta)
    {
        UpdateBinding(_rightHand);
        UpdateBinding(_leftHand);
        UpdateBinding(_muzzle);
        UpdateBinding(_camera);
    }

    private SocketBinding? ResolveSocket(NodePath directPath, string fallbackName, Color debugColor)
    {
        if (_searchRoot == null)
        {
            return null;
        }

        Node3D? directNode = null;
        if (!directPath.IsEmpty)
        {
            directNode = GetNodeOrNull<Node3D>(directPath);
        }

        directNode ??= FindDescendantByName(_searchRoot, fallbackName);

        int boneIndex = -1;
        string boneName = string.Empty;
        if (_skeleton != null)
        {
            boneName = FindBoneName(_skeleton, fallbackName) ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(boneName))
            {
                boneIndex = _skeleton.FindBone(boneName);
            }
        }

        if (directNode == null && boneIndex < 0)
        {
            return null;
        }

        var proxy = new Node3D
        {
            Name = $"{fallbackName}_proxy",
            TopLevel = true
        };
        AddChild(proxy);
        proxy.Owner = GetTree().EditedSceneRoot ?? this;

        if (DebugDrawSockets)
        {
            AddDebugMarker(proxy, debugColor);
        }

        return new SocketBinding(fallbackName, directNode, boneIndex, boneName, proxy);
    }

    private void UpdateBinding(SocketBinding? binding)
    {
        if (binding == null)
        {
            return;
        }

        if (binding.SourceNode != null && IsInstanceValid(binding.SourceNode))
        {
            binding.Proxy.GlobalTransform = binding.SourceNode.GlobalTransform;
            return;
        }

        if (_skeleton != null && binding.BoneIndex >= 0)
        {
            Transform3D bonePose = _skeleton.GetBoneGlobalPose(binding.BoneIndex);
            binding.Proxy.GlobalTransform = _skeleton.GlobalTransform * bonePose;
        }
    }

    private void AddDebugMarker(Node3D socket, Color color)
    {
        if (socket.GetNodeOrNull<MeshInstance3D>("DebugMarker") != null)
        {
            return;
        }

        var mesh = new MeshInstance3D
        {
            Name = "DebugMarker",
            Mesh = new SphereMesh
            {
                Radius = DebugMarkerRadius,
                Height = DebugMarkerRadius * 2f
            }
        };

        var material = new StandardMaterial3D
        {
            AlbedoColor = color,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            NoDepthTest = true
        };
        mesh.MaterialOverride = material;
        socket.AddChild(mesh);
        mesh.Owner = GetTree().EditedSceneRoot ?? this;
    }

    private static Skeleton3D? FindSkeletonRecursive(Node root)
    {
        foreach (Node child in root.GetChildren())
        {
            if (child is Skeleton3D skeleton)
            {
                return skeleton;
            }

            var nested = FindSkeletonRecursive(child);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

    private static string? FindBoneName(Skeleton3D skeleton, string expectedName)
    {
        string expected = expectedName.ToLowerInvariant();

        for (int i = 0; i < skeleton.GetBoneCount(); i++)
        {
            string boneName = skeleton.GetBoneName(i).ToString();
            string current = boneName.ToLowerInvariant();
            if (current == expected || current.Contains(expected))
            {
                return boneName;
            }
        }

        return null;
    }

    private static Node3D? FindDescendantByName(Node root, string nodeName)
    {
        string expected = nodeName.ToLowerInvariant();

        foreach (Node child in root.GetChildren())
        {
            if (child is Node3D node3D)
            {
                string current = child.Name.ToString().ToLowerInvariant();
                if (current == expected || current.Contains(expected))
                {
                    return node3D;
                }
            }

            var nested = FindDescendantByName(child, nodeName);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

    private static void LogBinding(string label, SocketBinding? binding)
    {
        if (binding == null)
        {
            GD.Print($"[EquipmentSockets] {label}: None");
            return;
        }

        string source = binding.SourceNode != null ? binding.SourceNode.GetPath().ToString() : $"bone:{binding.BoneName}";
        GD.Print($"[EquipmentSockets] {label}: source={source} proxy={binding.Proxy.GetPath()}");
    }

    private sealed class SocketBinding
    {
        public SocketBinding(string socketName, Node3D? sourceNode, int boneIndex, string boneName, Node3D proxy)
        {
            SocketName = socketName;
            SourceNode = sourceNode;
            BoneIndex = boneIndex;
            BoneName = boneName;
            Proxy = proxy;
        }

        public string SocketName { get; }
        public Node3D? SourceNode { get; }
        public int BoneIndex { get; }
        public string BoneName { get; }
        public Node3D Proxy { get; }
    }
}

