using Godot;

namespace RoadArmed.Game.Combat;

[GlobalClass]
public partial class ItemDefinition : Resource
{
    [Export]
    public string ItemId { get; set; } = string.Empty;

    [Export]
    public string DisplayName { get; set; } = "Item";

    [Export]
    public Texture2D? Icon { get; set; }
}
