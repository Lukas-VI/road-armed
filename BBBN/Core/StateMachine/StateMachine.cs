namespace BBBN.Core.StateMachine
{
    public sealed class StateMachine<TContext>
    {
        public BaseState<TContext> CurrentState { get; private set; }

        public void Initialize(BaseState<TContext> initialState, TContext context)
        {
            CurrentState = initialState;
            CurrentState.Enter(context);
        }

        public void ChangeState(BaseState<TContext> nextState, TContext context)
        {
            if (nextState == CurrentState)
            {
                return;
            }

            CurrentState?.Exit(context);
            CurrentState = nextState;
            CurrentState.Enter(context);
        }

        public void Update(TContext context, float delta)
        {
            CurrentState?.Update(context, delta);
        }

        public void PhysicsUpdate(TContext context, float delta)
        {
            CurrentState?.PhysicsUpdate(context, delta);
        }
    }
}
