using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private List<WayPoint> _wayPoints = new List<WayPoint>();
    [SerializeField] private int _requiredNumberOfLaps = 3;
    [SerializeField] private GameObject _pasueMenuObject = null;
    [SerializeField] private GameObject _pauseRestartButton = null;
    [SerializeField] private GameObject _raceOverMenuObject = null;
    [SerializeField] private TextMeshProUGUI LapText;
    [SerializeField] private AudioClip _buttonPressClip = null;
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private TextMeshProUGUI GameStartTimer = null;
    private int _currentWayPointIndex = 1;
    private int _lapsCounter;

    public static RaceManager Instance;
    int RaceCounter = 3;

    private void OnEnable()
    {
        Instance = this;

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
        Debug.Log(RaceCounter);
        if (RaceCounter < -1)
        {
            GameStartTimer.text = "";
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
    // Start is called before the first frame update
    void Start()
    {
        LapText.text = "Lap " + _lapsCounter.ToString() + "/" + _requiredNumberOfLaps.ToString();
        foreach (var wayPoint in _wayPoints)
        {
            wayPoint.WayPointDataObservable.Subscribe(OnWayPointData).AddTo(this);
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
                    MultiplayerManager.Instance.pushResult();
                   // OnRaceDone();
                }
            }
            else
            {
                print($"cross waypoint {_currentWayPointIndex}");
            }
        }
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
            Debug.LogError("TimeHandler instance is null");
        }


        if (GamePlayUIHandler.Instance && Constants.IsTournament)
        {
            GamePlayUIHandler.Instance.ToggleInputScreen_InputFieldUI(true);
            GamePlayUIHandler.Instance.SetWallet_InputFieldUI(FirebaseManager.Instance.PlayerData.WalletAddress);
            GamePlayUIHandler.Instance.SetInputUsername_InputFieldUI(FirebaseManager.Instance.PlayerData.UserName);
        }
        else if(GamePlayUIHandler.Instance && Constants.IsMultiplayer)
        {
            //Multiplayer Game end
           // _raceOverMenuObject.SetActive(true);
        }
        else if (GamePlayUIHandler.Instance && Constants.IsPractice)
        {
            _raceOverMenuObject.SetActive(true);
        }

            Time.timeScale = 0.1f;
    }
    public void RaceEnded()
    {
        //_raceOverMenuObject.SetActive(true);

        if(GamePlayUIHandler.Instance)
        {
            LeaderboardManager.Instance.EnableGameplayLeaderboard();
        }
    }

    public void TogglePauseMenu()
    {
        _pasueMenuObject.SetActive(!_pasueMenuObject.activeSelf);
        _pauseRestartButton.SetActive(false);

        if (Constants.IsPractice)
            _pauseRestartButton.SetActive(true);

        Time.timeScale = _pasueMenuObject.activeSelf ? 0 : 1;
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
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

}
