using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatform : MonoBehaviour
{
    public float maxDistance = 3.0f;
    public float maxHeight = 2.0f;
    public int numPlats = 5;
    void Start()
    {
        // so spawning doesn't recurse
        if (gameObject.name == "Platform") {
            Vector3 newPos = gameObject.transform.position;
            // change to while once desired x & z distances are set
            for (int i = 0; i < numPlats; i++) {
                // additional z to add; new platform will already be scaled at least by z size of previous platform
                float newZ = Random.Range(0f, maxDistance);
                // to normalize(?) random distance
                float euclidX = Mathf.Sqrt(maxDistance * maxDistance - newZ * newZ);
                // add scale of platform to account for changes in scaling
                // if platforms are diff sizes, might have to take halves of each platform size
                euclidX += gameObject.transform.localScale.x;
                float newX = Random.Range(-euclidX, euclidX);
                // offset by at least z scale so platforms aren't overlapping
                // *note: placed in this line so that euclidX calculates only on additional z distance
                newPos.z += gameObject.transform.localScale.z + newZ;
                newPos.x += newX;
                float newY = Random.Range(-maxHeight, maxHeight);
                newPos.y += newY;
                // spawn new platform above instead if it tries going under the floor
                if (newPos.y < 0.5f) {
                    newPos.y -= 2 * newY;
                }
                GameObject newPlat = Instantiate(gameObject, newPos, Quaternion.identity);
                // just to give them new names instead of Platform (clone) cuz it doesnt look as ugly
                newPlat.name = gameObject.name + (i+2).ToString();
            }
        }
    }
}
