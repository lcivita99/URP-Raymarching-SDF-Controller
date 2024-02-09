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
            canExitState = false;
            manager.rb.AddForce(manager.curStats.jumpForce * Vector3.up + manager.TotalMoveInput(), ForceMode.Impulse);
            manager.StartCoroutine(JumpTimer());
        }

        public override void UpdateState(StateManager manager)
        {
            
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
        
        IEnumerator JumpTimer()
        {
            yield return new WaitForSeconds(timeBeforeCanExitState);
            canExitState = true;
        }
    }
}