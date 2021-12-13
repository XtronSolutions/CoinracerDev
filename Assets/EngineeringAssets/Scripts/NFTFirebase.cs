using FirebaseWebGL.Scripts.FirebaseBridge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Newtonsoft.Json;

public class NFTData
{
    public int gas { get; set; }
    public string Crace { get; set; }
    public int gasLimit { get; set; }
    public string BNB { get; set; }
}

public class NFTFirebase : MonoBehaviour
{
    public static NFTFirebase Instance;
    public NFTData DataNFT;

    private string collectionID = "NFT";
    private string docID = "NFTData";
    void OnEnable()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        GetNFTData(collectionID, docID);
        //DataNFT = JsonConvert.DeserializeObject<NFTData>("{\"gas\":15,\"Crace\":\"270000000000000000000\",\"gasLimit\":526054,\"BNB\":\"110000000000000000\"}");

        //Constants.NFTAmount = BigInteger.Parse(DataNFT.Crace);
        //Constants.BnBValue = BigInteger.Parse(DataNFT.BNB);
        //Constants.GasLimit = DataNFT.gasLimit;
        //Constants.GasPrice = DataNFT.gas;

        //NFTUIManager.Instance.UpdateValues();

    }

    public void GetNFTData(string _collectionID, string _docID)
    {
        Constants.HaveNFTData = false;
        FirebaseFirestore.GetDocument(_collectionID, _docID, gameObject.name, "OnDocGet", "OnDocGetError");
    }

    public void OnDocGet(string info)
    {
        if (info == null || info == "null")
        {
            Constants.HaveNFTData = false;
            Debug.Log("info is null for OnDocGet");
        }
        else
        {
            Debug.Log("got nft data from DB");
            Constants.HaveNFTData = true;
            DataNFT = JsonConvert.DeserializeObject<NFTData>(info);

            Constants.NFTAmount = BigInteger.Parse(DataNFT.Crace);
            Constants.BnBValue = BigInteger.Parse(DataNFT.BNB);
            Constants.GasLimit = DataNFT.gasLimit;
            Constants.GasPrice= DataNFT.gas;

            NFTUIManager.Instance.UpdateValues();
        }
    }

    public void OnDocGetError(string error)
    {
        Constants.HaveNFTData = false;
        Debug.Log(error);
    }

}
