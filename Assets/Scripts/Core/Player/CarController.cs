using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private Transform leftWheelTransform;
        [SerializeField] private Transform rightWheelTransform;

        [Header("Settings")] 
        [SerializeField] private float turnSpeed = 180f;

        [SerializeField] private float maxWheelRotation;
        [SerializeField] private float forwardAccel = 8f;
        [SerializeField] private float reverseAccel = 4f;
        [SerializeField] private float maxSpeed = 50f;
        [SerializeField] private float gravityForce = 10f;
        [SerializeField] private float groundDrag = 3f;

        private float _accelInput;
        private float _turnInput;
        private float _speed;
        private bool _isGrounded;
        private bool _canMove = true;
        
        
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
            if (!_canMove)
            {
                _accelInput = 0;
                _turnInput = 0;
                return;
            }
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
            leftWheelTransform.localRotation = Quaternion.Lerp(leftWheelTransform.localRotation, Quaternion.Euler(leftWheelTransform.localRotation.x,(_turnInput * maxWheelRotation), leftWheelTransform.localRotation.z),0.05f);
            rightWheelTransform.localRotation = Quaternion.Lerp(rightWheelTransform.localRotation, Quaternion.Euler(rightWheelTransform.localRotation.x,(_turnInput * maxWheelRotation), rightWheelTransform.localRotation.z),0.05f);
        }
        private void FixedUpdate()
        {
            
            _isGrounded = Physics.Raycast(groundRayPoint.position, -transform.up, out RaycastHit hit ,1f, groundMask);

            if (_isGrounded)
            {
                Quaternion newRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = Quaternion.Lerp(transform.rotation,newRotation,0.1f);
            }
            
            if (_isGrounded && IsOwner)
            {
                sphereRb.linearDamping = groundDrag;
                if (Mathf.Abs(_accelInput) > 0 && sphereRb.linearVelocity.magnitude <= maxSpeed)
                {
                    sphereRb.AddForce(_speed * transform.forward);
                }
            }
            else
            { 
                sphereRb.linearDamping = 0.1f;
                sphereRb.AddForce(Vector3.down * (gravityForce * 100f));
            }
            
            Debug.DrawLine(groundRayPoint.position,hit.point);

        }

        public void SetCanMove(bool newCanMove)
        {
            _canMove = newCanMove;
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void HandleBoostServerRpc(float boostStrength, float boostTime) 
        {
            //must be sent to server first so server can ping all clients
            //note docs: https://docs-multiplayer.unity3d.com/netcode/current/advanced-topics/message-system/rpc/
            HandleBoostClientRpc(boostStrength,boostTime);
        }

        [ClientRpc]
        private void HandleBoostClientRpc(float boostStrength, float boostTime)
        {
            StartCoroutine(Boost(boostStrength, boostTime));
        }
        
        public IEnumerator Boost(float boostStrength, float boostTime)
        {
            //TODO: Add boost particle
            Debug.Log($"Boost started for {OwnerClientId}");
            forwardAccel = forwardAccel * boostStrength;
            yield return new WaitForSeconds(boostTime);
            forwardAccel = forwardAccel / boostStrength;
            Debug.Log("Boost ended");
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
