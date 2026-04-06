using Godot;

namespace BBBN.Character.Input
{
    public abstract class InputSourceBase : IInputSource
    {
        public Vector3 Movement { get; protected set; } = Vector3.Zero;

        public bool JumpPressed { get; protected set; }

        public bool Sprinting { get; protected set; }

        public bool ActionPressed { get; protected set; }

        public virtual void Update(float delta)
        {
            JumpPressed = false;
            ActionPressed = false;
        }
    }
}
