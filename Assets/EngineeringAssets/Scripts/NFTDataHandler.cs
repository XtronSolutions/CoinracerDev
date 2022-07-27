using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class IPFSdata
{
    public string name { get; set; }
    public string description { get; set; }
    public string image { get; set; }
}
public class NFTDataHandler : MonoBehaviour
{
    private IPFSdata dataIPFS;
    public Image AnimationRef;
    public TextMeshProUGUI ModelName;
    public TextMeshProUGUI ModelID;
    public Button ConsumableButton;
 
    private float speed = 0.052f;
    private int tokenID = 0;

    private void Start()
    {
        ConsumableButton.onClick.AddListener(AccessConsumables);
    }
    public void AccessConsumables()
    {
        MainMenuViewController.Instance.EnableConsumables_StoreUI();
    }
    public void AssignData(Sprite[] _sprites,string _name, string _id)
    {
        StartCoroutine(PlayAnimation(_sprites));
        ModelName.text = _name;
        ModelID.text = "#" + _id;
    }

    public IEnumerator PlayAnimation(Sprite[] _sprites)
    {
        for (int i = 0; i < _sprites.Length; i++)
        {
            AnimationRef.sprite = _sprites[i];
            yield return new WaitForSecondsRealtime(speed);
        }

        StartCoroutine(PlayAnimation(_sprites));
    }

    public IEnumerator GetJSONData(int _tokenID,string _URL)
    {
        tokenID = _tokenID;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_URL))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Getting IPFS : Error : " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("Getting IPFS : HTTP Error : " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(tokenID);
                    //Debug.Log(webRequest.downloadHandler.text);
                    dataIPFS = JsonConvert.DeserializeObject<IPFSdata>(webRequest.downloadHandler.text);

                    if(!Constants.StoredCarNames.Contains(dataIPFS.name))
                        Constants.StoredCarNames.Add(dataIPFS.name);

                    for (int i = 0; i < NFTGameplayManager.Instance.DataNFTModel.Count; i++)
                    {
                        if (dataIPFS.name.ToLower() == NFTGameplayManager.Instance.DataNFTModel[i].name.ToLower())
                        {
                            AssignData(NFTGameplayManager.Instance.DataNFTModel[i].AnimationSequence, NFTGameplayManager.Instance.DataNFTModel[i].name,tokenID.ToString());
                        }
                    }
                    break;
            }
        }
    }



}

