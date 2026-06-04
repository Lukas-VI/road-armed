using System.Linq;
using Godot;
using RoadArmed.Game.Combat;

namespace RoadArmed.Game.Character.Components;

public partial class EquipmentVisualDriver : Node3D
{
    [Export]
    public NodePath InventoryPath { get; set; } = new("../Inventory");

    [Export]
    public NodePath SocketsPath { get; set; } = new("../EquipmentSockets");

    private InventoryComponent? _inventory;
    private EquipmentSockets? _sockets;
    private string _equippedItemId = string.Empty;
    private Node3D? _currentVisual;
    private CharacterRuntimeContext? _runtime;
    private WeaponMuzzleBinding? _muzzleBinding;

    public void Setup(CharacterRuntimeContext runtime)
    {
        _runtime = runtime;
        _inventory = GetNodeOrNull<InventoryComponent>(InventoryPath);
        _sockets = GetNodeOrNull<EquipmentSockets>(SocketsPath);
    }

    public void SyncVisual()
    {
        _inventory ??= GetNodeOrNull<InventoryComponent>(InventoryPath);
        _sockets ??= GetNodeOrNull<EquipmentSockets>(SocketsPath);
        string itemId = _inventory?.EquippedItem?.ItemId ?? string.Empty;
        if (_equippedItemId != itemId)
        {
            _equippedItemId = itemId;
            RebuildVisual();
        }

        UpdateMuzzleProxy();
        PublishMuzzleState();
    }

    private void RebuildVisual()
    {
        if (_currentVisual != null)
        {
            _currentVisual.QueueFree();
            _currentVisual = null;
            _muzzleBinding = null;
        }

        if (_inventory?.EquippedItem?.WorldModelScene == null)
        {
            PublishMuzzleState();
            return;
        }

        Node3D? parentSocket = _sockets?.RightHandSocket ?? _sockets;
        if (parentSocket == null)
        {
            PublishMuzzleState();
            return;
        }

        if (_inventory.EquippedItem.WorldModelScene.Instantiate() is not Node3D visual)
        {
            PublishMuzzleState();
            return;
        }

        visual.Name = "EquippedVisual";
        parentSocket.AddChild(visual);
        visual.Owner = GetTree().EditedSceneRoot ?? this;
        visual.Position = _inventory.EquippedItem.HoldPositionOffset;
        visual.RotationDegrees = _inventory.EquippedItem.HoldRotationDegrees;
        visual.Scale = _inventory.EquippedItem.HoldScale;
        _currentVisual = visual;
        _muzzleBinding = BuildMuzzleBinding(visual, _inventory.EquippedItem);
        GD.Print($"[EquipmentVisualDriver] Equipped {_inventory.EquippedItem.DisplayName} on {parentSocket.GetPath()} muzzle={(_muzzleBinding != null ? _muzzleBinding.Proxy.GetPath().ToString() : "None")}");
        PublishMuzzleState();
    }

    private WeaponMuzzleBinding? BuildMuzzleBinding(Node3D weaponRoot, EquippableItemDefinition item)
    {
        string expectedName = item.MuzzleSocketName.ToString();
        Node3D? sourceNode = FindNode3DRecursive(weaponRoot, expectedName, "muzzle");
        Skeleton3D? skeleton = FindSkeletonRecursive(weaponRoot);
        int boneIndex = -1;
        string boneName = string.Empty;

        if (sourceNode == null && skeleton != null)
        {
            boneName = FindBoneName(skeleton, expectedName, "muzzle") ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(boneName))
            {
                boneIndex = skeleton.FindBone(boneName);
            }
        }

        var proxy = new Node3D
        {
            Name = "WeaponMuzzleProxy",
            TopLevel = true,
            Position = weaponRoot.GlobalPosition
        };
        AddChild(proxy);
        proxy.Owner = GetTree().EditedSceneRoot ?? this;

        if (item is WeaponDefinition weapon)
        {
            proxy.Position = weaponRoot.ToGlobal(weapon.MuzzleFallbackPosition);
            proxy.RotationDegrees = weapon.MuzzleFallbackRotationDegrees;
        }

        return new WeaponMuzzleBinding(sourceNode, skeleton, boneIndex, boneName, proxy, item as WeaponDefinition);
    }

    private void UpdateMuzzleProxy()
    {
        if (_muzzleBinding == null)
        {
            return;
        }

        if (_muzzleBinding.SourceNode != null && IsInstanceValid(_muzzleBinding.SourceNode))
        {
            _muzzleBinding.Proxy.GlobalTransform = _muzzleBinding.SourceNode.GlobalTransform;
            return;
        }

        if (_muzzleBinding.Skeleton != null && _muzzleBinding.BoneIndex >= 0)
        {
            Transform3D bonePose = _muzzleBinding.Skeleton.GetBoneGlobalPose(_muzzleBinding.BoneIndex);
            _muzzleBinding.Proxy.GlobalTransform = _muzzleBinding.Skeleton.GlobalTransform * bonePose;
            return;
        }

        if (_currentVisual != null && _muzzleBinding.WeaponDefinition != null)
        {
            _muzzleBinding.Proxy.GlobalPosition = _currentVisual.ToGlobal(_muzzleBinding.WeaponDefinition.MuzzleFallbackPosition);
            _muzzleBinding.Proxy.RotationDegrees = _currentVisual.GlobalRotationDegrees + _muzzleBinding.WeaponDefinition.MuzzleFallbackRotationDegrees;
        }
    }

    private void PublishMuzzleState()
    {
        if (_runtime == null)
        {
            return;
        }

        _runtime.WeaponMuzzlePath = _muzzleBinding != null ? _muzzleBinding.Proxy.GetPath().ToString() : string.Empty;
        _runtime.WeaponMuzzlePosition = _muzzleBinding != null ? _muzzleBinding.Proxy.GlobalPosition : Vector3.Zero;
    }

    private static Node3D? FindNode3DRecursive(Node root, params string[] keys)
    {
        var expected = keys.Select(key => key.ToLowerInvariant()).ToArray();

        foreach (Node child in root.GetChildren())
        {
            if (child is Node3D node3D)
            {
                string current = child.Name.ToString().ToLowerInvariant();
                if (expected.Any(key => current == key || current.Contains(key)))
                {
                    return node3D;
                }
            }

            var nested = FindNode3DRecursive(child, keys);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
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

    private static string? FindBoneName(Skeleton3D skeleton, params string[] keys)
    {
        var expected = keys.Select(key => key.ToLowerInvariant()).ToArray();

        for (int i = 0; i < skeleton.GetBoneCount(); i++)
        {
            string boneName = skeleton.GetBoneName(i).ToString();
            string current = boneName.ToLowerInvariant();
            if (expected.Any(key => current == key || current.Contains(key)))
            {
                return boneName;
            }
        }

        return null;
    }

    private sealed class WeaponMuzzleBinding
    {
        public WeaponMuzzleBinding(Node3D? sourceNode, Skeleton3D? skeleton, int boneIndex, string boneName, Node3D proxy, WeaponDefinition? weaponDefinition)
        {
            SourceNode = sourceNode;
            Skeleton = skeleton;
            BoneIndex = boneIndex;
            BoneName = boneName;
            Proxy = proxy;
            WeaponDefinition = weaponDefinition;
        }

        public Node3D? SourceNode { get; }
        public Skeleton3D? Skeleton { get; }
        public int BoneIndex { get; }
        public string BoneName { get; }
        public Node3D Proxy { get; }
        public WeaponDefinition? WeaponDefinition { get; }
    }
}

