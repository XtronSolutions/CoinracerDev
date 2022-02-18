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
    public Button EnterChipraceButton;
    public Button ClaimRewardButton;
    public Button UpdradeNFTButton;
    public Button EmergencyWithdrawButton;
    public GameObject Container;
    public Button ContainerBackButton;
    public Button EnableContButton;
    private string _poolId;
    private TotalNFTData TokenData = new TotalNFTData();

    double RemainingTimeSeconds=0;
    double TotalTimer = 86400;
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

    private void OnEnable()
    {
        EnableContButton.onClick.AddListener(EnableNFTDataScreen);
        ContainerBackButton.onClick.AddListener(DisableNFTDataScreen);
        EnterChipraceButton.onClick.AddListener(EnterChiprace);
        ClaimRewardButton.onClick.AddListener(ClaimReward);
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

        TotalTimer = 86400;
        TotalTimer -= RemainingTimeSeconds;
        UpdateTimer();
        _timerText.text = "0:00:00:00";
        timerStarted = true;

    }

    public void DisableNFTDataScreen()
    {
        Container.SetActive(false);
        timerStarted = false;
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
    public void AssignPoolDetail(bool _isHighlight, string _name,string _token,Sprite _img,string _poolID,string _level)
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
    }

    public void AssignTokenData(string _name,int _id,int _level,bool _isUpgradable,int _targetScore,bool _isRunningChipRace,string _remainingTime)
    {
        TotalTimer = 86400;
        TokenData.Name = _name;
        TokenData.ID = _id;
        TokenData.Level = _level;
        TokenData.IsUpgradable = _isUpgradable;
        TokenData.TargetScore = _targetScore;
        TokenData.IsRunningChipRace = _isRunningChipRace;
        TokenData.RemainingTime = _remainingTime;

        RemainingTimeSeconds = double.Parse(TokenData.RemainingTime);
       
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
            Debug.Log("running chiprace");
            if(MainMenuViewController.Instance)
                MainMenuViewController.Instance.ShowToast(3f, "Chiprace already running for selected NFT Token.");
        }
        else
        {
            Constants.NFTTokenApproval = TokenData.ID;
            CheckNFTApproval(TokenData.ID.ToString());
        }
    }

    async public void CheckNFTApproval(string token)
    {
        bool isApproved = await WalletManager.Instance.CheckNFTApproval(token);
        if(isApproved)
        {
            ChipraceHandler.Instance.NFTApprovalScreen.SetActive(false);

            if (WalletManager.Instance)
                WalletManager.Instance.enterChipRace(TokenData.Name, TokenData.ID.ToString(), _poolId);
        }
        else
        {
            ChipraceHandler.Instance.NFTApprovalScreen.SetActive(true);
        }
    }

    public void ClaimReward()
    {
        if (TokenData.IsRunningChipRace)
        {
            if (TotalTimer <= 0)
            {
                if (WalletManager.Instance)
                    WalletManager.Instance.claimRewards(TokenData.Name,TokenData.ID.ToString());
            }
            else
            {
                if (MainMenuViewController.Instance)
                    MainMenuViewController.Instance.ShowToast(3f, "Chiprace has not ended yet!");
            }
        }
        else
        {
            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.ShowToast(3f, "Chiprace not running, please enter chiprace with respective token.");

        }
    }

    public void SetTimer()
    {
        TotalTimer -= Time.deltaTime;
        if (TotalTimer <= 0)
        {
            _timerText.text = "0:00:00:00";
        }
        else
        {
            ConvertTime(TotalTimer);
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
