using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataDD : MonoBehaviour
{
    public int ID = 0;
    public TextMeshProUGUI PlayerNameText;
    public Image CarSprite;

    public void SetPlayerName(string _txt)
    {
        PlayerNameText.text = _txt;
    }

    public void SetID(int _id)
    {
        ID = _id;
    }
}
