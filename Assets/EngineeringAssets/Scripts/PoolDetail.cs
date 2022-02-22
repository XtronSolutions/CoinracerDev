using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PoolDetail : MonoBehaviour
{
    public GameObject _highlighter;
    public TextMeshProUGUI _nFTName;
    public TextMeshProUGUI _nFTToken;
    public Image _animationImg;
    public TextMeshProUGUI _levelText;
    public TextMeshProUGUI _timerText;
    public TextMeshProUGUI _earnedCraceText;
    public TextMeshProUGUI _expText;
    public Button EnterChipraceButton;
    public Button ClaimRewardButton;
    public Button UpdradeNFTButton;
    public Button EmergencyWithdrawButton;
    public GameObject Container;
    public Button ContainerBackButton;
    public Button EnableContButton;
    private string _poolId;
    private TotalNFTData TokenData = new TotalNFTData();

    public double UpgradableAmount;
    private double WithdrawFees;

    double RemainingTimeSeconds=0;
    double TotalTimer = 600;//86400//600
    bool timerStarted = false;

    float timeSpanConversionDays;//var to hold days after converstion from seconds
    float timeSpanConversionHours;//var to hold hours after converstion from seconds
    float timeSpanConversiondMinutes;//var to hold minutes after converstion from seconds
    float timeSpanConversionSeconds;//var to hold seconds after converstion from float seconds

    string textfielddays;//string store converstion of days into string for display
    string textfieldHours;//string store converstion of hours into string for display
    string textfieldMinutes;//string store converstion of minutes into string for display
    string textfieldSeconds;//string store converstion of seconds into string for display
    string MainTime;

    private double tempTimer=0;
    private void OnEnable()
    {
        EnableContButton.onClick.AddListener(EnableNFTDataScreen);
        ContainerBackButton.onClick.AddListener(DisableNFTDataScreen);
        EnterChipraceButton.onClick.AddListener(EnterChiprace);
        ClaimRewardButton.onClick.AddListener(ClaimReward);
        UpdradeNFTButton.onClick.AddListener(UpdateNFT);
        EmergencyWithdrawButton.onClick.AddListener(EmergencyWithdraw);
    }

    public void EnableNFTDataScreen()
    {
        Container.SetActive(true);

        if(TokenData.IsRunningChipRace)
        {
            ToggleClaimButton(true);
            ToggleChipraceButton(false);
            ToggleEmergencyButton(true);
        }
        else
        {
            ToggleClaimButton(false);
            ToggleChipraceButton(true);
            ToggleEmergencyButton(false);
        }

        if (TokenData.IsUpgradable)
            ToggleUpgradeButton(true);
        else
            ToggleUpgradeButton(false);

        _timerText.text = "0:0:0:0";
        _earnedCraceText.text = "$"+TokenData.Rewards.ToString();
    }

    public void DisableNFTDataScreen()
    {
        Container.SetActive(false);
    }

    public void ToggleChipraceButton(bool state1)
    {
        EnterChipraceButton.interactable = state1;
    }
    public void ToggleClaimButton(bool _state)
    {
        ClaimRewardButton.interactable = _state;
    }
    public void ToggleUpgradeButton(bool _state)
    {
        UpdradeNFTButton.interactable = _state;
    }
    public void ToggleEmergencyButton(bool _state)
    {
        EmergencyWithdrawButton.interactable = _state;
    }

    public void ToggleHighlight(bool _isHighlight)
    {
        _highlighter.SetActive(_isHighlight);
    }
    public void AssignPoolDetail(bool _isHighlight, string _name,string _token,Sprite _img,string _poolID,string _level,string _exp)
    {
        if (TokenData.IsRunningChipRace)
            ToggleHighlight(true);
        else
            ToggleHighlight(false);

        _nFTName.text = _name;
        _nFTToken.text = "#"+_token;
        _poolId = _poolID;
        _animationImg.sprite = _img;
        _levelText.text = "Level " + _level;
        _expText.text = "Exp " + _exp;
    }

    public void AssignTokenData(string _name, int _id, int _level, bool _isUpgradable, int _targetScore, bool _isRunningChipRace, string _remainingTime, double _upgradeAmount, int _reward)
    {
        TokenData.Name = _name;
        TokenData.ID = _id;
        TokenData.Level = _level;
        TokenData.IsUpgradable = _isUpgradable;
        TokenData.TargetScore = _targetScore;
        TokenData.IsRunningChipRace = _isRunningChipRace;
        TokenData.RemainingTime = _remainingTime;
        TokenData.Rewards = _reward;
        RemainingTimeSeconds = double.Parse(TokenData.RemainingTime);
        UpgradableAmount = _upgradeAmount;

        tempTimer = TotalTimer - RemainingTimeSeconds;
        UpdateTimer();

        if (tempTimer <= 0)
            timerStarted = false;
        else
            timerStarted = true;
    }

    public void UpdateTimer()
    {
        if (TokenData.IsRunningChipRace)
            _timerText.gameObject.SetActive(true);
        //else
            //_timerText.gameObject.SetActive(false);
    }

    public void EnterChiprace()
    {
        if(TokenData.IsRunningChipRace)
        {
            if(MainMenuViewController.Instance)
                MainMenuViewController.Instance.ShowToast(3f, "Chiprace already running for selected NFT Token.");
        }
        else
        {
            if (Constants.IsTest)
            {
                if (WalletManager.Instance)
                    WalletManager.Instance.enterChipRace(TokenData.Name, TokenData.ID.ToString(), _poolId);
            }
            else
            {
                Constants.NFTTokenApproval = TokenData.ID;
                CheckNFTApproval(TokenData.ID.ToString());
            }
        }
    }

    async public void CheckNFTApproval(string token)
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(true);
        bool isApproved = await WalletManager.Instance.CheckNFTApproval(token);
        if(isApproved)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            ChipraceHandler.Instance.NFTApprovalScreen.SetActive(false);

            if (WalletManager.Instance)
                WalletManager.Instance.enterChipRace(TokenData.Name, TokenData.ID.ToString(), _poolId);
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            ChipraceHandler.Instance.NFTApprovalScreen.SetActive(true);
        }
    }

    public void ClaimReward()
    {
        if (TokenData.IsRunningChipRace)
        {
            //if (tempTimer <= 0)
            //{
            if (WalletManager.Instance)
                WalletManager.Instance.claimRewards(TokenData.Name,TokenData.ID.ToString());
            //}
            //else
            //{
                //if (MainMenuViewController.Instance)
                    //MainMenuViewController.Instance.ShowToast(3f, "Chiprace has not ended yet!");
            //}
        }
        else
        {
            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.ShowToast(3f, "Chiprace not running, please enter chiprace with respective token.");

        }
    }

    async public void UpdateNFT()
    {
        if (TokenData.Level != 5)
        {
            if (TokenData.IsUpgradable)
            {
                if (TokenData.IsRunningChipRace)
                {
                    if (MainMenuViewController.Instance)
                        MainMenuViewController.Instance.ShowToast(3f, "Cannot upgrade while running race.");
                }
                else
                {
                    if (WalletManager.Instance)
                    {
                        if (WalletManager.Instance.CheckChipracebalance(UpgradableAmount))
                        {
                            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
                            bool _isApproved = await WalletManager.Instance.CheckCraceApprovalChiprace(UpgradableAmount);
                            if (_isApproved)
                            {
                                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                                WalletManager.Instance.upgradeNFT(TokenData.ID.ToString(), UpgradableAmount);
                            }
                            else
                            {
                                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                                ChipraceHandler.Instance.CraceChipraceApprovalScreen.SetActive(true);
                            }
                        }
                        else
                        {
                            MainMenuViewController.Instance.ShowToast(3f, "Insufficient Crace amount, need " + UpgradableAmount + " $Crace");
                        }
                    }
                }
            }
            else
            {
                if (MainMenuViewController.Instance)
                    MainMenuViewController.Instance.ShowToast(3f, "NFT is not upgradable yet!");
            }
        }else
        {
            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.ShowToast(3f, "NFT is fully upgraded!");
        }
    }

    async public void EmergencyWithdraw()
    {
        if (TokenData.IsRunningChipRace)
        {
            if (WalletManager.Instance)
            {
                if (WalletManager.Instance.CheckChipracebalanceWhole(Constants.ChipraceWithdrawFees))
                {
                    bool _isApproved = await WalletManager.Instance.CheckCraceApprovalChiprace(Constants.ChipraceWithdrawFees);
                    if (_isApproved)
                        WalletManager.Instance.emergencyExitChipRace(TokenData.Name, TokenData.ID.ToString(), Constants.ChipraceWithdrawFees);
                    else
                        ChipraceHandler.Instance.CraceChipraceApprovalScreen.SetActive(true);
                }
                else
                {
                    MainMenuViewController.Instance.ShowToast(3f, "Insufficient Crace amount, need " + Constants.ChipraceWithdrawFees + " $Crace");
                }
            }
        }
        else
        {
            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.ShowToast(3f, "No race is running!");
        }
    }

    public void SetTimer()
    {
        tempTimer -= Time.deltaTime;
        if (tempTimer <= 0)
        {
            timerStarted = false;
            _timerText.text = "0:0:0:0";
        }
        else
        {
            ConvertTime(tempTimer);
            DisplayTimer();
        }
    }

    public void ConvertTime(double _sec)
    {
        //Store TimeSpan into variable.
        timeSpanConversionDays = TimeSpan.FromSeconds(_sec).Days;
        timeSpanConversionHours = TimeSpan.FromSeconds(_sec).Hours;
        timeSpanConversiondMinutes = TimeSpan.FromSeconds(_sec).Minutes;
        timeSpanConversionSeconds = TimeSpan.FromSeconds(_sec).Seconds;

        //Convert TimeSpan variables into strings for textfield display
        textfielddays = timeSpanConversionDays.ToString();
        textfieldHours = timeSpanConversionHours.ToString();
        textfieldMinutes = timeSpanConversiondMinutes.ToString();
        textfieldSeconds = timeSpanConversionSeconds.ToString();
    }

    public void DisplayTimer()
    {
        MainTime = textfielddays + ":" + textfieldHours + ":" + textfieldMinutes + ":" + textfieldSeconds;
        _timerText.text = MainTime;
    }

    private void Update()
    {
        if (timerStarted)
        {
            if (TokenData != null)
            {
                if (TokenData.IsRunningChipRace)
                    SetTimer();
            }
        }
    }
}
