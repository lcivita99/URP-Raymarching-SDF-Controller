using UnityEngine;

namespace State_Machines.AniMUH
{
    public class MoveState : BaseState
    {
        // here we declare variables specific to the state

        // example
        // public float dashLength = 10f;

        public override void EnterState(StateManager manager, float number, Vector3 vec, string str)
        {
            //Debug.Log("entered state");
        }

        public override void UpdateState(StateManager manager)
        {
            
        }

        public override void FixedUpdateState(StateManager manager)
        {

        }

        // this function I found extremely important, cause sometimes
        // states don't wrap up so neatly if something other than itself
        // changes the object's state. This ensures that any of those imperfections
        // get fixed. I'll explain what an example of this could be below
        public override void ForcedOutOfState(StateManager manager)
        {
            // example: let's suppose you have a cat, and this is the jump state.
            // Typically, when the cat lands, we set the boolean jumping to false.
            // Suppose however, that an eagle slaps the cat mid-air, removing the cat
            // from jumping state. BUT! Suppose the eagle script is what puts the
            // cat into hitstun state. This function would be a way of ensuring that
            // jumping is now false. Example code:

            // jumping = false;
        }


        // we dedicate this section to the 3D "sprite" animation
        public override void AnimUpdateState(StateManager manager)
        {

        }
    }
}