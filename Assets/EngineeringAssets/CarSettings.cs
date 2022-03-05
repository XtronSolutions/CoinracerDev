using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car", menuName = "Cars/Create car settings", order = 1)]
public class CarSettings : ScriptableObject
{
    public enum CarType
    {
        Car1,Car2,Car3,Car4,Car5,Car6,Car7, Car8, Car9, Car10,  Car11, Car12, Car13, Car14, Car15
    }
    
    public CarType carType;
    public string Name;
    public GameObject CarPrefab;
    public GameObject CarMultiplayerPrefab;
    public Sprite Icon;

}
