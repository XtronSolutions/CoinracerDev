using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class GarageComponent
{
    public GameObject MainScreen;
    public TextMeshProUGUI CarName_Text;
    public TextMeshProUGUI CarID_Text;
    public GameObject CarSelectionContainer;
    public Transform MiddleCar;
    public Transform LeftCar;
    public Transform RightCar;
    public Button NextCarButton;
    public Button PrevCarButton;
    public Button RepairButton;
    public Button UpgradeButton;
    public Animator MiddleCarAnimator;
    public GameObject MiddleCarLoader;
    public GameObject LeftCarLoader;
    public GameObject RightCarLoader;
    public CarSelection CarBolt;
}
public class GarageHandler : MonoBehaviour
{
    public GarageComponent ComponentGarage;
    public static GarageHandler Instance;

    private void OnEnable()
    {
        Instance = this;
        ResetGarage();

        ComponentGarage.NextCarButton.onClick.AddListener(NextCar);
        ComponentGarage.PrevCarButton.onClick.AddListener(PrevCar);
        ComponentGarage.RepairButton.onClick.AddListener(OnRepairClicked);
    }

    public void ResetGarage()
    {
        ChangeCarName_Garage("----");
        ChangeCarID("----");
        //ToggleAnimator(false);
        ToggleCarContainers(true, true, true);
        ToggleLoaders(true, true, true);
    }

    public void ChangeCarName_Garage(string txt)
    {
        ComponentGarage.CarName_Text.text = txt;
    }

    public void ChangeCarID(string txt)
    {
        ComponentGarage.CarID_Text.text = "#"+txt;
    }

    public void ToggleAnimator(bool state)
    {
        ComponentGarage.MiddleCarAnimator.enabled = state;
    }

    public void ToggleLoaders(bool state1 = true, bool state2 = true, bool state3 = true)
    {
        ComponentGarage.MiddleCarLoader.SetActive(state1);
        ComponentGarage.LeftCarLoader.SetActive(state2);
        ComponentGarage.RightCarLoader.SetActive(state3);
    }

    public void ToggleCarContainers(bool state1 = true, bool state2 = true, bool state3 = true)
    {
        ComponentGarage.MiddleCar.gameObject.SetActive(state1);
        ComponentGarage.LeftCar.gameObject.SetActive(state2);
        ComponentGarage.RightCar.gameObject.SetActive(state3);
    }

    public void ResetSelectedCar()
    {
        MainMenuViewController.Instance.ResetSelectedCarStore();
        MainMenuViewController.Instance.AssignStoreGarageData(ComponentGarage.CarBolt.gameObject, 0, "Bolt", null, GarageHandler.Instance.ComponentGarage.CarSelectionContainer.transform, true, false);
    }


    public void NextCar()
    {
        MainMenuViewController.Instance.OnNextButtonClicked();
        MainMenuViewController.Instance.AssignStoreGarageCars(ComponentGarage.MiddleCar, ComponentGarage.LeftCar, ComponentGarage.RightCar, ComponentGarage.CarSelectionContainer.transform, ComponentGarage.CarName_Text, ComponentGarage.CarID_Text, true,false);
    }

    public void PrevCar()
    {
        MainMenuViewController.Instance.OnPrevButtonClicked();
        MainMenuViewController.Instance.AssignStoreGarageCars(ComponentGarage.MiddleCar, ComponentGarage.LeftCar, ComponentGarage.RightCar, ComponentGarage.CarSelectionContainer.transform, ComponentGarage.CarName_Text, ComponentGarage.CarID_Text, true,false);
    }

    public void DeleteData()
    {
        ResetGarage();
        ResetSelectedCar();
    }

    public void OnRepairClicked()
    {
        MainMenuViewController.Instance.OnRepairButtonCLicked();
    }
}
