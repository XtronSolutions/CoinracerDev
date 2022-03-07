using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StalkedNFT
{
    public List<string> NFTNameList = new List<string>();
    public List<int> NFTList = new List<int>();
}

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
    public List<Sprite> NFTSkins = new List<Sprite>();
    public List<double> LevelUpgradeFees = new List<double>();
    public List<TotalNFTData> NFTTotalData = new List<TotalNFTData>();
    public int TotalCars;
    public int TotalEarned;
}

[System.Serializable]
public class TotalNFTData
{
    public string Name;
    public Sprite Skin;
    public int ID;
    public int Level;
    public bool IsUpgradable;
    public int TargetScore;
    public bool IsRunningChipRace;
    public string RemainingTime;
    public int Rewards;
    public int runningCounter;
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

    public GameObject AnimationMainScreen;
    public GameObject[] AnimatingObjects;
}

public class ChipraceHandler : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SetStorage(string key, string val);

    [DllImport("__Internal")]
    private static extern string GetStorageClass(string key, string ObjectName, string callback);

    public ChipraceUI UIChiprace;
    public NFTPool[] PoolNFT;
    private List<GameObject> PoolRowList = new List<GameObject>();
    private List<GameObject> PoolList = new List<GameObject>();
    private List<GameObject> PoolDetailList = new List<GameObject>();
    int prefabCounter = 0;
    int rowCounter = 0;
    GameObject rowPrefab;
    GameObject poolPrefab;
    [HideInInspector]
    public StalkedNFT nftStalked;
    public GameObject NFTApprovalScreen;
    public GameObject CraceChipraceApprovalScreen;

    public static ChipraceHandler Instance;

    int _level;
    double _upgradeFee;

    private void Start()
    {
        Instance = this;
        SubscribeButtonEvent();
        nftStalked = new StalkedNFT();
        nftStalked.NFTList.Clear();
        nftStalked.NFTNameList.Clear();
        GetNFTData();
    }

    public void GetNFTData()
    {

        if (Constants.UseChipraceLocalDB)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        GetStorageClass(Constants.NFTKey,this.gameObject.name,"OnGetNFTData");
#endif
        }
        else
        {
            //string info = FirebaseManager.Instance.GetStalkedNFT();
            //nftStalked = JsonConvert.DeserializeObject<StalkedNFT>(info);
        }
    }

    public void OnGetNFTData(string info)
    {
        if (info != null && info != "null")
        {
            nftStalked=JsonConvert.DeserializeObject<StalkedNFT>(info);
        }
    }

    public void SetChipraceData(int _data,string _name)
    {
        nftStalked.NFTList.Add(_data);
        nftStalked.NFTNameList.Add(_name);

        string _json = JsonConvert.SerializeObject(nftStalked);

        if (Constants.UseChipraceLocalDB)
            SetLocalStorage(Constants.NFTKey, _json);
        else
            FirebaseManager.Instance.SetStalkedNFT(_json);
    }

    public void RemoveAndSetChipraceData(int _data, string _name)
    {
        nftStalked.NFTList.Remove(_data);
        nftStalked.NFTNameList.Remove(_name);

        string _json;

        if (nftStalked.NFTList.Count == 0)
            _json = "";
        else
            _json = JsonConvert.SerializeObject(nftStalked);

        if (Constants.UseChipraceLocalDB)
            SetLocalStorage(Constants.NFTKey, _json);
        else
            FirebaseManager.Instance.SetStalkedNFT(_json);
    }

    public void SetLocalStorage(string key, string data)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetStorage(key, data);
#endif
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
        DisablePoolAnimation();
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

            poolPrefab.GetComponent<ChipracePoolData>().AssignPoolData(prefabCounter, "Pool " + prefabCounter.ToString(), PoolNFT[prefabCounter-1].TotalCars.ToString(), PoolNFT[prefabCounter-1].TotalEarned.ToString(), false);
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

                    _level = PoolNFT[i].NFTTotalData[j].Level;
                    _upgradeFee = 0;

                    if (_level!=5)
                     _upgradeFee = PoolNFT[i].LevelUpgradeFees[_level];      

                    PoolObj.GetComponent<PoolDetail>().AssignTokenData(PoolNFT[i].NFTTotalData[j].Name, PoolNFT[i].NFTTotalData[j].ID, PoolNFT[i].NFTTotalData[j].Level, PoolNFT[i].NFTTotalData[j].IsUpgradable, PoolNFT[i].NFTTotalData[j].TargetScore, PoolNFT[i].NFTTotalData[j].IsRunningChipRace, PoolNFT[i].NFTTotalData[j].RemainingTime, _upgradeFee, PoolNFT[i].NFTTotalData[j].Rewards);
                    PoolObj.GetComponent<PoolDetail>().AssignPoolDetail(false, PoolNFT[i].NFTTotalData[j].Name, PoolNFT[i].NFTTotalData[j].ID.ToString(), PoolNFT[i].NFTTotalData[j].Skin, PoolNFT[i].PoolID.ToString(), PoolNFT[i].NFTTotalData[j].Level.ToString(), PoolNFT[i].NFTTotalData[j].TargetScore.ToString());

                    PoolCounter++;
                    PrefabCounter++;
                }

                if (PoolCounter != 0)
                {
                    //Debug.Log(PoolCounter);
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
            
            if (Constants.CheckAllNFT)
            {
                int totalNFTS = 0;
                for (int i = 0; i < Constants.NFTStored.Length; i++)
                {
                    totalNFTS += Constants.NFTStored[i];
                }
                if (totalNFTS == 0 && (nftStalked == null || nftStalked.NFTList.Count==0))
                {
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                    MainMenuViewController.Instance.ShowToast(3f, "No NFT was purchased, please purchase one.");
                }
                else
                {
                    ClearData();
                    Constants.ChipraceDataChecked = false;
                    UpdateChipraceData();
                    StartCoroutine(UpdateChiprace());
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

    async public void UpdateChipraceData()
    {
        for (int i = 0; i < PoolNFT.Length; i++)
        {
            for (int j = 0; j < PoolNFT[i].NFTTotalData.Count; j++)
            {
                TotalNFTData _tokenData = new TotalNFTData();
                _tokenData=await WalletManager.Instance.getAllDataFromFunc(PoolNFT[i].NFTTotalData[j].ID.ToString());

                PoolNFT[i].NFTTotalData[j].Level = _tokenData.Level+ 1;
                PoolNFT[i].NFTTotalData[j].IsRunningChipRace = _tokenData.IsRunningChipRace ;
                PoolNFT[i].NFTTotalData[j].IsUpgradable = _tokenData.IsUpgradable;
                PoolNFT[i].NFTTotalData[j].RemainingTime = _tokenData.RemainingTime;
                PoolNFT[i].NFTTotalData[j].TargetScore = _tokenData.TargetScore;
                PoolNFT[i].NFTTotalData[j].Rewards = _tokenData.Rewards;
                PoolNFT[i].NFTTotalData[j].runningCounter = _tokenData.runningCounter;
            }
        }

        Constants.ChipraceDataChecked = true;
    }

    public void UpdateTotal()
    {
        int _totalCars;
        int _totalEarned;
        for (int i = 0; i < PoolNFT.Length; i++)
        {
            _totalCars = 0;
            _totalEarned = 0;
            for (int j = 0; j < PoolNFT[i].NFTTotalData.Count; j++)
            {
                if (PoolNFT[i].NFTTotalData[j].IsRunningChipRace)
                    _totalCars++;

                _totalEarned += PoolNFT[i].NFTTotalData[j].Rewards;
            }

            PoolNFT[i].TotalCars = _totalCars;
            PoolNFT[i].TotalEarned = _totalEarned;
        }
    }

    IEnumerator UpdateChiprace()
    {
        yield return new WaitUntil(() => Constants.ChipraceDataChecked);
        Invoke("PopulateDelay", 0.25f);
    }

    public void PopulateDelay()
    {
        DisablePoolAnimation();
        UpdateTotal();
        PopulateChipracePool();
        InstantiatePoolDetail();
    }

    public void ForceUpdate()
    {
        ClearData();
        Constants.ChipraceDataChecked = false;
        UpdateChipraceData();
        StartCoroutine(UpdateChiprace());
        Invoke("ForceUpdateDelay", 1f);
    }

    public void ForceUpdateDelay()
    {
        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
    }

    public void ToggleAnimationScreen(bool _state)
    {
        UIChiprace.AnimationMainScreen.SetActive(_state);
    }

    public void ToggleAnimationObjects(int _index)
    {
        for (int i = 0; i < UIChiprace.AnimatingObjects.Length; i++)
        {
            if (_index == i)
                UIChiprace.AnimatingObjects[i].SetActive(true);
            else
                UIChiprace.AnimatingObjects[i].SetActive(false);
        }
    }

    public void EnablePoolAnimation(int index)
    {
        ToggleAnimationScreen(true);
        ToggleAnimationObjects(index);
    }

    public void DisablePoolAnimation()
    {
        ToggleAnimationScreen(false);
    }
}
