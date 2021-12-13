using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

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
    private void Start()
    {
        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            if(Settings.AutoConnect)
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
            UpdateConnectionText("Connecting...");
            Debug.Log("ConnectAndJoinRandom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = Settings.Version.ToString();
        }
    }

    public void ConnectionMaster()
    {
        UpdateConnectionText("Connected to master...");
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
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
        roomOptions.MaxPlayers = Settings.MaxPlayers;
        roomOptions.PublishUserId = true;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.CustomRoomProperties = customProperties;

        PhotonNetwork.CreateRoom("Room_"+roomCode.ToString(), roomOptions, TypedLobby.Default);
    }

    public void DisconnectPhoton()
    {
        PhotonNetwork.Disconnect();
    }

    #region PunCallbacks
    public override void OnConnectedToMaster()
    {
        ConnectionMaster();
    }
    public override void OnJoinedLobby()
    {
        UpdateConnectionText("Joined Lobby");
        Debug.Log("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion + "]. This script now calls: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ChangeRegionText_ConnectionUI("Selected Region : " + PhotonNetwork.CloudRegion);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        UpdateConnectionText("Creating Room");
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 6}, null);");
        CreateRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected(" + cause + ")");
        if (cause != DisconnectCause.DisconnectByClientLogic)
        {
            //somethingWentWrongPanel.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerCountText("Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        UpdateConnectionText("Joined Room : "+ PhotonNetwork.CurrentRoom.Name);
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running : "+ PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCountText("Player Count : "+PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        Debug.Log("OnPlayerEnteredRoom() called by PUN. Connected players " + newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCountText("Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        Debug.Log("OnPlayerLeftRoom() called by PUN."+otherPlayer.NickName);
    }
    #endregion
}


