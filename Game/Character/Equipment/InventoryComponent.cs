using Godot;
using RoadArmed.Game.Combat;
using RoadArmed.Game.Input;

namespace RoadArmed.Game.Character.Components;

public partial class InventoryComponent : Node
{
    [Export]
    public Godot.Collections.Array<EquippableItemDefinition> HotbarDefinitions { get; set; } = new();

    private CharacterRuntimeContext? _runtime;

    public int EquippedSlotIndex { get; private set; } = -1;
    public EquippableItemDefinition? EquippedItem => EquippedSlotIndex >= 0 && EquippedSlotIndex < HotbarDefinitions.Count
        ? HotbarDefinitions[EquippedSlotIndex]
        : null;

    public void Setup(CharacterRuntimeContext runtime)
    {
        _runtime = runtime;
        PublishEquippedState();
    }

    public void Process(ActorIntent intent)
    {
        if (intent.HotbarSlotRequested < 0)
        {
            return;
        }

        if (intent.HotbarSlotRequested == EquippedSlotIndex)
        {
            EquippedSlotIndex = -1;
        }
        else if (intent.HotbarSlotRequested < HotbarDefinitions.Count)
        {
            EquippedSlotIndex = intent.HotbarSlotRequested;
        }

        PublishEquippedState();
    }

    private void PublishEquippedState()
    {
        if (_runtime == null)
        {
            return;
        }

        _runtime.EquippedSlotIndex = EquippedSlotIndex;
        _runtime.EquippedItemId = EquippedItem?.ItemId ?? string.Empty;
        _runtime.EquippedItemName = EquippedItem?.DisplayName ?? "Unarmed";
    }
}
