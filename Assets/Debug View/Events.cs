using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static partial class Events
{
    public static event Action<Data> OnUpdateValue = null;
    public static void DoFireUpdateValue(Data data) => OnUpdateValue?.Invoke(data);

    public static event Func<Data, float> OnGetValue = null;
    public static float DoGetValue(Data data) => OnGetValue.Invoke(data);
}
