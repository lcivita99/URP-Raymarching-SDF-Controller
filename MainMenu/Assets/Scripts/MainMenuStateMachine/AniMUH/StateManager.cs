using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using AYellowpaper.SerializedCollections;
using UnityEngine.Serialization;

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

        [Header("Anim")] public Transform sprite3D;
        public SerializableTuple<Transform[], float>[] metaShapes;
        [SerializeField] private Transform[] habitatRefs;
        [HideInInspector] public List<List<Vector3>> originalPositions;
        [HideInInspector] public float moveAnimTimer = 0;

        [Header("Movement")] public LayerMask Environment;
        [SerializeField] private CapsuleCollider mainCollider;
        [HideInInspector] public bool isGrounded;
        private float _sphereCastRadius = 0.5f;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Vector3 pseudoForward;
        [HideInInspector] public float pseudoForwardRotSpeed = 2f;

        [Header("Materials")] public Material mat;

        public enum AniMuhID
        {
            Beetle = 0,
            Horce = 1,
            // Deer = 2,
            Monkey = 2
            // Deer = 4
        }

        public SerializedDictionary<AniMuhID, SerializableTuple<GameObject, AniMUHSO>> Animuhs;

        [HideInInspector] public AniMUHSO curStats;

        [Tooltip("minimum input to register character control")] [HideInInspector]
        public float minInput = 0.1f;

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

            SwitchAnimuh(AniMuhID.Beetle);

            SetOriginalPositions();

            // transWeights = new []{}
            // curStats = Animuhs[AniMuhID.Penguing].Item2;
        }

        private void SetOriginalPositions()
        {
            originalPositions = new List<List<Vector3>>();

            for (int i = 0; i < metaShapes.Length; i++)
            {
                originalPositions.Add(new List<Vector3>());
                for (int j = 0; j < metaShapes[i].Item1.Length; j++)
                {
                    originalPositions[i].Add(metaShapes[i].Item1[j].localPosition);
                }
            }
        }

        private void Start()
        {
            // rend = GetComponent<Renderer>();
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
                pseudoForward = Vector3.RotateTowards(pseudoForward, TotalMoveInput(),
                    Mathf.PI * pseudoForwardRotSpeed * Time.deltaTime, 0.0f);
            }
        }

        private void LateUpdate()
        {
            SpriteTransUpdate();
            currentState.AnimUpdateState(this);
            SetUniforms();
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
            if (!isGrounded) rb.AddForce(Vector3.down * (9.81f * rb.mass * curStats.gravScale), ForceMode.Force);

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

        // Shaders

        public void SetUniforms()
        {
            CalculateWeights();
            
            List<Vector3> weightedPositions = new List<Vector3>();

            for (int i = 0; i < metaShapes[0].Item1.Length; i++)
            {
                weightedPositions.Add(Vector3.zero);
            }
            
            for (int i = 0; i < metaShapes.Length; i++)
            {
                for (int j = 0; j < metaShapes[i].Item1.Length; j++)
                {
                    weightedPositions[j] += metaShapes[i].Item1[j].localPosition * metaShapes[i].Item2;
                }
            }

            for (int i = 0; i < metaShapes[0].Item1.Length; i++)
            {
                string posID = "_Pos" + (i + 1);
                mat.SetVector(posID, weightedPositions[i]);
            }
        }
        
        // CalculateWeights() written with the assistance of ChatGPT, modified
        private void CalculateWeights()
        {
            List<float> distances = new List<float>();

            float smoothThreshold = 0.7f;
            // Calculate distances to each reference vector
            for (int i = 0; i < metaShapes.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, habitatRefs[i].position);
                distances.Add(distance);
            }

            // Find the minimum distance and identify references within the smoothing threshold
            float minDistance = distances.Min();
            List<float> closeDistances = distances.Where(d => d <= minDistance + smoothThreshold).ToList();

            // Initial weights based on closeness
            List<float> weights = distances.Select(d => closeDistances.Contains(d) ? (1f - (d - minDistance) / smoothThreshold) : 0f).ToList();

            // Normalize weights to ensure they sum to 1
            NormalizeWeights(weights);
    
            // Update weight array
            for (int i = 0; i < metaShapes.Length; i++)
            {
                metaShapes[i].Item2 = weights[i];
            }
        }

        private void NormalizeWeights(List<float> weights)
        {
            float sum = weights.Sum();
            for (int i = 0; i < weights.Count; i++)
            {
                weights[i] /= sum;
            }
        }

    }
}