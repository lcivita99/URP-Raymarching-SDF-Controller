using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using AYellowpaper.SerializedCollections;

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

        [Header("Anim")]
        public Transform sprite3D;
        
        [Header("Movement")]
        public LayerMask Environment;
        [SerializeField] private CapsuleCollider mainCollider;
        [HideInInspector] public bool isGrounded;
        private float _sphereCastRadius = 0.5f;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Vector3 pseudoForward;
        [HideInInspector] public float pseudoForwardRotSpeed = 2f;
        
        public enum AniMuhID
        {
            Penguing = 0,
            Beetle = 1,
            Monkey = 2,
            Horce = 3,
            Deer = 4
        }
        
        public SerializedDictionary<AniMuhID, SerializableTuple<GameObject, AniMUHSO>> Animuhs;
        
        [HideInInspector] public AniMUHSO curStats;
        
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

            moveIn /= moveIn != Vector2.zero ? inCount : 1;
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
            pseudoForward = transform.forward;
            rb = GetComponent<Rigidbody>();
            // rb.mass = curStats.weight;
            
            currentState = IdleState;
            currentState.EnterState(this);
            
            SwitchAnimuh(AniMuhID.Penguing);
            // curStats = Animuhs[AniMuhID.Penguing].Item2;
        }

        private void Start()
        {
            isGrounded = true;
            if (Gamepad.all.Count == 0) Debug.Log("No controllers connected");
            
        }

        void Update()
        { 
            currentState.UpdateState(this);
            
            CalculateCapsuleBottom(mainCollider, out var bottom);
            isGrounded = Physics.SphereCast(bottom + Vector3.up * 1.1f, 1, Vector3.down, out RaycastHit hit, 0.3f,
                Environment);
            
            // Debug.Log(isGrounded + " bottom y = " + bottom.y);
            
            if (TotalMoveInput().magnitude > Utils.minimumInputValue)
            {
                pseudoForward = Vector3.RotateTowards(pseudoForward, TotalMoveInput(), Mathf.PI * pseudoForwardRotSpeed * Time.deltaTime, 0.0f);
            }
        }

        private void LateUpdate()
        {
            SpriteTransUpdate();
            currentState.AnimUpdateState(this);
        }

        private void SpriteTransUpdate()
        {
            sprite3D.position = transform.position;
            sprite3D.forward = pseudoForward;
        }

        private void FixedUpdate()
        {
            currentState.FixedUpdateState(this);
            ApplyConstantForces();
        }
        
        private void ApplyConstantForces()
        {
            // gravity
            if(!isGrounded) rb.AddForce(Vector3.down * (9.81f * rb.mass * curStats.gravScale), ForceMode.Force);
        
            // drag
            rb.AddForce(new Vector3(-rb.velocity.x, 0f, -rb.velocity.z) * curStats.movementDragStrength);
        }
        
        void CalculateCapsuleBottom(CapsuleCollider collider, out Vector3 bottom)
        {
            Vector3 worldCenter = transform.TransformPoint(mainCollider.center);
            float capsuleHeight = collider.height * transform.localScale.y;

            bottom = worldCenter - new Vector3(0, (capsuleHeight / 2), 0);
        }

        public void SwitchAnimuh(AniMuhID aniMuhID)
        {
            // set sprite 3D active if it is what was passed in. If not, disable
            // TODO maybe this is not instant, to allow for animation
            foreach (AniMuhID id in AniMuhID.GetValues(typeof(AniMuhID)))
            {
                try
                {
                    Animuhs[id].Item1.SetActive(id == aniMuhID);
                }
                catch
                {
                    // if this message runs, fix it in the serialized dictionary on the game object
                    // or remove the mentioned animuh from the enum list atop this script
                    Debug.Log(id + " animuh is not assigned to dictionary! (not severe)");
                }
            }
            
            // set current animal stats to appropriate animal
            curStats = Animuhs[aniMuhID].Item2;
            
            // TODO trigger any other visual indicator
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