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
 
    private float speed = 0.052f;
    public int tokenID = 0;
    public string CarName;
    public NFTMehanicsData Mechanics =new NFTMehanicsData();

  
    public void AccessConsumables()
    {
        StoreHandler.Instance.EnableConsumables_StoreUI(Mechanics,tokenID);
        Debug.Log(Mechanics.mechanicsData.CarName);
    }

    public void SetTokenID(int _id)
    {
        tokenID = _id;
    }

    public void SetCarName(string _name)
    {
        CarName = _name;
    }

    public void SetMechanics()
    {
        Mechanics = FirebaseMoralisManager.Instance.GetMechanics(tokenID);
    }
}

