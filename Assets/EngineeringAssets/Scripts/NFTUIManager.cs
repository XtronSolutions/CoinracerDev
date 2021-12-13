using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Video;
using TMPro;

[Serializable]
public class MetaClass
{
    public string name;
    public string description;
    public string image;
    public string IPFSJson;
    public string ClipName;
}

[Serializable]
public class NFTUI
{
    public GameObject ShowCaseObject;
    public TextMeshProUGUI PriceTitle;
    public TextMeshProUGUI PriceValue;
    public TextMeshProUGUI AmountTitle;
    public TextMeshProUGUI AmountValue;
    public Button MintButton;
    public Image[] Underlines;
}

public class NFTUIManager : MonoBehaviour
{
    public MetaClass[] MetaDataArray;
    public static NFTUIManager Instance;
    public NFTUI UINFT;

    public GameObject LoadingScreen;
    public GameObject MessagePopUScreen;
    public TextMeshProUGUI Messagetext;

    private int Counter = 0;
    private int Amount = 1;
    private int SelectedIndex = 0;
    private void OnEnable()
    {
        Instance = this;
        Counter = 0;
        Amount = 1;
        UpdateValues();
    }

    public void UpdateValues()
    {
        Constants.StoredNFTAmount = Amount * Constants.NFTAmount;
        Constants.StoredBNBAmount = Amount * Constants.BnBValue;
        UpdateUIData();
    }

    public void ToggleLoadingScreen(bool _state)
    {
        LoadingScreen.SetActive(_state);
    }

    public void ShowToast(string _msg,float _sec=3f)
    {
        MessagePopUScreen.SetActive(true);
        Messagetext.text = _msg;
        Invoke("DisableToast", _sec);

    }

    public void DisableToast()
    {
        MessagePopUScreen.SetActive(false);
    }
    public void SubscribeButtonEvent()
    {
        // UINFT.MintButton.onClick.AddListener(OnGoToCarSelection);
    }

    public IEnumerator GetGifData()
    {
        Debug.Log(MetaDataArray[Counter].image);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(MetaDataArray[Counter].image))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
    public void IncreaseAmount()
    {
        if (Amount < 5)
        {
            Amount++;
            Constants.StoredNFTAmount = Amount * Constants.NFTAmount;
            Constants.StoredBNBAmount = Amount * Constants.BnBValue;
            UpdateUIData();
        }
    }

    public void DecreaseAmount()
    {
        if (Amount > 1)
        {
            Amount--;
            Constants.StoredNFTAmount = Amount * Constants.NFTAmount;
            Constants.StoredBNBAmount = Amount * Constants.BnBValue;
            UpdateUIData();
        }
    }

    public void UpdateUIData()
    {
        double _bnb =(double) Constants.StoredBNBAmount / 1000000000000000000;
        UINFT.PriceValue.text = (Constants.StoredNFTAmount / 1000000000000000000).ToString()+" CRACE + " + _bnb.ToString() + " BNB";
        UINFT.AmountValue.text = Amount.ToString();
    }

    public int GetAmount()
    {
        return Amount;
    }
}
