using UnityEngine;
public class InvokeCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (!Constants.GameMechanics)
            return;

        DamageHandler.DoFireEventTrigger(col,this.gameObject);
    }

    void OnTriggerExit(Collider col)
    {
        if (!Constants.GameMechanics)
            return;

        DamageHandler.DoFireEventLeaveTrigger(col, this.gameObject);
    }
}
