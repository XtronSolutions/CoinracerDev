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

    private List<CarSelection> SelectedCars = new List<CarSelection>();

    int carIndex = 0;
    int nextCarIndex = 0;
    int PrevCarIndex = 0;
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
        ChangeCarName("----");
        ChangeCarID("----");
        ToggleAnimator(false);
        ToggleCarContainers(true, true, true);
        ToggleLoaders(true, true, true);
    }

    public void ChangeCarName(string txt)
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
        carIndex = 0;
        nextCarIndex = 0;
        PrevCarIndex = 0;
        SelectedCars.Clear();
        NFTGameplayManager.Instance.AssignNFTData(ComponentGarage.CarBolt.gameObject,0,"Bolt");
    }
    public List<CarSelection> GetSelectedCars()
    {
        return SelectedCars;
    }

    public void SelectedCar_Add(CarSelection _car)
    {
        SelectedCars.Add(_car);
    }

    public void AssignAllCars()
    {
        for (int i = 0; i < SelectedCars.Count; i++)
        {
            if (carIndex == i)
            {
                //Debug.Log("Car index : " + carIndex);
                nextCarIndex = carIndex + 1;
                PrevCarIndex = carIndex - 1;

                AssignMiddleCar(SelectedCars[i].gameObject);

                if (PrevCarIndex >= 0)
                    AssignLeftCar(SelectedCars[i - 1].gameObject);

                if (nextCarIndex < SelectedCars.Count)
                    AssignRightCar(SelectedCars[i + 1].gameObject);
            }
            else if(nextCarIndex!=i && PrevCarIndex!=i)
            {
                PlaceCarBack(SelectedCars[i].gameObject);
            }
        }
    }

    public void PlaceCarBack(GameObject _car)
    {
        //Debug.Log("Putting car back : " + _car.gameObject.name);
        _car.transform.SetParent(ComponentGarage.CarSelectionContainer.transform);
        _car.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void AssignMiddleCar(GameObject _car)
    {
       // Debug.Log("Middle : "+_car.gameObject.name);
        ToggleAnimator(true);
        _car.transform.SetParent(ComponentGarage.MiddleCar.transform);
        ResetCarTransform(_car);
        _car.transform.GetChild(0).gameObject.SetActive(true);

        var _ref = _car.GetComponent<NFTDataHandler>();
        ChangeCarName(_ref.CarName);
        ChangeCarID(_ref.tokenID.ToString());
    }

    public void AssignLeftCar(GameObject _car)
    {
       // Debug.Log("Left : "+_car.gameObject.name);
        _car.transform.SetParent(ComponentGarage.LeftCar.transform);
        ResetCarTransform(_car);
        _car.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void AssignRightCar(GameObject _tempcar)
    {
        //Debug.Log("Right : "+ _tempcar.gameObject.name);
        _tempcar.transform.SetParent(ComponentGarage.RightCar.transform);
        ResetCarTransform(_tempcar);
        _tempcar.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ResetCarTransform(GameObject _car)
    {
        _car.transform.localPosition = new Vector3(0, 0, 0);
        _car.transform.localEulerAngles = new Vector3(0, 0, 0);
        _car.transform.localScale = new Vector3(1, 1, 1);
    }

    public void NextCar()
    {
        if (carIndex + 1 < SelectedCars.Count)
        {
            carIndex++;
            nextCarIndex = carIndex + 1;
            PrevCarIndex = carIndex - 1;
            AssignAllCars();
        }
    }

    public void PrevCar()
    {
        if (carIndex - 1 >= 0)
        {
            carIndex--;
            nextCarIndex = carIndex + 1;
            PrevCarIndex = carIndex - 1;
            AssignAllCars();
        }
    }

    public void DeleteData()
    {
        for (int i = 0; i < SelectedCars.Count; i++)
        {
            Destroy(SelectedCars[i].gameObject);
        }

        ResetGarage();
        ResetSelectedCar();
    }

    public void OnRepairClicked()
    {
        SelectedCars[carIndex].GetComponent<NFTDataHandler>().AccessConsumables();
    }
}
