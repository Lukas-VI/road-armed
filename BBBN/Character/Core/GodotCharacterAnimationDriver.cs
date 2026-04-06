using Godot;

namespace BBBN.Character.Core
{
    public sealed class GodotCharacterAnimationDriver : ICharacterAnimationDriver
    {
        private readonly AnimationTree _animationTree;

        public GodotCharacterAnimationDriver(AnimationTree animationTree)
        {
            _animationTree = animationTree;
        }

        public void SetMovementSpeed(float speed)
        {
            if (_animationTree == null)
            {
                return;
            }

            _animationTree.Set("parameters/movement_speed/scale", speed);
        }

        public void SetIsSprinting(bool isSprinting)
        {
            if (_animationTree == null)
            {
                return;
            }

            _animationTree.Set("parameters/is_sprinting/active", isSprinting);
        }

        public void SetIsJumping(bool isJumping)
        {
            if (_animationTree == null)
            {
                return;
            }

            _animationTree.Set("parameters/is_jumping/active", isJumping);
        }

        public void SetAction(bool isActive)
        {
            if (_animationTree == null)
            {
                return;
            }

            _animationTree.Set("parameters/action/active", isActive);
        }
    }
}
