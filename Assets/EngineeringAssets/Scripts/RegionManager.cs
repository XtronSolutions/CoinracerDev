using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class RegionResponse
{
    public string code { get; set; }
    public string address { get; set; }
    public int masterPeerCount { get; set; }
    public int peerCount { get; set; }
    public int gameCount { get; set; }
}
public class RegionManager : MonoBehaviour
{
    public static RegionManager Instance;
    public SetManualRegion _RegionRef;
    public Dropdown RegionMainList;
    bool RegionPinged = false;
    Dictionary<string, string> RegionNames = new Dictionary<string, string>();
    List<RegionResponse> myDeserializedClass = new List<RegionResponse>();
    private string GetURL = "http://64.227.31.248:8008/";
    string ExampleResponse = "[{\"code\":\"eu\",\"address\":\"ws://GCAMS146.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"us\",\"address\":\"ws://5BFA7B2F847F6354744DE369F8496DF0.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"usw\",\"address\":\"ws://F943322039644213464E42C7AFB865A2.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"cae\",\"address\":\"ws://1E4C943035393DBEB337C36E0F8A2A09.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"asia\",\"address\":\"ws://5C41DFF7F36BC37011BD8F000BF90B38.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"jp\",\"address\":\"ws://AZJP005000001.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"au\",\"address\":\"ws://9BD5E99DD999F2851DBEC9F768E9BA23.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"sa\",\"address\":\"ws://GCSP004.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"in\",\"address\":\"ws://20C070ADE8D6F680D898A4EF4D626B57.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"ru\",\"address\":\"ws://GCMOS015.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"rue\",\"address\":\"ws://4746102D4930B4FA9C57B103E8D1EE91.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"kr\",\"address\":\"ws://AZKR003000000.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"za\",\"address\":\"ws://B33E312B471BD9630E44738C09976E08.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"tr\",\"address\":\"ws://GCIST001.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0}]";
    Dictionary<string, RegionResponse> _tempRegionData = new Dictionary<string, RegionResponse>();

    int playerCount;
    string[] dropdownCaptionSplit;
    string dropdownOptionTxt;
    string[] dropdownOptionSplit;
    private void Start()
    {
        Instance = this;
        //UpdateReference();
        ResetRegions();
        AddRegionNames();

        if (!PhotonNetwork.IsConnected)
        {
            PopulateRegionData(ExampleResponse);
            StartCoroutine(ShowPingedRegionList_ConnectionUI());
        }else
        {
            Constants.PingAPIFetched = true;
            PhotonNetwork.GotPingResult = true;
            UpdatePingList(Constants.StoredRegions, Constants.StoredPings);
        }

        InvokeRepeating(nameof(GetAllRegiosnData), 0.3f, 5f);

        for (int i = 0; i < Constants.StoredRegions.Count; i++)
        {
            Debug.Log(Constants.StoredRegions[i]);
        }

    }

    public bool UpdateReference()
    {
        if(SetManualRegion.Instance)
        {
            _RegionRef = SetManualRegion.Instance;
            RegionMainList = _RegionRef.gameObject.GetComponent<Dropdown>();
            return true;
        }
        else
        {
            Debug.Log("Dropdownref was lost, checking...");
            Invoke(nameof(UpdateReference), 0.2f);
            return false;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void PopulateRegionData(string _response)
    {
        Debug.Log(_response);
        _tempRegionData.Clear();
        myDeserializedClass = JsonConvert.DeserializeObject<List<RegionResponse>>(_response);

        for (int i = 0; i < myDeserializedClass.Count; i++)
            _tempRegionData.Add(myDeserializedClass[i].code, myDeserializedClass[i]);

        Constants.RegionData = _tempRegionData;
        
        //if (!PhotonNetwork.GotPingResult && PhotonNetwork.InLobby)
            //PhotonNetwork.GotPingResult = true;

        Debug.Log("PingAPIFetched: " + Constants.PingAPIFetched);
        if (PhotonNetwork.IsConnected)
             UpdateDropDownValues(RegionMainList);
    }

    public void AddRegionNames()
    {
        RegionNames.Clear();
        RegionNames.Add("asia", "Asia");
        RegionNames.Add("au", "Australia");
        RegionNames.Add("cae", "Canada, East");
        RegionNames.Add("cn", "Chinese Mainland");
        RegionNames.Add("eu", "Europe");
        RegionNames.Add("in", "India");
        RegionNames.Add("jp", "Japan");
        RegionNames.Add("ru", "Russia");
        RegionNames.Add("rue", "Russia, East");
        RegionNames.Add("za", "South Africa");
        RegionNames.Add("sa", "South America");
        RegionNames.Add("kr", "South Korea");
        RegionNames.Add("tr", "Turkey");
        RegionNames.Add("us", "USA, East");
        RegionNames.Add("usw", "USA, West");
    }
    public void ResetRegions()
    {
        RegionPinged = false;
        RegionMainList.interactable = false;
        RegionMainList.options.Clear();
        RegionMainList.options.Add(new Dropdown.OptionData() { text = "Connecting..." });
        RegionMainList.value = 0;
    }
    public IEnumerator ShowPingedRegionList_ConnectionUI()
    {
        Debug.Log("Got ping result "+ PhotonNetwork.GotPingResult);
        yield return new WaitUntil(() => (PhotonNetwork.GotPingResult));

        Constants.StoredRegions.Clear();
        Constants.StoredPings.Clear();

        for (int q = 0; q < PhotonNetwork.pingedRegions.Length; q++)
            Constants.StoredRegions.Add(PhotonNetwork.pingedRegions[q]);

        for (int p = 0; p < PhotonNetwork.pingedRegionPings.Length; p++)
            Constants.StoredPings.Add(PhotonNetwork.pingedRegionPings[p]);

        UpdatePingList(Constants.StoredRegions, Constants.StoredPings);
        PhotonNetwork.GotPingResult = false;
    }

    public void UpdatePingList(List<string> regions, List<string> pings)
    {
        if (!RegionPinged)
        {
            RegionPinged = true;
            var dropdown = RegionMainList;
            UpdateRegionsData(dropdown, _RegionRef);
        }else
        {
            Constants.PrintError("RegionPinged is false");
        }
    }

    public void UpdateRegionsData(Dropdown dropdown, SetManualRegion _RegionRef)
    {
        _RegionRef.SetRegionDone(false);
        RegionMainList.interactable = false;
        dropdown.options.Clear();
        List<string> _regions = new List<string>();
        if (Constants.StoredPings.Count > 0)
        {
            int minimumPing = int.Parse(Constants.StoredPings[0]);
            int currentPing;
            //dropdown.value = 1;
            for (int i = 0; i < Constants.StoredRegions.Count; i++)
            {
                playerCount = Constants.RegionData[Constants.StoredRegions[i]].peerCount + Constants.RegionData[Constants.StoredRegions[i]].masterPeerCount;
                dropdown.options.Add(new Dropdown.OptionData() { text = RegionNames[Constants.StoredRegions[i]] + "[" + Constants.StoredPings[i] + "ms"+"] "+ playerCount + " player/s" });
                _regions.Add(Constants.StoredRegions[i]);
                currentPing = int.Parse(Constants.StoredPings[i]);
                if (currentPing < minimumPing && Constants.SelectedRegion=="")
                {
                    minimumPing = currentPing;
                    dropdown.value = i + 1;
                }
            }

            _RegionRef.SetRegionString(_regions);

            RegionPinged = false;
            RegionMainList.interactable = true;
            Constants.PingAPIFetched = true;
            _RegionRef.SetRegionDone(true);
        }
        else
        {
            Constants.PrintError("region list is empty");
        }
    }

    public void UpdateDropDownValues(Dropdown dropdown)
    {
        Constants.SelectedRegionIndex = RegionMainList.value;
        if (Constants.StoredPings.Count > 0)
        {
            Constants.PrintLog("updating values.");
            for (int i = 0; i < Constants.StoredRegions.Count; i++)
            {
                playerCount = Constants.RegionData[Constants.StoredRegions[i]].peerCount + Constants.RegionData[Constants.StoredRegions[i]].masterPeerCount;
                dropdownOptionTxt= RegionNames[Constants.StoredRegions[i]] + "[" + Constants.StoredPings[i] + "ms" + "] " + playerCount + " player/s";
                dropdown.options[i].text = dropdownOptionTxt;
            }

            playerCount = Constants.RegionData[Constants.StoredRegions[dropdown.value]].peerCount + Constants.RegionData[Constants.StoredRegions[dropdown.value]].masterPeerCount;
            dropdown.captionText.text= RegionNames[Constants.StoredRegions[dropdown.value]] + "[" + Constants.StoredPings[dropdown.value] + "ms" + "] " + playerCount + " player/s";
        }
        else
        {
            Constants.PrintLog("stored pings in empty");
        }
    }

    async public Task<string> GetAllRegiosnData()
    {
        using UnityWebRequest request = UnityWebRequest.Get(GetURL);

        await request.SendWebRequest();
        //Constants.PrintLog("Region response : " + request.downloadHandler.text);
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            PopulateRegionData(ExampleResponse);
            Debug.LogError("region response was not success or empty so adding a default one");
            return "";
        }
        else
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                PopulateRegionData(request.downloadHandler.text);
                return request.downloadHandler.text;

            }
            else
            {
                PopulateRegionData(ExampleResponse);
                Debug.LogError("region response was not success or empty so adding a default one");
                return "";
            }
        }
    }

}
