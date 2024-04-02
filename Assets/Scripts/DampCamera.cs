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
    private bool flag = false;
    private GameObject obstacle;
    private Color opacity;

    void FixedUpdate()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = target.TransformPoint(new Vector3(0, height, distance));

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

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