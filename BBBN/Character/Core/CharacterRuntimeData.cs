using Godot;

namespace BBBN.Character.Core
{
    public sealed class CharacterRuntimeData : ICharacterRuntimeData
    {
        public Vector3 Velocity { get; set; } = Vector3.Zero;

        public bool IsGrounded { get; set; }

        public float DeltaTime { get; set; }
    }
}
