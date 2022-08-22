using UnityEngine;

public enum CarTier
{
    Top = 0,
    Medium = 1,
    Low = 2
}

public enum CarType
{
    AllCar=-1,
    DestructionDerby = 0,
    RaceTrack = 1,
    Rally = 2
}

[System.Serializable]
public class BaseStats
{
    public int ID;
    public string Name;
    public double Acceleration;
    public double TopSpeed;
    public double Cornering;
    public double HP;
    public int Price;
    public CarTier Tier;
    public CarType Type;

    public ConsumableSettings Settings;

    public BaseStats()
    {
        this.ID = 0;
        this.Name = "XYZ";
        this.Acceleration = 100;
        this.TopSpeed = 100;
        this.Cornering = 100;
        this.HP = 100;
        this.Price = 0;
        this.Tier = CarTier.Top;
        this.Type = CarType.AllCar;
        this.Settings = null ;
    }
}

[CreateAssetMenu(fileName = "Stats", menuName = "Game Mechanics/Create stats settings", order = 2)]
public class StatSettings : ScriptableObject
{
    public BaseStats CarStats;
}
