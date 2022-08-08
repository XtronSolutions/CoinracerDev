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

#region SuperClasses
public class CustomPlayerPropData
{
    [Tooltip("name of the player on PUN")]
    public string name;
    [Tooltip("Wallet address of the player on PUN")]
    public string walletAddress;
    [Tooltip("ID of the player on PUN")]
    public string ID;
    [Tooltip("Flag index of the player on PUN")]
    public int FlagIndex;
    [Tooltip("Total wins of the player on PUN")]
    public int TotalWins;
}

public class CustomRoomPropData
{
    [Tooltip("Room name on PUN")]
    public string RoomName;
    [Tooltip("Max room players on PUN")]
    public int RoomMaxPlayer;
    [Tooltip("Total wage amount of room on PUN")]
    public int WageAmount;
    [Tooltip("Level selected for room on PUN")]
    public int LevelSelected;
    [Tooltip("player data stored in room on PUN")]
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

public class WinData
{
    [Tooltip("ID of the winner of the race")]
    public string ID;
    [Tooltip("name of the race winner")]
    public string Name;
    [Tooltip("flag index of the race winner")]
    public int FlagIndex;
    [Tooltip("Total wins of the race winner")]
    public int TotalWins;
    [Tooltip("Total bet value combined for the race winner")]
    public int TotalBetValue;
    [Tooltip("lap runtime of the race winner")]
    public string RunTime;
    [Tooltip("Wallet address of the race winner")]
    public string WalletAddress;
    [Tooltip("variable to store if car is damaged/totaled")]
    public bool IsTotaled = false;
}

#endregion

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    #region DataMembers
    public static MultiplayerManager Instance; //static instance of the class
    public PhotonSetting Settings; //class instance for PhotonSetting
    [HideInInspector] public List<string> ActorNumbers = new List<string>(); //list of string to store actor numbers in room
    [HideInInspector] public List<WinData> winnerList = new List<WinData>(); //list to store instances of class WinData
    string _customPlayerPropString = ""; //string to store response of player data from PUN
    string _customRoomPropString = ""; //string to store response of room data from PUN
    private CustomRoomPropData DataRoomPropData; //class instance of CustomRoomPropData
    #endregion

    #region MultiplayerPhoton
    void Awake()
    {
        MultiplayerManager[] objs = GameObject.FindObjectsOfType<MultiplayerManager>();

         if (objs.Length > 1)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
            Constants.GetCracePrice();//get current crace price from coinbase
            Constants.isMultiplayerGameEnded = false; //reset isMultiplayerGameEnded bool 
            ActorNumbers.Clear(); //clear list of ActorNumbers

            Instance = this;//initializing static instance of this class

        if (Settings.AutoConnect)//auto connect to server if true
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
            MainMenuViewController.Instance.ChangePlayerCountText_ConnectionUI("Online Players : " + TxT);
    }

    public void ConnectToPhotonServer()
    {
        PhotonNetwork.GotPingResult = false;
        Constants.OpponentTokenID = "0";
        UpdatePlayerCountText("0");
        Constants.DepositDone = false;
        Constants.TimerRunning = false;
        Constants.OtherPlayerDeposit = false;
        UpdateTransactionData(false, false, "", false, false, true);

        if(MainMenuViewController.Instance)
            MainMenuViewController.Instance.ToggleWithdrawButton_ConnectionUI(false);
          
        Constants.CanWithdraw = false;

        ActorNumbers.Clear();

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GotPingResult = false;

            Constants.RegionChanged = false;
            DisconnectPhoton();

            Invoke("ConnectToPhotonServer", 2f);

            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.UpdateDeposit_ConnectionUI("", false);
        }
        else
        {
            UpdateConnectionText("Connecting...");

            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.UpdateDeposit_ConnectionUI("", false);

            PhotonNetwork.GotPingResult = false;

            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.StartCoroutine(MainMenuViewController.Instance.ShowPingedRegionList_ConnectionUI());

            Constants.PrintLog("ConnectAndJoinRandom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = Settings.Version.ToString();
        }
    }

    public void ConnectionMaster()
    {
        UpdateConnectionText("Connected to master...");
        Constants.PrintLog("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.AutomaticallySyncScene = true;

        float nameSuffix = Random.Range(1000, 9999);
        string name = "Player_" + nameSuffix.ToString();

        if (FirebaseManager.Instance)
        {
            if (FirebaseManager.Instance.PlayerData != null && FirebaseManager.Instance.PlayerData.UserName != "")
                name = FirebaseManager.Instance.PlayerData.UserName;
        }

        PhotonNetwork.LocalPlayer.NickName = name;

        if (PhotonNetwork.InLobby)
            LobbyConnection();
        else
            PhotonNetwork.JoinLobby();
    }

    public void LobbyConnection()
    { 
        UpdateConnectionText("Joined Lobby");
        Constants.PrintLog("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion + "]. This script now calls: PhotonNetwork.JoinRandomRoom();");

        CancelInvoke("UpdateOnlineStatus");
        UpdateOnlineStatus();
        JoinRoomRandom(Constants.SelectedLevel, Constants.SelectedWage, Settings.MaxPlayers);

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ChangeRegionText_ConnectionUI("Selected Region : " + PhotonNetwork.CloudRegion);
    }

    public void UpdateOnlineStatus()
    {
        if(PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
        {
            UpdatePlayerCountText(PhotonNetwork.CountOfPlayers.ToString());
            Invoke("UpdateOnlineStatus", 1f);
        }
    }  

    public void CreateRoom()
    {
        var roomCode = Random.Range(10000, 99999);

        if (!Constants.FreeMultiplayer)
        {
            if (WalletManager.Instance)
                WalletManager.Instance.isExistRoom(roomCode.ToString());

            if (Constants.PIDString=="true")
            {
                Constants.PrintLog("Room id already exists creating new one");
                Invoke("CreateRoom", 0.5f);
                return;
            }
        }

        Constants.StoredPID= roomCode.ToString(); 
        string roomName = roomCode.ToString();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = Settings.MaxPlayers;
        roomOptions.PublishUserId = true;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        if (Constants.FreeMultiplayer)
            Constants.SelectedWage = 0;

        roomOptions.CustomRoomPropertiesForLobby =new string [3]{ Constants.MAP_PROP_KEY,Constants.WAGE_PROP_KEY,Constants.MODE_PROP_KEY };
        roomOptions.CustomRoomProperties = new Hashtable { { Constants.MAP_PROP_KEY, Constants.SelectedLevel }, { Constants.WAGE_PROP_KEY, Constants.SelectedWage }, { Constants.MODE_PROP_KEY, Constants.FreeMultiplayer } };

        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    private void JoinRoomRandom(int mapCode, int wageAmount, byte expectedMaxPlayers)
    {
        if (Constants.FreeMultiplayer)
            wageAmount = 0;

        Hashtable expectedCustomRoomProperties = new Hashtable { { Constants.MAP_PROP_KEY, mapCode }, { Constants.WAGE_PROP_KEY, wageAmount }, { Constants.MODE_PROP_KEY, Constants.FreeMultiplayer } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers);
    }

    public void DisconnectPhoton()
    {
        ActorNumbers.Clear();

        if(PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
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

            Constants.PrintLog("prop set : " + isSet);
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

                Constants.PrintLog("room data : " + _temp);
            }
            else
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(_key))
                    _temp = (string)PhotonNetwork.LocalPlayer.CustomProperties[_key];

                _customPlayerPropString = _temp;

                Constants.PrintLog("player data : " + _temp);
            }
        }
    }

    IEnumerator callPropertiesWithDelay(bool isRoom,string _key,float _sec)
    {
        yield return new WaitForSeconds(_sec);
        GetCustomProps(isRoom, _key);
    }
    #endregion

    #region PunCallbacks
    public override void OnConnectedToMaster()
    {
        ConnectionMaster();
    }
    public override void OnJoinedLobby()
    {
        LobbyConnection();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        UpdateConnectionText("Creating Room");
        Constants.PrintLog("OnJoinRandomFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 6}, null);");
        CreateRoom();
    }
    public override void OnLeftRoom()
    {
        Constants.PrintLog("Left Room due to disconnection: "+ !Constants.isMultiplayerGameEnded);

        //base.OnLeftRoom();
        if(!Constants.isMultiplayerGameEnded && RaceManager.Instance)
        {
            RaceManager.Instance.showDisconnectScreen();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (Constants.RegionChanged == true)
        {
            Constants.RegionChanged = false;
            UpdateTransactionData(false, false, "", false, false, true);

            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.ToggleSecondDetail(false, "", "", 0);

            ConnectToPhotonServer();
        }

        Constants.PrintLog("OnDisconnected(" + cause + ")");
        if (cause != DisconnectCause.DisconnectByClientLogic)
        {
            //somethingWentWrongPanel.SetActive(true);
        }
    }

    public void UpdateTransactionData(bool _canWithdraw,bool _depositDone, string _context, bool _depostButtonActive, bool _withdrawButtonActive, bool _canDisableTimer)
    {
        if (MainMenuViewController.Instance)
        {
            Constants.CanWithdraw = _canWithdraw;
            Constants.DepositDone = _depositDone;

            MainMenuViewController.Instance.UpdateDeposit_ConnectionUI(_context, _depostButtonActive);
            MainMenuViewController.Instance.ToggleWithdrawButton_ConnectionUI(_withdrawButtonActive);

            if(_canDisableTimer)
                MainMenuViewController.Instance.DisableWithDrawTimer_ConnectionUI();
            else
                MainMenuViewController.Instance.EnableWithDrawTimer_ConnectionUI();
        }
    }
    public override void OnJoinedRoom()
    {
        UpdateConnectionText("Joined Room");
        Constants.StoredPID = PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.CurrentRoom.PlayerCount == Settings.MaxPlayers)
        {
            if (!Constants.FreeMultiplayer)
            {
                if (!PhotonNetwork.IsMasterClient)
                    UpdateTransactionData(false, false, "waiting for other player to deposit", false, false, true);
            }
        }
    }

    IEnumerator SetRoomPropWithDelay(CustomPlayerPropData _data, bool isRemoving=false, string ActorID="")
    {
        StartCoroutine(callPropertiesWithDelay(true, Constants.RoomDataKey, 0.5f));
        yield return new WaitForSeconds(0.55f);

        if (isRemoving)
        {
            if (_customRoomPropString == "")//if there is entry in room
            {
                DataRoomPropData = JsonConvert.DeserializeObject<CustomRoomPropData>(_customRoomPropString);

                foreach (var item in DataRoomPropData.Roomdata)
                {
                    if (item.ID == ActorID)
                    {
                        DataRoomPropData.Roomdata.Remove(item);
                        break;
                    }
                }

                string _json = JsonConvert.SerializeObject(DataRoomPropData);
                SetCustomProps(true, Constants.RoomDataKey, _json);
            }
        }
        else
        {
            if (_customRoomPropString == "")//if there is entry in room
            {
                DataRoomPropData = new CustomRoomPropData();
                DataRoomPropData.RoomName = PhotonNetwork.CurrentRoom.Name;
                DataRoomPropData.RoomMaxPlayer = Settings.MaxPlayers;
                DataRoomPropData.WageAmount = Constants.SelectedWage;
                DataRoomPropData.LevelSelected = Constants.SelectedLevel;
                DataRoomPropData.Roomdata.Add(_data);

                string _json = JsonConvert.SerializeObject(DataRoomPropData);
                SetCustomProps(true, Constants.RoomDataKey, _json);
            }
            else
            {
                DataRoomPropData = JsonConvert.DeserializeObject<CustomRoomPropData>(_customRoomPropString);
                DataRoomPropData.Roomdata.Add(_data);

                string _json = JsonConvert.SerializeObject(DataRoomPropData);
                SetCustomProps(true, Constants.RoomDataKey, _json);
            }

            yield return new WaitForSeconds(1f);
            StartCoroutine(callPropertiesWithDelay(true, Constants.RoomDataKey, 1f));
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == Settings.MaxPlayers)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;

                if (!Constants.FreeMultiplayer)
                {
                    if (Constants.DepositDone)
                    {
                        RPCCalls.Instance.PHView.RPC("DepositCompleted", RpcTarget.Others);
                    }
                    else
                    {
                        UpdateTransactionData(false, false, "please deposit the wage amount...", true, false, true);
                    }

                }
                string _tokenID = "0";

                if(!Constants.FreeMultiplayer)
                    _tokenID = Constants.TokenNFT[Constants._SelectedTokenNameIndex].ID[Constants._SelectedTokenIDIndex].ToString();

                RPCCalls.Instance.PHView.RPC("SyncConnectionData", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString(),Constants.UserName,Constants.TotalWins.ToString(),Constants.FlagSelectedIndex.ToString(), Constants.SelectedCurrencyAmount.ToString(), _tokenID);
            }
        }
    }

    public void LoadSceneDelay(float time=3f, bool loadWithoutAsycn=false)
    {
        if (loadWithoutAsycn)
            MainMenuViewController.Instance.LoadDesiredScene();
        else
            StartCoroutine(LoadAsyncScene());
    }

    public IEnumerator LoadAsyncScene()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == Settings.MaxPlayers)
        {
            if (TournamentManager.Instance.DataTournament.IsSingleMap)
                PhotonNetwork.LoadLevel(Constants.SelectedSingleLevel);
            else
                PhotonNetwork.LoadLevel(MainMenuViewController.Instance.getSelectedLevel() + 1);

            while (PhotonNetwork.LevelLoadingProgress < 1)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            if(Constants.OtherPlayerDeposit)
                MainMenuViewController.Instance.LoadDesiredScene();
            else
                MainMenuViewController.Instance.ToggleBackButton_ConnectionUI(true);
        }
        }

    public void SuccessDeposit()
    {
        if (!ActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber.ToString()))
            ActorNumbers.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());

        RPCCalls.Instance.PHView.RPC("SyncScene", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString());
    }

    public void CallStartRPC()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            RPCCalls.Instance.PHView.RPC("StartRace", RpcTarget.AllBufferedViaServer);
        }
    }
    public void CallEndMultiplayerGameRPC(bool isTotaled=false)
    {
        WinData _data = new WinData();
        _data.Name = PhotonNetwork.LocalPlayer.NickName;
        _data.ID = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        _data.TotalBetValue = Constants.SelectedCurrencyAmount+ Constants.SelectedCurrencyAmount;
        _data.RunTime = Constants.GameSeconds.ToString();
        _data.IsTotaled = isTotaled;

        if (!isTotaled)
            _data.TotalWins = Constants.TotalWins+1;

        _data.FlagIndex = Constants.FlagSelectedIndex;
        _data.WalletAddress = Constants.WalletAddress;

        string _Json = JsonConvert.SerializeObject(_data);
        RPCCalls.Instance.PHView.RPC("EndMultiplayerRace", RpcTarget.AllViaServer, _Json);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            return;

        //UpdatePlayerCountText("Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());

        if(PhotonNetwork.CurrentRoom.PlayerCount>0 && !Constants.OtherPlayerDeposit)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ToggleSecondDetail(false,"","", 0);

        if (!Constants.OtherPlayerDeposit)
            Invoke("CheckLeftPlayer", 0.5f);
        else if(Constants.OtherPlayerDeposit)
            RemovePlayer();

        Constants.PrintLog("OnPlayerLeftRoom() called by PUN." + otherPlayer.NickName);
    }

    public void CheckLeftPlayer()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            if (!Constants.DepositDone)
                UpdateTransactionData(false, false, "", false, false, true);
        }
    }

    public void RemovePlayer()
    {
        if(MainMenuViewController.Instance)
        {
            MainMenuViewController.Instance.ShowToast(3f, "other player has left, please find match again.");
            Invoke("DisconnectDelay", 3f);
        }
    }

    public void DisconnectDelay()
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        UpdateTransactionData(false, false, "", false, false, true);
        MainMenuViewController.Instance.DisableScreen_ConnectionUI();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Constants.PrintLog("Master Swithced");
    }
    #endregion

}


