using UnityEngine;

namespace Player
{
    public abstract class PlayerState
    {
        protected PlayerController player;
        protected PlayerStateMachine stateMachine;
        protected string animBoolName;
        protected float startTime;

        public PlayerState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
        {
            this.player = player;
            this.stateMachine = stateMachine;
            this.animBoolName = animBoolName;
        }

        public virtual void Enter()
        {
            startTime = Time.time;
            if (!string.IsNullOrEmpty(animBoolName) && player.HasParameter(animBoolName))
            {
                player.Anim.SetBool(animBoolName, true);
            }
        }

        public virtual void Exit()
        {
            if (!string.IsNullOrEmpty(animBoolName) && player.HasParameter(animBoolName))
            {
                player.Anim.SetBool(animBoolName, false);
            }
        }

        public virtual void LogicUpdate()
        {
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void AnimationTrigger()
        {
        }

        public virtual void AnimationFinishTrigger()
        {
        }
    }
}
