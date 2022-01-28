using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTween : MonoBehaviour
{

    public bool IsXAxis = false;
    public bool IsYAxis = false;
    public bool IsZAxis = false;
    public float Speed = 0.0f;
    public float Delay = 0.0f;
    public iTween.EaseType Type;
    public iTween.LoopType LoopType;
    // Start is called before the first frame update
    void OnEnable()
    {
        string selectedAxis = "";

        if (IsXAxis)
            selectedAxis = "x";
        else if (IsYAxis)
            selectedAxis = "y";
        else if (IsZAxis)
            selectedAxis = "z";

        iTween.RotateBy(gameObject, iTween.Hash(selectedAxis, Speed, "easeType", Type.ToString(), "loopType", LoopType.ToString(), "delay", Delay));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
