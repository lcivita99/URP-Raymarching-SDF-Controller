using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace State_Machines.AniMUH
{
    public class StateManager : MonoBehaviour
    {
        public BaseState currentState;
        public IdleState IdleState = new IdleState();
        public MoveState MoveState = new MoveState();
        public JumpState JumpState = new JumpState();
        // add other states below!
        //public OtherState OtherState = new OtherState();

        [Header("Movement")]
        public LayerMask Environment;
        [SerializeField] private CapsuleCollider mainCollider;
        [HideInInspector] public bool isGrounded;
        private float _sphereCastRadius = 0.5f;
        [HideInInspector] public Rigidbody rb;
        
        // [Header("Stats")]
        public List<AniMUHSO> animuhStats;
        public AniMUHSO curStats;

        [Tooltip("minimum input to register character control")]
        [HideInInspector] public float minInput = 0.1f;
        public Vector3 TotalMoveInput()
        {
            Vector2 moveIn = Vector2.zero;
            int inCount = 0;
            foreach (var pad in Gamepad.all)
            {
                Vector2 padIn = pad.leftStick.ReadValue();
                
                // only consider input if greater than min input
                if (padIn.magnitude >= minInput)
                {
                    moveIn += padIn.magnitude >= 0.1 ? pad.leftStick.ReadValue() : Vector2.zero;
                    inCount++;
                }
            }

            moveIn /= inCount;
            
            return new Vector3(moveIn.x, 0, moveIn.y);
        }

        public bool AnyJumpButtonPressedThisFrame()
        {
            foreach (var pad in Gamepad.all)
            {
                if (pad.buttonSouth.wasPressedThisFrame) return true;
            }
            return false;
        }

        void Awake()
        {
            currentState = IdleState;
            currentState.EnterState(this);
        }

        private void Start()
        {
        
        }

        void Update()
        {
            currentState.UpdateState(this);
            
            CalculateCapsuleBottom(mainCollider, out var bottom);
            isGrounded = Physics.SphereCast(bottom + Vector3.up * (1.2f * _sphereCastRadius), 1, Vector3.down, out RaycastHit hit, 0.3f,
                Environment);
        }

        private void LateUpdate()
        {
            currentState.AnimUpdateState(this);
        }

        private void FixedUpdate()
        {
            currentState.FixedUpdateState(this);
        }
        
        void CalculateCapsuleBottom(CapsuleCollider collider, out Vector3 bottom)
        {
            Vector3 worldCenter = transform.TransformPoint(mainCollider.center);
            float capsuleHeight = collider.height * transform.localScale.y;

            bottom = worldCenter - new Vector3(0, (capsuleHeight / 2), 0);
            //Debug.Log(bottom);
        }
        

        public void SwitchState(BaseState state)
        {
            currentState = state;
            currentState.EnterState(this);
        }

        public void SwitchState(BaseState state, float number)
        {
            currentState = state;
            currentState.EnterState(this);
        }

        public void SwitchState(BaseState state, float number, Vector3 vec)
        {
            currentState = state;
            currentState.EnterState(this, number, vec);
        }

        public void SwitchState(BaseState state, float number, Vector3 vec, string str)
        {
            currentState = state;
            currentState.EnterState(this, number, vec, str);
        }
    }
}