using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapHandler : MonoBehaviour
{
    [SerializeField] public Slider miniMap;

    public float progressCount = 0f;

        // Start is called before the first frame update
    void Start()
    {
        miniMap.value = 0;
    }
    IEnumerator changeProgressValue(float _val)
    {
        _val = _val - 0.01f;
        if (_val < 0)
        {
            yield break;
        }
        yield return new WaitForSeconds(0.1f);
        miniMap.value = miniMap.value+0.01f;
        StartCoroutine(changeProgressValue(_val));

    }

    public void startSinglePlayerProgressBar()
    {
        Debug.Log("increasing..");
        progressCount = RaceManager.Instance._miniMapCounter;
        miniMap.value = miniMap.value+progressCount;
        //StartCoroutine(changeProgressValue(progressCount));
         Debug.Log(progressCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
