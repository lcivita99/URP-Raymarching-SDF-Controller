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
            manager.moveAnimTimer += Time.deltaTime * manager.rb.velocity.magnitude / 3;
            
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
            // left
            sdfTrans(manager, beetleID, 0).localPosition =
                manager.originalPositions[(int) beetleID][0]
                + Mathf.Sin(manager.moveAnimTimer * 20) * Vector3.forward/20
                + Mathf.Cos(manager.moveAnimTimer * 20) * Vector3.up/20;
            // right
            sdfTrans(manager, beetleID, 1).localPosition =
                manager.originalPositions[(int) beetleID][1]
                + Mathf.Sin(manager.moveAnimTimer * 20 + Mathf.PI/2) * Vector3.forward/20
                + Mathf.Cos(manager.moveAnimTimer * 20 + Mathf.PI/2) * Vector3.up/20;
            
            // body
            sdfTrans(manager, beetleID, 5).localPosition =
                manager.originalPositions[(int) beetleID][5]
                + Mathf.Sin(Time.timeSinceLevelLoad * 3 + Mathf.PI/4) * Vector3.forward/35
                + Mathf.Cos(Time.timeSinceLevelLoad * 3 + Mathf.PI/4) * Vector3.up/20;
            

            // horce
            StateManager.AniMuhID horceID = StateManager.AniMuhID.Horce;
            
            // legs
            // left
            sdfTrans(manager, horceID, 0).localPosition =
                manager.originalPositions[(int) horceID][0]
                + Mathf.Sin(manager.moveAnimTimer * 20) * Vector3.forward/20
                + Mathf.Cos(manager.moveAnimTimer * 20) * Vector3.up/20;
            // right
            sdfTrans(manager, horceID, 1).localPosition =
                manager.originalPositions[(int) horceID][1]
                + Mathf.Sin(manager.moveAnimTimer * 20 + Mathf.PI/2) * Vector3.forward/20
                + Mathf.Cos(manager.moveAnimTimer * 20 + Mathf.PI/2) * Vector3.up/20;
            
            // hips
            sdfTrans(manager, horceID, 6).localPosition =
                manager.originalPositions[(int) horceID][6]
                + Mathf.Sin(manager.moveAnimTimer * 20 + Mathf.PI/4) * Vector3.forward/35
                + Mathf.Cos(manager.moveAnimTimer * 20 + Mathf.PI/4) * Vector3.up/20;
            
            // head
            sdfTrans(manager, horceID, 7).localPosition =
                manager.originalPositions[(int) horceID][7]
                + Mathf.Sin(manager.moveAnimTimer * 20 + Mathf.PI/4) * Vector3.forward/60
                + Mathf.Cos(manager.moveAnimTimer * 20 + Mathf.PI/4) * Vector3.up/40;
            
            // torso
            sdfTrans(manager, horceID, 5).localPosition =
                manager.originalPositions[(int) horceID][5]
                + Mathf.Sin(manager.moveAnimTimer * 20 + Mathf.PI) * Vector3.forward/60
                + Mathf.Cos(manager.moveAnimTimer * 20 + Mathf.PI) * Vector3.up/40;
            
            // neck
            sdfTrans(manager, horceID, 4).localPosition =
                manager.originalPositions[(int) horceID][4]
                + Mathf.Sin(manager.moveAnimTimer * 20 + Mathf.PI) * Vector3.forward/60
                + Mathf.Cos(manager.moveAnimTimer * 20 + Mathf.PI) * Vector3.up/40;

            // deer


            // monkey
            StateManager.AniMuhID monkeyID = StateManager.AniMuhID.Monkey;
            
            // arms
            // left
            sdfTrans(manager, monkeyID, 2).localPosition =
                manager.originalPositions[(int)monkeyID][2]
                + Mathf.Sin(manager.moveAnimTimer * 17) * Vector3.forward / 20
                + Mathf.Abs(Mathf.Cos(manager.moveAnimTimer * 17)) * Vector3.down / 20;
            // right
            sdfTrans(manager, monkeyID, 3).localPosition =
                manager.originalPositions[(int) monkeyID][3]
                + Mathf.Sin(manager.moveAnimTimer * 17 + Mathf.PI/2) * Vector3.forward/20
                + Mathf.Abs(Mathf.Cos(manager.moveAnimTimer * 17 + Mathf.PI/2)) * Vector3.down / 20;
            
            // legs
            // left
            sdfTrans(manager, monkeyID, 0).localPosition =
                manager.originalPositions[(int)monkeyID][0]
                + Mathf.Sin(manager.moveAnimTimer * 17 + Mathf.PI/2) * Vector3.forward / 20
                + Mathf.Clamp01(Mathf.Abs(Mathf.Cos(manager.moveAnimTimer * 17 + Mathf.PI/2))) * Vector3.down / 20;
            // right
            sdfTrans(manager, monkeyID, 1).localPosition =
                manager.originalPositions[(int) monkeyID][1]
                + Mathf.Sin(manager.moveAnimTimer * 17) * Vector3.forward/20
                + Mathf.Clamp01(Mathf.Cos(manager.moveAnimTimer * 17)) * Vector3.up / 20;
            
        }

        private Transform sdfTrans(StateManager manager, StateManager.AniMuhID aniMuh, int limbID)
        {
            return manager.metaShapes[(int)aniMuh].Item1[limbID];
        }
    }
}