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
}

#endregion

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static MultiplayerManager Instance; //static instance of the class
    public PhotonSetting Settings; //class instance for PhotonSetting
    private PhotonView PHView; //class instance of PhotonView
    private List<string> ActorNumbers = new List<string>(); //list of string to store actor numbers in room
    [HideInInspector] public List<WinData> winnerList = new List<WinData>(); //list to store instances of class WinData
    string _customPlayerPropString = ""; //string to store response of player data from PUN
    string _customRoomPropString = ""; //string to store response of room data from PUN
    private CustomRoomPropData DataRoomPropData; //class instance of CustomRoomPropData

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
            PHView = this.gameObject.AddComponent<PhotonView>(); //getting component of PhotonView place on gameobject

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
            MainMenuViewController.Instance.ChangePlayerCountText_ConnectionUI(TxT);
    }

    public void ConnectToPhotonServer()
    {
        if (PhotonNetwork.IsConnected)
        {
            ActorNumbers.Clear();
            ConnectionMaster();
        }
        else
        {
            ActorNumbers.Clear();
            UpdateConnectionText("Connecting...");

            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.UpdateDeposit_ConnectionUI("", false);

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
        var roomCode = Random.Range(10000, 99999);

        Constants.StoredPID= roomCode.ToString(); 
        //string roomName = "Room_" + roomCode.ToString();
        string roomName = roomCode.ToString();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = Settings.MaxPlayers;
        roomOptions.PublishUserId = true;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.CustomRoomPropertiesForLobby =new string [2]{ Constants.MAP_PROP_KEY,Constants.WAGE_PROP_KEY };
        roomOptions.CustomRoomProperties = new Hashtable { { Constants.MAP_PROP_KEY, Constants.SelectedLevel }, { Constants.WAGE_PROP_KEY, Constants.SelectedWage } };

        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
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
        MainMenuViewController.Instance.ShowPingedRegionList_ConnectionUI(PhotonNetwork.pingedRegions, PhotonNetwork.pingedRegionPings);
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
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room due to disconnection");
        //base.OnLeftRoom();
        if(!Constants.isMultiplayerGameEnded && RaceManager.Instance)
        {
            RaceManager.Instance.showDisconnectScreen();
        }
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
        Constants.StoredPID = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
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
        UpdatePlayerCountText("Player Count : "+PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        Debug.Log("OnPlayerEnteredRoom() called by PUN. Connected players " + newPlayer.NickName);
        Debug.Log("Maximum Players allowed: " + Settings.MaxPlayers);
        Debug.Log("is MasterClient: " + PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.CurrentRoom.PlayerCount == Settings.MaxPlayers)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                Debug.Log("calling sync connection Data to invoke load scene");
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PHView.RPC("SyncConnectionData", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString(),Constants.UserName,Constants.TotalWins.ToString(),Constants.FlagSelectedIndex.ToString());
            }
        }
    }

    public void LoadAsyncScene()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == Settings.MaxPlayers)
        {
            PhotonNetwork.LoadLevel(MainMenuViewController.Instance.getSelectedLevel() + 1);
        }
        else
        {
            MainMenuViewController.Instance.ToggleBackButton_ConnectionUI(true);
        }
            //Debug.Log("Selected Level is" + MainMenuViewController.Instance.getSelectedLevel());

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

    public void SuccessDeposit()
    {
        if (!ActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber.ToString()))
            ActorNumbers.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());

        PHView.RPC("SyncScene", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString());
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
        WinData _data = new WinData();
        _data.Name = PhotonNetwork.LocalPlayer.NickName;
        _data.ID = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        _data.TotalBetValue = Constants.SelectedWage+ Constants.SelectedWage;
        _data.RunTime = Constants.GameSeconds.ToString();
        _data.TotalWins = 0;
        _data.FlagIndex = Constants.FlagSelectedIndex;
        _data.WalletAddress = Constants.WalletAddress;

        string _Json = JsonConvert.SerializeObject(_data);
        PHView.RPC("EndMultiplayerRace", RpcTarget.AllViaServer, _Json);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCountText("Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ToggleSecondDetail(false,"","", 0);
        //Debug.Log("OnPlayerLeftRoom() called by PUN."+otherPlayer.NickName);
    }
    #endregion

    #region RPC Calls
    [PunRPC]
    public void SyncScene(string ID)
    {
        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.UpdateDeposit_ConnectionUI("waiting for other player to finish...", false);

        if (!ActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber.ToString()))
            ActorNumbers.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());

        if (!ActorNumbers.Contains(ID))
            ActorNumbers.Add(ID);

       if(PhotonNetwork.IsMasterClient)
        {
            if(ActorNumbers.Count==Settings.MaxPlayers)
            {
                Invoke("LoadAsyncScene", 3f);
                //Debug.Log("all players connected starting game");
                //PHView.RPC("StartRace", RpcTarget.AllViaServer);
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
    public void EndMultiplayerRace(string _data)
    {
        WinData _mainData = JsonConvert.DeserializeObject<WinData>(_data);
        MultiplayerManager.Instance.winnerList.Add(_mainData);
        if(_mainData.ID == PhotonNetwork.LocalPlayer.ActorNumber.ToString())
        {
            //TODO: Active END screen according to position
            int positionNumber = -1;
            Debug.Log("_mainData.ID: " + _mainData.ID);
            foreach(var item in MultiplayerManager.Instance.winnerList)
            {
                Debug.Log("item.ID: " + item.ID);
            }
            foreach (var item in MultiplayerManager.Instance.winnerList)
            {
                positionNumber++;
                if (item.ID == _mainData.ID)
                    break;
            }

            Debug.Log("My position is: " + positionNumber);

            if(positionNumber==0 && !Constants.IsTest)
            {
                if (WalletManager.Instance)
                    WalletManager.Instance.CallRaceWinner(MultiplayerManager.Instance.winnerList[positionNumber].WalletAddress);
            }

            RaceManager.Instance.showGameOverMenuMultiplayer(positionNumber);
        }
    }

    [PunRPC]
    public void SyncConnectionData(string _actor,string _name,string _wins,string _index)
    {
        if(PhotonNetwork.IsMasterClient)
        {
        
            MainMenuViewController.Instance.ToggleSecondDetail(true, _name, _wins, int.Parse(_index));
            //MainMenuViewController.Instance.ToggleBackButton_ConnectionUI(false);
            //Invoke("LoadAsyncScene", 3f);

            if (!Constants.IsTest)
            {
                if (MainMenuViewController.Instance)
                    MainMenuViewController.Instance.UpdateDeposit_ConnectionUI("waiting for other player to deposit...", true);
            }else
            {
                Invoke("LoadAsyncScene", 3f);
            }
        }
        else
        {
           
            //MainMenuViewController.Instance.ToggleBackButton_ConnectionUI(false);
            MainMenuViewController.Instance.ToggleSecondDetail(true, _name, _wins, int.Parse(_index));
            PHView.RPC("SyncConnectionData", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString(), Constants.UserName, Constants.TotalWins.ToString(), Constants.FlagSelectedIndex.ToString());

            if (!Constants.IsTest)
            {
                if (MainMenuViewController.Instance)
                    MainMenuViewController.Instance.UpdateDeposit_ConnectionUI("waiting for other player to deposit...", true);
            }
        }
    }

    #endregion
}


