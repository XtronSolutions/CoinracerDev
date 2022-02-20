using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChipracePoolData : MonoBehaviour
{
    public int _poolID;
    public TextMeshProUGUI _poolText;
    public TextMeshProUGUI _carStalkedText;
    public TextMeshProUGUI _totalEarnedText;
    public Button WatchButton;

    public void AssignPoolData(int _id, string _poolTxt,string _carTxt,string _earnedText,bool _isUpdate=false)
    {
        if(!_isUpdate)
        _poolID = _id;

        _poolText.text = _poolTxt;
        _carStalkedText.text = _carTxt;
        _totalEarnedText.text = _earnedText;
        SubscribeEvent();
    }

    public void SubscribeEvent()
    {
        WatchButton.onClick.AddListener(() => ChipraceHandler.Instance.EnablePoolAnimation(_poolID-1));
    }
}
