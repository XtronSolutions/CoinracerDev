using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TransformConstraint : MonoBehaviour
{
    [SerializeField] private Transform FollowObject;
    [SerializeField] private Transform TargetObject;
    [SerializeField] private Vector3 Offset;
    // Start is called before the first frame update

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (!FollowObject || !TargetObject)
           // return;

        //TargetObject.SetPositionAndRotation(FollowObject.position + Offset, FollowObject.rotation);
    }

    private void LateUpdate()
    {
        if (!FollowObject || !TargetObject)
            return;

        TargetObject.transform.position = FollowObject.transform.position + Offset;
        TargetObject.transform.rotation = FollowObject.transform.rotation;
    }
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
            
        if (!FollowObject || !TargetObject)
            return;

        TargetObject.SetPositionAndRotation(FollowObject.position + Offset, FollowObject.rotation);
    }
}
