using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CustomPlayerPropData
{
    public string name { get; set; }
    public string walletAddress { get; set; }
    public string ID { get; set; }
}

public class CustomRoomPropData
{
    public string RoomName;
    public int RoomMaxPlayer;
    public int WageAmount;
    public int LevelSelected;
    public List<CustomPlayerPropData> Roomdata = new List<CustomPlayerPropData>();
}


[System.Serializable]
public class PhotonSetting
{
    [Tooltip("Auto connect to photon on start.")]
    public bool AutoConnect = true;
    [Tooltip("Photon version number.")]
    public byte Version = 1;
    [Tooltip("Total Number of player in a room.")]
    public byte MaxPlayers;
}

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static MultiplayerManager Instance;
    public PhotonSetting Settings;
    private PhotonView PHView;
    private List<string> ActorNumbers = new List<string>();
    private List<string> winnerList = new List<string>();
    string _customPlayerPropString = "";
    string _customRoomPropString = "";
    private CustomRoomPropData DataRoomPropData;

    void Awake()
    {
        MultiplayerManager[] objs = GameObject.FindObjectsOfType<MultiplayerManager>();

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
            Constants.GetCracePrice();
            ActorNumbers.Clear();
            Instance = this;
            PHView = GetComponent<PhotonView>();

            if (Settings.AutoConnect)
                ConnectToPhotonServer();
    }

    public void UpdateConnectionText(string TxT)
    {
        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ChangeConnectionText_ConnectionUI(TxT);
    }

    public void UpdatePlayerCountText(string TxT)
    {
        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ChangePlayerCountText_ConnectionUI(TxT);
    }

    public void ConnectToPhotonServer()
    {
        if (PhotonNetwork.IsConnected)
        {
            ConnectionMaster();
        }
        else
        {
            ActorNumbers.Clear();
            UpdateConnectionText("Connecting...");
            //Debug.Log("ConnectAndJoinRandom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = Settings.Version.ToString();
        }
    }

    public void ConnectionMaster()
    {
        UpdateConnectionText("Connected to master...");
        //Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.AutomaticallySyncScene = true;

        float nameSuffix = Random.Range(1000, 9999);
        string name = "Player_" + nameSuffix.ToString();

        if (FirebaseManager.Instance)
        {
            if (FirebaseManager.Instance.PlayerData != null && FirebaseManager.Instance.PlayerData.UserName != "")
                name = FirebaseManager.Instance.PlayerData.UserName;
        }

        PhotonNetwork.LocalPlayer.NickName = name;
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        var roomCode = Random.Range(100000, 999999);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = Settings.MaxPlayers;
        roomOptions.PublishUserId = true;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.CustomRoomPropertiesForLobby =new string [2]{ Constants.MAP_PROP_KEY,Constants.WAGE_PROP_KEY };
        roomOptions.CustomRoomProperties = new Hashtable { { Constants.MAP_PROP_KEY, Constants.SelectedLevel }, { Constants.WAGE_PROP_KEY, Constants.SelectedWage } };

        PhotonNetwork.CreateRoom("Room_" + roomCode.ToString(), roomOptions, TypedLobby.Default);
    }

    private void JoinRoomRandom(int mapCode, int wageAmount, byte expectedMaxPlayers)
    {
        Hashtable expectedCustomRoomProperties = new Hashtable { { Constants.MAP_PROP_KEY, mapCode }, { Constants.WAGE_PROP_KEY, wageAmount } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers);
    }

    public void DisconnectPhoton()
    {
        ActorNumbers.Clear();
        PhotonNetwork.Disconnect();
    }
    public void pushResult()
    {
        //var winProperty = new ExitGames.Client.Photon.Hashtable();
        //for (int i = 0; i < 5; i++)
        //{
        //    winProperty.Add(i.ToString(), i + 1.ToString());
        //}
        //PhotonNetwork.CurrentRoom.SetCustomProperties(winProperty);
        //Debug.Log(winProperty);

        //winProperty = (Hashtable)PhotonNetwork.CurrentRoom.CustomProperties["winProperty"];
        //Debug.Log("won Custom Properties are: "+winProperty);

        int result = (int)PhotonNetwork.CurrentRoom.CustomProperties["winProperty"];
        //Debug.Log("Result is" + result);

    }

    public void TestCustom()
    {
        #region Setting player custom proeprty Example
        //Setting player custom property--------------------------------------
        CustomPlayerPropData _data = new CustomPlayerPropData();
        _data.name = "humza";
        _data.walletAddress = "090078601";
        _data.ID = "001";
        string _json = JsonConvert.SerializeObject(_data);
        SetCustomProps(false, Constants.PlayerDataKey, _json);
        StartCoroutine(callPropertiesWithDelay(false, Constants.PlayerDataKey, 0.6f)); //this should be called with 0.5-1 sec delay after setting properties
        #endregion

        #region Setting room custom proeprty Example
        StartCoroutine(SetRoomPropWithDelay());
        #endregion
    }

    public void SetCustomProps(bool IsRoom,string _key, string _temp)
    {
        if (PhotonNetwork.IsConnected)
        {
            bool isSet = false;
            Hashtable myCustomProperties = new Hashtable
        {
           {_key, _temp}
        };

            if (IsRoom)
                isSet = PhotonNetwork.CurrentRoom.SetCustomProperties(myCustomProperties);
            else
                isSet = PhotonNetwork.LocalPlayer.SetCustomProperties(myCustomProperties);

            Debug.Log("prop set : " + isSet);
        }
    }

    public void GetCustomProps(bool IsRoom,string _key)
    {
        if (PhotonNetwork.IsConnected)
        {
            string _temp = "";

            if (IsRoom)
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(_key))
                    _temp = (string)PhotonNetwork.CurrentRoom.CustomProperties[_key];

                _customRoomPropString = _temp;

                Debug.Log("room data : " + _temp);
            }
            else
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(_key))
                    _temp = (string)PhotonNetwork.LocalPlayer.CustomProperties[_key];

                _customPlayerPropString = _temp;

                Debug.Log("player data : " + _temp);
            }
        }
    }

    IEnumerator callPropertiesWithDelay(bool isRoom,string _key,float _sec)
    {
        yield return new WaitForSeconds(_sec);
        GetCustomProps(isRoom, _key);
    }

    #region PunCallbacks
    public override void OnConnectedToMaster()
    {
        ConnectionMaster();
    }
    public override void OnJoinedLobby()
    {
        UpdateConnectionText("Joined Lobby");
        //Debug.Log("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion + "]. This script now calls: PhotonNetwork.JoinRandomRoom();");

        JoinRoomRandom(Constants.SelectedLevel,Constants.SelectedWage,Settings.MaxPlayers);

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ChangeRegionText_ConnectionUI("Selected Region : " + PhotonNetwork.CloudRegion);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        UpdateConnectionText("Creating Room");
        //Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 6}, null);");
        CreateRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //Debug.Log("OnDisconnected(" + cause + ")");
        if (cause != DisconnectCause.DisconnectByClientLogic)
        {
            //somethingWentWrongPanel.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerCountText("Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        UpdateConnectionText("Joined Room : "+ PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());

        TestCustom();//for testing only
    }

    IEnumerator SetRoomPropWithDelay()
    {
        StartCoroutine(callPropertiesWithDelay(false, Constants.PlayerDataKey, 0.7f));
        yield return new WaitForSeconds(0.75f);

        //Setting room custom property--------------------------------------
        if (_customPlayerPropString != "")//data existed for the key
        {
            StartCoroutine(callPropertiesWithDelay(true,Constants.RoomDataKey, 0.2f));
            yield return new WaitForSeconds(0.3f);

            if(_customRoomPropString=="")//if there is entry in room
            {
                DataRoomPropData = new CustomRoomPropData();
                DataRoomPropData.RoomName = PhotonNetwork.CurrentRoom.Name;
                DataRoomPropData.RoomMaxPlayer = Settings.MaxPlayers;
                DataRoomPropData.WageAmount = Constants.SelectedWage;
                DataRoomPropData.LevelSelected = Constants.SelectedLevel;

                CustomPlayerPropData _data = JsonConvert.DeserializeObject<CustomPlayerPropData>(_customPlayerPropString);
                DataRoomPropData.Roomdata.Add(_data);

                string _json = JsonConvert.SerializeObject(DataRoomPropData);
                SetCustomProps(true, Constants.RoomDataKey, _json);
            }else
            {
                DataRoomPropData= JsonConvert.DeserializeObject<CustomRoomPropData>(_customRoomPropString);

                CustomPlayerPropData _data = JsonConvert.DeserializeObject<CustomPlayerPropData>(_customPlayerPropString);
                DataRoomPropData.Roomdata.Add(_data);

                string _json = JsonConvert.SerializeObject(DataRoomPropData);
                SetCustomProps(true, Constants.RoomDataKey, _json);
            }

            yield return new WaitForSeconds(1f);
            StartCoroutine(callPropertiesWithDelay(true, Constants.RoomDataKey, 1f));
        }
        else
        {
            Debug.LogError("no player property existed with key");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCountText("Player Count : "+PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        //Debug.Log("OnPlayerEnteredRoom() called by PUN. Connected players " + newPlayer.NickName);

        if(PhotonNetwork.CurrentRoom.PlayerCount == Settings.MaxPlayers)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                LoadAsyncScene();
                //StartCoroutine(LoadAsyncScene());
            }
        }
    }

    public void LoadAsyncScene()
    {
       //Debug.Log("Selected Level is" + MainMenuViewController.Instance.getSelectedLevel());
        PhotonNetwork.LoadLevel(MainMenuViewController.Instance.getSelectedLevel()+1);
        //yield return null;
        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level1",LoadSceneMode.Single);

        // Wait until the asynchronous scene fully loads
        //while (!asyncLoad.isDone)
        //{
        //    yield return null;
        //}

        //if(asyncLoad.isDone)
        //{
        //    Debug.Log("is loaded");

        //    if (PhotonNetwork.InRoom)
        //    {
        //        if (!ActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber.ToString()))
        //        {
        //            ActorNumbers.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
        //        }

        //        PHView.RPC("SyncScene", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString());
        //    }
        //}
    }

    public void CallStartRPC()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PHView.RPC("StartRace", RpcTarget.AllViaServer);
        }
    }
    public void CallEndMultiplayerGameRPC()
    {
        PHView.RPC("EndMultiplayerRace", RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.ActorNumber.ToString());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCountText("Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        //Debug.Log("OnPlayerLeftRoom() called by PUN."+otherPlayer.NickName);
    }
    #endregion

    #region RPC Calls
    [PunRPC]
    public void SyncScene(string ID)
    {
        if (!ActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber.ToString()))
            ActorNumbers.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());

        if (!ActorNumbers.Contains(ID))
            ActorNumbers.Add(ID);

       if(PhotonNetwork.IsMasterClient)
        {
            if(ActorNumbers.Count==Settings.MaxPlayers)
            {
                //Debug.Log("all players connected starting game");
                PHView.RPC("StartRace", RpcTarget.AllViaServer);
            }

        }else
        {
            PHView.RPC("SyncScene", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString());
        }
    }

    [PunRPC]
    public void StartRace()
    {
        if (RaceManager.Instance)
        {
            RaceManager.Instance.StartTheRaceTimer();
            MultiplayerManager.Instance.winnerList.Clear();
        }
    }
    [PunRPC]
    public void EndMultiplayerRace(string _ID)
    {
        MultiplayerManager.Instance.winnerList.Add(_ID);
        if(_ID == PhotonNetwork.LocalPlayer.ActorNumber.ToString())
        {
            //TODO: Active END screen according to position
            int positionNumber = 0;
            foreach(string str in MultiplayerManager.Instance.winnerList)
            {
                positionNumber++;
                if (str == _ID)
                    break;
            }
            Debug.Log("My position is: " + positionNumber);
            RaceManager.Instance.showGameOverMenuMultiplayer(positionNumber);
        }
        //var customProperties = new ExitGames.Client.Photon.Hashtable();


        //PhotonNetwork.room.SetCustomProperty("mapIndex", 1);
    }

    #endregion
}


