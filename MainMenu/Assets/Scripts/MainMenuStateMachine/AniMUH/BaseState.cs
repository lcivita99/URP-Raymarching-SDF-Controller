using UnityEngine;

namespace State_Machines.AniMUH
{
    public abstract class BaseState
    {
        public abstract void EnterState(StateManager manager, float number = 0, Vector3 vec = default(Vector3), string str = "");

        public abstract void UpdateState(StateManager manager);

        public abstract void FixedUpdateState(StateManager manager);

        public abstract void ForcedOutOfState(StateManager manager);

        public abstract void AnimUpdateState(StateManager manager);

    }
}
