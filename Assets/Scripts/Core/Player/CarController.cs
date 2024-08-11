using System;
using Input;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class CarController : NetworkBehaviour
    {
        [Header("References")] 
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Rigidbody sphereRb;
        [SerializeField] private Transform groundRayPoint;
        [SerializeField] private LayerMask groundMask;

        [Header("Settings")] 
        [SerializeField] private float turnSpeed = 180f;
        [SerializeField] private float forwardAccel = 8f;
        [SerializeField] private float reverseAccel = 4f;
        [SerializeField] private float maxSpeed = 50f;
        [SerializeField] private float gravityForce = 10f;
        [SerializeField] private float groundDrag = 3f;

        private float _accelInput;
        private float _turnInput;
        private float _speed;
        private bool isGrounded;
        
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                return;
            }

            inputReader.AccelerateEvent += HandleAccelerate;
            inputReader.TurnEvent += HandleTurn;
        }

        

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
            {
                return;
            }
            inputReader.AccelerateEvent -= HandleAccelerate;
            inputReader.TurnEvent -= HandleTurn;
        }

        private void Start()
        {
            sphereRb.transform.parent = null;
        }

        void Update()
        {
            transform.position = sphereRb.transform.position;

            if (!IsOwner)
            {
                return;
            }
            if (_accelInput > 0)
            {
                _speed = _accelInput * forwardAccel * 1000f;
            }
            else if (_accelInput < 0)
            {
                _speed = _accelInput * reverseAccel * 1000f;
            }
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f,_turnInput*_accelInput*turnSpeed*Time.deltaTime, 0f));
        }
        private void FixedUpdate()
        {
            
            isGrounded = Physics.Raycast(groundRayPoint.position, -transform.up, out RaycastHit hit ,1f, groundMask);

            if (isGrounded)
            {
                Quaternion newRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = Quaternion.Lerp(transform.rotation,newRotation,0.1f);
            }
            
            if (!IsOwner)
            {
                return;
            }
            
            Debug.DrawLine(groundRayPoint.position,hit.point);

            if (isGrounded)
            {
                sphereRb.drag = groundDrag;
                if (Mathf.Abs(_accelInput) > 0 && sphereRb.velocity.magnitude <= maxSpeed)
                {
                    sphereRb.AddForce(_speed * transform.forward);
                }
            }
            else
            { 
                sphereRb.drag = 0.1f;
                sphereRb.AddForce(Vector3.down * (gravityForce * 100f));
            }
        }

        private void HandleTurn(float turnInput)
        {
            _turnInput = turnInput;
        }
        
        private void HandleAccelerate(float accelInput)
        {
            _accelInput = accelInput;
        }
    }
}
