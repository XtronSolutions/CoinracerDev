using Smooth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsDetection : MonoBehaviour
{
    public SmoothSyncPUN2 SyncInstance;
    bool IsTriggered = false;
    void OnTriggerEnter(Collider col)
    {
        if (!Constants.IsMultiplayer)
            return;

        if (SyncInstance.EnableNetworkDetach && SyncInstance.photonView.IsMine)
        {
            if (col.gameObject.CompareTag("DamageCol"))
            {
                //Debug.Log("triggered with Network Car, detaching network sync on mirror for few seconds");

                IsTriggered = true;
                col.gameObject.GetComponent<PhysicsDetection>().SyncInstance.AddDelayForPhysics = true;
                col.gameObject.GetComponent<PhysicsDetection>().SyncInstance.EnableSync();
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (!Constants.IsMultiplayer)
            return;

        if (SyncInstance.EnableNetworkDetach && SyncInstance.photonView.IsMine)
        {
            if (col.gameObject.CompareTag("DamageCol") && IsTriggered)
            {
                //Debug.Log("exit.....");
                IsTriggered = false;
                col.gameObject.GetComponent<PhysicsDetection>().SyncInstance.EnableSyncStatic();
            }
        }
    }
}

