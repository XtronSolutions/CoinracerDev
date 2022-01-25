using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SetManualRegion : MonoBehaviour
{
    public Dropdown RegionDropDown;
    private List<string> RegionString=new List<string>();

    public void SetRegionString(List<string> _region)
    {
        RegionString = _region;
    }

    public List<string> GetRegionString()
    {
        return RegionString;
    }

    public void DropDownValueChanged()
    {
        int index = RegionDropDown.value;
        if (Constants.SelectedRegion != RegionString[index])
        {
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ChangeConnectionText_ConnectionUI("connecting...");
                MainMenuViewController.Instance.ChangeRegionText_ConnectionUI("Selected Region : n/a");
            }

            Constants.SelectedRegion = RegionString[index];
            PhotonNetwork.SelectedRegion = Constants.SelectedRegion;
            // Debug.Log("connected to region : " + RegionString[index]);
            Debug.Log("Region changed called");
            Constants.RegionChanged = true;
            MultiplayerManager.Instance.DisconnectPhoton();

            
            //RegionDropDown.interactable = false;

            //Invoke("ConnectPhotonAgain", 0.1f);
        }
    }

    public void ConnectPhotonAgain()
    {
        RegionDropDown.interactable = true;
        MultiplayerManager.Instance.ConnectToPhotonServer();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
