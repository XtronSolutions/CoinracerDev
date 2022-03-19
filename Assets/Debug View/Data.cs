using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Data
{
    public string Key;
    public float Value;
    public float DefaultValue;

    public Data(string key, float value, float defaultValue = 0)
    {
        Key = key;
        Value = value;
        DefaultValue = defaultValue;
    }
}
