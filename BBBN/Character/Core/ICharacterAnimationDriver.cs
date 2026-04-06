namespace BBBN.Character.Core
{
    public interface ICharacterAnimationDriver
    {
        void SetMovementSpeed(float speed);

        void SetIsSprinting(bool isSprinting);

        void SetIsJumping(bool isJumping);

        void SetAction(bool isActive);
    }
}
