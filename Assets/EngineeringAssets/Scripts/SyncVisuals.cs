using DavidJalbert;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SyncVisuals : MonoBehaviour
{
    public GameObject Visual;
    public Rigidbody Car;
    private TinyCarController carController;

    private void Start()
    {
        carController = Car.GetComponent<TinyCarController>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (carController.IsMultiplayer)
        {
            if (!carController.PHView.IsMine)
            {
                Visual.transform.position = Car.transform.position;
                Visual.transform.rotation = Car.transform.rotation;
            }
        }
    }
}
