using System;
using System.Collections;
using System.Collections.Generic;
using DavidJalbert;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Unity.VisualScripting;

[Serializable]
public class MultiplayerUI
{
    public GameObject MainScreen;
    public GameObject confirmationScreen; 
    public GameObject confirmationScreenContainer;
    public TextMeshProUGUI WinText;
    public TextMeshProUGUI WinnerNameText;
    public TextMeshProUGUI AmountWinText;
    public TextMeshProUGUI RunTimeText;
    public TextMeshProUGUI RaceOutcome;
    public Image FlagReference;

    public TextMeshProUGUI LoserWinText;
    public TextMeshProUGUI LoserNameText;
    public TextMeshProUGUI LoserAmountWinText;
    public TextMeshProUGUI LoserRunTimeText;
    public TextMeshProUGUI LoserRaceOutcome;
    public Image LoserFlagReference;
    
}

[RequireComponent(typeof(AudioSource))]
public class RaceManager : MonoBehaviour
{
    [SerializeField] private List<WayPoint> _wayPoints = new List<WayPoint>();
    [SerializeField] private int _requiredNumberOfLaps = 3;
    [SerializeField] private GameObject _pasueMenuObject = null;
    [SerializeField] private GameObject _pauseRestartButton = null;
    [SerializeField] private GameObject _raceOverMenuObject = null;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI secondpositionText;
    [SerializeField] private GameObject _gameEndMenuMultiplayer = null;
    [SerializeField] private GameObject _disconnectPopup = null;
    [SerializeField] private TextMeshProUGUI LapText;
    [SerializeField] private AudioClip _buttonPressClip = null;
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private TextMeshProUGUI GameStartTimer = null;
    [SerializeField] private Button ClaimRewardButton = null;
    [SerializeField] private GameObject LoadingScreen = null;
    [SerializeField] public GameObject miniMap = null;
    [SerializeField] public GameObject slider = null;
    [SerializeField] public GameObject sliderPos = null;
    [SerializeField] private GameObject fieldCanvas = null;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI positionNumber;
    [SerializeField] private GameObject positionLoader = null;
    [SerializeField] private TextMeshProUGUI racePosition = null;
    [SerializeField] private AudioClip counterStartSound = null;
    public GameObject[] sapwnableSlider = null;
    public GameObject DebugEndRacebutton;


    public int player1Position = 0;
    public int player2Position = 0;
    private int _currentWayPointIndex = 1;
    private int _lapsCounter;
    public float _miniMapCounter = 0;
    private float progressCount = 0f;

    public static RaceManager Instance;
    int RaceCounter = 5; //3
    public MultiplayerUI UIMultiplayer;

    private AudioSource audioSource;

    
    private void OnEnable()
    {
        Instance = this;

        if (Constants.IsDebugBuild)
        {
            if (DebugEndRacebutton)
                DebugEndRacebutton.SetActive(true);
        }else
        {
            if (DebugEndRacebutton)
                DebugEndRacebutton.SetActive(false);
        }

        setAudioSource();

        //push game start event
        if(AnalyticsManager.Instance)
        {
            AnalyticsManager.Instance.StoredProgression.TimeSeconds = 0;

            if (AnalyticsManager.Instance.StoredProgression.fields.ContainsKey("TimeSeconds"))
                AnalyticsManager.Instance.StoredProgression.fields["TimeSeconds"] = "0";
            else
                AnalyticsManager.Instance.StoredProgression.fields.Add("TimeSeconds","0");

            AnalyticsManager.Instance.PushProgressionEvent(true);
        }
    }

    //this function will be used to set audio source to play counter sound
    //@param {} no param
    //@return {} no return
    private void setAudioSource()
    {
        if (counterStartSound == null)
            return;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.clip = counterStartSound;
        audioSource.volume = 1.0f;
    }

    public void ToggleLoadingScreen(bool _state)
    {
        LoadingScreen.SetActive(_state);
    }

    public void ToggleClaimReward(bool _state)
    {
        ClaimRewardButton.gameObject.SetActive(_state);
    }

    public void ClaimReward()
    {
        if (!Constants.ClaimedReward)
        {
            if (WalletManager.Instance)
                WalletManager.Instance.CallRaceWinner();
        }
        else
        {
            if (GamePlayUIHandler.Instance)
                GamePlayUIHandler.Instance.ShowToast(3f, "Reward already claimed.");
        }
    }

    private void Start()
    {
        // _miniMapCounter = 0;
        LapText.text = "Lap " + _lapsCounter.ToString() + "/" + _requiredNumberOfLaps.ToString();
        foreach (var wayPoint in _wayPoints)
        {

            wayPoint.WayPointDataObservable.Subscribe(OnWayPointData).AddTo(this);
        }

        racePosition.enabled = false;
        if (Constants.IsMultiplayer)
        {
            racePosition.enabled = true;
            if (PhotonNetwork.IsConnected)
            {
                if (MultiplayerManager.Instance)
                    MultiplayerManager.Instance.winnerList.Clear();

                StartCoroutine(StartGameWithDelay());
            }

            int _count = MultiplayerManager.Instance.Settings.MaxPlayers;
        }
        else
            StartTheRaceTimer();


        _miniMapCounter = _requiredNumberOfLaps * 10;
        _miniMapCounter = 1 / _miniMapCounter;
    }

    IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(1.5f);
        StartTheRaceTimer();
    }

    public void StartTheRaceTimer()
    {
        if (MultiplayerManager.Instance)
            MultiplayerManager.Instance.winnerList.Clear();

        RaceCounter = 5; //3
        Constants.MoveCar = false;
        GameStartTimer.text = RaceCounter.ToString();
        StartCoroutine(StartTimerCountDown());
    }

    IEnumerator StartTimerCountDown()
    {
        if (RaceCounter < -1)
        {
            //if counter sound is then stop that sound
            if (audioSource.isPlaying)
                audioSource.Stop();
            GameStartTimer.text = "";

            if (Constants.IsMultiplayer)
                TimeHandler.Instance.timerIsRunning = true;

            Constants.MoveCar = true;
            yield return null;
        }
        else
        {
            //if counter sound is not playing then play that sound
            if (!audioSource.isPlaying)
                audioSource.Play();
            yield return new WaitForSeconds(1);
            RaceCounter--;

            if (RaceCounter > 0)
            {
                GameStartTimer.text = RaceCounter.ToString();
                StartCoroutine(StartTimerCountDown());
            }
            else
            {
                RaceCounter--;
                GameStartTimer.text = "GO!";
                StartCoroutine(StartTimerCountDown());
            }
        }
    }

    private void Update()
    {
        if (player1Position > player2Position)
            racePosition.text = "1st";
        else if (player2Position > player1Position)
            racePosition.text = "2nd";

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseMenu();

        if (TinyCarController.carSpeed > 0)
            speedText.text = TinyCarController.carSpeed.ToString();
        else
            speedText.text = "0";
    }

    IEnumerator changeProgressValue(float _val)
    {
        // _val = _val - 0.01f;
        // if (_val < 0)
        // {
        //     yield break;
        // }
        // yield return new WaitForSeconds(0.1f);
        // miniMap.value = miniMap.value+0.01f;
        // StartCoroutine(changeProgressValue(_val));
        yield return null;

    }

    public void startSinglePlayerprogressBar()
    {
        progressCount = _miniMapCounter;
        StartCoroutine(changeProgressValue(progressCount));
    }


    private void OnWayPointData(WayPointData data)
    {
        int indexOfPayPoint = _wayPoints.IndexOf(data.Waypoint);

        if (indexOfPayPoint % _wayPoints.Count == _currentWayPointIndex)
        {
            _currentWayPointIndex++;
            _currentWayPointIndex %= _wayPoints.Count;

            if (_currentWayPointIndex == 1)
            {
                _lapsCounter++;
                LapText.text = "Lap " + _lapsCounter.ToString() + "/" + _requiredNumberOfLaps.ToString();

                if(Constants.GameMechanics)
                {
                    MechanicsManager.Instance.IncreaseLaps(Constants.SelectedCarToken);
                    MechanicsManager.Instance.UpdateMechanicsData(Constants.SelectedCarToken,false);
                }

                if (_lapsCounter == _requiredNumberOfLaps)
                {
                    OnRaceDone();
                }
                else
                {
                    if (Constants.GameMechanics)
                    {
                        if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Tyres)
                        { GamePlayUIHandler.Instance.InstantiateGameOver_CarTotaled("Your tyres has worn out, better luck next time."); TinyCarController.speedMultiplier = 0.0001f; }
                        else if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Oil)
                        { GamePlayUIHandler.Instance.InstantiateGameOver_CarTotaled("Your Engine Oil is empty, better luck next time."); TinyCarController.speedMultiplier = 0.0001f; }
                        else if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Gas)
                        { GamePlayUIHandler.Instance.InstantiateGameOver_CarTotaled("Your Gas is empty, better luck next time."); TinyCarController.speedMultiplier = 0.0001f; }
                    }
                }
            }
            else
            {
                //print($"cross waypoint {_currentWayPointIndex}");
            }
        }
    }

    public void showDisconnectScreen()
    {
        _disconnectPopup.SetActive(true);
    }

    public void showGameOverMenuMultiplayer(int _position)
    {
        Constants.isMultiplayerGameEnded = true;

        if (_position >= 0)
        {
            bool isWinner = false;
            if (_position == 0)
            {
                isWinner = true;
                positionLoader.SetActive(true);
            }

            WinData _data = MultiplayerManager.Instance.winnerList[0];
            showPositions(true);
            ToggleScreen_MultiplayerUI(true);
            ChangeName_MultiplayerUI(_data.Name);
            ChangeWinAmount_MultiplayerUI(_data.TotalWins);
            ChangeAmount_MultiplayerUI(true, _data.TotalBetValue);
            ConvertTimeAndDisplay(true, double.Parse(_data.RunTime));
            UpdateFlag_MultiplayerUI(_data.FlagIndex);
            foreach (var item in MultiplayerManager.Instance.winnerList)
            {
                Debug.Log(item.Name);
            }
        }

        if (_position > 0) //only looser can access
        {
            //raise an event to disable position loader
            string _Json = JsonConvert.SerializeObject(MultiplayerManager.Instance.winnerList[1]);
            RPCCalls.Instance.PHView.RPC("ShowOtherPlayersPosition", RpcTarget.AllViaServer, _Json);

        }
    }

    public void ShowSecondPositionPlayer(string _data)
    {
        WinData _mainData = JsonConvert.DeserializeObject<WinData>(_data);
        
        ChangeName_LoserMultiplayerUI(_mainData.Name);
        ChangeWinAmount_LoserMultiplayerUI(_mainData.TotalWins);
        ChangeAmount_LoserMultiplayerUI(false, _mainData.TotalBetValue);
        ConvertTimeAndDisplay(false,double.Parse(_mainData.RunTime));
        UpdateFlag_LoserMultiplayerUI(_mainData.FlagIndex);
        
        positionLoader.SetActive(false);
    }

public void ConvertTimeAndDisplay(Boolean _winner ,double _sec)
    {
        //Store TimeSpan into variable.
        float timeSpanConversionHours = TimeSpan.FromSeconds(_sec).Hours;
        float timeSpanConversiondMinutes = TimeSpan.FromSeconds(_sec).Minutes;
        float timeSpanConversionSeconds = TimeSpan.FromSeconds(_sec).Seconds;
        float timeSpanConversionMiliSeconds = TimeSpan.FromSeconds(_sec).Milliseconds / 10;

        //Convert TimeSpan variables into strings for textfield display
        string textfieldHours = timeSpanConversionHours.ToString();
        string textfieldMinutes = timeSpanConversiondMinutes.ToString();
        string textfieldSeconds = timeSpanConversionSeconds.ToString();
        string textfieldMiliSeconds = timeSpanConversionMiliSeconds.ToString();

        if(_winner)
            ChangeRunTime_MultiplayerUI(textfieldHours+":"+ textfieldMinutes+":"+ textfieldSeconds+":"+ textfieldMiliSeconds);
        else
        {
            ChangeRunTime_LoserMultiplayerUI(textfieldHours+":"+ textfieldMinutes+":"+ textfieldSeconds+":"+ textfieldMiliSeconds);
        }
    }

    public string ReturnConvertedTime(double _sec)
    {
        //Store TimeSpan into variable.
        float timeSpanConversionHours = TimeSpan.FromSeconds(_sec).Hours;
        float timeSpanConversiondMinutes = TimeSpan.FromSeconds(_sec).Minutes;
        float timeSpanConversionSeconds = TimeSpan.FromSeconds(_sec).Seconds;
        float timeSpanConversionMiliSeconds = TimeSpan.FromSeconds(_sec).Milliseconds / 10;

        //Convert TimeSpan variables into strings for textfield display
        string textfieldHours = timeSpanConversionHours.ToString();
        string textfieldMinutes = timeSpanConversiondMinutes.ToString();
        string textfieldSeconds = timeSpanConversionSeconds.ToString();
        string textfieldMiliSeconds = timeSpanConversionMiliSeconds.ToString();

        return textfieldHours + ":" + textfieldMinutes + ":" + textfieldSeconds + ":" + textfieldMiliSeconds;
    }

    public void showPositions(bool winner)
    {
        if (winner)
        {
            positionText.text = "1st";
            secondpositionText.text = "2nd";
        }
        else if(!winner)
        {
            positionText.text = "1st";
            secondpositionText.text = "2nd";
        }
        //Add more if player increased
        
    }

    public void OnRaceDone()
    {
        if(Constants.GameMechanics)
            GamePlayUIHandler.Instance.GetHealthBarObject().SetActive(false);

        if (Constants.CarTotaled)
            return;

        Constants.GameSeconds = 0;

        if (TimeHandler.Instance)
        {
            Constants.GameSeconds = TimeHandler.Instance.TotalSeconds;
            TimeHandler.Instance.timerIsRunning = false;
        }else
        {
            Constants.PrintError("TH is null for OnRaceDone");
        }

        if(Constants.IsMultiplayer)
        {
            Constants.MoveCar = false;
            MultiplayerManager.Instance.CallEndMultiplayerGameRPC();
        }
        else if (GamePlayUIHandler.Instance && (Constants.IsTournament || Constants.IsSecondTournament))
        {
            GamePlayUIHandler.Instance.ToggleInputScreen_InputFieldUI(true);
            GamePlayUIHandler.Instance.SetWallet_InputFieldUI(FirebaseMoralisManager.Instance.PlayerData.WalletAddress);
            GamePlayUIHandler.Instance.SetInputUsername_InputFieldUI(FirebaseMoralisManager.Instance.PlayerData.UserName);
        }
        else if (GamePlayUIHandler.Instance && Constants.IsPractice)
        {
            _raceOverMenuObject.SetActive(true);
            AnimationsHandler.Instance.runPopupAnimation(_raceOverMenuObject);
        }


        if (Constants.IsMultiplayer)
            Time.timeScale = 1f;
        else
            Time.timeScale = 0.3f;


        //push game end progress
        if (AnalyticsManager.Instance)
        {
            AnalyticsManager.Instance.StoredProgression.TimeSeconds = (int)Constants.GameSeconds;

            if (AnalyticsManager.Instance.StoredProgression.fields.ContainsKey("TimeSeconds"))
                AnalyticsManager.Instance.StoredProgression.fields["TimeSeconds"] = ReturnConvertedTime(Constants.GameSeconds);
            else
                AnalyticsManager.Instance.StoredProgression.fields.Add("TimeSeconds", ReturnConvertedTime(Constants.GameSeconds));

            AnalyticsManager.Instance.PushProgressionEvent(false);
        }
    }
    public void RaceEnded()
    {
        //_raceOverMenuObject.SetActive(true);

        if(GamePlayUIHandler.Instance)
        {
            if(Constants.IsTournament)
                LeaderboardManager.Instance.EnableRespectiveLeaderboard(false);
            else if (Constants.IsSecondTournament)
                LeaderboardManager.Instance.EnableRespectiveLeaderboard(true);
        }
        else
        {
            Constants.PrintError("GUH is null for RaceEnded");
        }
    }

    public void TogglePauseMenu()
    {
        _pasueMenuObject.SetActive(!_pasueMenuObject.activeSelf);
        if(_pasueMenuObject.activeSelf)
            AnimationsHandler.Instance.runPopupAnimation(_pasueMenuObject);
        _pauseRestartButton.SetActive(false);

        if (Constants.IsPractice && !Constants.IsMultiplayer)
            _pauseRestartButton.SetActive(true);

        if(!Constants.IsMultiplayer)
            Time.timeScale = _pasueMenuObject.activeSelf ? 0 : 1;
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        //if user has not claimed the reward then activate the confirmation screen for going back to main menu and return from this function
        if (!Constants.ClaimedReward)
        {
            ToggleScreen_ConfirmationScreen(true);
            return;
        }
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        if (MultiplayerManager.Instance)
        {
            MultiplayerManager.Instance.DisconnectPhoton();
        }
        Constants.ResetData();
        SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
        Time.timeScale = 1;
    }
    
    public Vector3 GetNextWayPointPosition()
    {
        return _wayPoints[_currentWayPointIndex].transform.position;
    }

    public void PlayButtonDownAudioClip()
    {
        _audioSource.PlayOneShot(_buttonPressClip);
    }

    #region Multiplayer UI
    public void ToggleScreen_ConfirmationScreen(bool state)
    {
        UIMultiplayer.confirmationScreen.SetActive(state);
        AnimationsHandler.Instance.runPopupAnimation(UIMultiplayer.confirmationScreenContainer);
    }
    public void ToggleScreen_MultiplayerUI(bool state)
    {
        UIMultiplayer.MainScreen.SetActive(state);
    }

    public void ChangeName_MultiplayerUI(string _name)
    {
        UIMultiplayer.WinnerNameText.text = _name;
    }
    public void ChangeName_LoserMultiplayerUI(string _name)
    {
        UIMultiplayer.LoserNameText.text = _name;
    }
    public void ChangeWinAmount_MultiplayerUI(int _wins)
    {
        UIMultiplayer.WinText.text = "WINS : "+ _wins.ToString();
    }
    public void ChangeWinAmount_LoserMultiplayerUI(int _wins)
    {
        UIMultiplayer.LoserWinText.text = "WINS : "+ _wins.ToString();
    }
    public void ChangeAmount_MultiplayerUI(bool isWinner,int _amount)
    {
        if(isWinner)
            UIMultiplayer.AmountWinText.text =  _amount.ToString();
    }
    public void ChangeAmount_LoserMultiplayerUI(bool isWinner,int _amount)
    {
            UIMultiplayer.LoserAmountWinText.text = "0";
    }
    public void ChangeRunTime_MultiplayerUI(string _time)
    {
        UIMultiplayer.RunTimeText.text = "RUN TIME : " + _time;
    }
    public void ChangeRunTime_LoserMultiplayerUI(string _time)
    {
        UIMultiplayer.LoserRunTimeText.text = "RUN TIME : " + _time;
    }
    public void UpdateFlag_MultiplayerUI(int index)
    {
        UIMultiplayer.FlagReference.sprite= FlagSkins.Instance.FlagSpriteWithIndex(index);
    }
    public void UpdateFlag_LoserMultiplayerUI(int index)
    {
        UIMultiplayer.LoserFlagReference.sprite= FlagSkins.Instance.FlagSpriteWithIndex(index);
    }
    public void UpdateRaceOutcome_MultiplayerUI(bool isWinner)
    {
        if (isWinner)
            UIMultiplayer.RaceOutcome.text = "WINNER";
        else
            UIMultiplayer.RaceOutcome.text = "LOSER";
    }
    #endregion

}
