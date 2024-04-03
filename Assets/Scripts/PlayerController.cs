using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float speed = 100f;
    [SerializeField] private int jumpPower = 10;
    [SerializeField] private float dampConstant = 0.25f;

    private float horizontal;
    private float vertical;
    private float impactVelocity;
    private bool isGrounded = true;
    private bool facingRight = true;
    private Rigidbody rb;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        // get sprite renderer from sprite
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            anim.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            anim.SetTrigger("Transform");
        }

        Flip();

        anim.SetBool("Moving", (Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0));
    }

    private void FixedUpdate() {
        // add current y velocity instead of setting to 0 every time
        impactVelocity = rb.velocity.y;
        rb.velocity = (new Vector3(horizontal, 0, vertical).normalized) * speed * Time.fixedDeltaTime + new Vector3(0, rb.velocity.y, 0);
    }

    private void OnTriggerEnter(Collider hit) {
        if (hit.gameObject.CompareTag("Ground")) {
            rb.AddForce(Vector3.up * (-impactVelocity * Mathf.Sqrt(dampConstant)), ForceMode.VelocityChange);
            isGrounded = true;
            anim.SetBool("Hit Ground", true);
            anim.SetBool("Falling", false);
        }
    }

    private void OnTriggerExit(Collider hit) {
        if (hit.gameObject.CompareTag("Ground")) {
            isGrounded = false;
            anim.SetBool("Hit Ground", false);
            anim.SetBool("Falling", true);
        }
    }

    private void Flip()
    {
        if (facingRight && horizontal < 0 || !facingRight && horizontal > 0)
        {
            facingRight = !facingRight;
            sprite.flipX = !facingRight;
        }
    }
}
