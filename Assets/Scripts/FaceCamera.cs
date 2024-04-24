using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the direction from the sprite to the camera
        Vector3 directionToCamera = mainCamera.transform.position - transform.position;

        // Calculate the angle in radians
        Quaternion rotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);

        // Apply rotation only around the Z-axis (2D rotation)
        transform.rotation = rotation;
    }
}
