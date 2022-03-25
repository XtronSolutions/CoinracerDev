using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class DebugUIElement : BaseDebugUIElement
{
    [SerializeField] private TMP_Text KeyText;
    [SerializeField] private TMP_Text ValueText;
    [SerializeField] private string Key;
    [SerializeField] private Slider Slider;
    [SerializeField] private Button ResetButton;

    private object DefaultValue;

    public override void Init(Data data)
    {
        Slider.value = (float)data.Value;
        ValueText.text = ((float)data.Value).ToString("0.00");
        KeyText.text = Key = data.Key;
        DefaultValue = data.DefaultValue;

        Slider.onValueChanged.AddListener((float value) =>
        {
            OnValueChanged(value);
        });

        ResetButton.onClick.AddListener(OnDefaultButtonPressed);
    }

    protected override void OnValueChanged(object value)
    {
        ValueText.text = ((float)value).ToString("0.00");
        Events.DoFireUpdateValue(new Data(Key, value));
    }

    protected override void OnDefaultButtonPressed()
    {
        Slider.value = (float)DefaultValue;
        ValueText.text = ((float)DefaultValue).ToString("0.00");
        Events.DoFireUpdateValue(new Data(Key, DefaultValue));
    }
}
