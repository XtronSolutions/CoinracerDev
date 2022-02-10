using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NFTTokens
{
    public string Name;
    public List<int> ID=new List<int>();
}


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

    public Button ChipraceButton;

    
}
public class ChipraceHandler : MonoBehaviour
{
    public ChipraceUI UIChiprace;
    public NFTPool[] PoolNFT;
    private List<GameObject> PoolRowList = new List<GameObject>();
    private List<GameObject> PoolList = new List<GameObject>();
    private List<GameObject> PoolDetailList = new List<GameObject>();
    int prefabCounter = 0;
    int rowCounter = 0;
    GameObject rowPrefab;
    GameObject poolPrefab;

    public static ChipraceHandler Instance;

    private void Start()
    {
        Instance = this;
        SubscribeButtonEvent();
    }

    public void SubscribeButtonEvent()
    {
        UIChiprace.ChipraceButton.onClick.AddListener(PopulateChipraceData);
        UIChiprace.BackButton.onClick.AddListener(BackButtonClicked);
    }

    public void ToggleMainScreen(bool _state)
    {
        UIChiprace.MainScreen.SetActive(_state);
    }

    public void BackButtonClicked()
    {
        ClearData();
        ToggleMainScreen(false) ;
    }

    public void ClearData()
    {
        for (int i = 0; i < PoolRowList.Count; i++)
            Destroy(PoolRowList[i]);

        for (int i = 0; i < PoolDetailList.Count; i++)
            Destroy(PoolDetailList[i]);

        PoolRowList.Clear();
        PoolList.Clear();
        PoolDetailList.Clear();
        prefabCounter = 0;
        rowCounter = 0;
    }
    public void PopulateChipracePool()
    {
        ClearData();

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

    public void InstantiatePoolDetail()
    {
        if (Constants.CheckAllNFT)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            GameObject PoolObj;
            int PoolCounter;
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
                        PoolDetailList.Add(rowPrefab);
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

            RectTransform _Rect = UIChiprace.ScrollContent.GetComponent<RectTransform>();
            _Rect.sizeDelta = new Vector2(PrefabCounter * UIChiprace.ScrollWidth, _Rect.sizeDelta.y);

        }
        else
        {

            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            Invoke("InstantiatePoolDetail", 1f);
        }
    }

    public void PopulateChipraceData()
    {
        ToggleMainScreen(true);
        //UIChiprace.ScrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 140);

        if (Constants.IsTest)
            Constants.WalletConnected = true;

        if (Constants.WalletConnected)
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
                    Invoke("PopulateDelay", 0.25f);
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

    public void PopulateDelay()
    {
        PopulateChipracePool();
        InstantiatePoolDetail();
    }
}
