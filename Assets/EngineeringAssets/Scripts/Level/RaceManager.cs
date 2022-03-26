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
        //ClaimRewardButton.onClick.AddListener(ClaimReward);
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
           // racePosition.SetActive(true);
            if (PhotonNetwork.IsConnected)
            {
                if (MultiplayerManager.Instance)
                    MultiplayerManager.Instance.winnerList.Clear();

                StartCoroutine(StartGameWithDelay());
                //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                //  StartTheRaceTimer();
            }

            int _count = MultiplayerManager.Instance.Settings.MaxPlayers;
            // for (int i = 0; i < _count; i++)
            // {
            //     sapwnableSlider[i] =
            //         Instantiate(slider, sliderPos.transform.position, Quaternion.identity, fieldCanvas.transform) as
            //             GameObject;
            // }
        }
        else
            StartTheRaceTimer();


        _miniMapCounter = _requiredNumberOfLaps * 10;
        _miniMapCounter = 1 / _miniMapCounter;
        // miniMap = Instantiate(slider, sliderPos.transform.position, Quaternion.identity,fieldCanvas.transform) as GameObject;


        //slider.SetActive(true);
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
            GameStartTimer.text = "";

            if (Constants.IsMultiplayer)
                TimeHandler.Instance.timerIsRunning = true;

            Constants.MoveCar = true;
            yield return null;
        }
        else
        {

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
        {
            racePosition.text = "1st";
        }
        else if (player2Position > player1Position)
        {
            racePosition.text = "2nd";
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        //Debug.Log(TinyCarController.carSpeed);
        if (TinyCarController.carSpeed > 0)
        {
            speedText.text = TinyCarController.carSpeed.ToString();
            //Debug.Log(Mathf.Floor(body.velocity.magnitude));
        }
        else
        {
            speedText.text = "0";
        }
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

        //_miniMapCounter++;
        //Debug.Log(_miniMapCounter);
        //float val = 1/ (_requiredNumberOfLaps*10);
        //  Debug.Log(val);
        progressCount = _miniMapCounter;
        //lerpProgressbar(val);
        StartCoroutine(changeProgressValue(progressCount));
        // miniMap.value = val;
        // Debug.Log(_miniMapCounter);
        // Debug.Log(val);
    }


    private void OnWayPointData(WayPointData data)
    {
        int indexOfPayPoint = _wayPoints.IndexOf(data.Waypoint);
        //Debug.Log("indexofwaypoint");
        // miniMap.GetComponent<MinimapHandler>().startSinglePlayerProgressBar();
        //startSinglePlayerprogressBar();

        if (indexOfPayPoint % _wayPoints.Count == _currentWayPointIndex)
        {
            _currentWayPointIndex++;
            _currentWayPointIndex %= _wayPoints.Count;
            //  Debug.Log("currentWayPoint");
            //Debug.Log(_currentWayPointIndex);

            if (_currentWayPointIndex == 1)
            {
                _lapsCounter++;
                LapText.text = "Lap " + _lapsCounter.ToString() + "/" + _requiredNumberOfLaps.ToString();
                if (_lapsCounter == _requiredNumberOfLaps)
                {
                    OnRaceDone();
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
        //Debug.Log("position is:");
        //Debug.Log(_position);
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
            //positionText.text = (_position + 1).ToString();
            //secondpositionText.text
            showPositions(true);
            ToggleScreen_MultiplayerUI(true);
            ChangeName_MultiplayerUI(_data.Name);
            ChangeWinAmount_MultiplayerUI(_data.TotalWins);
            ChangeAmount_MultiplayerUI(true, _data.TotalBetValue);
            ConvertTimeAndDisplay(true, double.Parse(_data.RunTime));
            UpdateFlag_MultiplayerUI(_data.FlagIndex);
            //Updatef
            foreach (var item in MultiplayerManager.Instance.winnerList)
            {
                Debug.Log(item.Name);
            }

            // WinData _loserData = MultiplayerManager.Instance.winnerList[1];
            // ChangeName_LoserMultiplayerUI(_loserData.Name);
            // ChangeWinAmount_LoserMultiplayerUI(_loserData.TotalWins);
            // ChangeAmount_LoserMultiplayerUI(false, _loserData.TotalBetValue);
            // ConvertTimeAndDisplay(false,double.Parse(_loserData.RunTime));
            // UpdateFlag_LoserMultiplayerUI(_loserData.FlagIndex);

        }

        if (_position > 0) //only looser can access
        {
            //TODO: disable winner logo
            //raise an event to disable position loader
            string _Json = JsonConvert.SerializeObject(MultiplayerManager.Instance.winnerList[1]);
            RPCCalls.Instance.PHView.RPC("ShowOtherPlayersPosition", RpcTarget.AllViaServer, _Json);

        }
    }

    public void ShowSecondPositionPlayer(string _data)
    {
        WinData _mainData = JsonConvert.DeserializeObject<WinData>(_data);
        
        // WinData _loserData = MultiplayerManager.Instance.winnerList[1];
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
        else if (GamePlayUIHandler.Instance && Constants.IsTournament)
        {
            GamePlayUIHandler.Instance.ToggleInputScreen_InputFieldUI(true);
            GamePlayUIHandler.Instance.SetWallet_InputFieldUI(FirebaseManager.Instance.PlayerData.WalletAddress);
            GamePlayUIHandler.Instance.SetInputUsername_InputFieldUI(FirebaseManager.Instance.PlayerData.UserName);
        }
        else if (GamePlayUIHandler.Instance && Constants.IsPractice)
        {
            _raceOverMenuObject.SetActive(true);
            AnimationsHandler.Instance.runPopupAnimation(_raceOverMenuObject);
        }


        if (Constants.IsMultiplayer)
            Time.timeScale = 1f;
        else
            Time.timeScale = 0.1f;
    }
    public void RaceEnded()
    {
        //_raceOverMenuObject.SetActive(true);

        if(GamePlayUIHandler.Instance)
        {
            LeaderboardManager.Instance.EnableGameplayLeaderboard();
        }else
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
        // else
        //     UIMultiplayer.AmountWinText.text = "0";
    }
    public void ChangeAmount_LoserMultiplayerUI(bool isWinner,int _amount)
    {
        // if(isWinner)
        //     UIMultiplayer.LoserAmountWinText.text =  _amount.ToString();
        // else
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
