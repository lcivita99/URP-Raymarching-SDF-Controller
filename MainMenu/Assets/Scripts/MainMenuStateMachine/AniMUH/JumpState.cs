using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State_Machines.AniMUH
{
    public class JumpState : BaseState
    {
        public readonly float timeBeforeCanExitState = 0.2f;
        private bool canExitState;
        public override void EnterState(StateManager manager, float number, Vector3 vec, string str)
        {
            Debug.Log("entered jump state");
            
            canExitState = false;
            manager.rb.AddForce(manager.curStats.jumpForce * Vector3.up + manager.TotalMoveInput(), ForceMode.Impulse);
            manager.StartCoroutine(JumpTimer());
        }

        public override void UpdateState(StateManager manager)
        {
            if (canExitState && manager.isGrounded)
            {
                manager.SwitchState(manager.IdleState);
            }
        }

        public override void FixedUpdateState(StateManager manager)
        {
            manager.rb.AddForce(manager.TotalMoveInput() * manager.TotalMoveInput().magnitude.Map(Utils.minimumInputValue,
                1f,
                manager.curStats.jumpMinMoveSpeed, manager.curStats.jumpMaxMoveSpeed));
        }
        
        public override void ForcedOutOfState(StateManager manager)
        {
            
        }
        
        public override void AnimUpdateState(StateManager manager)
        {

        }
        
        IEnumerator JumpTimer()
        {
            yield return new WaitForSeconds(timeBeforeCanExitState);
            canExitState = true;
        }
    }
}