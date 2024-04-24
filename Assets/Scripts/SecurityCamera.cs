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
        Vector3 obTargetPos = target.position + new Vector3(0, -1f, 0);
        Debug.DrawRay(transform.position, obTargetPos - transform.position);
        if (Physics.Raycast(transform.position, obTargetPos - transform.position, out hit, Vector3.Distance(transform.position, target.position), layerMask)) {
            obstacle = hit.collider.gameObject;
            Material mat = obstacle.GetComponent<MeshRenderer>().material;
            opacity = mat.color;
            opacity.a = 0.5f;
            mat.color = opacity;
            mat.renderQueue = 3001;
            flag = true;
        } else if (flag == true) {
            opacity.a = 1f;
            Material mat = obstacle.GetComponent<MeshRenderer>().material;
            mat.color = opacity;
            mat.renderQueue = 2999;
            flag = false;
        }
    }
}