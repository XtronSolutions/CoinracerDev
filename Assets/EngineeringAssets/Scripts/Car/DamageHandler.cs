using DavidJalbert;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DamageCollider
{
    public string Name;
    public GameObject Collider;
    public double LimitForDamage;
    public DamageSettings ImpactInfo;
}
public class DamageHandler : MonoBehaviour
{
    public static Action<Collider,GameObject> TriggerEvent = null;
    public static void DoFireEventTrigger(Collider col, GameObject obj) => TriggerEvent?.Invoke(col, obj);

    public static Action<Collider, GameObject> TriggerLeaveEvent = null;
    public static void DoFireEventLeaveTrigger(Collider col, GameObject obj) => TriggerLeaveEvent?.Invoke(col, obj);

    public List<DamageCollider> ColliderDamage = new List<DamageCollider>();
    public HPSettings SettingsHP;

    public CarReferenceHandler Ref;

    private int StartRange = 0;
    private int EndRange = 0;
    private int CarSpeed = 0;
    private bool TriggerEnterted = false;
    private int StoredIndex = 0;
    private float CoolDownTime = 0.75f; //if there are consective collsions then if colldown period is passed only then damage is applied, so multiple damage is not given on multiple hits in few seconds
    private bool CanDamage = true;
    private int TempHealth;

    [HideInInspector] public int CarHealthStored = 100;
    
    void OnEnable()
    {
        Constants.CarTotaled = false; //check if car was totaled from db
        //CarHealthStored = 100;
        //Constants.StoredCarHealth = 100;
        TinyCarController.speedMultiplier = 1;

        if (!Constants.GameMechanics)
            return;

        TempHealth = Constants.StoredCarHealth;
        CarHealthStored = TempHealth;
        ApplySpeedFactor(TempHealth);
        TriggerEvent += OnTriggerEvent;
        TriggerLeaveEvent += OnTriggerLeaveEvent;
        StartCoroutine(PushHealth());
    }

    public IEnumerator PushHealth()
    {
        yield return new WaitForSeconds(10f);
        if(!Constants.CarTotaled)
        {
            if (TempHealth != CarHealthStored)
            {
                TempHealth = CarHealthStored;
                MechanicsManager.Instance.UpdateHealth(Constants.SelectedCarToken, TempHealth);
            }
            StartCoroutine(PushHealth());
        }
    }

    public void UpdateMiniBar()
    {
        if (Constants.IsMultiplayer)
        {
            if (Ref.tinyCarController.PHView.IsMine)
            {
                if (Ref.uIHealth)
                    Ref.uIHealth.UpdateHealth(CarHealthStored);
            }
        }
    }
    private void Start()
    {
        UpdateMiniBar();

        if (UIHealth.Instance)
            UIHealth.Instance.UpdateHealth(CarHealthStored);
        else
            Constants.PrintError("UIHealth instance is null");
    }

    void OnDisable()
    {
        TriggerEvent -= OnTriggerEvent;
        TriggerLeaveEvent -= OnTriggerLeaveEvent;
    }

    IEnumerator InitiateCoolDown()
    {
        CanDamage = false;
        yield return new WaitForSeconds(CoolDownTime);
        CanDamage = true;
    }
    void OnTriggerEvent(Collider col,GameObject obj)
    {
        if (!Constants.GameMechanics)
            return;

        if (!CanDamage)
            return;

        if(Constants.IsMultiplayer)
        {
            if (!Ref.tinyCarController.PHView.IsMine)
                return;
        }

        if (obj.tag == "DamageCol" && !TriggerEnterted && (col.gameObject.tag=="SideCollider" || col.transform.parent.tag== "SideCollider" || col.gameObject.tag == "DamageCol"))
        {
            if (!Constants.CarTotaled)
            {
                CarSpeed = (int)TinyCarController.carSpeed;
                TriggerEnterted = true;
                StoredIndex = int.Parse(obj.name.Split('_')[0]);

                for (int i = 0; i < ColliderDamage[StoredIndex].ImpactInfo.DamageInfo.Count; i++)
                {
                    StartRange = (int)ColliderDamage[StoredIndex].ImpactInfo.DamageInfo[i].SpeedForDamage;

                    if (i < ColliderDamage[StoredIndex].ImpactInfo.DamageInfo.Count - 1)
                        EndRange = (int)ColliderDamage[StoredIndex].ImpactInfo.DamageInfo[i + 1].SpeedForDamage;
                    else
                        EndRange = (int)ColliderDamage[StoredIndex].ImpactInfo.DamageInfo[ColliderDamage[StoredIndex].ImpactInfo.DamageInfo.Count - 1].SpeedForDamage;

                    if (Enumerable.Range(StartRange, EndRange).Contains(CarSpeed) || CarSpeed > EndRange)
                    {
                        StartCoroutine(InitiateCoolDown());

                        Constants.PrintLog("Damage Collider hit: " + obj.name + " with damage :" + ColliderDamage[StoredIndex].ImpactInfo.DamageInfo[i].HealthDeduction.ToString() + " at speed : " + TinyCarController.carSpeed.ToString());
                        CarHealthStored -= (int)ColliderDamage[StoredIndex].ImpactInfo.DamageInfo[i].HealthDeduction;

                        if (CarHealthStored <= 0)
                        {
                            Constants.CarTotaled = true;
                            CarHealthStored = 0;

                            StopCoroutine(PushHealth());
                            MechanicsManager.Instance.UpdateHealth(Constants.SelectedCarToken, CarHealthStored);
                            GameOver();
                            
                        }

                        UpdateMiniBar();

                        if (UIHealth.Instance)
                            UIHealth.Instance.UpdateHealth(CarHealthStored);
                        else
                            Constants.PrintError("UIHealth instance is null");

                        ApplySpeedFactor(CarHealthStored);

                        break;
                    }
                }
            }
        }
    }

    public void GameOver()
    {
        TimeHandler.Instance.timerIsRunning = false;
        if (GamePlayUIHandler.Instance)
            GamePlayUIHandler.Instance.InstantiateGameOver_CarTotaled("Car is destroyed, better luck next time.");
           
        if(Constants.IsMultiplayer)
        {
            Constants.MoveCar = false;
            MultiplayerManager.Instance.CallEndMultiplayerGameRPC(true);
        }

        if (AnalyticsManager.Instance)
        {
            AnalyticsManager.Instance.StoredProgression.TimeSeconds = (int)Constants.GameSeconds;

            if (AnalyticsManager.Instance.StoredProgression.fields.ContainsKey("TimeSeconds"))
                AnalyticsManager.Instance.StoredProgression.fields["TimeSeconds"] = 0;
            else
                AnalyticsManager.Instance.StoredProgression.fields.Add("TimeSeconds", 0);

            AnalyticsManager.Instance.PushProgressionEvent(false,true);
        }
    }
    public void ApplySpeedFactor(int _health)
    {
        if (!Constants.GameMechanics)
            return;

        if (!Constants.CarTotaled)
        {
            for (int k = 0; k < SettingsHP.HP_Speed.Count; k++)
            {
                StartRange = (int)SettingsHP.HP_Speed[k].HpPercentage;

                if (k < SettingsHP.HP_Speed.Count - 1)
                    EndRange = (int)SettingsHP.HP_Speed[k + 1].HpPercentage;
                else
                    EndRange = (int)SettingsHP.HP_Speed[SettingsHP.HP_Speed.Count - 1].HpPercentage;

                if (_health <= StartRange && _health >= EndRange)
                {
                    TinyCarController.speedMultiplier = (float)SettingsHP.HP_Speed[k].SpeedPercentage / 100;
                    break;
                }
            }
        }else
        {
            TinyCarController.speedMultiplier = 0.0001f;
        }
    }

    void OnTriggerLeaveEvent(Collider col, GameObject obj)
    {
        if (!Constants.GameMechanics)
            return;

        if (obj.tag == "DamageCol" && TriggerEnterted && (col.gameObject.tag == "SideCollider" || col.transform.parent.tag == "SideCollider" || col.gameObject.tag == "DamageCol"))
            TriggerEnterted = false;
    }

}
