using System;
using UnityEngine;

namespace Utilities
{
    public class OffsetFromGroundNormal : MonoBehaviour
    {
        [SerializeField] private float groundOffset;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Transform groundRayPoint;

        private void FixedUpdate()
        {
            Physics.Raycast(groundRayPoint.position, -transform.up, out RaycastHit hit ,10f, groundMask);
            
            Quaternion newRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = newRotation;
            transform.position = new Vector3(transform.position.x, hit.point.y + groundOffset,
                transform.position.z);
        }
    }
}
