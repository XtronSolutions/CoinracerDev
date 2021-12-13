using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : MonoBehaviour
{

    [SerializeField] private GameObject _blades = null;

    [SerializeField] private Vector3 _rotationAxis;
    [SerializeField] private float _rotationSpeed;

    private void Update()
    {
        Quaternion rotation = Quaternion.Euler(_rotationAxis*Time.deltaTime* _rotationSpeed);

        _blades.transform.rotation = rotation * _blades.transform.rotation;
    }
}
