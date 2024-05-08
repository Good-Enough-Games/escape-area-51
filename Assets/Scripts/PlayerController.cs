using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool hasScrewdriver = false; // if player has screwdriver
    public bool canCollectScrewdriver = false;
    public bool canOpenVent = false; // if player can open vent
    public bool showError = false; // show error msg if no screwdriver
    public Vent vent;
    public Image screwdriver; // indicates screwdriver in inventory
    [SerializeField] private Transform ventTransform; // the vent object
    [SerializeField] private Animator anim;

    [SerializeField] private float speed = 100f;
    [SerializeField] private int jumpPower = 10;
    [SerializeField] private float dampConstant = 0.25f;
    [SerializeField] private int transformStatus = 0; // 0 = self, 1 = first item in transforms array, etc
    [SerializeField] private int numLives = 3;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource movementSound;
    [SerializeField] private AudioSource transformSound;

    [Header("Transform Bar")]
    [SerializeField] private float transformDuration = 10; // seconds?? allowed to be transformed
    [SerializeField] private float transformRegenerationTime = 2; // seconds??  to fully restore transform from empty
    [SerializeField] private float transformRegenerationPauseTime = 2; // seconds before transform regen starts
    [SerializeField] private Image transformBar; // empty bar
    [SerializeField] private Image transformBarFill; // green inner part of bar
    [SerializeField] private Sprite transformBarRed; // to flash when bar is almost empty
    [SerializeField] private Sprite transformBarEmpty; // to flash when bar is almost empty

    private float horizontal;
    private float vertical;
    private float impactVelocity;
    private int groundTriggers = 0; // number of "ground" triggers player is currently touching
    private bool facingRight = true;
    private Rigidbody rb;
    private SpriteRenderer sprite;
    private bool invincible = false;
    private float transformLeftMillis; // how much time left in milliseconds

    // Start is called before the first frame update
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        // get sprite renderer from sprite
        sprite = gameObject.GetComponent<SpriteRenderer>();
        transformLeftMillis = transformDuration * 1000;
        movementSound.loop = true;
        movementSound.volume = 5f;
        StartCoroutine(Transform());
        vent = ventTransform.GetComponent<Vent>();
        screwdriver.enabled = false;
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

        if (Input.GetKeyDown(KeyCode.Space) && groundTriggers > 0) {
            jumpSound.PlayOneShot(jumpSound.clip, 1f);
            rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            anim.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            transformSound.PlayOneShot(transformSound.clip, 1f);
            transformBar.sprite = transformBarEmpty;
            // todo add logic for multiple transforms
            if (transformStatus == 0) // if currently "self", ie not transformed
            {
                if (transformLeftMillis > 0) {
                    StopAllCoroutines();
                    transformStatus = 1;
                    anim.SetTrigger("Transform");
                    anim.SetInteger("Transform Status", 1);
                    StartCoroutine(Transform());
                }
            }
            else // currently transformed into something else, tranform back to self
            {
                StopAllCoroutines();
                transformStatus = 0;
                anim.SetTrigger("Transform");
                anim.SetInteger("Transform Status", 0);
                StartCoroutine(RegenerateTransform());
            }
        }

        if (canOpenVent) {
            if (Input.GetKeyDown(KeyCode.F)) {
                if (hasScrewdriver) {
                    vent.isOpen = true;
                } else {
                    StartCoroutine(ShowError());
                }
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
            if (groundTriggers == 0) {
                anim.SetBool("Hit Ground", true);
                anim.SetBool("Falling", false);
            }
            groundTriggers++;
        }
        else if (hit.gameObject.CompareTag("Guard") && !invincible)
        {
            numLives--;
            gameObject.transform.localScale -= new Vector3(0.25f, 0.25f, 0.25f);
            invincible = true;
            StartCoroutine(ResetInvincibility());
            if (numLives < 1)
            {
                numLives = 3;
                gameObject.transform.position = new Vector3(-28f, 6.5f, -37f);
                gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        Debug.Log(hit.gameObject.tag);
    }

    private void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.CompareTag("Ground")) {
            groundTriggers--;
            if (groundTriggers == 0) {
                anim.SetBool("Hit Ground", false);
                anim.SetBool("Falling", true);
            }
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
        gameObject.tag = "Transform";
        int flashNum = 0;
        while (transformLeftMillis > 0)
        {
            transformLeftMillis -= 50;
            transformBarFill.fillAmount = transformLeftMillis / (transformDuration * 1000);
            if (transformLeftMillis / (transformDuration * 1000) < 0.25 && flashNum > 3)
            {
                if (transformBar.sprite == transformBarRed)
                {
                    transformBar.sprite = transformBarEmpty;
                }
                else
                {
                    transformBar.sprite = transformBarRed;
                }
                flashNum = 0;
            }
            else
            {
                flashNum++;
            }
            yield return new WaitForSeconds(0.05f);
        }
        transformBar.sprite = transformBarEmpty;
        transformStatus = 0; // force transform back to self
        anim.SetTrigger("Transform");
        anim.SetInteger("Transform Status", 0);
        transformSound.PlayOneShot(transformSound.clip, 1f);
        StartCoroutine(RegenerateTransform());
    }

    private IEnumerator RegenerateTransform()
    {
        gameObject.tag = "Player";
        yield return new WaitForSeconds(transformRegenerationPauseTime);
        while (transformLeftMillis < transformDuration * 1000)
        {
            transformLeftMillis += 50;
            transformBarFill.fillAmount = transformLeftMillis / (transformDuration * 1000);
            yield return new WaitForSeconds(transformRegenerationTime / 100);
        }
    }

    private IEnumerator ShowError() {
        showError = true;
        yield return new WaitForSeconds(2);
        showError = false;
    }

    private IEnumerator ResetInvincibility()
    {
        for (int i = 0; i < 5; i++)
        {
            GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, 0.6f);
            yield return new WaitForSeconds(0.5f);
            GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.5f);
        }
        invincible = false;
    }
}
