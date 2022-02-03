using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PoolDetail : MonoBehaviour
{
    public GameObject _highlighter;
    public TextMeshProUGUI _nFTName;
    public TextMeshProUGUI _nFTToken;
    public Image _animationImg;

    public void AssignPoolDetail(bool _isHighlight, string _name,string _token,Sprite _img)
    {
        _highlighter.SetActive(_isHighlight);
        _nFTName.text = _name;
        _nFTToken.text = "#"+_token;
       // _animationImg.sprite = _img;
    }
}
