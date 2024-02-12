using UnityEngine;

namespace State_Machines.AniMUH
{
    public class IdleState : BaseState
    {

        public override void EnterState(StateManager manager, float number, Vector3 vec, string str)
        {
            Debug.Log("entered idle state");
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
            
            
            // beetle
            StateManager.AniMuhID beetleID = StateManager.AniMuhID.Beetle;
            
            // chompers
            // left
            sdfTrans(manager, beetleID, 2).localPosition =
                manager.originalPositions[(int) beetleID][2] + Mathf.Sin(Time.timeSinceLevelLoad * 5) * Vector3.right/20;
            // right
            sdfTrans(manager, beetleID, 3).localPosition =
                manager.originalPositions[(int) beetleID][3] + Mathf.Sin(Time.timeSinceLevelLoad * 5) * Vector3.left/20;
            
            // legs
            // // left
            // sdfTrans(manager, beetleID, 0).localPosition =
            //     manager.originalPositions[(int) beetleID][0]
            //     + Mathf.Sin(Time.timeSinceLevelLoad * 20) * Vector3.forward/20
            //     + Mathf.Cos(Time.timeSinceLevelLoad * 20) * Vector3.up/20;
            // // right
            // sdfTrans(manager, beetleID, 1).localPosition =
            //     manager.originalPositions[(int) beetleID][1]
            //     + Mathf.Sin(Time.timeSinceLevelLoad * 20 + Mathf.PI/2) * Vector3.forward/20
            //     + Mathf.Cos(Time.timeSinceLevelLoad * 20 + Mathf.PI/2) * Vector3.up/20;
            
            // body
            sdfTrans(manager, beetleID, 5).localPosition =
                manager.originalPositions[(int) beetleID][5]
                + Mathf.Sin(Time.timeSinceLevelLoad * 3 + Mathf.PI/4) * Vector3.forward/35
                + Mathf.Cos(Time.timeSinceLevelLoad * 3 + Mathf.PI/4) * Vector3.up/20;

            // horce


            // deer


            // monkey

        }

        private Transform sdfTrans(StateManager manager, StateManager.AniMuhID aniMuh, int limbID)
        {
            return manager.metaShapes[(int)aniMuh].Item1[limbID];
        }
    }
}
