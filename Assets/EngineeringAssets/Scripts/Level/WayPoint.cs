using System;
using System.Collections;
using System.Collections.Generic;
using DavidJalbert;
using UniRx;
using UnityEngine;

public class WayPoint : MonoBehaviour
{

    public bool IsStartWayPoint = false;
    public bool IsIceCollider = false;
    public bool CheckIce = false;
    private bool GameStarted = false;
    private Subject<WayPointData> _wayPointDataSubject = new Subject<WayPointData>();

    public IObservable<WayPointData> WayPointDataObservable => _wayPointDataSubject;


    private void Start()
    {
        GameStarted = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger) return;

        TinyCarController carController = other.GetComponent<TinyCarController>();

        if (Constants.IsMultiplayer)
        {
            if (carController != null)
            {
                if (carController.PHView)
                {
                    if (!carController.PHView.IsMine)
                        return;
                }
            }
        }

        if (carController != null)
        {
            _wayPointDataSubject.OnNext(new WayPointData(){ CarController = carController,Waypoint = this});

            if(IsStartWayPoint & !GameStarted)
            {
                GameStarted = true;
                TimeHandler.Instance.timerIsRunning = true;
            }

            if(IsIceCollider)
            {
                Constants.OnIce = !Constants.OnIce;

                if(Constants.OnIce)
                    carController.lateralFriction = 10;
                else
                    carController.lateralFriction = 60;
            }

            if(CheckIce)
            {
                Constants.OnIce = false;
                carController.lateralFriction = 60;
            }
        }
    }
}

public class WayPointData
{
    public TinyCarController CarController;
    public WayPoint Waypoint;
}
