using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private float speed = 100f;
    [SerializeField] private int jumpPower = 10;
    [SerializeField] private float dampConstant = 0.25f;
    // seconds allowed to transform
    [SerializeField] private float transformDuration = 15;
    // seconds to restore 1 "unit" of transform energy (?)
    [SerializeField] private float cooldownLength = 1;
    // seconds before cooldown starts
    [SerializeField] private float cooldownOffset = 2;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource movementSound;
    [SerializeField] private AudioSource transformSound;

    private float horizontal;
    private float vertical;
    private float impactVelocity;
    private bool isGrounded = true;
    private bool facingRight = true;
    private int transformStatus = 0; // 0 = self, 1 = first item in transforms array, etc
    private Rigidbody rb;
    private SpriteRenderer sprite;
    // how much time left
    private float transformLeft;

    // Start is called before the first frame update
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        // get sprite renderer from sprite
        sprite = gameObject.GetComponent<SpriteRenderer>();
        transformLeft = transformDuration;
        movementSound.loop = true;
        movementSound.volume = 5f;
    }

    // Update is called once per frame
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0) {
            if (!movementSound.isPlaying)
            {
                movementSound.UnPause();
                movementSound.Play();
            }
        }
        else
        {
            movementSound.Pause();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            jumpSound.PlayOneShot(jumpSound.clip, 1f);
            rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            anim.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            transformSound.PlayOneShot(transformSound.clip, 1f);
            // todo add logic for multiple transforms
            if (transformStatus == 0)
            {
                if (transformLeft > 0) {
                    StopAllCoroutines();
                    transformStatus = 1;
                    anim.SetTrigger("Transform");
                    anim.SetInteger("Transform Status", 1);
                    StartCoroutine(Transform());
                }
            }
            else
            {
                StopAllCoroutines();
                transformStatus = 0;
                anim.SetTrigger("Transform");
                anim.SetInteger("Transform Status", 0);
                StartCoroutine(Cooldown());
            }
        }

        Flip();

        anim.SetBool("Moving", (Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0));

        if (rb.velocity.magnitude > 0 && !transform.gameObject.CompareTag("Player")) {
            transform.gameObject.tag = "Player";
        }
    }

    private void FixedUpdate()
    {
        // add current y velocity instead of setting to 0 every time
        impactVelocity = rb.velocity.y;
        rb.velocity = (new Vector3(horizontal, 0, vertical).normalized) * speed * Time.fixedDeltaTime + new Vector3(0, rb.velocity.y, 0);
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.CompareTag("Ground")) {
            rb.AddForce(Vector3.up * (-impactVelocity * Mathf.Sqrt(dampConstant)), ForceMode.VelocityChange);
            isGrounded = true;
            anim.SetBool("Hit Ground", true);
            anim.SetBool("Falling", false);
        }
    }

    private void OnTriggerExit(Collider hit)
    {
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

    private IEnumerator Transform()
    {
        transform.gameObject.tag = "Transform";
        while (transformLeft > 0) {
            transformLeft--;
            yield return new WaitForSeconds(1);
        }
        transformStatus = 0;
        anim.SetTrigger("Transform");
        anim.SetInteger("Transform Status", 0);
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        transform.gameObject.tag = "Player";
        yield return new WaitForSeconds(cooldownOffset);
        while (transformLeft < transformDuration) {
            transformLeft++;
            yield return new WaitForSeconds(cooldownLength);
        }
    }
}
