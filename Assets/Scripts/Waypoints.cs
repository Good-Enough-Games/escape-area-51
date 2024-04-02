using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Range(0f, 5f)]
    [SerializeField] private float size = 1f;
    private void OnDrawGizmos()
    {
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(t.position, size);
            Debug.DrawRay(t.position, t.forward * 5f);
        }
        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
        Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
    }

    public Transform getNextWaypoint(Transform current)
    {
        if (current == null)
        {
            return transform.GetChild(0);
        }
        if (current.GetSiblingIndex() < transform.childCount - 1)
        {
            return transform.GetChild(current.GetSiblingIndex() + 1);
        }
        return transform.GetChild(0);
    }

    public Transform getRandomWaypoint()
    {
        return transform.GetChild(Random.Range(0, transform.childCount));
    }
}
