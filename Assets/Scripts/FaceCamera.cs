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

        // Calculate the target rotation to face the camera
        Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);

        // Extract the Y-axis rotation from the target rotation
        Quaternion yRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        // Apply the Y-axis rotation to the object
        transform.rotation = yRotation;
    }
}
