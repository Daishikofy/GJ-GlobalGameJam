using UnityEngine;

public class CallStartHeal : MonoBehaviour
{
   public void callStartHeal()
    {
        GetComponentInParent<PlayerController>().animationTriggeredHealing();
    }
}
