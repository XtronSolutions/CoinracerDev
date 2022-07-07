using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageImpact
{
    public String Name;
    public double SpeedForDamage;
    public double HealthDeduction;
}

[CreateAssetMenu(fileName = "Damage1", menuName = "Game Mechanics/Create damage settings", order = 1)]
public class DamageSettings : ScriptableObject
{
    public enum DamageType
    {
        Side, Front, Back, Top
    }

    public DamageType damageType;
    public List<DamageImpact> DamageInfo = new List<DamageImpact>();
}
