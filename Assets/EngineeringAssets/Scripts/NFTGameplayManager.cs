using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class NFTModelData
{
    public string name;
    public int MetaID;
    public string description;
    public string metaDataURL;
    public CarSelection CarSelection;
    public StatSettings settings;
    public Sprite[] AnimationSequence;
}
public class NFTGameplayManager : MonoBehaviour
{
    public static NFTGameplayManager Instance;
    public List<NFTModelData> DataNFTModel = new List<NFTModelData>();
    List<GameObject> GeneratedPrefab = new List<GameObject>();

    private int prefabCounter = 0;
    private int rowCounter = 0;
    private GameObject rowPrefab;
    private IPFSdata dataIPFS;
    private StatSettings _statSettings;

    private void OnEnable()
    {
        Instance = this;
    }
    public void ProcessNFT()
    {
        if (Constants.DebugAllCars)
            Constants.GetMoralisData = true;

        if (Constants.CheckAllNFT && (Constants.GetMoralisData || Constants.DebugAllCars))
        {
            GarageHandler.Instance.ResetSelectedCar();

            prefabCounter = 0;
            rowCounter = 0;

            int totalNFTS = 0;
            for (int i = 0; i < Constants.NFTBought.Length; i++)
                totalNFTS += Constants.NFTBought[i];


            for (int i = 0; i < Constants.NFTBought.Length; i++)
            {
                for (int j = 0; j < Constants.NFTBought[i]; j++)
                {
                    StartCoroutine(GetJSONData(WalletManager.Instance.NFTTokens[i][j], WalletManager.Instance.metaDataURL[i][j]));
                }
            }

            if(totalNFTS==0) //no car exists on chain
            {
                InstantiateBoughtFromMoralis();
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                GarageHandler.Instance.ToggleLoaders(false, false, false);
                MainMenuViewController.Instance.AssignStoreGarageCars(GarageHandler.Instance.ComponentGarage.MiddleCar, GarageHandler.Instance.ComponentGarage.LeftCar, GarageHandler.Instance.ComponentGarage.RightCar, GarageHandler.Instance.ComponentGarage.CarSelectionContainer.transform, GarageHandler.Instance.ComponentGarage.CarName_Text, GarageHandler.Instance.ComponentGarage.CarID_Text, true, false, true);
            }
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            Invoke("ProcessNFT", 1f);
        }
    }

    public IEnumerator GetJSONData(int _tokenID, string _URL)
    {
       //tokenID = _tokenID; todo
        //Mechanics = FirebaseMoralisManager.Instance.GetMechanics(tokenID);todo

        if (Constants.DebugAllCars)
        {
            InstantiateAndSetData(_URL, _tokenID);
        }
        else
        {
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
                        dataIPFS = JsonConvert.DeserializeObject<IPFSdata>(webRequest.downloadHandler.text);
                        InstantiateAndSetData(dataIPFS.name, _tokenID);
                        break;
                }
            }
        }
    }

    int BoughtCounter = 0;
    async public void InstantiateAndSetData(string _data,int _tokenID)
    {
        if (!Constants.StoredCarNames.Contains(_data))
            Constants.StoredCarNames.Add(_data);

        for (int i = 0; i < DataNFTModel.Count; i++)
        {
            if (_data.ToLower() == DataNFTModel[i].name.ToLower())
            {
                _statSettings = StoreHandler.Instance.GetDealerDicIndex(DataNFTModel[i].MetaID);
                if (_statSettings == null) _statSettings = DataNFTModel[i].settings;

                MainMenuViewController.Instance.AssignStoreGarageData(DataNFTModel[i].CarSelection.gameObject, _tokenID, DataNFTModel[i].name, _statSettings, GarageHandler.Instance.ComponentGarage.CarSelectionContainer.transform, true, true);
            }
        }

        BoughtCounter = 0;

        for (int i = 0; i < Constants.NFTBought.Length; i++)
        {
            BoughtCounter += Constants.NFTBought[i];
        }

        Constants.PrintLog("Selected cars count : " + (MainMenuViewController.Instance.GetSelectedCar().Count-1).ToString() + " and NFT Bought Count : " + BoughtCounter);
        if (MainMenuViewController.Instance.GetSelectedCar().Count - 1 == BoughtCounter)
        {
            InstantiateBoughtFromMoralis();
        }
    }

    async public void InstantiateBoughtFromMoralis()
    {
        string _response = await apiRequestHandler.Instance.ProcessAllMyNFTRequest(Constants.WalletAddress);

        Constants.PrintLog(_response);
        if (!string.IsNullOrEmpty(_response))
        {
            MoralisNFTArrayResponse _dataNEW = new MoralisNFTArrayResponse();
            _dataNEW = JsonConvert.DeserializeObject<MoralisNFTArrayResponse>(_response);


            for (int i = 0; i < _dataNEW.result.Count; i++)
            {
                Constants.PrintLog("searching for : " + _dataNEW.result[i].name);
                if(!string.IsNullOrEmpty(_dataNEW.result[i].name))
                {
                    for (int k = 0; k < DataNFTModel.Count; k++)
                    {
                        if (_dataNEW.result[i].name.ToLower() == DataNFTModel[k].name.ToLower())
                        {
                            if (Constants.StoredCarNames.Contains(_dataNEW.result[i].name))
                            {
                                Constants.PrintLog("car already added from wallet, skipping....");
                            }
                            else
                            {
                                _statSettings = StoreHandler.Instance.GetDealerDicIndex(DataNFTModel[k].MetaID);
                                if (_statSettings == null) _statSettings = DataNFTModel[k].settings;

                                Constants.PrintLog("founed, adding: " + _dataNEW.result[i].name);

                                NFTMehanicsData _newData = new NFTMehanicsData();
                                _newData.OwnerWalletAddress = _dataNEW.result[i].ownerWallet;

                                string _carName = _dataNEW.result[i].name;
                                int _carHealth = Constants.MaxCarHealth;
                                float _carTyreLaps = 0;
                                float _carOilLaps = 0;
                                float _carGasLaps = 0;
                                string _carNameStats = "";
                                double _acceleration = 0;
                                double _topSpeed = 0;
                                double _cornering = 0;
                                double _hp = 0;
                                int _price = 0;
                                int _tier = 0;
                                int _type = 0;

                                if (!string.IsNullOrEmpty(_dataNEW.result[i].mechanics))
                                {
                                    JToken Jresponse = JObject.Parse(_dataNEW.result[i].mechanics);

                                    _carHealth = Jresponse.SelectToken("CarHealth") != null ? (int)Jresponse.SelectToken("CarHealth") : Constants.MaxCarHealth;
                                    _carTyreLaps = Jresponse.SelectToken("Tyre_Laps") != null ? (float)Jresponse.SelectToken("Tyre_Laps") : 0;
                                    _carOilLaps = Jresponse.SelectToken("EngineOil_Laps") != null ? (float)Jresponse.SelectToken("EngineOil_Laps") : 0;
                                    _carGasLaps = Jresponse.SelectToken("Gas_Laps") != null ? (float)Jresponse.SelectToken("Gas_Laps") : 0;

                                    _carNameStats = Jresponse.SelectToken("Stats").SelectToken("Name") != null ? (string)Jresponse.SelectToken("Stats").SelectToken("Name") : _carName;
                                    _acceleration = Jresponse.SelectToken("Stats").SelectToken("Acceleration") != null ? (double)Jresponse.SelectToken("Stats").SelectToken("Acceleration") : 100;
                                    _topSpeed = Jresponse.SelectToken("Stats").SelectToken("TopSpeed") != null ? (double)Jresponse.SelectToken("Stats").SelectToken("TopSpeed") : 100;
                                    _cornering = Jresponse.SelectToken("Stats").SelectToken("Cornering") != null ? (double)Jresponse.SelectToken("Stats").SelectToken("Cornering") : 100;
                                    _hp = Jresponse.SelectToken("Stats").SelectToken("HP") != null ? (double)Jresponse.SelectToken("Stats").SelectToken("HP") : Constants.MaxCarHealth;
                                    _price = Jresponse.SelectToken("Stats").SelectToken("Price") != null ? (int)Jresponse.SelectToken("Stats").SelectToken("Price") : 250;
                                    _tier = Jresponse.SelectToken("Stats").SelectToken("Tier") != null ? (int)Jresponse.SelectToken("Stats").SelectToken("Tier") : 2;
                                    _type = Jresponse.SelectToken("Stats").SelectToken("Type") != null ? (int)Jresponse.SelectToken("Stats").SelectToken("Type") : 0;
                                }

                                Stats _MoralisStatsettings = new Stats();
                                _MoralisStatsettings.Name = _carNameStats;
                                _MoralisStatsettings.Acceleration = _acceleration;
                                _MoralisStatsettings.TopSpeed = _topSpeed;
                                _MoralisStatsettings.Cornering = _cornering;
                                _MoralisStatsettings.HP = _hp;
                                _MoralisStatsettings.Price = _price;
                                _MoralisStatsettings.Tier = _tier;
                                _MoralisStatsettings.Type = _type;

                                _newData.MetaData = _dataNEW.result[i].metadata;
                                _newData.mechanicsData = new MechanicsData(_carName, _carHealth, _carTyreLaps, _carOilLaps, _carGasLaps, _MoralisStatsettings);
                                FirebaseMoralisManager.Instance.SetMechanics(int.Parse(_dataNEW.result[i].tokenId), _newData,true);

                                MainMenuViewController.Instance.AssignStoreGarageData(DataNFTModel[k].CarSelection.gameObject, int.Parse(_dataNEW.result[i].tokenId), DataNFTModel[k].name, _statSettings, GarageHandler.Instance.ComponentGarage.CarSelectionContainer.transform, true, true);
                            }
                        }
                    }
                }
            }
        }

        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        GarageHandler.Instance.ToggleLoaders(false, false, false);
        MainMenuViewController.Instance.AssignStoreGarageCars(GarageHandler.Instance.ComponentGarage.MiddleCar, GarageHandler.Instance.ComponentGarage.LeftCar, GarageHandler.Instance.ComponentGarage.RightCar, GarageHandler.Instance.ComponentGarage.CarSelectionContainer.transform, GarageHandler.Instance.ComponentGarage.CarName_Text, GarageHandler.Instance.ComponentGarage.CarID_Text, true, false, true);
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
