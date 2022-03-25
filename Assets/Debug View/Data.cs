using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Data
{
    public string Key;
    public object Value;
    public object DefaultValue;

    public Data(string key, object value, object defaultValue = null)
    {
        Key = key;
        Value = value;
        DefaultValue = defaultValue;
    }
}
