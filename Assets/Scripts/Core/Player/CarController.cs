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

        [Header("Settings")] 
        [SerializeField] private float turnSpeed = 180f;
        [SerializeField] private float forwardAccel = 8f;
        [SerializeField] private float reverseAccel = 4f;
        [SerializeField] private float maxSpeed = 50f;

        private float _accelInput;
        private float _turnInput;
        private float _speed;
        
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
            if (!IsOwner)
            {
                return;
            }

            if (Mathf.Abs(_accelInput) > 0)
            {
                sphereRb.AddForce(_speed * transform.forward);
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
