using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Speed_HP
{
    public double HpPercentage;
    public double SpeedPercentage;
}

[CreateAssetMenu(fileName = "HPSetting", menuName = "Game Mechanics/Create HP-Damage settings", order = 1)]
public class HPSettings : ScriptableObject
{
    public List<Speed_HP> HP_Speed = new List<Speed_HP>();
}
