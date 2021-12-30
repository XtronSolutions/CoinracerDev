using System.Collections.Generic;
using Cinemachine;
using DavidJalbert;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class CarSelectionHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] _spawnLocation = null;
    [SerializeField] private CarSettings _defualtCarSettings = null;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = null;
    [SerializeField] private WayPointPointer _wayPointPointer = null;
    
    
    // Start is called before the first frame update
    void Start()
    {
        CarSettings settings = MainMenuViewController.SelectedCar != null ? MainMenuViewController.SelectedCar : _defualtCarSettings;
        //CarSettings settings = _defualtCarSettings;

        //Debug.LogError(settings.Name);
        GameObject car;

        if (Constants.IsMultiplayer)
        {
            int _index = 0;

            if (PhotonNetwork.IsMasterClient)
                _index = 1;

            car = PhotonNetwork.Instantiate(settings.CarMultiplayerPrefab.name, _spawnLocation[_index].transform.position,_spawnLocation[_index].transform.rotation) as GameObject;
        }else
        {
            car = Instantiate(settings.CarPrefab, _spawnLocation[0].transform.position, _spawnLocation[0].transform.rotation) as GameObject;
        }

        TinyCarController controller = car.GetComponentInChildren<TinyCarController>();
        _virtualCamera.Follow = controller.transform;
        _virtualCamera.LookAt = controller.transform;
        _wayPointPointer.Controller = controller;
    }
}
