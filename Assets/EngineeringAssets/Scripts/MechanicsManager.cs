using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    None,Health,Tyres,Oil,Gas
}
public class MechanicsManager : MonoBehaviour
{
    public ConsumableSettings _consumableSettings;
    public static MechanicsManager Instance;

    private float RemainingTyreLaps;
    private float RemainingOilLaps;
    private float RemainingGasLaps;

    private ConsumableType _consumableType;

    public float GetRemainingTyreLaps()
    {
        return RemainingTyreLaps;
    }

    public float GetRemainingOilLaps()
    {
        return RemainingOilLaps;
    }

    public float GetRemainingGasLaps()
    {
        return RemainingGasLaps;
    }

    private void OnEnable()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void UpdateConsumables(NFTMehanicsData _data)
    {
       RemainingTyreLaps = _consumableSettings.Tyres.LapLimit - _data.mechanicsData.Tyre_Laps<=0?0: _consumableSettings.Tyres.LapLimit - _data.mechanicsData.Tyre_Laps;
       RemainingOilLaps = _consumableSettings.EngineOil.LapLimit - _data.mechanicsData.EngineOil_Laps <=0?0: _consumableSettings.EngineOil.LapLimit - _data.mechanicsData.EngineOil_Laps;
       RemainingGasLaps = _consumableSettings.Gas.LapLimit - _data.mechanicsData.Gas_Laps <=0?0 : _consumableSettings.Gas.LapLimit - _data.mechanicsData.Gas_Laps;
    }

    public ConsumableType CheckConsumables()
    {
        if (Constants.StoredCarHealth <= 0)
        { return ConsumableType.Health; }
        else if (MechanicsManager.Instance.GetRemainingTyreLaps() <= 0)
        { return ConsumableType.Tyres; }
        else if (MechanicsManager.Instance.GetRemainingOilLaps() <= 0)
        { return ConsumableType.Oil; }
        else if (MechanicsManager.Instance.GetRemainingGasLaps() <= 0)
        { return ConsumableType.Gas; }
        else
        { return ConsumableType.None; }
    }
}
