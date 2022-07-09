using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConsumableData
{
    public float LapLimit;
    public float VC_Cost;
}

[CreateAssetMenu(fileName = "Consumable", menuName = "Game Mechanics/Create consumable settings", order = 1)]
public class ConsumableSettings : ScriptableObject
{
    public ConsumableData DamageRepair;
    public ConsumableData Tyres;
    public ConsumableData EngineOil;
    public ConsumableData Gas;
}
