using Godot;
using RoadArmed.Game.Character.Nodes;

namespace RoadArmed.Game.World;

public partial class WeaponDebugTracer : Node3D
{
    [Export]
    public NodePath ActorPath { get; set; } = new("../JjCharacter");

    [Export]
    public float Radius { get; set; } = 0.04f;

    private PlayerActor? _actor;
    private MeshInstance3D? _muzzleMarker;
    private MeshInstance3D? _hitMarker;
    private MeshInstance3D? _lineVisual;

    public override void _Ready()
    {
        _actor = !ActorPath.IsEmpty ? GetNodeOrNull<PlayerActor>(ActorPath) : null;
        _muzzleMarker = CreateMarker("MuzzleMarker", new Color(1f, 0.8f, 0.1f));
        _hitMarker = CreateMarker("HitMarker", new Color(1f, 0.2f, 1f));
        _lineVisual = new MeshInstance3D { Name = "ShotLine" };
        var lineMaterial = new StandardMaterial3D
        {
            AlbedoColor = new Color(1f, 0.5f, 0.1f),
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            NoDepthTest = true
        };
        _lineVisual.MaterialOverride = lineMaterial;
        AddChild(_lineVisual);
        _lineVisual.Owner = GetTree().EditedSceneRoot ?? this;
    }

    public override void _Process(double delta)
    {
        if (_actor == null || _muzzleMarker == null || _hitMarker == null || _lineVisual == null)
        {
            return;
        }

        var runtime = _actor.RuntimeContext;
        bool hasMuzzle = runtime.WeaponMuzzlePath.Length > 0;
        bool hasHit = runtime.LastShotHitPosition != Vector3.Zero;

        _muzzleMarker.Visible = hasMuzzle;
        _hitMarker.Visible = hasHit;

        if (hasMuzzle)
        {
            _muzzleMarker.GlobalPosition = runtime.WeaponMuzzlePosition;
        }

        if (hasHit)
        {
            _hitMarker.GlobalPosition = runtime.LastShotHitPosition;
        }

        if (hasMuzzle && hasHit)
        {
            Vector3 from = runtime.WeaponMuzzlePosition;
            Vector3 to = runtime.LastShotHitPosition;
            Vector3 deltaPos = to - from;
            float length = deltaPos.Length();
            if (length > 0.001f)
            {
                _lineVisual.Mesh = new CylinderMesh
                {
                    TopRadius = Radius * 0.5f,
                    BottomRadius = Radius * 0.5f,
                    Height = length
                };
                _lineVisual.Visible = true;
                _lineVisual.GlobalPosition = from + deltaPos * 0.5f;
                _lineVisual.LookAt(to, Vector3.Up);
                _lineVisual.RotateObjectLocal(Vector3.Right, Mathf.Pi * 0.5f);
            }
        }
        else
        {
            _lineVisual.Visible = false;
        }
    }

    private MeshInstance3D CreateMarker(string name, Color color)
    {
        var marker = new MeshInstance3D
        {
            Name = name,
            Mesh = new SphereMesh
            {
                Radius = Radius,
                Height = Radius * 2f
            }
        };
        var material = new StandardMaterial3D
        {
            AlbedoColor = color,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            NoDepthTest = true
        };
        marker.MaterialOverride = material;
        AddChild(marker);
        marker.Owner = GetTree().EditedSceneRoot ?? this;
        return marker;
    }
}
