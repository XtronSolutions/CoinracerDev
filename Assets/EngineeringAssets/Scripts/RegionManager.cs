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
    public Dropdown RegionMainList;
    bool RegionPinged = false;
    Dictionary<string, string> RegionNames = new Dictionary<string, string>();
    Dictionary<string, RegionResponse> RegionData = new Dictionary<string, RegionResponse>();
    List<RegionResponse> myDeserializedClass = new List<RegionResponse>();
    string[] StoredRegions;
    string[] StoredPings;
    private string GetURL = "http://64.227.31.248:8008/";
    string ExampleResponse = "[{\"code\":\"eu\",\"address\":\"ws://GCAMS146.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"us\",\"address\":\"ws://5BFA7B2F847F6354744DE369F8496DF0.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"usw\",\"address\":\"ws://F943322039644213464E42C7AFB865A2.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"cae\",\"address\":\"ws://1E4C943035393DBEB337C36E0F8A2A09.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"asia\",\"address\":\"ws://5C41DFF7F36BC37011BD8F000BF90B38.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"jp\",\"address\":\"ws://AZJP005000001.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"au\",\"address\":\"ws://9BD5E99DD999F2851DBEC9F768E9BA23.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"sa\",\"address\":\"ws://GCSP004.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"in\",\"address\":\"ws://20C070ADE8D6F680D898A4EF4D626B57.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"ru\",\"address\":\"ws://GCMOS015.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"rue\",\"address\":\"ws://4746102D4930B4FA9C57B103E8D1EE91.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"kr\",\"address\":\"ws://AZKR003000000.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"za\",\"address\":\"ws://B33E312B471BD9630E44738C09976E08.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0},{\"code\":\"tr\",\"address\":\"ws://GCIST001.exitgames.com:9090\",\"masterPeerCount\":0,\"peerCount\":0,\"gameCount\":0}]";

    private void Awake()
    {
        Instance = this;
        AddRegionNames();
        Constants.PingAPIFetched = false;
        InvokeRepeating(nameof(GetAllRegiosnData), 0.2f, 5f);
    }

    public void PopulateRegionData(string _response)
    {
        RegionData.Clear();
        myDeserializedClass = JsonConvert.DeserializeObject<List<RegionResponse>>(_response);

        for (int i = 0; i < myDeserializedClass.Count; i++)
        {
            RegionData.Add(myDeserializedClass[i].code, myDeserializedClass[i]);
        }

        Constants.PingAPIFetched = true;
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
        //ChangeConnectionText_ConnectionUI("connecting...");
        //ChangeRegionText_ConnectionUI("Selected Region : n/a");
        //ToggleScreen_ConnectionUI(false);

        RegionPinged = false;
        RegionMainList.interactable = false;
        RegionMainList.options.Clear();
        RegionMainList.options.Add(new Dropdown.OptionData() { text = "Connecting..." });
        RegionMainList.value = 0;
        Constants.RegionChanged = false;
        PhotonNetwork.SelectedRegion = "";
    }

    public IEnumerator ShowPingedRegionList_ConnectionUI()
    {
        yield return new WaitUntil(() => (PhotonNetwork.GotPingResult && Constants.PingAPIFetched));

        StoredRegions = PhotonNetwork.pingedRegions;
        StoredPings = PhotonNetwork.pingedRegionPings;

        UpdatePingList(StoredRegions, StoredPings);
        PhotonNetwork.GotPingResult = false;
        Constants.PingAPIFetched = false;
    }

    public IEnumerator UpdateRegionList_Connection()
    {
        yield return new WaitUntil(() => (Constants.PingAPIFetched));

        StoredRegions = PhotonNetwork.pingedRegions;
        StoredPings = PhotonNetwork.pingedRegionPings;

        UpdatePingList(StoredRegions, StoredPings);
        Constants.PingAPIFetched = false;
    }

    public void UpdatePingList(string[] regions, string[] pings)
    {
        if (!RegionPinged)
        {
            RegionPinged = true;
            var dropdown = RegionMainList;
            SetManualRegion _RegionRef = this.gameObject.GetComponent<SetManualRegion>();
            UpdateRegionsData(dropdown, _RegionRef);
        }
    }

    public void UpdateRegionsData(Dropdown dropdown, SetManualRegion _RegionRef)
    {
        dropdown.options.Clear();
        List<string> _regions = new List<string>();
        if (StoredPings.Length > 0)
        {
            int minimumPing = int.Parse(StoredPings[0]);
            int currentPing;
            dropdown.value = 1;
            for (int i = 0; i < StoredRegions.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData() { text = RegionNames[StoredRegions[i]] + " " + StoredPings[i] + "ms"+" "+ RegionData[StoredRegions[i]].peerCount+" players" });
                _regions.Add(StoredRegions[i]);
                currentPing = int.Parse(StoredPings[i]);
                if (currentPing < minimumPing)
                {
                    minimumPing = currentPing;
                    dropdown.value = i + 1;
                }
            }

            _RegionRef.SetRegionString(_regions);
        }
        else
        {
            Debug.LogError("region list is empty");
        }
    }

    async public Task<string> GetAllRegiosnData()
    {
        Constants.PingAPIFetched = false;
        using UnityWebRequest request = UnityWebRequest.Get(GetURL);

        await request.SendWebRequest();
        Constants.PrintLog("Region response : " + request.downloadHandler.text);
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            PopulateRegionData(ExampleResponse);
            Debug.LogError("region response was not success or empty so adding a default one");
            //MainMenuViewController.Instance.SomethingWentWrongMessage();
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
                //MainMenuViewController.Instance.SomethingWentWrongMessage();
                PopulateRegionData(ExampleResponse);
                Debug.LogError("region response was not success or empty so adding a default one");
                return "";
            }
        }
    }

}
