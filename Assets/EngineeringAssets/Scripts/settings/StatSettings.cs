using UnityEngine;

public enum CarTier
{
    Top = 0,
    Medium = 1,
    Low = 2
}

public enum CarType
{
    DestructionDerby = 0,
    RaceTrack = 1,
    Rally = 2
}

[System.Serializable]
public class BaseStats
{
    public string Name;
    public double Acceleration;
    public double TopSpeed;
    public double Cornering;
    public double HP;
    public int Price;
    public CarTier Tier;
    public CarType Type;

    public ConsumableSettings Settings;
}

[CreateAssetMenu(fileName = "Stats", menuName = "Game Mechanics/Create stats settings", order = 2)]
public class StatSettings : ScriptableObject
{
    public BaseStats CarStats;
}
