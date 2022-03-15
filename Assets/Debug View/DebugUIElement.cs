using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class DebugUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text KeyText;
    [SerializeField] private TMP_Text ValueText;
    [SerializeField] private string Key;
    [SerializeField] private Slider Slider;

    public void Init(Data data)
    {
        Slider.value = data.Value;
        ValueText.text = data.Value.ToString("0.00");
        KeyText.text = Key = data.Key;

        Slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        ValueText.text = value.ToString("0.00");
        Events.DoFireUpdateValue(new Data(Key, value));
    }
}
