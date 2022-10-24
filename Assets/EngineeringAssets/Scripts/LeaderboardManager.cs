using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


[Serializable]
public class LeaderBoardDataUI
{
    public Button CoinracerLeaderboard;
    public Button GramiceLeaderboard;
    public GameObject MainScreen;
    public GameObject ObjectPrefab;
    public Transform ScrollContent;
    public GameObject LoaderObj;
    public int ContentHeight;

}
public class LeaderboardManager : MonoBehaviour
{
    public LeaderBoardDataUI LeaderBoardUIData;
    public static LeaderboardManager Instance;
    List<GameObject> BoardObjects = new List<GameObject>();
    #region Leaderboard

    private void OnEnable()
    {
        Instance = this;
    }

    public void EnableRespectiveLeaderboard(bool IsSecondTour)
    {
        LeaderBoardUIData.MainScreen.SetActive(true);
        LeaderBoardUIData.LoaderObj.SetActive(true);

        FirebaseMoralisManager.Instance.QueryDB("", "", IsSecondTour);
        ToggleCoinracerButton(IsSecondTour);
        ToggleSecondTourButton(!IsSecondTour);
    }

    public void EnableGameplayLeaderboard()
    {
        LeaderBoardUIData.MainScreen.SetActive(true);
        LeaderBoardUIData.LoaderObj.SetActive(true);

        FirebaseMoralisManager.Instance.QueryDB("", "",false);
        ToggleCoinracerButton(false);
        ToggleSecondTourButton(true);
    }

    public void EnableSecondTourGameplayLeaderboard()
    {
        LeaderBoardUIData.MainScreen.SetActive(true);
        LeaderBoardUIData.LoaderObj.SetActive(true);

        FirebaseMoralisManager.Instance.QueryDB("", "",true);
        ToggleCoinracerButton(true);
        ToggleSecondTourButton(false);
    }

    public void ToggleCoinracerButton(bool state)
    {
        LeaderBoardUIData.CoinracerLeaderboard.interactable = state;
    }

    public void ToggleSecondTourButton(bool state)
    {
        LeaderBoardUIData.GramiceLeaderboard.interactable = state;
    }

    public void CloseLeaderBoard()
    {
        LeaderBoardUIData.MainScreen.SetActive(false);
    }

    public void ClearLeaderboard()
    {
        for (int i = 0; i < BoardObjects.Count; i++)
        {
            Destroy(BoardObjects[i]);
        }

        BoardObjects.Clear();
    }

    public void PopulateLeaderboardData(UserData[] _data, bool IsSecondTour)
    {
        ClearLeaderboard();

        for (int i = 0; i < _data.Length; i++)
        {
            GameObject _obj = Instantiate(LeaderBoardUIData.ObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            LeaderboardUI _UIInstance = _obj.GetComponent<LeaderboardUI>();

            //string _time = FirebaseMoralisManager.Instance.EncryptDecrypt(_data[i].TimeSeconds);
            //float _floatime = float.Parse(_time);
            float Seconds = (float)_data[i].TimeSeconds;

            if(IsSecondTour)
                Seconds = 0;//(float)_data[i].GTimeSeconds

            _UIInstance.SetPrefabData((i + 1).ToString(), _data[i].UserName, _data[i].WalletAddress, Seconds, _data[i].AvatarID);
            _obj.transform.SetParent(LeaderBoardUIData.ScrollContent);
            _obj.transform.localScale = new Vector3(1, 1, 1);
            LeaderBoardUIData.ScrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, LeaderBoardUIData.ContentHeight * (i + 1));

            BoardObjects.Add(_obj);
        }

        LeaderBoardUIData.LoaderObj.SetActive(false);
    }
    #endregion
}
