using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleConstraint : MonoBehaviour
{
    [SerializeField] private Transform FollowObject;
    [SerializeField] private Transform[] TargetObject;
    [SerializeField] private Vector3 Offset;
    // Start is called before the first frame update

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!FollowObject || TargetObject.Length == 0)
            return;

        foreach (var obj in TargetObject)
            obj.SetPositionAndRotation(FollowObject.position + Offset, FollowObject.rotation);
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        if (!FollowObject || TargetObject.Length==0)
            return;

        foreach (var obj in TargetObject)
            obj.SetPositionAndRotation(FollowObject.position + Offset, FollowObject.rotation);
    }
}
