using DavidJalbert;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class Speed_HP
{
    public double HpPercentage;
    public double SpeedPercentage;
}

[System.Serializable]
public class DamageImpact
{
    public String Name;
    public double SpeedForDamage;
    public double HealthDeduction;
}

[System.Serializable]
public class DamageCollider
{
    public string Name;
    public GameObject Collider;
    public double LimitForDamage;
    public List<DamageImpact> ImpactInfo = new List<DamageImpact>();
}
public class DamageHandler : MonoBehaviour
{
    public static Action<Collider,GameObject> TriggerEvent = null;
    public static void DoFireEventTrigger(Collider col, GameObject obj) => TriggerEvent?.Invoke(col, obj);

    public static Action<Collider, GameObject> TriggerLeaveEvent = null;
    public static void DoFireEventLeaveTrigger(Collider col, GameObject obj) => TriggerLeaveEvent?.Invoke(col, obj);

    public List<DamageCollider> ColliderDamage = new List<DamageCollider>();
    public List<Speed_HP> HP_Speed = new List<Speed_HP>();

    private int StartRange = 0;
    private int EndRange = 0;
    private int CarSpeed = 0;
    private bool TriggerEnterted = false;
    private int StoredIndex = 0;
    void OnEnable()
    {
        TriggerEvent += OnTriggerEvent;
        TriggerLeaveEvent += OnTriggerLeaveEvent;
    }

    void OnDisable()
    {
        TriggerEvent -= OnTriggerEvent;
        TriggerLeaveEvent -= OnTriggerLeaveEvent;
    }

    void OnTriggerEvent(Collider col,GameObject obj)
    {
        if (obj.tag == "DamageCol" && !TriggerEnterted && (col.gameObject.tag=="SideCollider" || col.transform.parent.tag== "SideCollider"))
        {
            CarSpeed = (int)TinyCarController.carSpeed;
            TriggerEnterted = true;
            StoredIndex = int.Parse(obj.name.Split('_')[0]);

            for (int i = 0; i < ColliderDamage[StoredIndex].ImpactInfo.Count; i++)
            {
                StartRange = (int)ColliderDamage[StoredIndex].ImpactInfo[i].SpeedForDamage;

                if (i< ColliderDamage[StoredIndex].ImpactInfo.Count-1)
                    EndRange = (int)ColliderDamage[StoredIndex].ImpactInfo[i+1].SpeedForDamage;
                else
                    EndRange = (int)ColliderDamage[StoredIndex].ImpactInfo[ColliderDamage[StoredIndex].ImpactInfo.Count-1].SpeedForDamage;

                if(Enumerable.Range(StartRange, EndRange).Contains(CarSpeed) || CarSpeed> EndRange)
                {
                    Debug.Log("Damage Collider hit: " + obj.name + " with damage :" + ColliderDamage[StoredIndex].ImpactInfo[i].HealthDeduction.ToString()+" at speed : "+ TinyCarController.carSpeed.ToString());
                    TinyCarController.CarHealth -=(int) ColliderDamage[StoredIndex].ImpactInfo[i].HealthDeduction;
                    GamePlayUIHandler.Instance.SetHealthText(TinyCarController.CarHealth.ToString());
                    break;
                }
            }
        }
    }

    void OnTriggerLeaveEvent(Collider col, GameObject obj)
    {
        if (obj.tag == "DamageCol" && TriggerEnterted && (col.gameObject.tag == "SideCollider" || col.transform.parent.tag == "SideCollider"))
            TriggerEnterted = false;
    }

}
