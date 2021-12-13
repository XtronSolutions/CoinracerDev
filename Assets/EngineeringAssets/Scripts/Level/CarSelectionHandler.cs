using System.Collections.Generic;
using Cinemachine;
using DavidJalbert;
using UnityEngine;

public class CarSelectionHandler : MonoBehaviour
{
    [SerializeField] private GameObject _spawnLocation = null;
    [SerializeField] private CarSettings _defualtCarSettings = null;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = null;
    [SerializeField] private WayPointPointer _wayPointPointer = null;
    
    
    // Start is called before the first frame update
    void Start()
    {
        CarSettings settings = MainMenuViewController.SelectedCar != null ? MainMenuViewController.SelectedCar : _defualtCarSettings; 
        GameObject car =Instantiate(settings.CarPrefab, _spawnLocation.transform.position,
            _spawnLocation.transform.rotation);

        TinyCarController controller = car.GetComponentInChildren<TinyCarController>();
        _virtualCamera.Follow = controller.transform;
        _virtualCamera.LookAt = controller.transform;
        _wayPointPointer.Controller = controller;
    }
}
