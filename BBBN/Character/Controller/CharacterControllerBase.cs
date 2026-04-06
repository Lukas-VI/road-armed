using BBBN.Core.StateMachine;
using BBBN.Character.Core;
using BBBN.Character.Input;

namespace BBBN.Character.Controller
{
    public abstract class CharacterControllerBase
    {
        protected readonly StateMachine<CharacterControllerBase> StateMachine;

        public IInputSource InputSource { get; }

        public ICharacterMotionDriver MotionDriver { get; }

        public ICharacterAnimationDriver AnimationDriver { get; }

        public ICharacterRuntimeData RuntimeData { get; }

        public float MoveSpeed { get; }

        public float SprintSpeed { get; }

        public float JumpStrength { get; }

        protected CharacterControllerBase(
            IInputSource inputSource,
            ICharacterMotionDriver motionDriver,
            ICharacterAnimationDriver animationDriver,
            ICharacterRuntimeData runtimeData,
            float moveSpeed,
            float sprintSpeed,
            float jumpStrength)
        {
            InputSource = inputSource;
            MotionDriver = motionDriver;
            AnimationDriver = animationDriver;
            RuntimeData = runtimeData;
            MoveSpeed = moveSpeed;
            SprintSpeed = sprintSpeed;
            JumpStrength = jumpStrength;
            StateMachine = new StateMachine<CharacterControllerBase>();
        }

        public void Initialize(BaseState<CharacterControllerBase> initialState)
        {
            StateMachine.Initialize(initialState, this);
        }

        public void Update(float delta)
        {
            InputSource.Update(delta);
            RuntimeData.DeltaTime = delta;
            RuntimeData.IsGrounded = MotionDriver.IsGrounded;
            StateMachine.Update(this, delta);
            AnimationDriver.SetMovementSpeed(RuntimeData.Velocity.Length());
            AnimationDriver.SetIsJumping(!RuntimeData.IsGrounded);
            AnimationDriver.SetIsSprinting(InputSource.Sprinting);
            AnimationDriver.SetAction(InputSource.ActionPressed);
        }

        public void PhysicsUpdate(float delta)
        {
            MotionDriver.ApplyGravity(delta);
            StateMachine.PhysicsUpdate(this, delta);
            MotionDriver.Move(RuntimeData.Velocity, delta);
        }

        public void ChangeState(BaseState<CharacterControllerBase> nextState)
        {
            StateMachine.ChangeState(nextState, this);
        }
    }
}
