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
    [SerializeField] private Button ResetButton;
    
    private float DefaultValue;

    public void Init(Data data)
    {
        Slider.value = data.Value;
        ValueText.text = data.Value.ToString("0.00");
        KeyText.text = Key = data.Key;
        DefaultValue = data.DefaultValue;

        Slider.onValueChanged.AddListener(OnValueChanged);
        ResetButton.onClick.AddListener(OnDefaultButtonPressed);
    }

    private void OnValueChanged(float value)
    {
        ValueText.text = value.ToString("0.00");
        Events.DoFireUpdateValue(new Data(Key, value));
    }

    private void OnDefaultButtonPressed()
    {
        Slider.value = DefaultValue;
        ValueText.text = DefaultValue.ToString("0.00");
        Events.DoFireUpdateValue(new Data(Key, DefaultValue));
    }
}
