// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShieldBar : StatusBar
{
    [SerializeField] private GameObject shield;

    // Update is called once per frame
    void Update()
    {
        float pct = (float)shield.GetComponent<Shield>().GetHealth() / (float)Constants.kShipInitialShieldHealth;
        UpdateBar(pct);
    }
}
