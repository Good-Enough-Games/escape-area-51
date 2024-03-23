using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float speed = 5;
    float y;
    float initY;
    float dirY = 1;
    // Start is called before the first frame update
    void Start()
    {
        initY = gameObject.transform.position.y;
        y = initY;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.Rotate(0, speed * Time.fixedDeltaTime, 0);
        if ((y > (initY + 0.5)) || (y < initY)){
            dirY=-dirY;
        }
        y += dirY*Time.fixedDeltaTime;
        Vector3 pos = gameObject.transform.position;
        pos.y = y;
        gameObject.transform.position = pos;
    }
}
