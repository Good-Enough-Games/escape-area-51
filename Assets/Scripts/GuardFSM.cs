using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardFSM : MonoBehaviour
{
    public float patrolSpeed = 3f;
    public float chaseSpeed = 3f;
    public float delayTime = 2f;
    public float rotationSpeed = 0.05f;
    enum GuardState {MOVE, CHASE, IDLE, ATTACK};
    GuardState currentState;
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float threshold = 0.1f;
    // how long in GuardState.MOVE before checking for idle
    [SerializeField] private float walkOffset = 2f;
    // delay after guard attacks
    [SerializeField] private float attackDelay = 0.5f;
    private Transform currWaypoint;
    // stores time guard was walking a little before checking for idle
    private float walkTime;
    private bool isIdle;
    private bool attacked;
    private Quaternion toRot;
    private Vector3 targetDir;
    private float turnTime;
    private FieldOfView fov;
    // Start is called before the first frame update
    void Start()
    {
        walkTime = 0;
        turnTime = 0;
        currentState = GuardState.MOVE;
        currWaypoint = waypoints.getRandomWaypoint();
        transform.position = currWaypoint.position;
        currWaypoint = waypoints.getNextWaypoint(currWaypoint);
        transform.LookAt(currWaypoint);
        fov = GetComponent<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState) {
            case GuardState.MOVE:
                if (fov.visibleTargets.Count > 0) {
                    currentState = GuardState.CHASE;
                } else if (Vector3.Distance(transform.position, currWaypoint.position) < threshold && walkTime > walkOffset) {
                    currentState = GuardState.IDLE;
                } else {
                    isIdle = false;
                    attacked = false;
                    StopAllCoroutines();
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
                if (fov.visibleTargets.Count > 0) {
                    currentState = GuardState.CHASE;
                } else {
                    if (!isIdle) {
                        StartCoroutine(Idle());
                    }
                }
                break;
            case GuardState.CHASE:
                if (fov.visibleTargets.Count == 0) {
                    // so guard stops for a moment in confusion
                    currentState = GuardState.IDLE;
                } else {
                    isIdle = false;
                    attacked = false;
                    targetDir = fov.visibleTargets[0].position - transform.position;
                    targetDir.y = 0;
                    transform.rotation = Quaternion.LookRotation(targetDir);
                    Vector3 targetPos = fov.visibleTargets[0].position;
                    targetPos.y = transform.position.y;
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);
                }
                break;
            case GuardState.ATTACK:
                if (!attacked) {
                    StartCoroutine(Attack());
                }
                break;
        }
    }

    private IEnumerator Idle() {
        isIdle = true;
        walkTime = 0;
        yield return new WaitForSeconds(delayTime);
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
}
