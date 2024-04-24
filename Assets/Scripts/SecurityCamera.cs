// Smooth towards the target

using UnityEngine;
using System.Collections;

public class SecurityCamera : MonoBehaviour
{
    public Transform target; // the object to follow
    public float smoothTime = 0.9f; // smoothing time for camera movement
    private bool flag = false;
    private GameObject obstacle;
    private Color opacity;

    void FixedUpdate()
    {
        // Rotate towards the target
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // Apply rotation to the camera
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothTime);

        // detect if object blocks player
        // layermasks work as bitmasks; player is layer 6, so make bitmask to test everything but layer 6
        int layerMask = 1 << 6;
        layerMask = ~layerMask;
        RaycastHit hit;
        Debug.DrawRay(transform.position, target.position - transform.position);
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, Vector3.Distance(transform.position, target.position), layerMask)) {
            obstacle = hit.collider.gameObject;
            opacity = obstacle.GetComponent<MeshRenderer>().material.color;
            opacity.a = 0.5f;
            obstacle.GetComponent<MeshRenderer>().material.color = opacity;
            flag = true;
        } else if (flag == true) {
            opacity.a = 1f;
            obstacle.GetComponent<MeshRenderer>().material.color = opacity;
            flag = false;
        }
    }
}