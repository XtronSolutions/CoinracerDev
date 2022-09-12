using Smooth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsDetection : MonoBehaviour
{
    public SmoothSyncPUN2 SyncInstance;
    void OnTriggerEnter(Collider col)
    {
        if (SyncInstance.EnableNetworkDetach && SyncInstance.photonView.IsMine)
        {
            if (col.gameObject.CompareTag("DamageCol"))
            {
                Debug.Log("triggered with Network Car, detaching network sync on mirror for few seconds");

                col.gameObject.GetComponent<PhysicsDetection>().SyncInstance.AddDelayForPhysics = true;
                col.gameObject.GetComponent<PhysicsDetection>().SyncInstance.EnableSync();
            }
        }
    }
}
