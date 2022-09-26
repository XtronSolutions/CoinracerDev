﻿using System.Collections;
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
    public void ShowOtherPlayersPosition(string _data)
    {
       RaceManager.Instance.ShowSecondPositionPlayer(_data);
    }
    [PunRPC]
    public void EndMultiplayerRace(string _data)
    {
        WinData _mainData = JsonConvert.DeserializeObject<WinData>(_data);

        if(!_mainData.IsTotaled)
        MultiplayerManager.Instance.winnerList.Add(_mainData);

        if (_mainData.ID == PhotonNetwork.LocalPlayer.ActorNumber.ToString())
        {
            int positionNumber = -1;
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

            if (positionNumber == 0)
            {
                Constants.ClaimedReward = false;

                if (RaceManager.Instance)
                {
                    if (!Constants.FreeMultiplayer)
                    {
                        RaceManager.Instance.ToggleClaimReward(true);
                        Constants.PushingWins = true;
                        FirebaseMoralisManager.Instance.PlayerData.TotalWins++;
                        Constants.TotalWins = FirebaseMoralisManager.Instance.PlayerData.TotalWins;
                        FirebaseMoralisManager.Instance.UpdatedFireStoreData(FirebaseMoralisManager.Instance.PlayerData);
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

            if (!_mainData.IsTotaled)
                RaceManager.Instance.showGameOverMenuMultiplayer(positionNumber);
        }
    }

    [PunRPC]
    public void SyncConnectionData(string _actor, string _name, string _wins, string _index,string _crace,string _ID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Constants.OpponentTokenID = _ID;
            MainMenuViewController.Instance.ToggleSecondDetail(true, _name, _wins, int.Parse(_index));
            if (Constants.FreeMultiplayer || Constants.IsDestructionDerby)
                MultiplayerManager.Instance.LoadSceneDelay();
        }
        else
        {
            //Constants.OpponentTokenID = _ID;

            string _tokenID = "0";

            if (!Constants.FreeMultiplayer)
                _tokenID = Constants.TokenNFT[Constants._SelectedTokenNameIndex].ID[Constants._SelectedTokenIDIndex].ToString();

            Debug.Log("SyncConnectionData to be callled on not master : " + _tokenID);
            Constants.SelectedCurrencyAmount = int.Parse(_crace);
            MainMenuViewController.Instance.ToggleSecondDetail(true, _name, _wins, int.Parse(_index));
            PHView.RPC("SyncConnectionData", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber.ToString(), Constants.UserName, Constants.TotalWins.ToString(), Constants.FlagSelectedIndex.ToString(), Constants.SelectedCurrencyAmount.ToString(), _tokenID);
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
