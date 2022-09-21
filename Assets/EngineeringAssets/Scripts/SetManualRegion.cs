using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SetManualRegion : MonoBehaviour
{
    public static SetManualRegion Instance;
    public Dropdown RegionDropDown;
    private List<string> RegionString=new List<string>();
    bool settingRegionDone = false;

    private void OnEnable()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    public void SetRegionDone(bool _state)
    {
        settingRegionDone = _state;
    }
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
        if (settingRegionDone)
        {
            int index = RegionDropDown.value;
            //Debug.Log(index);
            if (RegionString.Count > 0 && index < RegionString.Count)
            {
                if (Constants.SelectedRegion != RegionString[index])
                {
                    if (MainMenuViewController.Instance)
                    {
                        MainMenuViewController.Instance.ChangeConnectionText_ConnectionUI("Connecting...");
                        MainMenuViewController.Instance.ChangeRegionText_ConnectionUI("Selected Region : n/a");
                    }

                    Constants.SelectedRegionIndex = index;
                    Constants.PingAPIFetched = false;
                    Constants.SelectedRegion = RegionString[index];
                    PhotonNetwork.SelectedRegion = Constants.SelectedRegion;
                    // Debug.Log("connected to region : " + RegionString[index]);
                    Debug.Log("Region changed called for : "+ PhotonNetwork.SelectedRegion);
                    Constants.RegionChanged = true;
                    MultiplayerManager.Instance.DisconnectPhoton(true);

                    //RegionDropDown.interactable = false;
                    //Invoke("ConnectPhotonAgain", 0.1f);
                }
            }
            else
            {
                //Debug.Log("region string is empty");
            }
        }
    }

    public void ConnectPhotonAgain()
    {
        RegionDropDown.interactable = true;
        MultiplayerManager.Instance.ConnectToPhotonServer();
    }
}
