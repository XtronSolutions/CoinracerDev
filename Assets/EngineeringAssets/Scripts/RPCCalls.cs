using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine;

public class RPCCalls : MonoBehaviour
{
    public static RPCCalls Instance;
    [HideInInspector] public PhotonView PHView;

    private void Awake()
    {
        Instance = this;
        PHView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void SyncScene(string ID)
    {
        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.UpdateDeposit_ConnectionUI("waiting for other player to finish...", false);

        if (!MultiplayerManager.Instance.ActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber.ToString()))
            MultiplayerManager.Instance.ActorNumbers.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());

        if (!MultiplayerManager.Instance.ActorNumbers.Contains(ID))
            MultiplayerManager.Instance.ActorNumbers.Add(ID);

        if (PhotonNetwork.IsMasterClient)
        {
            if (MultiplayerManager.Instance.ActorNumbers.Count == MultiplayerManager.Instance.Settings.MaxPlayers)
            {
                MultiplayerManager.Instance.LoadSceneDelay();
            }
        }
        else
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
        if (_mainData.ID == PhotonNetwork.LocalPlayer.ActorNumber.ToString())
        {
            //TODO: Active END screen according to position
            int positionNumber = -1;
            Debug.Log("_mainData.ID: " + _mainData.ID);
            foreach (var item in MultiplayerManager.Instance.winnerList)
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

            if (positionNumber == 0 && !Constants.IsTest)
            {
                if (WalletManager.Instance)
                    WalletManager.Instance.CallRaceWinner(MultiplayerManager.Instance.winnerList[positionNumber].WalletAddress);
            }

            RaceManager.Instance.showGameOverMenuMultiplayer(positionNumber);
        }
    }

    [PunRPC]
    public void SyncConnectionData(string _actor, string _name, string _wins, string _index)
    {
        if (PhotonNetwork.IsMasterClient)
        {

            MainMenuViewController.Instance.ToggleSecondDetail(true, _name, _wins, int.Parse(_index));
            //MainMenuViewController.Instance.ToggleBackButton_ConnectionUI(false);
            //Invoke("LoadAsyncScene", 3f);

            if (!Constants.DisableCSP)
            {
                if (MainMenuViewController.Instance)
                    MainMenuViewController.Instance.UpdateDeposit_ConnectionUI("waiting for other player to deposit...", true);
            }
            else
            {
                MultiplayerManager.Instance.LoadSceneDelay();
            }
        }
        else
        {
            //MainMenuViewController.Instance.ToggleBackButton_ConnectionUI(false);
            MainMenuViewController.Instance.ToggleSecondDetail(true, _name, _wins, int.Parse(_index));
            PHView.RPC("SyncConnectionData", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString(), Constants.UserName, Constants.TotalWins.ToString(), Constants.FlagSelectedIndex.ToString());

            if (!Constants.DisableCSP)
            {
                if (MainMenuViewController.Instance)
                    MainMenuViewController.Instance.UpdateDeposit_ConnectionUI("waiting for other player to deposit...", true);
            }
        }
    }

}
