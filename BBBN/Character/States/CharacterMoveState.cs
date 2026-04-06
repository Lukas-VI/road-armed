using Godot;
using BBBN.Character.Controller;
using BBBN.Character.Core;
using BBBN.Core.StateMachine;

namespace BBBN.Character.States
{
    public sealed class CharacterMoveState : BaseState<CharacterControllerBase>
    {
        public override string Name => "Move";

        public override void Enter(CharacterControllerBase context)
        {
            context.AnimationDriver.SetIsJumping(!context.MotionDriver.IsGrounded);
        }

        public override void Update(CharacterControllerBase context, float delta)
        {
            var inputDirection = context.InputSource.Movement;
            if (inputDirection.Length() <= 0.1f)
            {
                context.ChangeState(new CharacterIdleState());
                return;
            }

            var speed = context.InputSource.Sprinting ? context.SprintSpeed : context.MoveSpeed;
            var moveDirection = inputDirection.Normalized() * speed;

            context.RuntimeData.Velocity = new Vector3(
                moveDirection.X,
                context.MotionDriver.Velocity.Y,
                moveDirection.Z);

            if (context.InputSource.JumpPressed)
            {
                context.MotionDriver.Jump(context.JumpStrength);
            }
        }

        public override void PhysicsUpdate(CharacterControllerBase context, float delta)
        {
            context.MotionDriver.Move(context.RuntimeData.Velocity, delta);
        }
    }
}
