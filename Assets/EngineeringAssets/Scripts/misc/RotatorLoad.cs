using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorLoad : MonoBehaviour
{
    public bool ForYAxis = false;
    public float speed=0;
    void Update()
    {
        if(!ForYAxis)
            transform.Rotate(0, 0, -speed * Time.deltaTime);
        else
            transform.Rotate(0, -speed * Time.deltaTime,0 );
    }
}
