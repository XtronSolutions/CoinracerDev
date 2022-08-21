using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;



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

    private void OnEnable()
    {
        Instance = this;
    }
    public void ProcessNFT()
    {
        if (Constants.CheckAllNFT && (Constants.GetMoralisData || Constants.DebugAllCars))
        {
            //MainMenuViewController.Instance.LoadingScreen.SetActive(false);
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

    public void InstantiateAndSetData(string _data,int _tokenID)
    {
        if (!Constants.StoredCarNames.Contains(_data))
            Constants.StoredCarNames.Add(_data);

        for (int i = 0; i < DataNFTModel.Count; i++)
        {
            if (_data.ToLower() == DataNFTModel[i].name.ToLower())
                MainMenuViewController.Instance.AssignStoreGarageData(DataNFTModel[i].CarSelection.gameObject, _tokenID, DataNFTModel[i].name, null, GarageHandler.Instance.ComponentGarage.CarSelectionContainer.transform, true, false);
        }

        if (MainMenuViewController.Instance.GetSelectedCar().Count - 1 == Constants.NFTBought.Length)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            GarageHandler.Instance.ToggleLoaders(false, false, false);
            MainMenuViewController.Instance.AssignStoreGarageCars(GarageHandler.Instance.ComponentGarage.MiddleCar, GarageHandler.Instance.ComponentGarage.LeftCar, GarageHandler.Instance.ComponentGarage.RightCar, GarageHandler.Instance.ComponentGarage.CarSelectionContainer.transform, GarageHandler.Instance.ComponentGarage.CarName_Text, GarageHandler.Instance.ComponentGarage.CarID_Text, true,false);
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
