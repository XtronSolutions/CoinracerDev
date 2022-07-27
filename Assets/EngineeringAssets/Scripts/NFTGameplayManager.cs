using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NFTModelData
{
    public string name;
    public string description;
    public string metaDataURL;
    public Sprite[] AnimationSequence;
}
public class NFTGameplayManager : MonoBehaviour
{
    public static NFTGameplayManager Instance;
    public List<NFTModelData> DataNFTModel = new List<NFTModelData>();
    List<GameObject> GeneratedPrefab = new List<GameObject>();

    private int prefabCounter=0;
    private int rowCounter = 0;
    private GameObject rowPrefab;

    private void OnEnable()
    {
        Instance = this;
    }
    public void InstantiateNFT()
    {
        if (Constants.CheckAllNFT)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            prefabCounter = 0;
            rowCounter = 0;
            MainMenuViewController.Instance.UIGarage.ScrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

            int totalNFTS = 0;
            for (int i = 0; i < Constants.NFTBought.Length; i++)
                totalNFTS += Constants.NFTBought[i];

            for(int i = 0; i < Constants.NFTBought.Length; i++)
            {
                for (int j = 0; j < Constants.NFTBought[i]; j++)
                {
                    if (prefabCounter % 3 == 0)
                    {

                        rowCounter++;
                        rowPrefab = Instantiate(MainMenuViewController.Instance.UIGarage.RowPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        rowPrefab.transform.SetParent(MainMenuViewController.Instance.UIGarage.ScrollContent.transform);
                        rowPrefab.transform.localScale = new Vector3(1, 1, 1);
                        GeneratedPrefab.Add(rowPrefab);
                        MainMenuViewController.Instance.UIGarage.ScrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, MainMenuViewController.Instance.UIGarage.ContentHeight * rowCounter);

                    }

                    GameObject _prefabNFT = Instantiate(MainMenuViewController.Instance.UIGarage.NFTPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    _prefabNFT.transform.SetParent(rowPrefab.transform);
                    _prefabNFT.transform.localScale = new Vector3(1, 1, 1);
                    prefabCounter++;

                    _prefabNFT.GetComponent<NFTDataHandler>().StartCoroutine(_prefabNFT.GetComponent<NFTDataHandler>().GetJSONData(WalletManager.Instance.NFTTokens[i][j], WalletManager.Instance.metaDataURL[i][j]));
                }
            }
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            Invoke("InstantiateNFT", 1f);
        }
    }

    public void RemoveSavedPrefabs()
    {
        for (int k = 0; k < GeneratedPrefab.Count; k++)
        {
            Destroy(GeneratedPrefab[k].gameObject);
        }

        GeneratedPrefab.Clear();
    }
}
