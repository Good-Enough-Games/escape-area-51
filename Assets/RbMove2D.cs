using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RbMove2D : MonoBehaviour
{
    public float speed = 100f;
    public int allowableJumps = 1;
    public int jumpSpeed = 10;
    public float dampConstant = 0.75f;
    public Vector3 movement;
    private float upVel = 1;
    private float impactVelocity;
    private int jumpCount = 0;
    private bool isJumping = false;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        if (jumpCount < allowableJumps) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                upVel = jumpSpeed;
                isJumping = true;
            }
        }
    }

    void FixedUpdate() {
        // add current y velocity instead of setting to 0 every time
        rb.velocity = movement * speed * Time.fixedDeltaTime + new Vector3(0, rb.velocity.y, 0);
        impactVelocity = rb.velocity.y;
        if (isJumping){
            // mass independent force
            rb.AddForce(Vector3.up * upVel, ForceMode.VelocityChange);
            isJumping = false;
        }
        Debug.Log(jumpCount);
    }

    void OnCollisionEnter(Collision hit) {
        jumpCount = allowableJumps;
        upVel = -impactVelocity * Mathf.Sqrt(dampConstant);
        isJumping = true;
        // time perfectly to get big jump (speedrun strat)
        jumpCount = 0;
    }

    void OnCollisionExit(Collision hit) {
        // prevents midair jump
        jumpCount++;
    }
}
