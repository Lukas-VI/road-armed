using Godot;

namespace BBBN.Character.Core
{
    public sealed class GodotCharacterMotionDriver : ICharacterMotionDriver
    {
        private readonly CharacterBody3D _body;

        public GodotCharacterMotionDriver(CharacterBody3D body)
        {
            _body = body;
            Velocity = _body.Velocity;
        }

        public Vector3 Velocity { get; set; }

        public bool IsGrounded => _body.IsOnFloor();

        public void ApplyGravity(float delta)
        {
            if (!IsGrounded)
            {
                Velocity += Vector3.Down * ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle() * delta;
            }
            else if (Velocity.Y < 0)
            {
                Velocity = new Vector3(Velocity.X, 0f, Velocity.Z);
            }
        }

        public void Move(Vector3 velocity, float delta)
        {
            Velocity = velocity;
            _body.Velocity = Velocity;
            _body.MoveAndSlide();
        }

        public void Jump(float strength)
        {
            if (IsGrounded)
            {
                Velocity = new Vector3(Velocity.X, strength, Velocity.Z);
                _body.Velocity = Velocity;
            }
        }
    }
}
