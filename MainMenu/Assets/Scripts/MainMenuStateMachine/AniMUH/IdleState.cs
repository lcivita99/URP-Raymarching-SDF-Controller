using UnityEngine;

namespace State_Machines.AniMUH
{
    public class IdleState : BaseState
    {

        public override void EnterState(StateManager manager, float number, Vector3 vec, string str)
        {
            //Debug.Log("entered state");
        }

        public override void UpdateState(StateManager manager)
        {
            if (manager.TotalMoveInput().magnitude > Utils.minimumInputValue)
            {
                manager.SwitchState(manager.MoveState);
            }
            
            if (manager.AnyJumpButtonPressedThisFrame() && manager.isGrounded)
            {
                manager.SwitchState(manager.JumpState);
            }
        }

        public override void FixedUpdateState(StateManager manager)
        {

        }
        
        public override void ForcedOutOfState(StateManager manager)
        {

        }
        
        public override void AnimUpdateState(StateManager manager)
        {

        }
    }
}
