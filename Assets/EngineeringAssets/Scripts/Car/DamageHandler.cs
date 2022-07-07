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

    private int StartRange = 0;
    private int EndRange = 0;
    private int CarSpeed = 0;
    private bool TriggerEnterted = false;
    private int StoredIndex = 0;
    private bool CarTotaled = false;
    private float CoolDownTime = 0.75f; //if there are consective collsions then if colldown period is passed only then damage is applied, so multiple damage is not given on multiple hits in few seconds
    private bool CanDamage = true;
    
    void OnEnable()
    {
        CarTotaled = false; //check if car was totaled from db
        Constants.StoredCarHealth = 100;
        TinyCarController.speedMultiplier = 1;

        if (!Constants.GameMechanics)
            return;

        ApplySpeedFactor(Constants.StoredCarHealth);
        TriggerEvent += OnTriggerEvent;
        TriggerLeaveEvent += OnTriggerLeaveEvent;
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

        if (obj.tag == "DamageCol" && !TriggerEnterted && (col.gameObject.tag=="SideCollider" || col.transform.parent.tag== "SideCollider" || col.gameObject.tag == "DamageCol"))
        {
            if (!CarTotaled)
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

                        Debug.Log("Damage Collider hit: " + obj.name + " with damage :" + ColliderDamage[StoredIndex].ImpactInfo.DamageInfo[i].HealthDeduction.ToString() + " at speed : " + TinyCarController.carSpeed.ToString());
                        Constants.StoredCarHealth -= (int)ColliderDamage[StoredIndex].ImpactInfo.DamageInfo[i].HealthDeduction;

                        if (Constants.StoredCarHealth <= 0)
                        {
                            CarTotaled = true;
                            Constants.StoredCarHealth = 0;

                            if (GamePlayUIHandler.Instance)
                                GamePlayUIHandler.Instance.InstantiateGameOver_CarTotaled();
                        }

                        if (UIHealth.Instance)
                            UIHealth.Instance.UpdateHealth(Constants.StoredCarHealth);
                        else
                            Debug.LogError("UIHealth instance is null");

                        ApplySpeedFactor(Constants.StoredCarHealth);

                        break;
                    }
                }
            }
        }
    }

    public void ApplySpeedFactor(int _health)
    {
        if (!Constants.GameMechanics)
            return;

        if (!CarTotaled)
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

        if (obj.tag == "DamageCol" && TriggerEnterted && (col.gameObject.tag == "SideCollider" || col.transform.parent.tag == "SideCollider"))
            TriggerEnterted = false;
    }

}
