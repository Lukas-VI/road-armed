using Godot;

namespace BBBN.Character.Core
{
    public interface ICharacterRuntimeData
    {
        Vector3 Velocity { get; set; }

        bool IsGrounded { get; set; }

        float DeltaTime { get; set; }
    }
}
