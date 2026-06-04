using Godot;
using RoadArmed.Game.Camera;
using RoadArmed.Game.Character.Components;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Nodes;

public partial class PlayerActor : CharacterBody3D
{
    [Export]
    public NodePath VisualRootPath { get; set; } = new("VisualRoot");

    [Export]
    public NodePath PlaceholderVisualPath { get; set; } = new("VisualRoot/BodyMesh");

    [Export]
    public NodePath ControlSourcePath { get; set; } = new("InputSource");

    [Export]
    public NodePath MotionRouterPath { get; set; } = new("MotionRouter");

    [Export]
    public NodePath ActionCoordinatorPath { get; set; } = new("ActionCoordinator");

    [Export]
    public NodePath CombatCoordinatorPath { get; set; } = new("CombatCoordinator");

    [Export]
    public NodePath InventoryPath { get; set; } = new("Inventory");

    [Export]
    public NodePath EquipmentVisualDriverPath { get; set; } = new("EquipmentVisualDriver");

    [Export]
    public NodePath OverrideCoordinatorPath { get; set; } = new("OverrideCoordinator");

    [Export]
    public NodePath AnimationDriverPath { get; set; } = new("AnimationDriver");

    [Export]
    public NodePath CameraRigPath { get; set; } = new();

    private readonly CharacterRuntimeContext _runtime = new();
    private readonly ControlFrame _frame = new();
    private readonly ActorIntent _intent = new();

    private ControlSourceNode? _controlSource;
    private ActorMotionRouter? _motionRouter;
    private CharacterActionCoordinator? _actionCoordinator;
    private CombatCoordinator? _combatCoordinator;
    private InventoryComponent? _inventory;
    private EquipmentVisualDriver? _equipmentVisualDriver;
    private AnimationOverrideCoordinator? _overrideCoordinator;
    private ICharacterAnimationDriver? _animationDriver;
    private PlayerCameraRig? _cameraRig;

    public CharacterRuntimeContext RuntimeContext => _runtime;
    public ActorIntent CurrentIntent => _intent;

    public override void _Ready()
    {
        HidePlaceholderVisual();

        _controlSource = GetNodeOrNull<ControlSourceNode>(ControlSourcePath) ?? GetNodeOrNull<ControlSourceNode>("InputSource");
        _motionRouter = GetNodeOrNull<ActorMotionRouter>(MotionRouterPath) ?? GetNodeOrNull<ActorMotionRouter>("MotionRouter");
        _actionCoordinator = GetNodeOrNull<CharacterActionCoordinator>(ActionCoordinatorPath) ?? GetNodeOrNull<CharacterActionCoordinator>("ActionCoordinator");
        _combatCoordinator = GetNodeOrNull<CombatCoordinator>(CombatCoordinatorPath) ?? GetNodeOrNull<CombatCoordinator>("CombatCoordinator");
        _inventory = GetNodeOrNull<InventoryComponent>(InventoryPath) ?? GetNodeOrNull<InventoryComponent>("Inventory");
        _equipmentVisualDriver = GetNodeOrNull<EquipmentVisualDriver>(EquipmentVisualDriverPath) ?? GetNodeOrNull<EquipmentVisualDriver>("EquipmentVisualDriver");
        _overrideCoordinator = GetNodeOrNull<AnimationOverrideCoordinator>(OverrideCoordinatorPath) ?? GetNodeOrNull<AnimationOverrideCoordinator>("OverrideCoordinator");
        _animationDriver = GetNodeOrNull(AnimationDriverPath) as ICharacterAnimationDriver ?? GetNodeOrNull("AnimationDriver") as ICharacterAnimationDriver;
        _cameraRig = !CameraRigPath.IsEmpty ? GetNodeOrNull<PlayerCameraRig>(CameraRigPath) : null;

        _runtime.ControlMode = ControlMode.ThirdPerson;
        _runtime.BodyYaw = Rotation.Y;
        _runtime.ControlSourceName = ResolveControlSourceName();

        _inventory?.Setup(_runtime);
        _equipmentVisualDriver?.Setup(_runtime);
        _overrideCoordinator?.Setup(_runtime);
        _motionRouter?.Setup(this, _runtime);
        _actionCoordinator?.Setup(_runtime);
        _combatCoordinator?.Setup(_runtime);
        _animationDriver?.Setup(_runtime);
        _cameraRig?.Setup(this, _runtime);
    }

    public override void _Input(InputEvent @event)
    {
        _controlSource?.HandleInput(@event);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _controlSource?.HandleInput(@event);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_controlSource == null || _motionRouter == null || _actionCoordinator == null)
        {
            return;
        }

        _runtime.IsFiring = false;
        _controlSource.Capture(_runtime.ControlMode, _frame, delta);
        _runtime.ControlSourceName = ResolveControlSourceName();
        _runtime.ControlMode = _frame.RequestedMode;

        _cameraRig?.UpdateRig(_frame, delta);
        PlayerIntentProcessor.BuildIntent(_frame, _runtime, _cameraRig, _intent);
        _actionCoordinator.Process(_intent, delta);
        _combatCoordinator?.Process(_intent, delta);
        _equipmentVisualDriver?.SyncVisual();
        _motionRouter.Simulate(_intent, delta);
        _animationDriver?.Apply(_runtime, _intent, delta);
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new System.Collections.Generic.List<string>();

        if (_controlSource == null && GetNodeOrNull<ControlSourceNode>(ControlSourcePath) == null && GetNodeOrNull<ControlSourceNode>("InputSource") == null)
        {
            warnings.Add("PlayerActor requires a ControlSourceNode child or a valid ControlSourcePath.");
        }

        if (_motionRouter == null && GetNodeOrNull<ActorMotionRouter>(MotionRouterPath) == null && GetNodeOrNull<ActorMotionRouter>("MotionRouter") == null)
        {
            warnings.Add("PlayerActor requires an ActorMotionRouter child or a valid MotionRouterPath.");
        }

        if (_actionCoordinator == null && GetNodeOrNull<CharacterActionCoordinator>(ActionCoordinatorPath) == null && GetNodeOrNull<CharacterActionCoordinator>("ActionCoordinator") == null)
        {
            warnings.Add("PlayerActor requires a CharacterActionCoordinator child or a valid ActionCoordinatorPath.");
        }

        return warnings.ToArray();
    }

    private void HidePlaceholderVisual()
    {
        if (GetNodeOrNull<GeometryInstance3D>(PlaceholderVisualPath) is not GeometryInstance3D placeholder)
        {
            return;
        }

        var visualRoot = GetNodeOrNull<Node>(VisualRootPath);
        if (visualRoot != null && visualRoot.GetNodeOrNull<Node>("Model") != null)
        {
            placeholder.Visible = false;
        }
    }

    private string ResolveControlSourceName()
    {
        return _controlSource switch
        {
            ControlSourceRouter router => router.ActiveSourceName,
            null => "None",
            _ => _controlSource.GetType().Name
        };
    }
}
