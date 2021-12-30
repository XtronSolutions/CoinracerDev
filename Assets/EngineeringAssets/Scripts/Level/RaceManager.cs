using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class MultiplayerUI
{
    public GameObject MainScreen;
    public TextMeshProUGUI WinText;
    public TextMeshProUGUI WinnerNameText;
    public TextMeshProUGUI AmountWinText;
    public TextMeshProUGUI RunTimeText;
    public TextMeshProUGUI RaceOutcome;
    public Image FlagReference;
    
}
public class RaceManager : MonoBehaviour
{
    [SerializeField] private List<WayPoint> _wayPoints = new List<WayPoint>();
    [SerializeField] private int _requiredNumberOfLaps = 3;
    [SerializeField] private GameObject _pasueMenuObject = null;
    [SerializeField] private GameObject _pauseRestartButton = null;
    [SerializeField] private GameObject _raceOverMenuObject = null;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private GameObject _gameEndMenuMultiplayer = null;
    [SerializeField] private GameObject _disconnectPopup = null;
    [SerializeField] private TextMeshProUGUI LapText;
    [SerializeField] private AudioClip _buttonPressClip = null;
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private TextMeshProUGUI GameStartTimer = null;
    private int _currentWayPointIndex = 1;
    private int _lapsCounter;

    public static RaceManager Instance;
    int RaceCounter = 3;
    public MultiplayerUI UIMultiplayer;

    private void OnEnable()
    {
        Instance = this;

    }

    private void Start()
    {
        LapText.text = "Lap " + _lapsCounter.ToString() + "/" + _requiredNumberOfLaps.ToString();
        foreach (var wayPoint in _wayPoints)
        {
            wayPoint.WayPointDataObservable.Subscribe(OnWayPointData).AddTo(this);
        }

        if (Constants.IsMultiplayer)
            MultiplayerManager.Instance.CallStartRPC();
        else
            StartTheRaceTimer();


    }

    public void StartTheRaceTimer()
    {
        RaceCounter = 3;
        Constants.MoveCar = false;
        GameStartTimer.text = RaceCounter.ToString();
        StartCoroutine(StartTimerCountDown());
    }

    IEnumerator StartTimerCountDown()
    {
        //Debug.Log(RaceCounter);
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
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
        Constants.isMultiplayerGameEnded = true;
        if (_position >= 0)
        {
            bool isWinner = false;
            if (_position == 0)
                isWinner = true;

            WinData _data = MultiplayerManager.Instance.winnerList[0];
            positionText.text = (_position + 1).ToString();
            ToggleScreen_MultiplayerUI(true);
            ChangeName_MultiplayerUI(_data.Name);
            ChangeWinAmount_MultiplayerUI(_data.TotalWins);
            ChangeAmount_MultiplayerUI(true, _data.TotalBetValue);
            ConvertTimeAndDisplay(double.Parse(_data.RunTime));
            UpdateFlag_MultiplayerUI(_data.FlagIndex);
            //UpdateRaceOutcome_MultiplayerUI(isWinner);
        }
    }

    public void ConvertTimeAndDisplay(double _sec)
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

        ChangeRunTime_MultiplayerUI(textfieldHours+":"+ textfieldMinutes+":"+ textfieldSeconds+":"+ textfieldMiliSeconds);
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
            Debug.LogError("TH is null for OnRaceDone");
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
        }


        if(Constants.IsMultiplayer)
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
            Debug.LogError("GUH is null for RaceEnded");
        }
    }

    public void TogglePauseMenu()
    {
        _pasueMenuObject.SetActive(!_pasueMenuObject.activeSelf);
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
    public void ToggleScreen_MultiplayerUI(bool state)
    {
        UIMultiplayer.MainScreen.SetActive(state);
    }

    public void ChangeName_MultiplayerUI(string _name)
    {
        UIMultiplayer.WinnerNameText.text = _name;
    }

    public void ChangeWinAmount_MultiplayerUI(int _wins)
    {
        UIMultiplayer.WinText.text = "WINS : "+ _wins.ToString();
    }
    public void ChangeAmount_MultiplayerUI(bool isWinner,int _amount)
    {
        if(isWinner)
            UIMultiplayer.AmountWinText.text = "AMOUNT : " + _amount.ToString()+" $CRACE";
        else
            UIMultiplayer.AmountWinText.text = "AMOUNT : 0 $CRACE";
    }

    public void ChangeRunTime_MultiplayerUI(string _time)
    {
        UIMultiplayer.RunTimeText.text = "RUN TIME : " + _time;
    }

    public void UpdateFlag_MultiplayerUI(int index)
    {
        UIMultiplayer.FlagReference.sprite= FlagSkins.Instance.FlagSpriteWithIndex(index);
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
