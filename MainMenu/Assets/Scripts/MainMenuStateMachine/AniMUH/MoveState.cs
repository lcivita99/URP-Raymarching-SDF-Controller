using UnityEngine;

namespace State_Machines.AniMUH
{
    public class MoveState : BaseState
    {
        public override void EnterState(StateManager manager, float number, Vector3 vec, string str)
        {
            Debug.Log("entered move state");
        }

        public override void UpdateState(StateManager manager)
        {
            // Debug.Log(manager.TotalMoveInput().magnitude);
            if (manager.TotalMoveInput().magnitude <= Utils.minimumInputValue)
            {
                manager.SwitchState(manager.IdleState);
            }
            
            if (manager.AnyJumpButtonPressedThisFrame() && manager.isGrounded)
            {
                manager.SwitchState(manager.JumpState);
            }
        }

        public override void FixedUpdateState(StateManager manager)
        {
            manager.rb.AddForce(manager.TotalMoveInput() * manager.TotalMoveInput().magnitude.Map(Utils.minimumInputValue,
                1f,
                manager.curStats.minMoveSpeed, manager.curStats.maxMoveSpeed));
        }
        
        public override void ForcedOutOfState(StateManager manager)
        {

        }


        // we dedicate this section to the 3D "sprite" animation
        public override void AnimUpdateState(StateManager manager)
        {

        }
    }
}