using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanScript : MonoBehaviour
{
    public float reach = 5f;
    public float fanStrength = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit item;
        Vector3 detection = transform.position;
        Debug.DrawRay(detection, transform.forward * reach, Color.red);
        if (Physics.SphereCast(detection, 0.5f, transform.forward, out item, reach)) {
            if (item.transform.CompareTag("Player")) {
                Rigidbody rb = item.collider.attachedRigidbody;
                // to decrease fan force by proportion of distance
                Vector3 distance = item.transform.position - transform.position;
                // negate player velocity by adding velocity opposite to current velocity
                Vector3 fanForce = (transform.forward * (fanStrength / distance.magnitude))+(Vector3.up * -rb.velocity.y);
                // Vector3 fanForce = transform.forward * (fanStrength / distance.magnitude);
                rb.AddForceAtPosition(fanForce, transform.position, ForceMode.Impulse);
            }
        }
    }
}
