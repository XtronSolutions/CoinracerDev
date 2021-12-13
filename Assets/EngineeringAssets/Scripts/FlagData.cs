using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagData : MonoBehaviour
{
    public int FlagID;
    [HideInInspector]
    public GameObject HighlightImage;
    [HideInInspector]
    public Button SelectButton;

    private void OnEnable()
    {
        SelectButton = this.gameObject.GetComponent<Button>();
        SelectButton.onClick.AddListener(SelectFlagIndex);
        HighlightImage = this.gameObject.transform.GetChild(0).gameObject;
    }

    public void ToggleHighlightImage(bool _state)
    {
        HighlightImage.SetActive(_state);
    }

    public void SelectFlagIndex()
    { 
        if(FlagHandler.Instance)
        {
            FlagHandler.Instance.SelectFlag(FlagID);
        }
    }
}
