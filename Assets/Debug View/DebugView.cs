using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DebugView : MonoBehaviour
{
    [SerializeField] private BaseDebugUIElement ElementPrefabFloats, ElementPrefabBools;
    [SerializeField] private GameObject ViewObject;
    [SerializeField] private Transform Container;
    [SerializeField] private List<BaseDebugUIElement> DebugElements;
    private DebugConstants DebugConstants;

    private void Awake()
    {
        Init();
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    private void OnEnable()
    {
        Events.OnUpdateValue += OnUpdateValue;
        Events.OnGetDebugConstants += OnGetDebugConstants;
    }

    private void OnDisable()
    {
        Events.OnUpdateValue -= OnUpdateValue;
        Events.OnGetDebugConstants -= OnGetDebugConstants;
    }

    private DebugConstants OnGetDebugConstants() => DebugConstants;

    private void Init()
    {
        if (this.DebugConstants == null)
        {
            this.DebugConstants = new DebugConstants();

            var debugConsts = this.DebugConstants;
            var fields = debugConsts.GetType().GetFields();
            var prefab = ElementPrefabFloats;

            foreach (var field in fields)
            {
                var element = Instantiate(GetValidPrefab(field.FieldType), Container);
                var defaultValue = field.GetValue(debugConsts);

                // var prefsValue = PlayerPrefs.GetFloat(field.Name, (float)defaultValue);
                var data = new Data(field.Name, defaultValue, defaultValue);
                field.SetValue(debugConsts, defaultValue);
                element.Init(data);
                element.gameObject.SetActive(true);
                DebugElements.Add(element);
            }
        }
    }

    private BaseDebugUIElement GetValidPrefab(Type t)
    {
        if (t == typeof(bool))
        {
            return ElementPrefabBools;
        }

        if (t == typeof(float))
        {
            return ElementPrefabFloats;
        }

        return null;
    }

    private void OnUpdateValue(Data data)
    {
        // PlayerPrefs.SetFloat(data.Key, (float)data.Value);
        DebugConstants.GetType().GetField(data.Key)?.SetValue(DebugConstants, data.Value);
    }

    public void Show()
    {
        ViewObject.SetActive(!ViewObject.activeSelf);
    }

    public void Reset()
    {
        foreach (var element in DebugElements) element.ResetToDefault();
    }
}
