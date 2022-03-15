using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugView : MonoBehaviour
{
    [SerializeField] private DebugUIElement ElementPrefab;
    [SerializeField] private GameObject ViewObject;
    [SerializeField] private Transform Container;
    [SerializeField] private List<DebugUIElement> DebugElements;
    private DebugConstants DebugConstants;

    private void Awake()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    private void OnEnable()
    {
        Events.OnUpdateValue += OnUpdateValue;
    }

    private void OnDisable()
    {
        Events.OnUpdateValue -= OnUpdateValue;
    }

    private void OnUpdateValue(Data data)
    {
        PlayerPrefs.SetFloat(data.Key, data.Value);
        DebugConstants.GetType().GetField(data.Key)?.SetValue(DebugConstants, data.Value);
    }

    public void Show()
    {
        ViewObject.SetActive(!ViewObject.activeSelf);

        if (this.DebugConstants == null)
        {
            this.DebugConstants = new DebugConstants();

            var debugConsts = this.DebugConstants;
            var fields = debugConsts.GetType().GetFields();
            var prefab = ElementPrefab;

            foreach (var field in fields)
            {
                var element = Instantiate(prefab, Container);
                var value = (float)field.GetValue(debugConsts);
                var prefsValue = PlayerPrefs.GetFloat(field.Name, value);
                field.SetValue(debugConsts, prefsValue);

                var data = new Data(field.Name, prefsValue);
                element.Init(data);
                element.gameObject.SetActive(true);
            }
        }
    }
}
