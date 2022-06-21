using UnityEngine;
public class InvokeCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        DamageHandler.DoFireEventTrigger(col,this.gameObject);
    }

    void OnTriggerExit(Collider col)
    {
        DamageHandler.DoFireEventLeaveTrigger(col, this.gameObject);
    }
}
