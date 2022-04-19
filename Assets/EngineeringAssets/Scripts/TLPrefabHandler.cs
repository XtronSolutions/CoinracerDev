using UnityEngine.UI;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TLPrefabData
{
    public TextMeshProUGUI _poolAmountText;
    public TextMeshProUGUI _poolIndexText;
    public TextMeshProUGUI _ticketPriceText;
    public Button _enterTournament;
}
public class TLPrefabHandler : MonoBehaviour
{
    public TLPrefabData DataTLPrefab;

    public void SetPrefabData(string poolText,string poolIndex,int ticketPrice)
    {
        DataTLPrefab._poolAmountText.text = poolText;
        DataTLPrefab._poolIndexText.text = poolIndex;
        DataTLPrefab._ticketPriceText.text = ticketPrice.ToString();
    }
}
