﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleTears : CollectibleItem
{
    [SerializeField]
    int value;
    protected override void onCollecting(PlayerController player)
    {
        player.HealMeter = value;
    }
}
