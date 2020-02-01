using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class CollectibleItem : MonoBehaviour
{


    [SerializeField]
    string itemName = "None";

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("entrou");
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            onCollecting(player);
            Destroy(this.gameObject);
        }
    }

    protected abstract void onCollecting(PlayerController player);
}
    
