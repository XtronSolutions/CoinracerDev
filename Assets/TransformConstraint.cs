using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformConstraint : MonoBehaviour
{
    [SerializeField] private Transform FollowObject;
    [SerializeField] private Transform TargetObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TargetObject.SetPositionAndRotation(FollowObject.position, FollowObject.rotation);
    }
}
