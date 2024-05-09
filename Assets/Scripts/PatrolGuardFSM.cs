using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolGuardFSM : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private float rotationSpeed = 0.05f;

    enum GuardState {MOVE, CHASE, IDLE, ATTACK};
    GuardState currentState;

    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float threshold = 0.1f;
    // how long in GuardState.MOVE before checking for idle
    [SerializeField] private float walkOffset = 2f;
    // delay after guard attacks
    [SerializeField] private float attackDelay = 0.5f;
    // obstacle detection distance
    [SerializeField] private float obstacleLength = 2f;
    // obstacle detection height
    [SerializeField] private float obstacleHeight = 2f;

    private Transform currWaypoint;
    private Vector3 prevPosition;
    private float walkTime; // stores time guard was walking a little before checking for idle
    private bool isIdle;
    private bool attacked;
    private Quaternion toRot;
    private Vector3 targetDir;
    private float turnTime;
    private FieldOfView fov;
    private bool facingRight;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        walkTime = 0;
        turnTime = 0;
        currentState = GuardState.MOVE;
        currWaypoint = waypoints.getRandomWaypoint();
        prevPosition = transform.position;
        transform.position = currWaypoint.position;
        currWaypoint = waypoints.getNextWaypoint(currWaypoint);
        transform.LookAt(currWaypoint);
        fov = GetComponent<FieldOfView>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        facingRight = transform.rotation.eulerAngles.y > 180f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSprite();

        Debug.DrawRay(transform.position - new Vector3(0, obstacleHeight, 0), transform.forward * obstacleLength);
        switch(currentState) {
            case GuardState.MOVE:
                anim.SetBool("Moving", true);
                if (fov.visibleTargets.Count > 0) {
                    currentState = GuardState.CHASE;
                } else if (Vector3.Distance(transform.position, currWaypoint.position) < threshold && walkTime > walkOffset) {
                    currentState = GuardState.IDLE;
                } else {
                    isIdle = false;
                    attacked = false;
                    if (Vector3.Angle(transform.forward, targetDir.normalized) > threshold) {
                        transform.rotation = Quaternion.Slerp(transform.rotation, toRot, turnTime);
                        turnTime += rotationSpeed * Time.deltaTime;
                    } else {
                        transform.position = Vector3.MoveTowards(transform.position, currWaypoint.position, patrolSpeed * Time.deltaTime);
                        walkTime += 0.01f;
                    }
                }
                break;
            case GuardState.IDLE:
                anim.SetBool("Moving", false);
                if (fov.visibleTargets.Count > 0) {
                    currentState = GuardState.CHASE;
                } else {
                    if (!isIdle) {
                        StartCoroutine(Idle());
                    }
                }
                break;
            case GuardState.CHASE:
                anim.SetBool("Moving", true);
                if (fov.visibleTargets.Count == 0) {
                    // so guard stops for a moment in confusion or smth
                    currentState = GuardState.IDLE;
                } else {
                    isIdle = false;
                    attacked = false;
                    targetDir = fov.visibleTargets[0].position - transform.position;
                    targetDir.y = 0;
                    transform.rotation = Quaternion.LookRotation(targetDir);
                    Vector3 targetPos = fov.visibleTargets[0].position;
                    targetPos.y = transform.position.y;
                    if (!Physics.Raycast(transform.position - new Vector3(0, obstacleHeight, 0), transform.forward, obstacleLength, fov.obstacleMask)) {
                        transform.position = Vector3.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);
                    }
                }
                break;
            case GuardState.ATTACK:
                anim.SetBool("Moving", true);
                if (!attacked) {
                    StartCoroutine(Attack());
                }
                break;
        }
    }

    private IEnumerator Idle() {
        isIdle = true;
        walkTime = 0;
        if (Vector3.Distance(transform.position, currWaypoint.position) < threshold) {
            turnTime = 0;
            while (Vector3.Angle(transform.forward, currWaypoint.forward) > threshold) {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(currWaypoint.forward), turnTime);
                turnTime += rotationSpeed * Time.deltaTime;
                // update loop framewise or else it executes in single frame
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForSeconds(idleTime);
        currWaypoint = waypoints.getNextWaypoint(currWaypoint);
        targetDir = currWaypoint.position - transform.position;
        toRot = Quaternion.LookRotation(targetDir);
        turnTime = 0;
        currentState = GuardState.MOVE;
    }

    private IEnumerator Attack() {
        attacked = true;
        // delay after attacking to give players some time to move
        yield return new WaitForSeconds(attackDelay);
        // go back to chasing after attacking
        currentState = GuardState.CHASE;
    }

    private void OnCollisionEnter(Collision hit) {
        currentState = GuardState.ATTACK;
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb !=null){
            Vector3 forceDirection = hit.gameObject.transform.position - transform.position;

            forceDirection.y=0;
            forceDirection.Normalize();
            rb.AddForceAtPosition(forceDirection * 100f, transform.position, ForceMode.Impulse);
        }
    }

    private void UpdateSprite() 
    {
        // update sprite animation based on rotation... Rotation parameter in animation: 1 = up, -1 = down, 0 = side
        float currentRotation = transform.rotation.eulerAngles.y;
        if (currentRotation > 315f || currentRotation < 45f)
        {
            anim.SetInteger("Rotation", 1);
        }
        else if (currentRotation > 135f && currentRotation < 225f)
        {
            anim.SetInteger("Rotation", -1);
        }
        else
        {
            anim.SetInteger("Rotation", 0);
            if (facingRight && currentRotation < 180f || !facingRight && currentRotation > 180f)
            {
                facingRight = !facingRight;
                sprite.flipX = !facingRight;
            }
        }
    }
}
