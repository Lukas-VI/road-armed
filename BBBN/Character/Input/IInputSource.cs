using Godot;

namespace BBBN.Character.Input
{
    public interface IInputSource
    {
        Vector3 Movement { get; }

        bool JumpPressed { get; }

        bool Sprinting { get; }

        bool ActionPressed { get; }

        void Update(float delta);
    }
}
