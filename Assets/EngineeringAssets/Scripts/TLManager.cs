using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class TLData
{
    public string _poolName;
    public int _poolAmount;
    public string _poolIndex;
    public int _ticketPrice;
    public bool _limitForCars;
    public List<string> _limitCars = new List<string>();
}

[Serializable]
public class TournamentLeagueData
{
    public GameObject _poolPrefab;
    public Transform _scrollRectContent;
    public List<TLData> DataTL = new List<TLData>();
}
public class TLManager : MonoBehaviour
{
    public TournamentLeagueData DataTournamentLeague;

    private void Start()
    {
        PopulateTLData();
    }
    public void PopulateTLData()
    {
        for (int i = 0; i < DataTournamentLeague.DataTL.Count; i++)
        {
            GameObject _obj = Instantiate(DataTournamentLeague._poolPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            TLPrefabHandler _UIInstance = _obj.GetComponent<TLPrefabHandler>();

            _UIInstance.SetPrefabData(DataTournamentLeague.DataTL[i]._poolAmount.ToString()+" Crace", DataTournamentLeague.DataTL[i]._poolName, DataTournamentLeague.DataTL[i]._ticketPrice);
            _obj.transform.SetParent(DataTournamentLeague._scrollRectContent);
            _obj.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
