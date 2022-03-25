using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class DebugUIElementBool : BaseDebugUIElement
{
    [SerializeField] private TMP_Text KeyText;
    [SerializeField] private TMP_Text ValueText;
    [SerializeField] private string Key;
    [SerializeField] private Toggle Toggle;
    [SerializeField] private Button ResetButton;

    private object DefaultValue;

    public override void Init(Data data)
    {
        Toggle.isOn = (bool)data.Value;
        ValueText.text = data.Value.ToString();
        KeyText.text = Key = data.Key;
        DefaultValue = data.DefaultValue;

        Toggle.onValueChanged.AddListener((bool value) =>
        {
            OnValueChanged(value);
        });

        ResetButton.onClick.AddListener(OnDefaultButtonPressed);
    }

    protected override void OnValueChanged(object value)
    {
        ValueText.text = value.ToString();
        Events.DoFireUpdateValue(new Data(Key, value));
    }

    protected override void OnDefaultButtonPressed()
    {
        Toggle.isOn = (bool)DefaultValue;
        ValueText.text = (DefaultValue).ToString();
        Events.DoFireUpdateValue(new Data(Key, DefaultValue));
    }
}
