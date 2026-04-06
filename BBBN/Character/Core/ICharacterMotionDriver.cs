using Godot;

namespace BBBN.Character.Core
{
    public interface ICharacterMotionDriver
    {
        Vector3 Velocity { get; set; }

        bool IsGrounded { get; }

        void ApplyGravity(float delta);

        void Move(Vector3 velocity, float delta);

        void Jump(float strength);
    }
}
