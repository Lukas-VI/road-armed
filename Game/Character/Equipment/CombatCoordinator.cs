using Godot;
using RoadArmed.Game.Combat;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public partial class CombatCoordinator : Node
{
    [Export]
    public NodePath InventoryPath { get; set; } = new("../Inventory");

    [Export]
    public NodePath OverrideCoordinatorPath { get; set; } = new("../OverrideCoordinator");

    [Export]
    public float DebugShotDistance { get; set; } = 60f;

    private CharacterRuntimeContext? _runtime;
    private InventoryComponent? _inventory;
    private AnimationOverrideCoordinator? _overrideCoordinator;
    private float _fireCooldown;
    private float _reloadRemaining;

    public string DebugName => nameof(CombatCoordinator);

    public void Setup(CharacterRuntimeContext runtime)
    {
        _runtime = runtime;
        _inventory = GetNodeOrNull<InventoryComponent>(InventoryPath);
        _overrideCoordinator = GetNodeOrNull<AnimationOverrideCoordinator>(OverrideCoordinatorPath);
        runtime.ActiveCombatCoordinator = DebugName;
        runtime.EquippedItemName = "Unarmed";
    }

    public void Process(ActorIntent intent, double delta)
    {
        if (_runtime == null)
        {
            return;
        }

        _inventory?.Process(intent);
        _overrideCoordinator?.Process(intent, delta);

        _fireCooldown = Mathf.Max(0f, _fireCooldown - (float)delta);
        _reloadRemaining = Mathf.Max(0f, _reloadRemaining - (float)delta);
        _runtime.ReloadRemaining = _reloadRemaining;
        _runtime.ActiveCombatCoordinator = DebugName;

        if (_reloadRemaining > 0f)
        {
            _runtime.IsReloading = true;
            _runtime.WantsFire = false;
            return;
        }

        _runtime.IsReloading = false;
        _runtime.CanFire = CanFireCurrentWeapon();

        if (intent.WantsReload)
        {
            BeginReload();
            return;
        }

        if (intent.WantsFire && _runtime.CanFire && _fireCooldown <= 0f)
        {
            FireCurrentWeapon(intent);
        }
    }

    private bool CanFireCurrentWeapon()
    {
        return _inventory?.EquippedItem is WeaponDefinition;
    }

    private void BeginReload()
    {
        if (_inventory?.EquippedItem is not WeaponDefinition weapon || _runtime == null)
        {
            return;
        }

        _reloadRemaining = weapon.ReloadDuration;
        _runtime.IsReloading = true;
        _runtime.ActionPhase = ActionPhase.Reloading;
        _overrideCoordinator?.RequestOverride(weapon.ReloadAnimationKey, weapon.ReloadDuration);
    }

    private void FireCurrentWeapon(ActorIntent intent)
    {
        if (_inventory?.EquippedItem is not WeaponDefinition weapon || _runtime == null)
        {
            return;
        }

        _fireCooldown = weapon.FireRate > 0.001f ? 1f / weapon.FireRate : 0f;
        _runtime.IsFiring = true;
        _runtime.ActionPhase = ActionPhase.Firing;
        _runtime.LastFiredItemId = weapon.ItemId;
        _runtime.LastFireAnimationKey = weapon.FireAnimationKey;
        ResolveShot(intent);
    }

    private void ResolveShot(ActorIntent intent)
    {
        if (_runtime == null)
        {
            return;
        }

        Vector3 from = _runtime.WeaponMuzzlePosition;
        Vector3 direction = intent.AimDirection;
        if (direction.LengthSquared() <= 0.0001f)
        {
            direction = Vector3.Forward;
        }
        direction = direction.Normalized();
        Vector3 to = from + direction * DebugShotDistance;

        _runtime.LastShotBlocked = false;
        _runtime.LastShotHitCollider = string.Empty;
        _runtime.LastShotHitPosition = to;

        if (GetParent() is not Node3D worldNode)
        {
            return;
        }

        var world3D = worldNode.GetWorld3D();
        if (world3D == null)
        {
            return;
        }

        var query = PhysicsRayQueryParameters3D.Create(from, to);
        if (worldNode is CollisionObject3D selfBody)
        {
            query.Exclude = new Godot.Collections.Array<Rid> { selfBody.GetRid() };
        }

        var result = world3D.DirectSpaceState.IntersectRay(query);
        if (result.Keys.Count == 0)
        {
            return;
        }

        if (result.TryGetValue("position", out Variant hitPosition))
        {
            _runtime.LastShotHitPosition = hitPosition.AsVector3();
        }

        if (result.TryGetValue("collider", out Variant colliderVariant) && colliderVariant.Obj is Node hitNode)
        {
            _runtime.LastShotHitCollider = hitNode.Name;
            _runtime.LastShotBlocked = true;
        }
    }
}
