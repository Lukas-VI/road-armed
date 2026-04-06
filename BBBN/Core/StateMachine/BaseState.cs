namespace BBBN.Core.StateMachine
{
    public abstract class BaseState<TContext>
    {
        public abstract string Name { get; }

        public virtual void Enter(TContext context) { }

        public virtual void Exit(TContext context) { }

        public virtual void Update(TContext context, float delta) { }

        public virtual void PhysicsUpdate(TContext context, float delta) { }
    }
}
