using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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
            //Debug.Log("_mainData.ID: " + _mainData.ID);
            foreach (var item in MultiplayerManager.Instance.winnerList)
            {
                //Debug.Log("item.ID: " + item.ID);
            }
            foreach (var item in MultiplayerManager.Instance.winnerList)
            {
                positionNumber++;
                if (item.ID == _mainData.ID)
                    break;
            }
            // RaceManager.Instance.showPositions(positionNumber);

            //Debug.Log("My position is: " + positionNumber);

            if (positionNumber == 0)
            {
                Constants.ClaimedReward = false;

                if (RaceManager.Instance)
                {
                    if (!Constants.FreeMultiplayer)
                    {
                        RaceManager.Instance.ToggleClaimReward(true);
                        Constants.PushingWins = true;
                        FirebaseManager.Instance.PlayerData.TotalWins++;
                        Constants.TotalWins = FirebaseManager.Instance.PlayerData.TotalWins;
                        FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);
                    }
                    else
                    {
                        RaceManager.Instance.ToggleClaimReward(false);
                    }
                }
            }
            else
            {
                if (RaceManager.Instance)
                    RaceManager.Instance.ToggleClaimReward(false);
            }

            RaceManager.Instance.showGameOverMenuMultiplayer(positionNumber);
        }
    }

    [PunRPC]
    public void SyncConnectionData(string _actor, string _name, string _wins, string _index,string _crace,string _ID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Constants.OpponentTokenID = _ID;
            MainMenuViewController.Instance.ToggleSecondDetail(true, _name, _wins, int.Parse(_index));
            if (Constants.FreeMultiplayer)
                MultiplayerManager.Instance.LoadSceneDelay();
        }
        else
        {
            Constants.OpponentTokenID = _ID;
            string _tokenID = Constants.TokenNFT[Constants._SelectedTokenNameIndex].ID[Constants._SelectedTokenIDIndex].ToString();
            Constants.SelectedCrace = int.Parse(_crace);
            MainMenuViewController.Instance.ToggleSecondDetail(true, _name, _wins, int.Parse(_index));
            PHView.RPC("SyncConnectionData", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString(), Constants.UserName, Constants.TotalWins.ToString(), Constants.FlagSelectedIndex.ToString(), Constants.SelectedCrace.ToString(), _tokenID);
        }
    }

    [PunRPC]
    public void DepositCompleted()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(MultiplayerManager.Instance)
                MultiplayerManager.Instance.LoadSceneDelay(0.5f);
        }
        else
        {
            Constants.OtherPlayerDeposit = true;
            MultiplayerManager.Instance.UpdateTransactionData(false, false, "please deposit the wage amount...", true, false, true);
        }
    }

}
