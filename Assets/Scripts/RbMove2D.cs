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
    private float horizontal;
    private float vertical;
    private bool facingRight = true;
    private Rigidbody rb;

    [SerializeField] private Animator anim;

    // Start is called before the first frame update
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        movement = new Vector3(horizontal, 0, vertical).normalized;
        if (jumpCount < allowableJumps) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                upVel = jumpSpeed;
                isJumping = true;
                anim.SetTrigger("Jump");
            }
        }

        Flip();

        anim.SetBool("Moving", Mathf.Abs(horizontal + vertical) > 0);
    }

    private void FixedUpdate() {
        // add current y velocity instead of setting to 0 every time
        rb.velocity = movement * speed * Time.fixedDeltaTime + new Vector3(0, rb.velocity.y, 0);
        impactVelocity = rb.velocity.y;
        if (isJumping){
            // mass independent force
            rb.AddForce(Vector3.up * upVel, ForceMode.VelocityChange);
            isJumping = false;
        }
    }

    private void OnCollisionEnter(Collision hit) {
        jumpCount = allowableJumps;
        upVel = -impactVelocity * Mathf.Sqrt(dampConstant);
        isJumping = true;
        // time perfectly to get big jump (speedrun strat)
        jumpCount = 0;
    }

    private void OnCollisionExit(Collision hit) {
        // prevents midair jump
        jumpCount++;
    }

    private void Flip()
    {
        if (facingRight && horizontal < 0 || !facingRight && horizontal > 0)
        {
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
