using Godot;
using BBBN.Character.Controller;
using BBBN.Character.Core;
using BBBN.Character.Input;
using BBBN.Core.StateMachine;

namespace BBBN.Character.States
{
    public sealed class CharacterIdleState : BaseState<CharacterControllerBase>
    {
        public override string Name => "Idle";

        public override void Enter(CharacterControllerBase context)
        {
            context.RuntimeData.Velocity = new Vector3(0f, context.RuntimeData.Velocity.Y, 0f);
            context.AnimationDriver.SetIsJumping(!context.MotionDriver.IsGrounded);
        }

        public override void Update(CharacterControllerBase context, float delta)
        {
            if (context.InputSource.JumpPressed)
            {
                context.MotionDriver.Jump(context.JumpStrength);
                context.ChangeState(new CharacterMoveState());
                return;
            }

            if (context.InputSource.Movement.Length() > 0.1f)
            {
                context.ChangeState(new CharacterMoveState());
            }
        }
    }
}
