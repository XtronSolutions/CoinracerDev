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
    [Tooltip("Total Number of player in a destruction derby.")]
    public byte MaxDDPlayers;
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

        Instance = this;//initializing static instance of this class
    }

    private void Start()
    {
        Constants.GetCracePrice();//get current crace price from coinbase
        Constants.isMultiplayerGameEnded = false; //reset isMultiplayerGameEnded bool 
        ActorNumbers.Clear(); //clear list of ActorNumbers

        Constants.RegionChanged = false;
        PhotonNetwork.SelectedRegion = "";
        Constants.SelectedRegion = "";

        //if (Settings.AutoConnect)//auto connect to server if true
            //ConnectToPhotonServer();
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

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ToggleWithdrawButton_ConnectionUI(false);

        Constants.CanWithdraw = false;

        ActorNumbers.Clear();

        if (PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.GotPingResult = false;

            //Constants.RegionChanged = false;
            //DisconnectPhoton();

            //Invoke("ConnectToPhotonServer", 2f);

            //if (MainMenuViewController.Instance)
            //MainMenuViewController.Instance.UpdateDeposit_ConnectionUI("", false);

            Constants.RoomConnectionInit = true;

            if (PhotonNetwork.InLobby)
                LobbyConnection();
            else
                PhotonNetwork.JoinLobby();
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
        PhotonNetwork.SerializationRate = 15;
        PhotonNetwork.SendRate = 40;
        //Debug.LogError(PhotonNetwork.SerializationRate);
        UpdateConnectionText("Connected to master...");
        Constants.PrintLog("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.AutomaticallySyncScene = true;

        float nameSuffix = Random.Range(1000, 9999);
        string name = "Player_" + nameSuffix.ToString();

        if (FirebaseMoralisManager.Instance)
        {
            if (FirebaseMoralisManager.Instance.PlayerData != null && FirebaseMoralisManager.Instance.PlayerData.UserName != "")
                name = FirebaseMoralisManager.Instance.PlayerData.UserName;
        }

        PhotonNetwork.LocalPlayer.NickName = name;

        //if (PhotonNetwork.InLobby)
            //LobbyConnection();
        //else
            //PhotonNetwork.JoinLobby();
    }

    public void LobbyConnection()
    {
        UpdateConnectionText("Joined Lobby");
        Constants.PrintLog("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion + "]. This script now calls: PhotonNetwork.JoinRandomRoom();");

        CancelInvoke("UpdateOnlineStatus");
        UpdateOnlineStatus();

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ToggleUI_DD(false);

        if (Constants.RoomConnectionInit)
        {
            Constants.RoomConnectionInit = false;
            JoinRoomRandom(Constants.SelectedLevel, Constants.SelectedWage, Settings.MaxPlayers);
        }

            if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ChangeRegionText_ConnectionUI("Selected Region : " + PhotonNetwork.CloudRegion);
    }

    public void UpdateOnlineStatus()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
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

            if (Constants.PIDString == "true")
            {
                Constants.PrintLog("Room id already exists creating new one");
                Invoke("CreateRoom", 0.5f);
                return;
            }
        }

        Constants.StoredPID = roomCode.ToString();
        string roomName = roomCode.ToString();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = Settings.MaxPlayers;
        roomOptions.PublishUserId = true;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        if (Constants.FreeMultiplayer)
            Constants.SelectedWage = 0;

        int _level = Constants.SelectedLevel;

        if (Constants.IsDestructionDerby)
        {
            _level = 6;
            roomOptions.MaxPlayers = Settings.MaxDDPlayers;
        }

        roomOptions.CustomRoomPropertiesForLobby = new string[3] { Constants.MAP_PROP_KEY, Constants.WAGE_PROP_KEY, Constants.MODE_PROP_KEY };
        roomOptions.CustomRoomProperties = new Hashtable { { Constants.MAP_PROP_KEY, _level }, { Constants.WAGE_PROP_KEY, Constants.SelectedWage }, { Constants.MODE_PROP_KEY, Constants.FreeMultiplayer } };

        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    private void JoinRoomRandom(int mapCode, int wageAmount, byte expectedMaxPlayers)
    {
        if (Constants.FreeMultiplayer)
            wageAmount = 0;


        int _level = mapCode;

        if (Constants.IsDestructionDerby)
        {
            _level = 6;
            expectedMaxPlayers = Settings.MaxDDPlayers;
        }


        Hashtable expectedCustomRoomProperties = new Hashtable { { Constants.MAP_PROP_KEY, _level }, { Constants.WAGE_PROP_KEY, wageAmount }, { Constants.MODE_PROP_KEY, Constants.FreeMultiplayer } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers);
    }

    public void DisconnectPhoton(bool _photonDisconnect = false)
    {
        ActorNumbers.Clear();
        startTimer = false;
        MainMenuViewController.Instance.ToggleUI_DD(false);

        if (_photonDisconnect)
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        }
        else
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();
        }
    }

    public void SetCustomProps(bool IsRoom, string _key, string _temp)
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

    public void GetCustomProps(bool IsRoom, string _key)
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

    IEnumerator callPropertiesWithDelay(bool isRoom, string _key, float _sec)
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
        Constants.PrintLog("Left Room due to disconnection: " + !Constants.isMultiplayerGameEnded);

        //base.OnLeftRoom();
        if (!Constants.isMultiplayerGameEnded && RaceManager.Instance)
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

    public void UpdateTransactionData(bool _canWithdraw, bool _depositDone, string _context, bool _depostButtonActive, bool _withdrawButtonActive, bool _canDisableTimer)
    {
        if (MainMenuViewController.Instance)
        {
            Constants.CanWithdraw = _canWithdraw;
            Constants.DepositDone = _depositDone;

            MainMenuViewController.Instance.UpdateDeposit_ConnectionUI(_context, _depostButtonActive);
            MainMenuViewController.Instance.ToggleWithdrawButton_ConnectionUI(_withdrawButtonActive);

            if (_canDisableTimer)
                MainMenuViewController.Instance.DisableWithDrawTimer_ConnectionUI();
            else
                MainMenuViewController.Instance.EnableWithDrawTimer_ConnectionUI();
        }
    }

    double startTime;
    double timerIncrementValue;
    bool startTimer = false;

    void Update()
    {
        if (!startTimer) return;

        timerIncrementValue = PhotonNetwork.Time - startTime;
        MainMenuViewController.Instance.SetDDTimerText_DD((Constants.DD_WaitTime-timerIncrementValue).ToString("0")+" s");
        if (timerIncrementValue >= Constants.DD_WaitTime)
        {
            startTimer = false;
            Debug.Log("Timer Completed");
            MainMenuViewController.Instance.SetDDTimerText_DD("0 s");

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
                ForceStartGame();
            else
            {
                MainMenuViewController.Instance.ShowToast(5f, "No player found online, try again some time later or switch to a different region.", false);
                MainMenuViewController.Instance.DisableScreen_ConnectionUI();
            }
           
            //Timer Completed
            //Do What Ever You What to Do Here
        }
    }
  
    public override void OnJoinedRoom()
    {
        UpdateConnectionText("Joined Room");
        Constants.StoredPID = PhotonNetwork.CurrentRoom.Name;

        if (MainMenuViewController.Instance && !Constants.IsDestructionDerby)
            MainMenuViewController.Instance.ToggleUI_DD(false);

        if (PhotonNetwork.CurrentRoom.PlayerCount == Settings.MaxPlayers && !Constants.IsDestructionDerby)
        {
            if (!Constants.FreeMultiplayer)
            {
                if (!PhotonNetwork.IsMasterClient)
                    UpdateTransactionData(false, false, "waiting for other player to deposit", false, false, true);
            }
        }else if(Constants.IsDestructionDerby)
        {
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ToggleUI_DD(true);
                MainMenuViewController.Instance.SetPlayerConnectedText_DD(PhotonNetwork.CurrentRoom.PlayerCount);
            }


            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                var CustomeValue = new ExitGames.Client.Photon.Hashtable();
                startTime = PhotonNetwork.Time;
                startTimer = true;
                CustomeValue.Add(Constants.ROOM_GameTimer, startTime);
                PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
            }
            else
            {
                startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties[Constants.ROOM_GameTimer].ToString());
                startTimer = true;
            }
        }
    }

    IEnumerator SetRoomPropWithDelay(CustomPlayerPropData _data, bool isRemoving = false, string ActorID = "")
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
        byte _count = Settings.MaxPlayers;
        if (Constants.IsDestructionDerby)
        {
            _count = Settings.MaxDDPlayers;
            MainMenuViewController.Instance.SetPlayerConnectedText_DD(PhotonNetwork.CurrentRoom.PlayerCount);
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == _count)
        {
            if (PhotonNetwork.IsMasterClient)
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

                if (!Constants.FreeMultiplayer)
                    _tokenID = Constants.TokenNFT[Constants._SelectedTokenNameIndex].ID[Constants._SelectedTokenIDIndex].ToString();

                Debug.LogError("calling sync on onplayerenteredroom");
                RPCCalls.Instance.PHView.RPC("SyncConnectionData", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString(), Constants.UserName, Constants.TotalWins.ToString(), Constants.FlagSelectedIndex.ToString(), Constants.SelectedCurrencyAmount.ToString(), _tokenID);
            }
        }
    }

    public void ForceStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
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

            if (!Constants.FreeMultiplayer)
                _tokenID = Constants.TokenNFT[Constants._SelectedTokenNameIndex].ID[Constants._SelectedTokenIDIndex].ToString();

            Debug.LogError("force start game called");

            MultiplayerManager.Instance.LoadSceneDelay();

            //RPCCalls.Instance.PHView.RPC("SyncConnectionData", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString(), Constants.UserName, Constants.TotalWins.ToString(), Constants.FlagSelectedIndex.ToString(), Constants.SelectedCurrencyAmount.ToString(), _tokenID);
        }
    }

    public void LoadSceneDelay(float time = 3f, bool loadWithoutAsycn = false)
    {
        Debug.LogError("starting scene with delay");
        if (loadWithoutAsycn)
            MainMenuViewController.Instance.LoadDesiredScene();
        else
            StartCoroutine(LoadAsyncScene());
    }

    public IEnumerator LoadAsyncScene()
    {
        int _count = Settings.MaxPlayers;
        if (Constants.IsDestructionDerby)
            _count = Settings.MaxDDPlayers;

        if (PhotonNetwork.CurrentRoom.PlayerCount == _count || Constants.IsDestructionDerby)
        {
            if (Constants.IsDestructionDerby)
                PhotonNetwork.LoadLevel(6);
            else if (TournamentManager.Instance.DataTournament.IsSingleMap)
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
            if (Constants.OtherPlayerDeposit)
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
        if (PhotonNetwork.IsMasterClient)
        {
            RPCCalls.Instance.PHView.RPC("StartRace", RpcTarget.AllBufferedViaServer);
        }
    }
    public void CallEndMultiplayerGameRPC(bool isTotaled = false)
    {
        WinData _data = new WinData();
        _data.Name = PhotonNetwork.LocalPlayer.NickName;
        _data.ID = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        _data.TotalBetValue = Constants.SelectedCurrencyAmount + Constants.SelectedCurrencyAmount;
        _data.RunTime = Constants.GameSeconds.ToString();
        _data.IsTotaled = isTotaled;

        if (!isTotaled)
            _data.TotalWins = Constants.TotalWins + 1;

        _data.FlagIndex = Constants.FlagSelectedIndex;
        _data.WalletAddress = Constants.WalletAddress;

        string _Json = JsonConvert.SerializeObject(_data);
        RPCCalls.Instance.PHView.RPC("EndMultiplayerRace", RpcTarget.AllViaServer, _Json);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            return;


        if(Constants.IsDestructionDerby)
            MainMenuViewController.Instance.SetPlayerConnectedText_DD(PhotonNetwork.CurrentRoom.PlayerCount);

        //UpdatePlayerCountText("Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());

        if (PhotonNetwork.CurrentRoom.PlayerCount > 0 && !Constants.OtherPlayerDeposit)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ToggleSecondDetail(false, "", "", 0);

        if (!Constants.OtherPlayerDeposit)
            Invoke("CheckLeftPlayer", 0.5f);
        else if (Constants.OtherPlayerDeposit)
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
        if (MainMenuViewController.Instance)
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

