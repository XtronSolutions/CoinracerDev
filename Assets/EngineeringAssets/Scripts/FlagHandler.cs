using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagHandler : MonoBehaviour
{
    public static FlagHandler Instance;
    public GameObject[] FlagObjectRows;

    private int _flagCounter = 0;
    private int _selectionFlagCounter = 0;
    private void OnEnable()
    {
        Instance = this;
    }

    public void EnableFlags()
    {
        AssignFlagSkins_FlagSelection();
        _flagCounter = 0;
    }

    public void AssignFlagSkins_FlagSelection()
    {
        for (int i = 0; i < FlagObjectRows.Length; i++)
        {
            for (int j = 0; j < FlagObjectRows[i].transform.childCount; j++)
            {
                FlagObjectRows[i].transform.GetChild(j).GetComponent<Image>().sprite = FlagSkins.Instance.FlagSprites[_flagCounter];
                FlagObjectRows[i].transform.GetChild(j).GetComponent<FlagData>().FlagID = _flagCounter;

                if (_flagCounter == Constants.FlagSelectedIndex)
                    FlagObjectRows[i].transform.GetChild(j).GetComponent<FlagData>().ToggleHighlightImage(true);
                else
                    FlagObjectRows[i].transform.GetChild(j).GetComponent<FlagData>().ToggleHighlightImage(false);

                _flagCounter++;
            }
        }
    }

    public void SelectFlag(int index)
    {
        Constants.FlagSelectedIndex = index;
        _selectionFlagCounter = 0;
        for (int i = 0; i < FlagObjectRows.Length; i++)
        {
            for (int j = 0; j < FlagObjectRows[i].transform.childCount; j++)
            {
                if (_selectionFlagCounter == Constants.FlagSelectedIndex)
                    FlagObjectRows[i].transform.GetChild(j).GetComponent<FlagData>().ToggleHighlightImage(true);
                else
                    FlagObjectRows[i].transform.GetChild(j).GetComponent<FlagData>().ToggleHighlightImage(false);

                _selectionFlagCounter++;
            }
        }
    }

    
}
