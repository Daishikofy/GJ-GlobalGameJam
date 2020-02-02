#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleTears : CollectibleItem
{
    [SerializeField]
    int value;

    [SerializeField]
    float timeBeforeVanishing;
    float currentTime;
    protected override void onCollecting(PlayerController player)
    {
        player.HealMeter += value;
    }

    private void Update()
    {
        if (currentTime >= timeBeforeVanishing)
            Destroy(this.gameObject);
        currentTime += Time.deltaTime;
    }
}
