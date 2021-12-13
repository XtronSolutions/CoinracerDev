using System.Collections;
using System.Collections.Generic;
using DavidJalbert;
using UnityEngine;

public class WayPointPointer : MonoBehaviour
{
    public TinyCarController Controller;
    [SerializeField] private RaceManager _racerManager = null;
    [SerializeField] private float _speed = 5;
    [SerializeField] private Camera _camera = null;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Controller != null)
        {
            Vector3 dirForNextWaypoint = _racerManager.GetNextWayPointPosition() - Controller.transform.position;
            dirForNextWaypoint.y = 0;
            dirForNextWaypoint.Normalize();

            var targetRotation = Quaternion.LookRotation(dirForNextWaypoint);
            var cameraRotation = Quaternion.Euler(new Vector3(0,_camera.transform.eulerAngles.y,0));
            cameraRotation = Quaternion.Inverse(cameraRotation);
            targetRotation = cameraRotation * targetRotation;
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,_speed*Time.deltaTime);
        }
    }
}
