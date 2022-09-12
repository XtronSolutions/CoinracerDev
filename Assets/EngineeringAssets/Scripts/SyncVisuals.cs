using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncVisuals : MonoBehaviour
{
    public GameObject Visual;
    public Rigidbody Car;

    // Update is called once per frame
    void LateUpdate()
    {
        if (Car)
        {
            Visual.transform.position = Car.transform.position;
            Visual.transform.rotation = Car.transform.rotation;
        }
    }
}
