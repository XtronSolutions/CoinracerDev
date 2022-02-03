using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NFTPool
{
    public int PoolID;
    public string[] Name;
    public List<TotalNFTData> NFTTotalData = new List<TotalNFTData>();
}

[System.Serializable]
public class TotalNFTData
{
    public string Name;
    public int ID;
}

[System.Serializable]
public class ChipraceUI
{
    public GameObject MainScreen;
    public Button BackButton;
    public GameObject PoolContainerCol;
    public GameObject RowPoolChipracePrefab;
    public GameObject PoolChipracePrefab;

    public GameObject ScrollContent;
    public GameObject ScrollContainer;

    public GameObject PoolDetailPrefab;
    public GameObject DetailPrefab;

    public float ScrollWidth = 210f;
    public float PoolPrefabWidth = 193f;

    
}
public class ChipraceHandler : MonoBehaviour
{
    public ChipraceUI UIChiprace;
    public NFTPool[] PoolNFT;
    private List<GameObject> PoolRowList = new List<GameObject>();
    private List<GameObject> PoolList = new List<GameObject>();
    int prefabCounter = 0;
    int rowCounter = 0;
    GameObject rowPrefab;
    GameObject poolPrefab;

    public static ChipraceHandler Instance;

    private void Start()
    {
        Instance = this;
        PopulateChipraceData();
    }

    public void PopulateChipracePool()
    {
        for (int i = 0; i < PoolRowList.Count; i++)
        {
            Destroy(PoolRowList[i]);
        }
        PoolRowList.Clear();
        PoolList.Clear();

        prefabCounter = 0;
        rowCounter = 0;

        for (int j = 0; j < Constants.PoolCounter; j++)
        {
            if (prefabCounter % 3 == 0)
            {
                rowPrefab = Instantiate(UIChiprace.RowPoolChipracePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                rowPrefab.transform.SetParent(UIChiprace.PoolContainerCol.transform);
                rowPrefab.transform.localScale = new Vector3(1, 1, 1);
                PoolRowList.Add(rowPrefab);
                rowPrefab.GetComponent<HorizontalLayoutGroup>().padding.left = rowCounter * 100;
                rowCounter++;
            }

            poolPrefab = Instantiate(UIChiprace.PoolChipracePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            poolPrefab.transform.SetParent(rowPrefab.transform);
            poolPrefab.transform.localScale = new Vector3(1, 1, 1);
            PoolList.Add(poolPrefab);
            prefabCounter++;

            poolPrefab.GetComponent<ChipracePoolData>().AssignPoolData(prefabCounter, "Pool " + prefabCounter.ToString(), "0", "0", false);
        }

    }

    public void PopulateChipraceData()
    {
        if (Constants.IsTest)
            Constants.WalletConnected = true;

        if (Constants.WalletConnected)//WalletConnected
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            if (Constants.CheckAllNFT || Constants.NFTStored == 0)
            {
                if (Constants.NFTStored == 0)
                {
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                    MainMenuViewController.Instance.ShowToast(3f, "No NFT was purchased, please purchase one.");
                }
                else
                {
                    PopulateChipracePool();
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);

                    InstantiatePoolDetail();
                }
            }
            else
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(true);
                Invoke("PopulateChipraceData", 1f);
            }
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Please connect your wallet first.");
        }
    }

    public void InstantiatePoolDetail()
    {
        if (Constants.CheckAllNFT)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            prefabCounter = 0;
            rowCounter = 0;
            GameObject PoolObj;
            UIChiprace.ScrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 140);
            int PoolCounter = 0;
            int PrefabCounter = 0;

            for (int i = 0; i < PoolNFT.Length; i++)
            {
                PoolCounter = 0;
                for (int j = 0; j < PoolNFT[i].NFTTotalData.Count; j++)
                {
                    if (PoolCounter == 0)
                    {
                        rowPrefab = Instantiate(UIChiprace.PoolDetailPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        rowPrefab.transform.SetParent(UIChiprace.ScrollContainer.transform);
                        rowPrefab.transform.localScale = new Vector3(1, 1, 1);
                    }
                    
                    PoolObj = Instantiate(UIChiprace.DetailPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    PoolObj.transform.SetParent(rowPrefab.GetComponent<ChipracePoolDetail>()._poolContainer.transform);
                    PoolObj.transform.localScale = new Vector3(1, 1, 1);
                    PoolObj.GetComponent<PoolDetail>().AssignPoolDetail(false, PoolNFT[i].NFTTotalData[j].Name, PoolNFT[i].NFTTotalData[j].ID.ToString(), null);
                    PoolCounter++;
                    PrefabCounter++;
                }

                if (PoolCounter != 0)
                {
                    Debug.Log(PoolCounter);
                    rowPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(PoolCounter * UIChiprace.PoolPrefabWidth, 142);
                    rowPrefab.GetComponent<ChipracePoolDetail>()._overlay.GetComponent<RectTransform>().sizeDelta = new Vector2(PoolCounter * UIChiprace.PoolPrefabWidth, 208);
                    rowPrefab.GetComponent<ChipracePoolDetail>()._poolText.GetComponent<RectTransform>().sizeDelta = new Vector2(PoolCounter * UIChiprace.PoolPrefabWidth, 50);

                    rowPrefab.GetComponent<ChipracePoolDetail>()._poolText.text = "Pool " + (i + 1).ToString();
                }
            }

            UIChiprace.ScrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(PrefabCounter * UIChiprace.ScrollWidth, 140);

        }
        else
        {

            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            Invoke("InstantiatePoolDetail", 1f);
        }
    }

}
