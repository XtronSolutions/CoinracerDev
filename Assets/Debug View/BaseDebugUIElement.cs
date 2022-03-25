using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class BaseDebugUIElement<T> : MonoBehaviour
{
    public abstract void Init(Data data);
    protected abstract void OnValueChanged(T value);
    protected abstract void OnDefaultButtonPressed();
    public void ResetToDefault() => OnDefaultButtonPressed();
}

public class BaseDebugUIElement : BaseDebugUIElement<object>
{
    public override void Init(Data data)
    {
        
    }

    protected override void OnDefaultButtonPressed()
    {
    }

    protected override void OnValueChanged(object value)
    {
    }
}