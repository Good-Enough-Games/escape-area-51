// Smooth towards the target
    
    using UnityEngine;
    using System.Collections;
    
    public class DampCamera : MonoBehaviour
    {
        public Transform target;
        public float smoothTime = 0.3F;
        public float height = 7;
        public float distance = -10;
        private Vector3 velocity = Vector3.zero;
    
        void FixedUpdate()
        {
            // Define a target position above and behind the target transform
            Vector3 targetPosition = target.TransformPoint(new Vector3(0, height, distance));
    
            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }