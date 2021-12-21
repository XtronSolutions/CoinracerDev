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
    private void Start()
    {
        if(!Instance)
        {
            Constants.GetCracePrice();
            ActorNumbers.Clear();
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            PHView = GetComponent<PhotonView>();

            if (Settings.AutoConnect)
                ConnectToPhotonServer();
        }
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
        string name = "Player_"+ nameSuffix.ToString();

        if (FirebaseManager.Instance)
        {
            if(FirebaseManager.Instance.PlayerData!=null && FirebaseManager.Instance.PlayerData.UserName!="")
                name = FirebaseManager.Instance.PlayerData.UserName;
        }

        PhotonNetwork.LocalPlayer.NickName = name;
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        var roomCode = Random.Range(100000, 999999);
        var customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties.Add("m", "1");
        customProperties.Add("t", "2");

        RoomOptions roomOptions = new RoomOptions();
       // roomOptions.customRoomPropertiesForLobby = new Hashtable(1) { { "level", MainMenuViewController.Instance.getSelectedLevel() } };
        roomOptions.MaxPlayers = Settings.MaxPlayers;
        roomOptions.PublishUserId = true;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.CustomRoomProperties = customProperties;

        PhotonNetwork.CreateRoom("Room_"+roomCode.ToString(), roomOptions, TypedLobby.Default);


       
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
    #region PunCallbacks
    public override void OnConnectedToMaster()
    {
        ConnectionMaster();
    }
    public override void OnJoinedLobby()
    {
        UpdateConnectionText("Joined Lobby");
        //Debug.Log("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion + "]. This script now calls: PhotonNetwork.JoinRandomRoom();");


     //   private Hashtable expectedCustomRoomProperties = new Hashtable(1) { { "level", MainMenuViewController.Instance.getSelectedLevel() } };

       /// PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, Settings.MaxPlayers);
        
        PhotonNetwork.JoinRandomRoom();

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
        //Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running : "+ PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCountText("Player Count : "+PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        //Debug.Log("OnPlayerEnteredRoom() called by PUN. Connected players " + newPlayer.NickName);

        if(PhotonNetwork.CurrentRoom.PlayerCount == Settings.MaxPlayers)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(LoadAsyncScene());
            }
        }
    }

    public IEnumerator LoadAsyncScene()
    {
       //Debug.Log("Selected Level is" + MainMenuViewController.Instance.getSelectedLevel());
        PhotonNetwork.LoadLevel(MainMenuViewController.Instance.getSelectedLevel()+1);
        yield return null;
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
        }
    }
    [PunRPC]
    public void EndMultiplayerRace()
    {
        //var customProperties = new ExitGames.Client.Photon.Hashtable();


        //PhotonNetwork.room.SetCustomProperty("mapIndex", 1);
    }

    #endregion
}


